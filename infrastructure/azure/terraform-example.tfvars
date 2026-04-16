# Azure Configuration
azure_subscription_id = "your-subscription-id-here"
resource_group_name   = "bikesherpa-logging-rg"
location              = "eastus"
prefix                = "bikesherpa-logs"

# Log Analytics
log_retention_days = 30

# Loki Configuration
loki_version       = "3.7.1"
loki_cpu           = 0.5
loki_memory        = "1Gi"
loki_data_quota_gb = 10

# Grafana Configuration
grafana_version           = "13.0.0"
grafana_cpu               = 0.5
grafana_memory            = "1Gi"
grafana_data_quota_gb     = 5
grafana_admin_user        = "admin"
grafana_admin_password    = "ChangeMe123!" # CHANGE THIS TO A STRONG PASSWORD
grafana_anonymous_enabled = "false"
grafana_anonymous_role    = "Viewer"
grafana_domain            = "localhost"

# NGINX Configuration
nginx_version        = "alpine"
cors_allowed_origins = "https://bikesherpa-backend.azurecontainerapps.io,https://yourdomain.com"

# Optional: Restrict access to specific IPs (uncomment to use)
# nginx_allowed_ips = [
#   "1.2.3.4/32",      # Your office IP
#   "10.0.0.0/24",     # Your VPN range
# ]

# Tags
tags = {
  Environment = "production"
  Project     = "BikeSherpa"
  ManagedBy   = "Terraform"
  Owner       = "DevOps Team"
}
