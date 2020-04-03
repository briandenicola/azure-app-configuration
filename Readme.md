# Introduction
This is a quick demo on how to use Azure App Configuration with Managed Identitie and Azure App Service slots

# Setup
## Automated
* cd .\infra
* Update azuredeploy.parameters.json for unique values to your environment
* New-AzResourceGroup -ResourceGroupName DevSub01_AppConfig02_RG -location centralus
* New-AzResourceGroupDeployment -Name appconfig -ResourceGroupName DevSub01_AppConfig02_RG -TemplateParameterFile .\azuredeploy.parameters.json -TemplateFile .\azuredeploy.json -Verbose

## Manual 
* Create two App Configuration falues
    * Key Vault Reference - config::kv::test001
        * Select the Key Vault created in the Resource Group deployment
        * You may have to grant yourself to the Key Vault acess policies 
    * Key-Value - config::basic::test001
        * Value - "this came from AppConfig"

## Web Site Deployment
* Deploy the code in .\src to your Azure App Service slot. I use Visual Studio Cdoe for my deploy

# Test
* Invoke-RestMethod -UseBasicParsing -Uri https://bjdweb002-uat.azurewebsites.net/appconfig | ConvertTo-Json
    * Assume your Azure App Service is called bjdweb002
* Response: 
    ```
    [
        {
            "date": "2020-04-03T14:45:12.1391804-05:00",
            "keyVaultReference": "this is a secret from key vault",
            "directReference": "this came from AppConfig"
        }
    ]
    ```