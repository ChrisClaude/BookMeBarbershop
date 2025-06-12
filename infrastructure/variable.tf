variable "environment" {
  type    = string
  default = "dev"
  validation {
    condition     = contains(["dev", "qa", "prod"], var.environment)
    error_message = "Environment must be one of: dev, qa, prod."
  }
}

variable "location" {
  type    = string
  default = "eastus"
  validation {
    condition     = contains(["eastus", "westus", "westeurope"], var.location)
    error_message = "Location must be one of: eastus, westus, westeurope."
  }
}

resource "random_password" "sql_password" {
  length  = 16
  special = true
}

variable "sql_admin_password" {
  description = "SQL password if not generated"
  type        = string
  default     = ""
  sensitive   = true
}

locals {
  sql_password = var.sql_admin_password != "" ? var.sql_admin_password : random_password.sql_password.result
  suffix       = "${lower(var.environment)}-${lower(var.location)}"
}

variable "app_service_sku_name" {
  type        = string
  description = "SKU name for the service plan (e.g., F1, B1, S1)"
  default     = "F1"
}

variable "sql_edition" {
  default = "Basic"
}

variable "sql_service_objective" {
  default = "Basic"
}