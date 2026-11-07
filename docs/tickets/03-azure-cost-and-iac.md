---
title: Azure monthly cost of v1 and the Terraform + OIDC setup facts
type: research
for: epic
status: closed
blocked_by: []
---

## Question
1. What does v1's Azure footprint cost per month on a free-tier subscription, and which choice is cheapest for each piece?
   - Container Apps, consumption plan: free grant
   - Postgres Flexible Server B1ms: 12-month free offer
   - Service Bus Basic
   - Redis: self-hosted container on Container Apps vs. Azure Managed Redis smallest SKU
   - Log Analytics / Application Insights ingestion
   - Container registry: ACR Basic vs. GitHub Container Registry
2. What are the current facts for:
   - Terraform `azurerm` provider version
   - remote state in an Azure Storage account
   - GitHub Actions OIDC (federated credentials on an app registration or a user-assigned identity, required permissions)
   - deploying a new image revision to Container Apps from a workflow

Sprint 2 (CD) stories depend on this.

## Resolution
Researched 2026-11-03. Prices are pay-as-you-go list prices in USD for **West Europe**, pulled from the Azure Retail Prices API (`https://prices.azure.com/api/retail/prices`, docs: https://learn.microsoft.com/rest/api/cost-management/retail-prices/azure-retail-prices). The azure.microsoft.com pricing pages render prices client-side, so they could not be read directly. Other regions differ slightly. A month is 730 h or about 2.59M s.

### 1. Monthly cost

| Component | Option | Monthly cost | Source |
|---|---|---|---|
| Container Apps, consumption | Free grant per subscription: 180k vCPU-s, 360k GiB-s, 2M requests. After that: active $0.000034/vCPU-s, **idle $0.000004/vCPU-s**, $0.000004/GiB-s (active and idle), $0.56 per 1M requests | — | https://learn.microsoft.com/azure/container-apps/billing; Retail API (`serviceName eq 'Azure Container Apps'`) |
| API container (0.5 vCPU / 1 GiB) | `min_replicas = 0` (cold start on first hit) | ~$0 (inside the grant) | billing doc above: "scaled to zero … no resource consumption charges" |
| API container | `min_replicas = 1`, idle all month | ~$5 idle-rate, ~$22 if billed active | computed: 1.3M vCPU-s + 2.6M GiB-s, minus the grant |
| Worker (0.25 / 0.5) | KEDA Service Bus scaler, `min_replicas = 0` | ~$0 | https://learn.microsoft.com/azure/container-apps/scale-app |
| Redis as a container (0.25 / 0.5) | `min_replicas = 1`, internal TCP ingress, no volume | **~$5.6 idle-rate** (~$19.6 if billed active) | computed: 648k vCPU-s + 1.3M GiB-s, minus the grant |
| Azure Managed Redis | Balanced **B0**, the smallest SKU, non-HA | $0.019/h ≈ **$13.9** (HA costs more; multiplier unverified) | Retail API (`Azure Managed Redis - Balanced`, B0) |
| SPA | **Static Web Apps Free**: 100 GB bandwidth, 2 custom domains, no SLA | **$0** (Standard is $9/app/month) | https://azure.microsoft.com/pricing/details/app-service/static/; Retail API |
| SPA | nginx container on Container Apps, min 0 | ~$0, but adds cold starts | billing doc |
| Postgres Flexible B1ms, year 1 | Free account: 750 h B1MS + 32 GB storage + 32 GB backup per month for 12 months | **$0** | https://learn.microsoft.com/azure/postgresql/configure-maintain/how-to-deploy-on-azure-free-account |
| Postgres Flexible B1ms, after year 1 | $0.0199/h compute plus 32 GB at $0.1369/GB-month | **~$18.9** (backup storage beyond the free amount is $0.103/GB) | Retail API (B1MS; Flex Server Storage; Backup Storage LRS) |
| Service Bus | **Basic**: $0.05 per 1M operations, queues only | ~$0.05 | Retail API (`Basic Messaging Operations`). That Basic has no topics comes from the pricing page feature table and was not re-verified |
| Service Bus | Standard (needed for topics or subscriptions) | ~$10 base (13M ops included) | Retail API (`Standard Base Unit` $10/month) |
| Log Analytics / App Insights | 5 GB/month free per billing account, then $2.99/GB. Retention free for 31 days (90 days for workspace-based App Insights), then $0.13/GB-month | **$0** at idle | https://azure.microsoft.com/pricing/details/monitor/; Retail API |
| Registry | **ghcr.io**: public packages are free, and container storage and bandwidth are "currently free" | **$0** | https://docs.github.com/en/billing/concepts/product-billing/github-packages |
| Registry | ACR Basic: $0.1666/day, 10 GB included, then $0.10/GB | ~$5.0 | Retail API; https://azure.microsoft.com/pricing/details/container-registry/ |
| TF state storage account | Standard LRS, a few KB | < $0.10 (unverified, negligible) | — |

**v1 idle month** (recommended config: API, worker and SPA at zero, Redis container at min 1, ghcr, SWA Free):
- Year 1: about **$6/month**. Redis container ~$5.6, Service Bus ~$0.05, everything else $0.
- After year 1: about **$25/month**, because Postgres adds ~$18.9.
- With Azure Managed Redis B0 in place of the container: add about $8.
- With the API kept warm (`min 1`): add about $5–22.

**k6 test**: example run is 30 minutes, 5 API replicas at 0.5 vCPU / 1 GiB, 1M requests. Compute is about 4.5k vCPU-s and 9k GiB-s, roughly $0.20 at active rates even if the grant is already used up. Requests cost $0.56 only beyond the 2M free. Service Bus is about $0.15 per 1M messages (3 ops each, assumed). In practice a test costs **< $1**. The real risk is telemetry: per-request App Insights traces at ~1 KB × 1M requests ≈ 1 GB, and every GB over 5 GB/month costs $2.99. Sample or cap telemetry during tests.

**Cost traps**
- **Dedicated plan management fee, $0.10/h (~$73/month).** It is charged when the environment has any Dedicated workload profile, and also for **private endpoints and planned maintenance**, even on the Consumption plan. Use the Consumption profile only, and set no maintenance window or private endpoint. Sources: billing doc; Retail API `Dedicated Plan Management` / `Environment Private Endpoint` / `Environment Planned Maintenance Hour`.
- **min_replicas ≥ 1 is billed all month.** The idle rate applies only while a replica uses < 0.01 vCPU and < 1,000 B/s of network. Otherwise the active rate applies, which is 8.5× higher for vCPU (billing doc). A worker that polls constantly, or chatty health checks, can lose the idle rate.
- **Custom VNet.** It can add billable resources such as a load balancer and public IP (billing doc: "additional charges might apply"). A Standard static IPv4 is $0.005/h ≈ $3.65 (Retail API). External TCP ingress needs a custom VNet, but internal TCP (Redis) does not (https://learn.microsoft.com/azure/container-apps/connect-apps#transport-protocols).
- **Log Analytics.** Raising retention above 31 days, or switching to a commitment tier, adds cost. Set a daily cap.
- **Postgres after month 12.** Billing starts silently. HA, geo-redundant backup, or storage above 32 GB are billed even during year 1.
- **Azure Cache for Redis** has been closed to new customers since 2026-04-01 and retires on 2028-09-30, so it is not an option (https://learn.microsoft.com/azure/azure-cache-for-redis/cache-whats-new).
- **Redis container persistence.** Container and replica storage is ephemeral, so data is lost on every restart, revision or replica move. Only an Azure Files mount persists (https://learn.microsoft.com/azure/container-apps/storage-mounts), and running Redis AOF/RDB on SMB is slow and unverified for correctness. Treat the Redis container as a **cache only**, never as a source of truth. Also keep `max_replicas = 1`.

### 2. Terraform
- **Provider version.** `hashicorp/azurerm` latest is **5.12.0** (2026-10-29). **Major 5** began with 5.0.0 on 2026-07-28, and a `5.0-upgrade-guide` exists. Pin `~> 5.12`. Many Learn samples still show `~>4.0`, which is stale. Sources: https://registry.terraform.io/providers/hashicorp/azurerm/latest; https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/guides/5.0-upgrade-guide.
- **Container Apps resources.**
  - `azurerm_log_analytics_workspace`, `azurerm_container_app_environment` and `azurerm_container_app`.
  - The app resource takes `revision_mode` (Single or Multiple), `template.container.image`, `ingress`, `registry`, `identity` and `workload_profile_name`.
  - Sources: https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/container_app and …/container_app_environment.
- **Remote state backend `azurerm`.**
  - Set `resource_group_name`, `storage_account_name`, `container_name`, `key`, plus `use_oidc = true` (`ARM_USE_OIDC`) and `use_azuread_auth = true` (`ARM_USE_AZUREAD`).
  - Grant the role **Storage Blob Data Contributor** on the state container.
  - Locking uses native blob capabilities.
  - Source: https://developer.hashicorp.com/terraform/language/backend/azurerm.
  - OIDC alone does not switch the backend to Entra auth. Without `use_azuread_auth`, the backend falls back to fetching the account key (https://learn.microsoft.com/azure/foundry/agents/how-to/set-up-ci-cd-cli#configure-terraform-state).
- **Bootstrap problem.** The backend's storage account must exist before `terraform init`. The documented options are to create it with `az` CLI or with a separate Terraform config (https://learn.microsoft.com/azure/developer/terraform/store-state-in-azure-storage). The usual pattern is a `infra/bootstrap` root with **local state** that a human runs once with Owner rights. It creates:
  - the state RG, storage account and container
  - the deploy identity and its federated credentials
  - its role assignments
  
  Commit the bootstrap outputs (the IDs are not secrets) and keep its tiny state file out of git, or migrate it with `terraform init -migrate-state`. Set `prevent_destroy` on the state account (same doc).

### 3. GitHub Actions OIDC to Azure
- **Action version.** **`azure/login@v3`** is current (v3.1.0, 2026-09-10). v2 is maintenance-only and v1 is end-of-life. v3 adds `mask-client-id`. Source: https://github.com/Azure/login.
- **Workflow permissions.** `permissions: id-token: write` (plus `contents: read`) at workflow or job level (same source).
- **Federated credential.** Issuer `https://token.actions.githubusercontent.com`, audience `api://AzureADTokenExchange`. Subject formats:
  - branch: `repo:OWNER/REPO:ref:refs/heads/main`
  - environment: `repo:OWNER/REPO:environment:production`
  - pull request: `repo:OWNER/REPO:pull_request`
  
  A job that declares `environment:` gets the **environment** subject, not the branch one. Source: https://learn.microsoft.com/azure/app-service/deploy-github-actions#manually-set-up-a-github-actions-workflow.
- **Identity type.** Both an **Entra app registration** and a **user-assigned managed identity** are supported (https://learn.microsoft.com/azure/developer/github/connect-from-azure-openid-connect#prerequisites). A UAMI is an ordinary Azure resource: it needs no Entra directory permissions and can be built entirely in azurerm (`azurerm_user_assigned_identity` + `azurerm_federated_identity_credential`). An app registration needs Entra rights or the `azuread` provider.
- **Minimal roles.**
  - **Contributor** on the SlotDrop resource group.
  - **Storage Blob Data Contributor** on the tfstate container.
  - Add **Role Based Access Control Administrator** on the RG, scoped by condition, only if Terraform itself creates role assignments (e.g. AcrPull). This is unverified as the minimal choice; User Access Administrator is the broader alternative.
  - A deploy-only job can use a narrower role on the container apps, but "Container Apps Contributor" as a built-in role is unverified.
  - The Learn IaC sample splits a read-only identity for PR plans from a read/write identity for the `production` environment (https://learn.microsoft.com/devops/deliver/iac-github-actions#prerequisites).

### 4. Deploying a new image revision
- **`azure/container-apps-deploy-action@v1`** (the README uses v1; the latest GitHub release tag reads `v2`, dated 2023-10-17, so verify before pinning). It can build, push and deploy, or deploy an existing image via `imageToDeploy`, or apply a YAML file. It wraps `az containerapp up/update`, and you run `azure/login` first. It is opinionated and heavier. Source: https://github.com/Azure/container-apps-deploy-action.
- **`az containerapp update -n <app> -g <rg> --image ghcr.io/…:<sha>`.** An image change is a revision-scope change, so it creates a new revision. This is the simplest and most explicit option. Sources: https://learn.microsoft.com/cli/azure/containerapp#az-containerapp-update; https://learn.microsoft.com/azure/container-apps/revisions.
- **Terraform owns the tag** (`-var image_tag=$GITHUB_SHA` → `terraform apply`).
  - Pros: one source of truth and no drift.
  - Cons: every app deploy becomes a full infra apply. That is slower, needs the broad Contributor identity on every push, couples unrelated infra changes to releases, and makes a broken plan block hotfixes.
- **Hybrid.** Terraform creates the app with an initial image and sets `lifecycle { ignore_changes = [template[0].container[0].image] }`, while CI swaps images with `az containerapp update`.
  - Pros: fast, low-privilege deploys.
  - Cons: state no longer records the running image, and a replace of the app resets it to the placeholder. Document this, and re-run deploy after any replacement.
- **Registry auth.** ghcr.io is pulled with registry username and password secret; managed identity only works with ACR (https://learn.microsoft.com/azure/container-apps/containers#container-registries). Public ghcr images need no credentials at all.

### Recommended for SlotDrop
- Use **one Consumption-only Container Apps environment**: no Dedicated profile, no VNet, no private endpoint, no planned-maintenance window. That avoids the $73/month management fee.
- Run the **API and worker at `min_replicas = 0`**, with a KEDA Service Bus scaler on the worker. Accept cold starts, or set API `min 1` only around demos and k6 runs.
- Run **Redis as a single internal-TCP container** (`min = max = 1`, ~$6/month) and treat it as a disposable cache. Move to Azure Managed Redis B0 (~$14/month) only if durability is needed. Azure Cache for Redis is not an option.
- Host the **SPA on Static Web Apps Free**. Call the API cross-origin with CORS, since linked backends are Standard-only (unverified).
- Use **Postgres B1ms with 32 GB** and no HA, and set a calendar reminder for month 11 (it costs ~$19/month after that). Use **Service Bus Basic** unless topics are needed; Standard costs $10/month.
- Use **ghcr.io with public images**: $0 and no pull secret. If the images are private, use a fine-grained read-only token stored as a Container Apps secret.
- Pin **azurerm `~> 5.12`**. Use the `azurerm` backend with `use_oidc` + `use_azuread_auth`. Create a local-state `bootstrap/` root, run once by hand, for the state storage, UAMI, federated credentials and RBAC.
- CI uses **`azure/login@v3`** with a UAMI and `id-token: write`. Give it **environment-scoped** federated credentials (`production`) for apply, and a `pull_request` credential with a read-only identity for plan. Deploy images with `az containerapp update --image …:<sha>`, with Terraform ignoring `image`. Set a Log Analytics daily cap and a budget alert at about $10/month.
