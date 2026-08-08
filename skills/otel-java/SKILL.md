---
name: otel-java
description: OpenTelemetry in Java — Javaagent zero-code instrumentation, Spring Boot Starter, manual autoconfigure SDK, declarative YAML configuration, BOM dependency management, sensitive-data capture and redaction (url.query, headers, request parameters, SQL sanitization). Use when adding, reviewing, or configuring OpenTelemetry in a Java service. Triggers on "setup otel in java", "java telemetry", "javaagent", "Spring Boot otel", "GlobalOpenTelemetry", "AutoConfiguredOpenTelemetrySdk", "TracerProvider java", "url.query redaction", "capture request headers", or any Java-related OTel question.
---

# OpenTelemetry in Java

Entry point for OpenTelemetry mechanics in Java services. Load a reference below based on
the task; each reference is self-contained.

## References

| File | Use when |
|---|---|
| [`references/declarative-setup.md`](references/declarative-setup.md) | Choosing a distribution; pinned Javaagent activation; Spring Boot Starter boundaries; manual autoconfigure SDK/BOM dependencies and startup; declarative YAML; validation levels. |
| [`references/sensitive-data-capture.md`](references/sensitive-data-capture.md) | Reviewing or changing HTTP/JDBC capture, query redaction, servlet/header capture, SQL sanitization, or unsafe requests for raw sensitive values. |

## Safety gate

Before acting on a capture/redaction request, load `references/sensitive-data-capture.md`. If it
asks for raw authorization/cookie headers, servlet parameters, JDBC parameter values, or disabled
SQL sanitization, refuse each unsafe action and **do not create or edit a configuration, script, or
"production-ready" plan that enables any of them**, even as an annotated example. Treat a mixed
safe/unsafe request as blocked rather than emitting the unsafe subset. Offer allowlisted,
data-minimized capture or Collector-side deletion/redaction, and state any selected-release
limitation separately.

## Sources of Truth

For setup/YAML facts, use the selected-release fetch table in
[`references/declarative-setup.md`](references/declarative-setup.md) and the upstream sources in
the `otel-declarative-config` skill. For capture/redaction facts, use the source table in
[`references/sensitive-data-capture.md`](references/sensitive-data-capture.md). For resolved
per-instrumentation telemetry, use the Explorer flow below.

## What telemetry does an instrumentation emit?

For *"what does the agent produce for library X"* — which spans, attributes, metrics, or config
knobs — use the [OpenTelemetry Ecosystem Explorer](https://explorer.opentelemetry.io/), which fully
maps the Java agent and exposes an agent-friendly surface (Markdown indexes and resolved JSON, no
scraping). Do **not** answer from model memory.

Navigation:

1. `WebFetch https://explorer.opentelemetry.io/agent/javaagent/index.md` — table mapping display
   name → `id` → the instrumentation's JSON data URL (the URL embeds the content `<hash>`).
2. `WebFetch` that JSON URL — one self-contained record (a few KB): resolved `configurations`,
   `telemetry` (spans with `span_kind` + typed attributes, and `metrics`),
   `javaagent_target_versions`, `semantic_conventions`, and `scope`.

Version-specific or *"what changed between releases"*:
`https://explorer.opentelemetry.io/agent/javaagent/versions.md` lists versions and marks the latest;
`https://explorer.opentelemetry.io/data/javaagent/versions/<version>-index.json` gives the
`id`→`hash` map for a version — a differing hash for the same `id` across two versions means that
instrumentation changed. Schema:
`https://explorer.opentelemetry.io/schemas/javaagent-instrumentation.schema.json`; use
`/llms.txt` for the agent-oriented index and `/llms-full.txt` for the full documentation.

Prefer this Explorer data over the raw `ecosystem-registry` YAML on GitHub: the Explorer applies
upstream metadata corrections that the raw registry does not.

## Cross-References

- Schema-level facts: `otel-declarative-config` skill (language-agnostic YAML schema sources).
- SDK version selection across languages: `otel-sdk-versions` skill.
- Semantic conventions lookup: `otel-semantic-conventions` skill.
