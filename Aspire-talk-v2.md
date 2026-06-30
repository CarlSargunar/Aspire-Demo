# New Talk Plan: AI-Assisted Umbraco + .NET Aspire 13 + Azure Container Apps

> Status: **Planning document only** — no code has been written yet. This is the
> design/research artifact to guide implementation in a later session.

## 1. Goal / Story Arc

Build a brand-new, purpose-built demo (not a refactor of the existing repo folders)
that tells this story for a conference talk:

1. **Local dev**: Spin up a distributed app with the **Aspire CLI v13** — an Umbraco
   site, a backing API, SQL Server, and supporting services — fully orchestrated,
   with OpenTelemetry logs/metrics/traces flowing into the Aspire dashboard.
2. **AI-assisted extension building**: Use **Umbraco Backoffice Extension Agent
   Skills** to build a custom backoffice dashboard/property editor live, with an AI
   coding agent (tool-agnostic, via MCP/Agent Skills) doing the heavy lifting.
3. **AI-assisted operations**: Connect an AI agent to the **Aspire MCP server** so
   it can inspect resources, logs, structured logs, and distributed traces of the
   *running* local app. Script a deliberate bug/failure and have the agent diagnose
   (and ideally propose/apply a fix for) it live, using only MCP tool calls — no
   copy-pasting logs into chat.
4. **Ship it**: Deploy the same app to **Azure Container Apps** using **azd** (Azure
   Developer CLI), with CI/CD via **GitHub Actions** (`azd pipeline config --provider
   github`), demonstrating the same telemetry story now flowing to Azure Monitor /
   Application Insights in "production".

The throughline: *Aspire gives you first-class telemetry from local dev to
production, and that telemetry is what makes AI agents genuinely useful for
day-to-day debugging — not just code completion.*

## 2. Key Technology Versions & Facts (researched, verify again closer to build time)

- **Aspire 13.x** (current as of mid-2026; 13.2 is the latest minor at research
  time) — requires **.NET 10 SDK or later** to author/update C# AppHost projects.
  Apps it orchestrates can still target .NET 8+.
- Install/update CLI:
  - `curl -sSL https://aspire.dev/install.sh | bash` (macOS/Linux)
  - `irm https://aspire.dev/install.ps1 | iex` (Windows)
  - Upgrade an existing apphost with `aspire update`.
- Aspire 13 CLI highlights to feature in the talk:
  - `aspire new` / `aspire init` — language-aware scaffolding (C#, TypeScript,
    Python, mixed AppHosts).
  - `aspire run --detach` (`aspire start`), `aspire ps`, `aspire stop` — background
    apphost management, good for live demos (keep terminal free).
  - `aspire resource <resource> <command>` — start/stop/restart/rebuild from CLI.
  - `aspire describe` — CLI access to the same resource detail as the dashboard.
  - `aspire doctor` — environment diagnostics (also exposed as an MCP tool).
  - `--isolated` flag — randomized ports + isolated user secrets; useful for
    running the "broken" demo instance alongside a known-good one.
  - `aspire agent init` — scaffolds MCP config files for detected AI clients
    (`.vscode/mcp.json`, `.mcp.json`, `~/.copilot/mcp-config.json`,
    `opencode.jsonc`, etc.).
  - `aspire agent mcp` — starts the MCP server (STDIO transport, local
    child-process only, no network ports, no auth needed).
- **Aspire MCP server tool catalog** (what the agent can call):
  `list_resources`, `list_console_logs`, `list_structured_logs`, `list_traces`,
  `list_trace_structured_logs`, `execute_resource_command`, `list_apphosts`,
  `select_apphost`, `list_integrations`, `get_integration_docs`, `list_docs`,
  `search_docs`, `get_doc`, `doctor`, `refresh_tools`.
  - Does **not** expose source code, env var/secret values, raw network traffic,
    or host access beyond the AppHost process — good talking point on safety.
  - Resources can be opted out via `.ExcludeFromMcp()` on the resource builder —
    useful if part of the demo should be "invisible" to the agent.
- **Umbraco AI / Agent Skills**:
  - `Umbraco.AI` core package + provider package (e.g. `Umbraco.AI.OpenAI`) enables
    **Umbraco Copilot**, a backoffice sidebar AI assistant (Content/Media
    sections), with human-in-the-loop approval for write actions.
  - Separately, Umbraco published **Backoffice Extension Agent Skills** (Anthropic
    Agent Skills format) — 66+ skills covering backoffice extension types
    (dashboards, sections, property editors, etc.), with a root routing skill
    `umbraco-backoffice` that progressively loads only what's relevant to the
    current task. These are consumed by the *developer's* coding agent (not by
    Umbraco Copilot), so they're a natural fit for the "build a backoffice
    extension live with AI" segment.
  - Confirm latest supported Umbraco CMS version compatible with Agent Skills
    (skills reference versions 8–17; target latest stable LTS, expected Umbraco 15
    or later at build time — **re-verify exact version before implementation**).
- **azd + Aspire + ACA**:
  - `azd init` in the AppHost directory, `azd auth login`, `azd up` to provision +
    deploy.
  - `azd pipeline config --provider github` scaffolds a GitHub Actions workflow
    using federated credentials (OIDC) — no long-lived secrets.
  - Aspire's `aspire publish` / `aspire deploy` split: `publish` emits
    parameterized deployment assets (Bicep/manifests), `deploy` injects
    environment values/secrets at deploy time — keeps secrets out of source
    control. Good fit with GitHub Actions environments/secrets.
  - Confirm current `azd` minimum version and whether Aspire 13's manifest format
    needs a specific `azd` release before implementation.

