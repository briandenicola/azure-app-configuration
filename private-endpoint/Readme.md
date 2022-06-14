# Overview

A quick example how to connect to Azure AppConfigurating from on-premises over a private endpoint and an SPN

# Deployment 
```bash
cd infrastructure
terrafrom apply -var "subnet_name=private-endpoints" -var "virtual_network_name=vnet001" -var "virtual_network_rg=networking_rg"

cd src 
export AZURE_TENANT_ID=(Obtainer from Terraform Ouput)                                   
export AZURE_CLIENT_ID=(Obtainer from Terraform Ouput)                               
export AZURE_CLIENT_SECRET=(Create via the Azure Portal)
bash ./pubish.sh
./publish/linux/appconfig-demo -a  https://my-appconfig.azconfig.io
```