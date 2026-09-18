---
name: grafana-dashboard-management
description: Export and manage Grafana dashboards using the Grafana HTTP API and locally supplied credentials.
tier: standard
applyTo: 'grafana/**'
---

# Grafana Dashboard Management

Use this skill when exporting, importing, or updating Grafana dashboards in
this repository.

## Local access

Grafana API access requires these environment variables:

```bash
GRAFANA_URL=https://your-stack.grafana.net
GRAFANA_TOKEN=replace-with-a-read-only-service-account-token
```

- `GRAFANA_URL` is the Grafana stack or instance base URL, without a trailing
  slash. It is not `https://grafana.com`.
- `GRAFANA_TOKEN` is a Grafana service-account token. Use a token with
  `dashboards:read` permission for exports and grant write permission only
  when the task requires dashboard updates.

The ignored local `.env` file may contain these values, but neither Bash nor
Copilot CLI loads it automatically. Load it in a trusted shell before starting
Copilot CLI:

```bash
set -a
source .env
set +a
copilot
```

After changing `.env`, restart Copilot CLI from a shell that has reloaded the
file. Resume the prior session with `/resume` if needed.

Never print, log, commit, or include `GRAFANA_TOKEN` in command output,
dashboard JSON, documentation, or generated artifacts. Validate its presence
without revealing it:

```bash
printf 'URL=%s\nTOKEN_SET=%s\n' \
  "$GRAFANA_URL" \
  "$(test -n "$GRAFANA_TOKEN" && echo yes || echo no)"
```

## Exporting a dashboard

Find the dashboard UID by title or slug:

```bash
curl --fail-with-body --silent --show-error \
  --get "$GRAFANA_URL/api/search" \
  --data-urlencode "query=<dashboard-title-or-slug>" \
  -H "Authorization: Bearer $GRAFANA_TOKEN"
```

Export the dashboard by UID. Keep only the `dashboard` object because the API
response's `meta` object is instance-specific:

```bash
mkdir -p grafana/dashboards
curl --fail-with-body --silent --show-error \
  "$GRAFANA_URL/api/dashboards/uid/<dashboard-uid>" \
  -H "Authorization: Bearer $GRAFANA_TOKEN" |
  jq '.dashboard' > grafana/dashboards/<dashboard-slug>.json
```

Use a temporary file and move it into place only after `curl` and `jq`
succeed, so an authentication or API failure cannot replace a valid dashboard
with an empty or error file. Preserve the dashboard UID unless the task
explicitly requires creating a separate dashboard.