## 3. Proposed Application Shape (new, minimal, purpose-built)

Keep the app small enough to build/explain in a single talk, but rich enough to
have a believable bug + meaningful telemetry:

- **UmbracoSite** — Umbraco CMS, hosts the custom AI-built backoffice extension.
- **CatalogApi** (or similar small ASP.NET Core minimal API) — backing service the
  Umbraco site/extension calls, talking to a database. This is where we inject the
  scripted failure (e.g., a bad connection string / N+1 query / unhandled
  exception under specific input) for the agent-diagnosis segment.
- **SQL Server** (Aspire `AddSqlServer` resource, persistent lifetime) — database
  for CatalogApi (and Umbraco if using SQL Server instead of SQLite — recommend
  SQL Server for ACA, since SQLite needs persistent volume/Azure Files wiring on
  ACA, which is extra complexity worth flagging as an explicit open question).
- **ServiceDefaults** — shared telemetry/health-check/service-discovery project
  (standard Aspire pattern), referenced by both UmbracoSite and CatalogApi.
- **AppHost** — orchestrates all of the above, the thing that gets `azd up`'d.

This deliberately drops RabbitMQ/MessageProcessor/AnalyticsApp from the old repo
to keep the new build focused — open question below on whether any async
messaging piece is wanted to showcase trace propagation across a queue.

## 4. Demo Script Outline (for the talk)

1. `aspire new` → scaffold AppHost + ServiceDefaults.
2. Add Umbraco + CatalogApi + SQL Server resources to AppHost; wire references.
3. `aspire run` (or `aspire start` detached) → show dashboard: resources, logs,
   traces, metrics flowing.
4. Use Umbraco Backoffice Extension Skills with a coding agent to scaffold a
   custom dashboard/property editor in the backoffice — show the agent
   discovering and using the `umbraco-backoffice` routing skill and sub-skills.
5. Run `aspire agent init` to wire MCP config; start `aspire agent mcp`.
6. Trigger the scripted bug (e.g. via the new backoffice extension calling
   CatalogApi). Ask the AI agent (tool-agnostic MCP client) to investigate using
   only `list_resources` / `list_structured_logs` / `list_traces` /
   `list_trace_structured_logs` — narrate the tool calls so the audience sees the
   agent "doing the work," not magic.
   - Use `--isolated` or a second apphost instance to keep a "known good" baseline
     running side-by-side if useful for comparison.
7. Agent identifies root cause; apply the fix (manually or agent-assisted),
   `aspire resource <name> restart`, confirm telemetry is healthy again.
8. `azd init` / `azd up` → deploy to Azure Container Apps.
9. `azd pipeline config --provider github` → show generated GitHub Actions
   workflow; push a change, watch CI/CD deploy.
10. Show production telemetry (Azure Monitor/App Insights/Container Apps
    environment logs) to close the loop — same OpenTelemetry data, different
    destination.

## 5. Open Questions / Decisions Still Needed Before Implementation

- [ ] **Umbraco data store on ACA**: SQL Server (recommended, simplest on ACA) vs
  SQLite (needs persistent storage wiring — more complex, maybe not worth the
  talk time)?
- [ ] **Exact Umbraco CMS version** to target — confirm latest version with full
  Agent Skills + Umbraco.AI support at build time.
- [ ] **Specific backoffice extension to build** — pick a concrete, demo-friendly
  example (e.g., "content health dashboard," "bulk image alt-text generator
  using AI," "custom property editor for a product SKU picker calling
  CatalogApi"). Needs a concrete pick before scripting the demo.
- [ ] **Nature of the scripted bug** — decide exact failure mode (bad connection
  string, slow/N+1 query, unhandled exception, timeout to a dependency) and
  confirm it's clearly visible in traces/structured logs without too much
  red-herring noise.
- [ ] **Messaging/async component** — confirm whether to include a
  RabbitMQ/Service Bus + worker piece to show trace propagation across an async
  boundary, or keep purely synchronous HTTP for simplicity.
- [ ] **azd vs Aspire's own `aspire publish`/`aspire deploy`** — confirm whether
  azd wraps Aspire's manifest output directly in the current version, and pin
  exact `azd` CLI version required for compatibility with Aspire 13 manifests.
- [ ] **GitHub Actions auth model** — confirm OIDC/federated credential setup
  steps to script in the talk (avoid storing Azure secrets in GitHub).
- [ ] **Repo placement** — confirmed: this plan lives at
  `/Aspire-talk-v2.md`; actual new app code location (new top-level folder vs new
  repo) still to be decided when implementation starts.

## 6. References

- Aspire MCP server: https://aspire.dev/get-started/aspire-mcp-server/
- What's new in Aspire 13.2: https://aspire.dev/whats-new/aspire-13-2/
- What's new in Aspire 13: https://aspire.dev/whats-new/aspire-13/
- Aspire prerequisites (.NET 10 SDK requirement): https://aspire.dev/get-started/prerequisites/
- Umbraco Agent-Ready CMS / Backoffice Extension Skills:
  https://umbraco.com/blog/the-agent-ready-cms-introducing-agent-skills-umbraco-backoffice-extension/
- Umbraco Copilot docs: https://docs.umbraco.com/ai-in-umbraco/add-ons/agent-copilot/copilot
- Deploy Aspire to ACA via azd + GitHub Actions:
  https://learn.microsoft.com/en-sg/dotnet/aspire/deployment/azure/aca-deployment-github-actions
- Aspire deployment/azd in-depth: https://github.com/dotnet/docs-aspire/blob/main/docs/deployment/azd/aca-deployment-azd-in-depth.md
