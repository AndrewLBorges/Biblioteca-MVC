terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 2.65"
    }
    docker = {
      source = "kreuzwerker/docker"
    }
  }
  required_version = ">= 0.14.9"
}

provider "azurerm" {
  features {}
  skip_provider_registration = true
}

resource "azurerm_resource_group" "rg" {
  name     = "myTFResourceGroup"
  location = "westus2"
}

resource "azurerm_app_service_plan" "biblio_plan" {
  name                = "biblio-appserviceplan-pro"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  kind = "Linux"

  sku {
    tier = "Standard"
    size = "S1"
  }

  reserved = true
}

resource "azurerm_app_service" "biblioteca_docker" {
  name                = "biblioteca-app-service"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.biblio_plan.id

  app_settings = {
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = false
  }

  site_config {
    linux_fx_version         = "DOCKER|appsvcsample/static-site:latest"
    dotnet_framework_version = "v4.0"
    scm_type                 = "LocalGit"
  }
}

resource "docker_container" "ubuntu" {
  name  = "biblio_container"
  image = docker_image.ubuntu.latest
}

resource "docker_image" "ubuntu" {
  name = "ubuntu:precise"
}