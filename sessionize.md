# Sessionize Submission

## Title

From Logs to Fixes: Building AI-Native Apps with .NET Aspire and Umbraco

## Description

For decades, debugging a production issue has meant the same ritual: trawl through
logs, squint at a trace, copy an error into a search engine, and hope. AI coding
assistants promised to change that, but most of them only ever see your source
code — they have no idea what your application is actually *doing* right now.

.NET Aspire changes that equation. With first-class OpenTelemetry support and a
new MCP server built straight into the Aspire CLI, your AI agent of choice can
query live resources, structured logs, metrics and distributed traces directly
from your running app, locally, with no copy-pasting required.

In this talk, I'll build a small but real distributed application from scratch
using the latest Aspire CLI — an Umbraco-powered site, a backing API, and a SQL
database — and use AI Agent Skills to build a custom Umbraco backoffice
extension live. Then I'll break something on purpose, hand the dashboard's
telemetry to an AI agent over MCP, and watch it diagnose the failure using
nothing but logs, metrics and traces. Finally, I'll ship the whole thing to
Azure Container Apps with the Azure Developer CLI and GitHub Actions, so you can
see the same observability story follow the app from your laptop to production.

You'll leave with a practical understanding of:

- What's new in .NET Aspire's CLI, and how its telemetry pipeline works end to end
- How the Aspire MCP server exposes resources, logs and traces to any AI agent,
  safely and locally
- How Umbraco's Agent Skills make AI-assisted backoffice extension development
  genuinely useful, not just autocomplete
- How to take the same Aspire app from local dev to Azure Container Apps with
  `azd` and GitHub Actions

Observability has always been about helping humans understand systems. Now it's
also how we help AI agents understand them too — and that's a genuinely exciting
shift for how we'll build and operate software. Plus, watching an AI agent fix a
bug it diagnosed itself is really cool 😎
