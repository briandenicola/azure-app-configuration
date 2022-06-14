terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm  = {
      source = "hashicorp/azurerm"
      version = "2.98.0"
    }
  }
}

provider "azurerm" {
  features  {}
}

data "azurerm_client_config" "current" {}

resource "random_id" "this" {
  byte_length = 2
}

resource "random_pet" "this" {
  length = 1
  separator  = ""
}

locals {
    location                    = "southcentralus"
    resource_name               = "${random_pet.this.id}-${random_id.this.dec}"
}
    
resource "azurerm_resource_group" "this" {
  name                  = "${local.resource_name}_rg"
  location              = local.location
}

resource "azurerm_app_configuration" "this" {
  name                = "${local.resource_name}-config"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
}

resource "azurerm_role_assignment" "admin" {
  scope                = azurerm_app_configuration.this.id
  role_definition_name = "App Configuration Data Owner"
  principal_id         = data.azurerm_client_config.current.object_id 
}

resource "azurerm_app_configuration_feature" "beta" {
  configuration_store_id      = azurerm_app_configuration.this.id
  name                        = "beta"
  enabled                     = true
  
  targeting_filter {
    default_rollout_percentage  = 0
    users                       = [ "beta@example.com" ]
  }
}