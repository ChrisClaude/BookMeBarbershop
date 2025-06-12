output "app_service_default_hostname" {
  value = azurerm_linux_web_app.app_service.default_hostname
}

output "application_insights_instrumentation_key" {
  value     = azurerm_application_insights.app_insights.instrumentation_key
  sensitive = true
}