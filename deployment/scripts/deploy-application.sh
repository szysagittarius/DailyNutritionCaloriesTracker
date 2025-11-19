#!/bin/bash
# Script: Deploy Application to Azure Functions
# Usage: ./deploy-application.sh <environment>

set -e

ENVIRONMENT=${1:-dev}
FUNCTION_APP="func-nutrition-tracker-$ENVIRONMENT"
RESOURCE_GROUP="rg-nutrition-tracker-$ENVIRONMENT"
PROJECT_PATH="../../src/Adapters/Input/NutritionTracker.AzureFunctions"

echo "Deploying application to: $FUNCTION_APP"
echo ""

# Navigate to project directory
cd "$(dirname "$0")"
cd $PROJECT_PATH

# Build and publish
echo "Building and publishing project..."
dotnet publish \
  --configuration Release \
  --output ./publish

# Deploy to Azure
echo "Deploying to Azure Functions..."
cd publish
func azure functionapp publish $FUNCTION_APP --dotnet-isolated

# Get Function URL
cd ../..
FUNCTION_URL=$(az functionapp show \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --query defaultHostName -o tsv)

echo ""
echo "✅ Application deployed successfully!"
echo ""
echo "Function App: $FUNCTION_APP"
echo "URL: https://$FUNCTION_URL"
echo ""
echo "Available endpoints:"
echo "  GET  https://$FUNCTION_URL/api/users"
echo "  GET  https://$FUNCTION_URL/api/foodnutrition"
echo "  GET  https://$FUNCTION_URL/api/foodlog"
echo ""
echo "Test with: curl https://$FUNCTION_URL/api/users"
