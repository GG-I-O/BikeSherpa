output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.rg.name
}

output "container_app_environment_id" {
  description = "Container App Environment ID"
  value       = azurerm_container_app_environment.env.id
}

output "container_app_environment_domain" {
  description = "Container App Environment default domain"
  value       = azurerm_container_app_environment.env.default_domain
}

output "grafana_admin_user" {
  description = "Grafana admin username"
  value       = var.grafana_admin_user
}

output "grafana_login_info" {
  description = "Grafana login information"
  value       = "Login to Grafana at ${azurerm_container_app.grafana.latest_revision_fqdn} with username: ${var.grafana_admin_user}"
}

output "storage_account_name" {
  description = "Storage account name for persistent data"
  value       = azurerm_storage_account.storage_account.name
}

output "loki_storage_share" {
  description = "Loki data storage share name"
  value       = azurerm_storage_share.loki_data.name
}

output "grafana_storage_share" {
  description = "Grafana data storage share name"
  value       = azurerm_storage_share.grafana_data.name
}

output "log_analytics_workspace_id" {
  description = "Log Analytics Workspace ID"
  value       = azurerm_log_analytics_workspace.law.id
}

# Useful for debugging
output "loki_internal_fqdn" {
  description = "Loki internal FQDN (for reference)"
  value       = "${azurerm_container_app.loki.name}.internal.${azurerm_container_app_environment.env.default_domain}"
}

output "grafana_internal_fqdn" {
  description = "Grafana internal FQDN (for reference)"
  value       = "${azurerm_container_app.grafana.name}.internal.${azurerm_container_app_environment.env.default_domain}"
}
