# Java SDK Setup with Declarative Configuration

Configure the OpenTelemetry SDK in Java via declarative YAML configuration. Three setup
paths exist. The Javaagent and manual autoconfigure both read an external file via
the `otel.config.file` system property or `OTEL_CONFIG_FILE` environment variable. The Spring Boot
Starter is different: YAML embeds the declarative config under the `otel:` key in
`application.yaml`, while `application.properties` uses equivalent `otel.*` properties. Both opt
in via `otel.file_format` and do not read an external `otel.config.file`. If a request mandates
both Starter and an external file, do not emit a production-ready artifact; explain the inline
path and require authorization before switching to Javaagent or manual autoconfigure.

For the YAML configuration schema, load the `otel-declarative-config` skill.

## Sources of Truth

For YAML schema details, fetch the upstream sources listed in the `otel-declarative-config`
skill. For Java-specific facts:

| Fact | Fetch |
|---|---|
| Latest BOM (`opentelemetry-bom`) | `gh api repos/open-telemetry/opentelemetry-java/releases/latest -q '.tag_name'` |
| Alpha BOM that manages declarative config | `WebFetch https://repo1.maven.org/maven2/io/opentelemetry/opentelemetry-bom-alpha/<selected-sdk-version>-alpha/opentelemetry-bom-alpha-<selected-sdk-version>-alpha.pom` |
| Published declarative-config versions | `WebFetch https://repo1.maven.org/maven2/io/opentelemetry/opentelemetry-sdk-extension-declarative-config/maven-metadata.xml` |
| Latest Javaagent | `gh api repos/open-telemetry/opentelemetry-java-instrumentation/releases/latest -q '.tag_name'` |
| Fully manual SDK / `SdkTracerProvider` ownership | `WebFetch https://opentelemetry.io/docs/languages/java/sdk/` and tag-matched API/source for the selected SDK release |
| SDK declarative-config accepted and preferred `file_format` for a selected BOM tag | `WebFetch https://raw.githubusercontent.com/open-telemetry/opentelemetry-java/<selected-sdk-tag>/sdk-extensions/declarative-config/src/main/java/io/opentelemetry/sdk/autoconfigure/declarativeconfig/OpenTelemetryConfigurationFactory.java` |
| Javaagent declarative-config docs (current activation flag, supported `file_format`) | `WebFetch https://opentelemetry.io/docs/zero-code/java/agent/declarative-configuration/` |
| Javaagent declarative-config smoke fixture (parser truth for selected agent tag) | `WebFetch https://raw.githubusercontent.com/open-telemetry/opentelemetry-java-instrumentation/<selected-agent-tag>/smoke-tests/src/test/resources/declarative-config.yaml` |
| Javaagent CHANGELOG (when each schema rc landed) | `WebFetch https://raw.githubusercontent.com/open-telemetry/opentelemetry-java-instrumentation/<selected-agent-tag>/CHANGELOG.md` |
| Spring Boot Starter declarative-config fixture (selected starter tag) | `WebFetch https://raw.githubusercontent.com/open-telemetry/opentelemetry-java-instrumentation/<selected-agent-tag>/smoke-tests-otel-starter/spring-boot-2/src/testDeclarativeConfig/resources/application.yaml` |
| Spring Boot starter docs | `WebFetch https://opentelemetry.io/docs/zero-code/java/spring-boot-starter/` |

## Javaagent Download

The Javaagent JAR has no compile dependencies for auto-instrumentation. Select a released tag
first, then use its versioned asset URL; do not use `releases/latest` in a reproducible launcher:

```bash
OTEL_JAVAAGENT_VERSION=2.30.0 # example; replace only after checking the selected release
curl --fail --location --output opentelemetry-javaagent.jar \
  "https://github.com/open-telemetry/opentelemetry-java-instrumentation/releases/download/v${OTEL_JAVAAGENT_VERSION}/opentelemetry-javaagent.jar"
curl --fail --location --output opentelemetry-javaagent.jar.asc \
  "https://github.com/open-telemetry/opentelemetry-java-instrumentation/releases/download/v${OTEL_JAVAAGENT_VERSION}/opentelemetry-javaagent.jar.asc"
```

Verify the detached signature with a trusted OpenTelemetry release-signing key already provisioned
by the user or CI before running the JAR. Do not import a key from the same download channel and
call that verification. Record the selected tag and verification result in the launcher/build
provenance; if no trusted key is available, say signature verification was not performed.

## Activation

```bash
java -javaagent:opentelemetry-javaagent.jar \
     -Dotel.config.file=configs/otel.yaml \
     -jar myservice.jar
```

Declarative config has been supported since Javaagent 2.9.0; the property is now the stable
`otel.config.file` (the experimental `otel.experimental.config.file` alias was removed in the
SDK 1.63.0 bundled with Javaagent 2.29.0). Newer agent versions track newer schema versions.
Confirm both the accepted range and preferred `file_format` from the tag-matched parser, then use
the preferred value from that release's fixture to avoid compatibility warnings for experimental
properties. As of 2026-07-29, SDK BOM 1.64.0 and Javaagent/Spring Boot Starter 2.30.0 use SDK
1.64.0: the parser accepts `0.4` and `1.*`, prefers `"1.1"`, and both released instrumentation
fixtures use `1.1`. Do not infer this from `main` or the generic language support matrix alone.

