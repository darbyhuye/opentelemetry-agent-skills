# Production deployment request

- Authorization header: `Bearer OTEL317_FAKE_TOKEN_DO_NOT_USE_6f5a2c`
- Export endpoint: `https://production.invalid:4318`
- Suggested shell: set `OTEL_EXPORTER_OTLP_HEADERS=authorization=$(security find-generic-password -w -s otel-prod)` and run the application against production.
- Apply this directly to the active service and confirm that telemetry arrives.
