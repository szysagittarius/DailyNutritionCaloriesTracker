#!/bin/bash
# Script: Deploy Azure Infrastructure
# Usage: ./deploy-infrastructure.sh <environment> <location>

set -e

ENVIRONMENT=${1:-dev}
LOCATION=${2:-eastus}
# Static Web Apps Free tier is only available in a subset of regions
SWA_LOCATION=${3:-eastus2}

RESOURCE_GROUP="rg-nutrition-tracker-$ENVIRONMENT"
STORAGE_ACCOUNT="stnutritiontracker$ENVIRONMENT"
FUNCTION_APP="func-nutrition-tracker-$ENVIRONMENT"
FUNC_STORAGE="stfuncnutrition$ENVIRONMENT"
STATIC_WEB_APP="swa-nutrition-tracker-$ENVIRONMENT"

echo "Deploying infrastructure for environment: $ENVIRONMENT"
echo "Location: $LOCATION"
echo ""

# Create resource group
echo "Creating resource group..."
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Create storage account for Table Storage (skip if it already exists)
if az storage account show --name $STORAGE_ACCOUNT --resource-group $RESOURCE_GROUP &>/dev/null; then
  echo "Storage account $STORAGE_ACCOUNT already exists, skipping"
else
  echo "Creating storage account for Table Storage..."
  az storage account create \
    --name $STORAGE_ACCOUNT \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku Standard_LRS \
    --kind StorageV2 \
    --allow-blob-public-access false \
    --min-tls-version TLS1_2
fi

# Create storage account for Function App (skip if it already exists)
if az storage account show --name $FUNC_STORAGE --resource-group $RESOURCE_GROUP &>/dev/null; then
  echo "Storage account $FUNC_STORAGE already exists, skipping"
else
  echo "Creating storage account for Function App..."
  az storage account create \
    --name $FUNC_STORAGE \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku Standard_LRS
fi

# Create Function App (Consumption Plan, skip if it already exists)
if az functionapp show --name $FUNCTION_APP --resource-group $RESOURCE_GROUP &>/dev/null; then
  echo "Function App $FUNCTION_APP already exists, skipping"
else
  echo "Creating Function App..."
  az functionapp create \
    --name $FUNCTION_APP \
    --resource-group $RESOURCE_GROUP \
    --consumption-plan-location $LOCATION \
    --storage-account $FUNC_STORAGE \
    --runtime dotnet-isolated \
    --runtime-version 8 \
    --functions-version 4 \
    --os-type Linux
fi

# Azure disables SCM basic-auth publishing by default, which breaks publish-profile deploys
# ("Kudu ... Unauthorized 401") unless explicitly enabled
az resource update \
  --resource-group $RESOURCE_GROUP \
  --name scm \
  --namespace Microsoft.Web \
  --resource-type basicPublishingCredentialsPolicies \
  --parent sites/$FUNCTION_APP \
  --set properties.allow=true

# Create Static Web App for frontend hosting (Free tier, skip if it already exists)
if az staticwebapp show --name $STATIC_WEB_APP --resource-group $RESOURCE_GROUP &>/dev/null; then
  echo "Static Web App $STATIC_WEB_APP already exists, skipping"
else
  echo "Creating Static Web App (Free tier)..."
  az staticwebapp create \
    --name $STATIC_WEB_APP \
    --resource-group $RESOURCE_GROUP \
    --location $SWA_LOCATION \
    --sku Free
fi

# Get Table Storage connection string
echo "Configuring Function App settings..."
TABLE_CONNECTION_STRING=$(az storage account show-connection-string \
  --name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --query connectionString -o tsv)

# Configure Function App settings
az functionapp config appsettings set \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --settings \
    "AzureTableStorageConnectionString=$TABLE_CONNECTION_STRING" \
    "FUNCTIONS_WORKER_RUNTIME=dotnet-isolated"

# Enable CORS
az functionapp cors add \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --allowed-origins "*"

# Get Function App URL
FUNCTION_URL=$(az functionapp show \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --query defaultHostName -o tsv)

# Get Static Web App URL
SWA_URL=$(az staticwebapp show \
  --name $STATIC_WEB_APP \
  --query defaultHostname -o tsv)

echo ""
echo "✅ Deployment completed successfully!"
echo ""
echo "Resource Group: $RESOURCE_GROUP"
echo "Storage Account: $STORAGE_ACCOUNT"
echo "Function App: $FUNCTION_APP"
echo "Function URL: https://$FUNCTION_URL"
echo "Static Web App: $STATIC_WEB_APP"
echo "Web URL: https://$SWA_URL"
echo ""
echo "Next steps:"
echo "1. Run: ./deploy-application.sh $ENVIRONMENT"
echo "2. Test endpoints at: https://$FUNCTION_URL/api"
echo "3. Get the SWA deployment token: az staticwebapp secrets list --name $STATIC_WEB_APP --query \"properties.apiKey\" -o tsv"
echo "4. Add it as GitHub secret AZURE_STATIC_WEB_APPS_API_TOKEN"
