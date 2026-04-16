variable "azure_subscription_id" {
  description = "Azure Subscription ID"
  type        = string
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
  default     = "bikesherpa-logging-rg"
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "prefix" {
  description = "Prefix for resource names"
  type        = string
  default     = "bikesherpa-logs"
}

variable "log_retention_days" {
  description = "Log Analytics workspace retention in days"
  type        = number
  default     = 30
}

# Loki Configuration
variable "loki_version" {
  description = "Loki Docker image version"
  type        = string
  default     = "3.7.1"
}

variable "loki_cpu" {
  description = "CPU allocation for Loki container"
  type        = number
  default     = 0.5
}

variable "loki_memory" {
  description = "Memory allocation for Loki container"
  type        = string
  default     = "1Gi"
}

variable "loki_data_quota_gb" {
  description = "Loki data storage quota in GB"
  type        = number
  default     = 10
}

# Grafana Configuration
variable "grafana_version" {
  description = "Grafana Docker image version"
  type        = string
  default     = "13.0.0"
}

variable "grafana_cpu" {
  description = "CPU allocation for Grafana container"
  type        = number
  default     = 0.5
}

variable "grafana_memory" {
  description = "Memory allocation for Grafana container"
  type        = string
  default     = "1Gi"
}

variable "grafana_data_quota_gb" {
  description = "Grafana data storage quota in GB"
  type        = number
  default     = 5
}

variable "grafana_admin_user" {
  description = "Grafana admin username"
  type        = string
  default     = "admin"
}

variable "grafana_admin_password" {
  description = "Grafana admin password (will be stored in Azure Container App secrets)"
  type        = string
  sensitive   = true
}

variable "grafana_anonymous_enabled" {
  description = "Enable anonymous access to Grafana"
  type        = string
  default     = "false"
}

variable "grafana_anonymous_role" {
  description = "Anonymous user role in Grafana"
  type        = string
  default     = "Viewer"
}

variable "grafana_domain" {
  description = "Custom domain for Grafana (leave as localhost for default)"
  type        = string
  default     = "localhost"
}

# NGINX Configuration
variable "nginx_version" {
  description = "NGINX Docker image version"
  type        = string
  default     = "alpine"
}

variable "cors_allowed_origins" {
  description = "CORS allowed origins (use * for all or specify domain)"
  type        = string
}

# Tags
variable "tags" {
  description = "Tags to apply to all resources"
  type        = map(string)
  default = {
    Environment = "production"
    Project     = "BikeSherpa"
    ManagedBy   = "Terraform"
  }
}
