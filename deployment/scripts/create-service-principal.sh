#!/bin/bash
# Script: Create Azure Service Principal for CI/CD
# Usage: ./create-service-principal.sh <subscription-id> <sp-name>

set -e

SUBSCRIPTION_ID=${1:-$(az account show --query id -o tsv)}
SP_NAME=${2:-"github-nutrition-tracker-deploy"}

echo "Creating service principal for subscription: $SUBSCRIPTION_ID"
echo "Service principal name: $SP_NAME"

# Create service principal
SP_OUTPUT=$(az ad sp create-for-rbac \
  --name "$SP_NAME" \
  --role Contributor \
  --scopes /subscriptions/$SUBSCRIPTION_ID \
  --sdk-auth)

echo ""
echo "✅ Service Principal created successfully!"
echo ""
echo "Add this as GitHub Secret 'AZURE_CREDENTIALS':"
echo "================================================"
echo "$SP_OUTPUT"
echo "================================================"
echo ""

# Extract values for Azure DevOps
CLIENT_ID=$(echo $SP_OUTPUT | jq -r '.clientId')
CLIENT_SECRET=$(echo $SP_OUTPUT | jq -r '.clientSecret')
TENANT_ID=$(echo $SP_OUTPUT | jq -r '.tenantId')

echo "For Azure DevOps, create a service connection with:"
echo "================================================"
echo "Subscription ID: $SUBSCRIPTION_ID"
echo "Client ID: $CLIENT_ID"
echo "Client Secret: $CLIENT_SECRET"
echo "Tenant ID: $TENANT_ID"
echo "================================================"
