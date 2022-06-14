terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm  = {
      source = "hashicorp/azurerm"
      version = "3.10.0"
    }
    azuread = {
      source = "hashicorp/azuread"
      version = "2.18.0"
    }
  }
}

provider "azurerm" {
  features  {}
}

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

data "azurerm_client_config" "current" {}

data "azurerm_virtual_network" "this" {
  name                = var.virtual_network_name
  resource_group_name = var.virtual_network_rg
}

data "azurerm_subnet" "this" {
  name                 = var.subnet_name
  virtual_network_name = var.virtual_network_name
  resource_group_name  = var.virtual_network_rg
}

resource "azuread_application" "this" {
  display_name = "${local.resource_name}-identity"
  owners       = [data.azurerm_client_config.current.object_id]
}

resource "azuread_service_principal" "this" {
  application_id               = azuread_application.this.application_id
  app_role_assignment_required = false
  owners                       = [data.azurerm_client_config.current.object_id]
}

resource "azuread_application_password" "this" {
  application_object_id = azuread_application.this.object_id
}

resource "azurerm_resource_group" "this" {
  name                  = "${local.resource_name}_rg"
  location              = local.location
}

resource "azurerm_private_dns_zone" "privatelink_azconfig_io" {
  name                = "privatelink.azconfig.io"
  resource_group_name = azurerm_resource_group.this.name
}

resource "azurerm_private_dns_zone_virtual_network_link" "privatelink_azconfig_io" {
  name                  = "vnet-link"
  private_dns_zone_name = azurerm_private_dns_zone.privatelink_azconfig_io.name
  resource_group_name   = azurerm_resource_group.this.name
  virtual_network_id    = data.azurerm_virtual_network.this.id
}

resource "azurerm_app_configuration" "this" {
  name                = "${local.resource_name}-config"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  sku                 = "standard"
}

resource "azurerm_app_configuration_key" "this" {
  configuration_store_id = azurerm_app_configuration.this.id
  key                    = "sample"
  value                  = "quick example test"
}

resource "azurerm_private_endpoint" "this" {
  name                      = "${local.resource_name}-config-endpoint"
  resource_group_name       = azurerm_resource_group.this.name
  location                  = azurerm_resource_group.this.location
  subnet_id                 = data.azurerm_subnet.this.id

  private_service_connection {
    name                           = "${local.resource_name}-config-endpoint"
    private_connection_resource_id = azurerm_app_configuration.this.id
    subresource_names              = [ "configurationStores" ]
    is_manual_connection           = false
  }

  private_dns_zone_group {
    name                          = azurerm_private_dns_zone.privatelink_azconfig_io.name
    private_dns_zone_ids          = [ azurerm_private_dns_zone.privatelink_azconfig_io.id ]
  }
}

resource "azurerm_role_assignment" "reader" {
  scope                = azurerm_app_configuration.this.id
  role_definition_name = "App Configuration Data Reader"
  principal_id         =  azuread_service_principal.this.object_id
}

resource "azurerm_role_assignment" "admin" {
  scope                = azurerm_app_configuration.this.id
  role_definition_name = "App Configuration Data Owner"
  principal_id         =  data.azurerm_client_config.current.object_id
}

output "client_id" {
  value = azuread_service_principal.this.application_id
}

output "client_secret" {
  value = azuread_application_password.this.value
  sensitive = true
}

output "tenant_id" {
  value = data.azurerm_client_config.current.tenant_id
}