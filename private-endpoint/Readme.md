# Overview

A quick example how to connect to Azure AppConfigurating from on-premises over a private endpoint and an SPN

# Deployment 
```bash
cd infrastructure
terrafrom apply -var "subnet_name=private-endpoints" -var "virtual_network_name=vnet001" -var "virtual_network_rg=networking_rg"
export AZURE_TENANT_ID=`terraform output tenant_id | tr -d \"`                                   
export AZURE_CLIENT_ID=`terraform output client_id | tr -d \"`                               
export AZURE_CLIENT_SECRET=`terraform output client_secret | tr -d \"`

cd src 
bash ./pubish.sh
./publish/linux/appconfig-demo -a  https://my-appconfig.azconfig.io
```