# Overview

A working example of https://docs.microsoft.com/en-us/azure/azure-app-configuration/howto-targetingfilter-aspnet-core

# Deployment 
```bash
cd infrastructure
terrafrom apply 

cd src 
config_string=`az appconfig credential list -n ${app_name}-config -o tsv --query "[0].connectionString"`
dotnet user-secrets init
dotnet user-secrets set ConnectionStrings:AppConfig "${config_string}"
dotnet run
```