variable "subnet_name" {
  description   = "The subnet name for priovate endpoints"
  type          = string
  default       = "private-endpoints"
}

variable "virtual_network_name" {
  description   = "The names of the virutal network"
  type          = string
  default       = "DevSub01-Vnet-001"
}

variable "virtual_network_rg" {
  description   = "The Resource Group of the virutal network"
  type          = string
  default       = "DevSub01_Network_RG"
}