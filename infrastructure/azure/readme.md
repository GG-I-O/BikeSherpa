``` markdown
# BikeSherpa - Loki/Grafana Logging Stack on Azure Container Apps

This Terraform configuration deploys a production-ready Loki/Grafana logging stack to Azure Container Apps with HTTPS and CORS support for mobile applications.

## 🏗️ Architecture
```

Mobile App (HTTPS) ↓ NGINX Reverse Proxy (CORS + SSL) ↓ ├── Loki (Internal) → Azure Files Storage └── Grafana (
Internal) → Azure Files Storage```

## 📋 Prerequisites

- **Azure CLI** installed and authenticated (`az login`)
- **Terraform** >= 1.5.0 installed
- **Azure subscription** with Container Apps enabled
- **Permissions** to create resources in your subscription

## 🚀 Quick Start

### 1. Configure Variables

```

bash
Copy the example file
cp terraform.tfvars.example terraform.tfvars
Edit with your values
nano terraform.tfvars``` 

**Required changes:**
- Set your `azure_subscription_id`
- **IMPORTANT:** Set a strong `grafana_admin_password`
- Optionally customize other values

### 2. Initialize Terraform
```

bash terraform init```

### 3. Plan Deployment

```

bash terraform plan``` 

Review the resources that will be created:
- Resource Group
- Log Analytics Workspace
- Container Apps Environment
- Storage Account with File Shares
- 3 Container Apps (Loki, Grafana, NGINX)

### 4. Deploy
```

bash terraform apply```

Type `yes` when prompted.

Deployment takes approximately **5-10 minutes**.

### 5. Get Endpoints and Credentials

```

bash
Get all outputs
terraform output
Get Grafana URL
terraform output grafana_url
Get admin username
terraform output grafana_admin_user
NOTE: Password is in terraform.tfvars (sensitive, not shown in outputs)``` 

## 🔐 Grafana Access

After deployment, access Grafana:
```

bash
Get the URL
GRAFANA_URL=(terraform output -raw grafana_url) echo "Grafana URL:GRAFANA_URL"
Open in browser
open $GRAFANA_URL```

**Default Credentials:**

- **Username:** `admin` (or as configured in `grafana_admin_user`)
- **Password:** As set in `terraform.tfvars` (`grafana_admin_password`)

### Security Recommendations

1. **Use a strong password:** Minimum 12 characters with mixed case, numbers, and symbols
2. **Store password securely:** Consider using Azure Key Vault or a password manager
3. **Rotate password regularly:** Update `terraform.tfvars` and run `terraform apply`
4. **Enable MFA:** Configure in Grafana settings after first login

### Changing Admin Password

Edit `terraform.tfvars`:

```

hcl grafana_admin_password = "NewStrongPassword123!"``` 

Then apply:
```

bash terraform apply```

Grafana will restart with the new password.

## 📱 Configure Your Mobile App

Update your React Native LokiTransporter configuration:

```

typescript import { createLokiTransport } from './services/LokiTransporter';
const lokiTransporter = createLokiTransport({ host: 'https://bikesherpa-logs-nginx.YOUR-REGION.azurecontainerapps.io/loki', labels: { app: 'bikesherpa-mobile', platform: Platform.OS, environment: 'production', }, batching: { enabled: true, interval: 5000, maxBatchSize: 10, }, });``` 

Replace `YOUR-REGION` with your actual region (get from `terraform output loki_url`).

## 🔧 Configure Your .NET Backend

Update your `appsettings.json`:
```

json { "Serilog": { "WriteTo":    }, "
GrafanaLoki": "https://bikesherpa-logs-nginx.YOUR-REGION.azurecontainerapps.io/loki" }latex_unknown_tag```

## 🔒 Security Configuration

### Option 1: IP Restrictions

Edit `terraform.tfvars`:

```

hcl nginx_allowed_ips =   latex_unknown_tag``` 

Then apply changes:
```

bash terraform apply```

### Option 2: Custom CORS Origins

Edit `terraform.tfvars`:

```

hcl cors_allowed_origins = "https://yourdomain.com,https://app.yourdomain.com"``` 

## 📊 Using Grafana

### First Login

1. Navigate to the Grafana URL (from `terraform output grafana_url`)
2. Login with admin credentials
3. Loki datasource is pre-configured

### View Logs

1. Go to **Explore** (compass icon on the left)
2. Select **Loki** datasource
3. Use LogQL queries:
   ```logql
   {app="bikesherpa-backend"}
   {app="bikesherpa-mobile", platform="ios"}
   {level="error"}
   ```

### Create Dashboards

1. Go to **Dashboards** → **New** → **New Dashboard**
2. Add panels with Loki queries
3. Save dashboard (automatically persisted to Azure Files)

## 🧪 Test the Deployment

### Test Health Endpoint

```

bash NGINX_URL=(terraform output -raw nginx_url) curlNGINX_URL/health``` 

Expected output: `healthy`

### Test Loki Push Endpoint

```bash
LOKI_ENDPOINT=$(terraform output -raw loki_push_endpoint)

curl -X POST $LOKI_ENDPOINT \
  -H "Content-Type: application/json" \
  -d '{
    "streams": [
      {
        "stream": {
          "app": "test",
          "level": "info"
        },
        "values": [
          ["'$(date +%s%N)'", "Test log message from curl"]
        ]
      }
    ]
  }'
```

```
