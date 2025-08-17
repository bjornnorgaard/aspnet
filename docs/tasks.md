# Improvement Tasks Checklist

A logically ordered, actionable checklist covering architectural and code-level improvements across the solution.

1. [ ] Solution-wide architecture review and documentation
   - [ ] Document the current architecture (Api, Application, Platform, Tests) and responsibilities for each project
   - [ ] Define clear layering rules (Api = transport; Application = business; Platform = cross-cutting; no reverse deps)
   - [ ] Add a simple architecture decision record (ADR) template in docs/adr/ for future decisions

2. [ ] Configuration and environments
   - [ ] Add strongly-typed options for DB connection (ConnectionStrings) and feature flags
   - [ ] Provide appsettings.{Environment}.json samples and document env usage
   - [ ] Add guard/fallback when DefaultConnection is missing (fail fast with clear error)

3. [ ] Database and EF Core
   - [ ] Add initial EF Core migration and ensure it is applied in CI
   - [ ] Replace EnsureCreated in tests with Migrate to match prod behavior
   - [ ] Add index recommendations (e.g., on CreatedAt) and model-level constraints
   - [ ] Add seed data option for local dev/testing (toggle via configuration)

4. [ ] API design consistency and versioning
   - [ ] Standardize endpoint routes and HTTP verbs (e.g., todos POST /, GET /{id}, PUT /{id}, GET /)
   - [ ] Ensure description/tags follow a convention and appear in OpenAPI/Scalar
   - [ ] Provide consistent response shapes (problem details for errors, envelopes if needed)

5. [ ] Validation and error handling
   - [ ] Centralize request validation (e.g., FluentValidation or custom validators)
   - [ ] Return RFC 7807 ProblemDetails for validation and domain errors
   - [ ] Expand GlobalExceptionMiddleware to include correlation id and trace id in responses

6. [ ] Telemetry and observability
   - [ ] Add correlation id middleware and propagate it to logs/traces
   - [ ] Enrich logs with request/response context (method, route, user, status)
   - [ ] Add database and HTTP client dependency metrics dashboards guidance (Grafana/OTLP)
   - [ ] Make telemetry exporters configurable (console/OTLP) per environment

7. [ ] Performance and resiliency
   - [ ] Add cancellation token usage across all async I/O boundaries (verified)
   - [ ] Add Polly policies for HTTP clients (retry, circuit breaker, timeout)
   - [ ] Add EF Core performance tuning (tracking behavior, batching, connection pool sizing)

8. [ ] Security and hardening
   - [ ] Add authentication/authorization scaffolding (JWT bearer) and secure endpoints where applicable
   - [ ] Validate/limit request sizes and add input length constraints (Title/Description already constrained)
   - [ ] Add HTTPS redirection/HSTS configuration and document local exceptions

9. [ ] Pagination and data access patterns
   - [ ] Replace GUID-based cursor comparison with CreatedAt or deterministic key set paging
   - [ ] Add total count endpoint or include metadata when necessary
   - [ ] Provide example clients for pagination usage in docs

10. [ ] Feature organization (Application layer)
    - [ ] Introduce a common Result/Problem pattern for features (success, error, messages)
    - [ ] Add unit tests per feature (domain logic) independent of API
    - [ ] Avoid leaking EF models to API responses (use DTOs consistently)

11. [ ] Platform abstractions
    - [ ] Add base endpoint helpers (e.g., MapCrud<T> generator or conventions)
    - [ ] Make reflection-based registration resilient (exclude types via attribute, cache scans)
    - [ ] Add assembly anchor guidance and examples

12. [ ] OpenAPI/Swagger improvements
    - [ ] Add XML comments and enable IncludeXmlComments to enrich the schema
    - [ ] Ensure request/response examples for key endpoints
    - [ ] Tag endpoints by feature and version consistently

13. [ ] Testing strategy
    - [ ] Stabilize testcontainers Postgres lifecycle (proper wait strategy, Migrate not EnsureCreated)
    - [ ] Add negative tests (validation errors, 404, 400 for bad GUID)
    - [ ] Add performance tests or simple load checks (k6/Gatling scripts in docs/examples)

14. [ ] CI/CD pipeline
    - [ ] Add GitHub Actions workflow: build, test, generate coverage, publish artifacts
    - [ ] Cache NuGet packages and EF migration bundles to speed builds
    - [ ] Optionally add container build/push stage (Dockerfile for Api) and scan images

15. [ ] Developer experience
    - [ ] Add launchSettings profiles for different environments
    - [ ] Add local docker-compose for running Postgres + OTLP collector + Grafana/Tempo
    - [ ] Provide Makefile/PowerShell scripts for common tasks (build, test, run, migrate)

16. [ ] Code quality and analyzers
    - [ ] Enable nullable warnings as errors where safe and add .editorconfig
    - [ ] Add Roslyn analyzers/StyleCop and fix warnings incrementally
    - [ ] Add code coverage thresholds in test runs

17. [ ] Documentation
    - [ ] Expand README with setup, run, test, migrate instructions
    - [ ] Add CONTRIBUTING.md and CODE_OF_CONDUCT.md templates
    - [ ] Keep a changelog (docs/CHANGELOG.md) and ADRs up to date

18. [ ] Release readiness
    - [ ] Add health checks (liveness, readiness) and map endpoints
    - [ ] Add version info endpoint (from TelemetryOptions) for diagnostics
    - [ ] Provide migration strategy notes and rollback plan
