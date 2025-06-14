# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.96.0"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-book-me-${local.suffix}-001"
  location = lower(var.location)
}

// create a web app
resource "azurerm_service_plan" "app_service_plan" {
  name                = "app-service-plan-book-me-${local.suffix}-001"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"

  sku_name = var.app_service_sku_name

  tags = {
    environment = var.environment
    location    = var.location
    project     = "book-me"
  }
}
resource "azurerm_linux_web_app" "app_service" {
  name                = "app-service-book-me-${local.suffix}-001"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  https_only          = true

  identity {
    type = "SystemAssigned"
  }

  site_config {
    always_on = true
    application_stack {
      docker_image     = "mcr.microsoft.com/dotnet/aspnet"
      docker_image_tag = "9.0"
    }
    health_check_path = "/healthz"
  }

  app_settings = {
    "ConnectionStrings__BookMeDb"           = "@Microsoft.KeyVault(SecretUri=https://${azurerm_key_vault.kv.name}.vault.azure.net/secrets/${azurerm_key_vault_secret.sql_admin_password.name}/)"
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.app_insights.connection_string
    "ASPNETCORE_ENVIRONMENT"                = var.environment
  }

  tags = {
    environment = var.environment
    location    = var.location
    project     = "book-me"
  }
}

resource "azurerm_key_vault" "kv" {
  name                       = "kv-bm-${var.environment}-${var.location}"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  purge_protection_enabled   = true
  soft_delete_retention_days = 7
  enable_rbac_authorization  = false
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get",
      "List",
      "Set",
    ]
  }
}

# Retrieve information about the currently authenticated Azure client/account used by Terraform
# Required for Key Vault and other AAD-aware resources
data "azurerm_client_config" "current" {}

resource "azurerm_key_vault_secret" "sql_admin_password" {
  name         = "SqlConnectionString"
  value        = "Server=${azurerm_mssql_server.sql_server.fully_qualified_domain_name};Database=${azurerm_mssql_database.sql_database.name};User ID=${azurerm_mssql_server.sql_server.administrator_login};Password=${local.sql_password};Encrypt=true;Connection Timeout=30;"
  key_vault_id = azurerm_key_vault.kv.id
}

# Grant the web app access to Key Vault
# Because we need to reference the web app's identity, we have to create the web app first and then grant it access to Key Vault (That might mean to comment out the access policy and apply, then uncomment and apply again)
resource "azurerm_key_vault_access_policy" "web_app_policy" {
  key_vault_id = azurerm_key_vault.kv.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_linux_web_app.app_service.identity[0].principal_id

  secret_permissions = [
    "Get",
    "List"
  ]

  depends_on = [
    azurerm_linux_web_app.app_service,
    azurerm_key_vault.kv
  ]
}

// create an azure sql database dtu
resource "azurerm_mssql_server" "sql_server" {
  name                         = "sql-server-book-me-${local.suffix}-001"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "sqladmin"
  administrator_login_password = local.sql_password

  tags = {
    environment = var.environment
    location    = var.location
    project     = "book-me"
  }
}

resource "azurerm_mssql_database" "sql_database" {
  name        = "sql-database-book-me-${local.suffix}-001"
  server_id   = azurerm_mssql_server.sql_server.id
  collation   = "SQL_Latin1_General_CP1_CI_AS"
  max_size_gb = 2
  sku_name    = "Basic"
  tags = {
    environment = var.environment
    location    = var.location
    project     = "book-me"
  }
}

// create an application insights resource
resource "azurerm_log_analytics_workspace" "law" {
  name                = "log-analytics-book-me-${var.environment}-${var.location}-001"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
  tags = {
    environment = var.environment
    location    = var.location
    project     = "book-me"
  }
}

resource "azurerm_application_insights" "app_insights" {
  name                = "app-insights-book-me-${local.suffix}-001"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.law.id
  tags = {
    environment = var.environment
    location    = var.location
    project     = "book-me"
  }
}
