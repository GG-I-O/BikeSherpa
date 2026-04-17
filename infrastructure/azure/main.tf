terraform {
  required_version = ">= 1.5.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.azure_subscription_id
}

# Resource Group
resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location

  tags = var.tags
}

# Log Analytics Workspace for Container Apps
resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.prefix}-law"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = var.log_retention_days

  tags = var.tags
}

# Container Apps Environment
resource "azurerm_container_app_environment" "env" {
  name                       = "${var.prefix}-env"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id

  tags = var.tags
}

# Storage Account for Loki data persistence and NGINX config
resource "azurerm_storage_account" "storage_account" {
  name                     = "${replace(var.prefix, "-", "")}data"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"

  tags = var.tags
}

# Azure File Share for Loki data
resource "azurerm_storage_share" "loki_data" {
  name               = "loki-data"
  storage_account_id = azurerm_storage_account.storage_account.id
  quota              = var.loki_data_quota_gb
}

# Azure File Share for Grafana data
resource "azurerm_storage_share" "grafana_data" {
  name               = "grafana-data"
  storage_account_id = azurerm_storage_account.storage_account.id
  quota              = var.grafana_data_quota_gb
}

# Azure File Share for NGINX configuration
resource "azurerm_storage_share" "nginx_config" {
  name               = "nginx-config"
  storage_account_id = azurerm_storage_account.storage_account.id
  quota              = 1
}


# Storage mount for Loki data
resource "azurerm_container_app_environment_storage" "loki" {
  name                         = "loki-storage"
  container_app_environment_id = azurerm_container_app_environment.env.id
  account_name                 = azurerm_storage_account.storage_account.name
  share_name                   = azurerm_storage_share.loki_data.name
  access_key                   = azurerm_storage_account.storage_account.primary_access_key
  access_mode                  = "ReadWrite"
}

# Storage mount for Grafana data
resource "azurerm_container_app_environment_storage" "grafana" {
  name                         = "grafana-storage"
  container_app_environment_id = azurerm_container_app_environment.env.id
  account_name                 = azurerm_storage_account.storage_account.name
  share_name                   = azurerm_storage_share.grafana_data.name
  access_key                   = azurerm_storage_account.storage_account.primary_access_key
  access_mode                  = "ReadWrite"
}

# Loki Container App (Internal Only)
resource "azurerm_container_app" "loki" {
  name                         = "${var.prefix}-loki"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Single"

  template {
    container {
      name   = "loki"
      image  = "grafana/loki:${var.loki_version}"
      cpu    = var.loki_cpu
      memory = var.loki_memory

      volume_mounts {
        name = "loki-data"
        path = "/loki"
      }
    }

    volume {
      name         = "loki-data"
      storage_type = "AzureFile"
      storage_name = azurerm_container_app_environment_storage.loki.name
    }

    min_replicas = 1
    max_replicas = 1
  }

  # Internal ingress only - accessed via NGINX
  ingress {
    external_enabled = true
    target_port      = 3100
    transport        = "http"

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
    
    cors {
      allowed_origins = var.cors_allowed_origins
      allowed_methods = ["GET","OPTIONS", "POST","PUT","DELETE"]
      allowed_headers = ["DNT","User-Agent","X-Requested-With","If-Modified-Since","Cache-Control","Content-Type","Range","Authorization"]
    }
  }

  tags = var.tags
}

resource "azurerm_container_app" "grafana" {
  name                         = "${var.prefix}-grafana"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.rg.name
  revision_mode                = "Single"

  # Secret for Grafana admin password
  secret {
    name  = "grafana-admin-password"
    value = var.grafana_admin_password
  }

  template {
    container {
      name   = "grafana"
      image  = "grafana/grafana:${var.grafana_version}"
      cpu    = var.grafana_cpu
      memory = var.grafana_memory

      # Mount Grafana data volume
      volume_mounts {
        name = "grafana-data"
        path = "/var/lib/grafana"
      }

      env {
        name  = "GF_PATHS_PROVISIONING"
        value = "/etc/grafana/provisioning"
      }

      env {
        name  = "GF_PATHS_DATA"
        value = "/var/lib/grafana"
      }

      # Admin user configuration
      env {
        name  = "GF_SECURITY_ADMIN_USER"
        value = var.grafana_admin_user
      }

      env {
        name        = "GF_SECURITY_ADMIN_PASSWORD"
        secret_name = "grafana-admin-password"
      }

      # Disable admin user password change requirement
      env {
        name  = "GF_SECURITY_ADMIN_PASSWORD_CHANGE"
        value = "false"
      }

      env {
        name  = "GF_AUTH_ANONYMOUS_ENABLED"
        value = var.grafana_anonymous_enabled
      }

      env {
        name  = "GF_AUTH_ANONYMOUS_ORG_ROLE"
        value = var.grafana_anonymous_role
      }

      env {
        name  = "GF_FEATURE_TOGGLES_ENABLE"
        value = "alertingSimplifiedRouting,alertingQueryAndExpressionsStepMode"
      }

      env {
        name  = "GF_SERVER_ROOT_URL"
        value = "https://${var.prefix}-grafana.${var.location}.azurecontainerapps.io"
      }

      env {
        name  = "GF_SERVER_SERVE_FROM_SUB_PATH"
        value = "true"
      }

      env {
        name  = "GF_SERVER_DOMAIN"
        value = var.grafana_domain
      }

      # Increase startup timeout
      env {
        name  = "GF_SERVER_HTTP_PORT"
        value = "3000"
      }

      # Disable usage stats to speed up startup
      env {
        name  = "GF_ANALYTICS_REPORTING_ENABLED"
        value = "false"
      }

      env {
        name  = "GF_ANALYTICS_CHECK_FOR_UPDATES"
        value = "false"
      }
    
      env {
        name  = "LOKI_URL"
        value = "https://${azurerm_container_app.loki.ingress[0].fqdn}"
      }

      # Simplified startup command - provision Loki datasource
      command = ["/bin/sh"]
      args = [
        "-c",
        <<-EOT
          echo "Starting Grafana configuration..."
          
          # Create provisioning directory
          mkdir -p /etc/grafana/provisioning/datasources
          
          # Provision Loki datasource
          cat > /etc/grafana/provisioning/datasources/loki.yaml <<EOF
apiVersion: 1
datasources:
  - name: Loki
    type: loki
    access: proxy
    orgId: 1
    url: "https://${azurerm_container_app.loki.ingress[0].fqdn}"
    basicAuth: false
    isDefault: true
    version: 1
    editable: false
    jsonData:
      timeout: 60
      maxLines: 1000
EOF
          
          echo "Loki datasource configured"
          echo "Starting Grafana server..."
          
          # Start Grafana
          exec /run.sh
        EOT
      ]
    }

    # Use EmptyDir for now to avoid Azure Files permission issues
    # Change to AzureFile once working
    volume {
      name         = "grafana-data"
      storage_type = "EmptyDir"
    }

    min_replicas = 1
    max_replicas = 1
  }

  # Internal ingress only - accessed via NGINX
  ingress {
    external_enabled = true
    target_port      = 3000
    transport        = "http"

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }

  tags = var.tags

  depends_on = [azurerm_container_app.loki]
}


