# Provisioning

 The Azure resources for the orchestrator are provisioned through ARM templates.

## Create a resource group

```bash
az login
az group create -n <resourceGroupName> -l <region>
```

Save the Resource Group Name for the next steps.

## Create a service principal

This service principal will be used by the web apis to access secrets stored in Key Vault. For more details on how to create a service principal, see the [official docs](https://docs.microsoft.com/en-us/cli/azure/create-an-azure-service-principal-azure-cli?view=azure-cli-latest).

```bash
az login
az ad sp create-for-rbac -n <servicePrincipalName> --skip-assignment
```

Save the Client Id (also known as App Id) and Client Secret for the service principal for the next steps.

## Deploy Key Vault

```bash
cd Provisioning
az group deployment create \
    -n keyvaultdeploy -g <resourceGroupName> \
    --template-file keyvault.json \
    --parameters keyvault.parameters.json \
    --parameters keyVaultClientId=<clientId> \
    --parameters keyVaultClientSecret=<clientSecret>
```

This command will deploy the following resources:

- Key Vault
- Key Vault Access Policy for Service Principal

Key vault is deployed in a separate ARM template because of a current restriction that access policies defined in the template will override manual access policies set in the portal.

Because of this, we recommend only deploying this template once at the beginning of the project, if you have set any manual access policies.

## Deploy All Other Azure Resources

```bash
cd Provisioning
az group deployment create \
    -n orchestratordeploy -g <resourceGroupName> \
    --template-file azuredeploy.json \
    --parameters azuredeploy.parameters.json \
    --parameters keyVaultClientId=<clientId> \
    --parameters keyVaultClientSecret=<clientSecret>
```

This command will deploy the following resources:

- IotHub
- EventHub Namespace
  - EventHub for Fleet Manager Routing
  - EventHub for Order Manager Routing
- Azure Container Registry (ACR)
- Storage Account
- Cosmos DB Account
- App Service Plan (Windows)
- Web Apps
  - Order Producer
  - Order Manager
  - Fleet Manager
  - Dispatcher
  - UI
- App Settings Configs for All Web Apps
- Key Vault Secrets needed for apps to run

It may take around 10 minutes to deploy.

The azuredeploy.json ARM template is safe to deploy as many times as needed, and changes will be automatically detected and deployed. Take care that app settings should be set only in the arm template, and manual app settings will be overridden by the template.

The default modifier for the project is the envName 'dev'. If deploying to a second environment, you can add an optional `--parameters envName=prod` or other envName to the last two steps to recreate the entire environment.