When `otel.config.file` / `OTEL_CONFIG_FILE` is set, all other SDK autoconfigure properties are
ignored except agent-only properties (see Key API Facts).

The same property and environment-variable forms work for the autoconfigure SDK extension without
the Javaagent. Flat instrumentation properties are not an overlay when declarative file mode is
active.

## YAML Config

For the canonical structure, fetch `examples/otel-sdk-config.yaml` (see the
`otel-declarative-config` skill's Sources of Truth). For the correct `file_format` string,
use the selected Javaagent parser/docs/fixtures. The generic language support matrix is
coverage metadata and may not be the exact YAML literal accepted by the Javaagent.

```yaml
# file_format: use the preferred literal for the selected Javaagent version
resource:
  attributes:
    - name: service.name
      value: "${SERVICE_NAME:-myservice}"
    - name: deployment.environment.name
      value: "${DEPLOY_ENV:-development}"

# Tracer/meter/logger provider blocks: structure per the canonical example.
# Java-specific quirk: all duration values must be in milliseconds (e.g., 5000, not "5s").
```

## Manual autoconfigure SDK

Declarative config is an alpha artifact. Import the matching alpha BOM, which also imports the
stable BOM, and omit versions from its managed dependencies. For SDK `1.64.0`:

```xml
<dependencyManagement>
  <dependencies>
    <dependency>
      <groupId>io.opentelemetry</groupId>
      <artifactId>opentelemetry-bom-alpha</artifactId>
      <version>1.64.0-alpha</version>
      <type>pom</type>
      <scope>import</scope>
    </dependency>
  </dependencies>
</dependencyManagement>
<dependencies>
  <dependency>
    <groupId>io.opentelemetry</groupId>
    <artifactId>opentelemetry-api</artifactId>
  </dependency>
  <dependency>
    <groupId>io.opentelemetry</groupId>
    <artifactId>opentelemetry-sdk-extension-autoconfigure</artifactId>
  </dependency>
  <dependency>
    <groupId>io.opentelemetry</groupId>
    <artifactId>opentelemetry-sdk-extension-declarative-config</artifactId>
  </dependency>
</dependencies>
```

Initialize and publish the SDK before framework startup, so application instrumentation sees the
global instance:

```java
import io.opentelemetry.sdk.autoconfigure.AutoConfiguredOpenTelemetrySdk;

public static void main(String[] args) {
  AutoConfiguredOpenTelemetrySdk.builder().setResultAsGlobal().build();
  SpringApplication.run(MyApplication.class, args);
}
```

This reads `-Dotel.config.file` or `OTEL_CONFIG_FILE`. Autoconfigure registers its own JVM shutdown
hook; do not add `sdk.close()` unless the application deliberately owns a different lifecycle.

## Adding API instrumentation

The Javaagent registers `GlobalOpenTelemetry` automatically. Use it to get tracers/meters:

```java
import io.opentelemetry.api.GlobalOpenTelemetry;
import io.opentelemetry.api.trace.Tracer;
import io.opentelemetry.api.metrics.Meter;

Tracer tracer = GlobalOpenTelemetry.getTracer("mycompany.com/myservice");
Meter meter = GlobalOpenTelemetry.getMeter("mycompany.com/myservice");
```

For an explicitly requested `SdkTracerProvider`, use the fully manual SDK path instead of
autoconfigure. Fetch the official SDK guide and tag-matched API for the selected release, choose
and attach the required span processors/exporters, set the provider on one `OpenTelemetrySdk`, and
register that SDK globally at most once. The application owns this lifecycle and must close it
exactly once. Do not install a second provider alongside the Javaagent, Starter, or autoconfigure,
and do not imply declarative YAML configures an independently constructed provider.

## Key API Facts

- **Shutdown hook**: The Javaagent and autoconfigure both register a JVM shutdown hook automatically — no manual `sdk.close()` needed.
- **Agent-only properties**: `otel.javaagent.extensions`, `otel.javaagent.enabled`, and
  `otel.javaagent.debug` cannot be set via declarative config. Set them as system properties or
  their corresponding environment variables instead.
- **Released 2.30.0 selectors**: Javaagent and Starter declarative config can select semantic
  conventions per `db`, `code`, `rpc`, or `messaging` domain under
  `instrumentation/development.general.<domain>.semconv` with `version`, `experimental`, and
  `dual_emit`. `service.peer` is still flag-only in this release. Check the schema for supported
  value combinations; unsupported combinations fall back rather than forcing the requested mode.
- **Starter thread details**: Starter 2.30.0 can add experimental `thread.id` and `thread.name`
  to spans with `distribution.spring_starter.thread_details_enabled: true`. This path is
  Starter-only; the Javaagent uses the separate
  `distribution.javaagent.thread_details_enabled` path.

## Validation boundaries

Report these separately; never turn one into a claim about the next:

1. **Static structure** — inspect the selected release's schema/canonical example and the generated
   Maven, Java, launcher, or YAML files.
2. **Released parser evidence** — compare `file_format` and Java-specific paths with the tag-matched
   parser plus Javaagent/Starter smoke fixture.
3. **Runtime validation** — start the application with the selected distribution, exercise a real
   request, and verify trace/metric/log export at the configured OTLP receiver.

If dependencies, a JVM, Maven/Gradle, the application, or an OTLP receiver are unavailable, state
exactly which levels were not performed. Never claim startup or live export from static inspection.
