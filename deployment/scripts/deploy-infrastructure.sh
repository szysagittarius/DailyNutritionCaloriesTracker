#!/bin/bash
# Script: Deploy Azure Infrastructure
# Usage: ./deploy-infrastructure.sh <environment> <location>

set -e

ENVIRONMENT=${1:-dev}
LOCATION=${2:-eastus}

RESOURCE_GROUP="rg-nutrition-tracker-$ENVIRONMENT"
STORAGE_ACCOUNT="stnutritiontracker$ENVIRONMENT"
FUNCTION_APP="func-nutrition-tracker-$ENVIRONMENT"
FUNC_STORAGE="stfuncnutrition$ENVIRONMENT"

echo "Deploying infrastructure for environment: $ENVIRONMENT"
echo "Location: $LOCATION"
echo ""

# Create resource group
echo "Creating resource group..."
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Create storage account for Table Storage
echo "Creating storage account for Table Storage..."
az storage account create \
  --name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Standard_LRS \
  --kind StorageV2 \
  --allow-blob-public-access false \
  --min-tls-version TLS1_2

# Create storage account for Function App
echo "Creating storage account for Function App..."
az storage account create \
  --name $FUNC_STORAGE \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Standard_LRS

# Create Function App (Consumption Plan)
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

echo ""
echo "✅ Deployment completed successfully!"
echo ""
echo "Resource Group: $RESOURCE_GROUP"
echo "Storage Account: $STORAGE_ACCOUNT"
echo "Function App: $FUNCTION_APP"
echo "Function URL: https://$FUNCTION_URL"
echo ""
echo "Next steps:"
echo "1. Run: ./deploy-application.sh $ENVIRONMENT"
echo "2. Test endpoints at: https://$FUNCTION_URL/api"
