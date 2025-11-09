## 1. Backward-Compatible Contract Evolution

For API backward compatibility, I follow a **versioned API approach**. V1 endpoints remain stable for existing clients, while V2 introduces additive fields or new endpoints. 
Optional fields are added with default values so clients that do not expect them are unaffected. 
Deprecated fields are not removed immediately; instead, they are documented and flagged for future removal. This ensures smooth evolution without breaking existing consumers.

---

## 2. Retries, Timeouts, and Circuit Breakers

For upstream service calls, I use **retry policies with exponential backoff**, short **timeouts**, and **circuit breakers** to prevent cascading failures. Defaults: 3 retries, 2–5s timeout per call, circuit opens after 3 consecutive failures for 30s. This balances reliability and responsiveness, protecting the gateway from slow or failing ERP/Warehouse systems.
Metrics from retries and failures help tune these budgets in production.

---

## 3. Idempotency Strategies

Idempotency is enforced on write operations (POST/PUT) using a combination of `Idempotency-Key + operation type + request body hash + request path`. Incoming requests with the same key and payload are deduplicated to prevent multiple writes. Responses are cached for a configurable TTL to handle retries or replay scenarios. This ensures safe retries without unintended side effects on upstream systems.

---

## 4. Observability

I prefer using Azure application insights for observability as it includes:
 - tracking the time taken by dependencies
 - overall time taken to process request
 - custom telemetry tracking (User IP Address, User-Agent, request headers etc.)
 - exceptions and its details
 - logging custom traces

If we have to do it manually using then I would use the below strategy:
Logs, metrics, and traces are emitted for all critical operations: request start/end, upstream calls, cache hits/misses, retries, and idempotency events. Structured logs (JSON) are used to correlate requests with unique IDs. Metrics include request latency, error rates, and circuit breaker status. Distributed tracing (e.g., OpenTelemetry) allows end-to-end visibility for debugging slow or failed integrations.

---

## 5. Security Controls

Security can be enforced at multiple layers:  
- **AuthN/Z:** I would implement AD SSO authentication if it is an internal application. If the application is public facing we can use Azure AD B2C.
- **Input validation:** Payload + query params validation to prevent injection or malformed data.
- **Rate limiting:** I prefer applying rate limiting rules at the edge (e.g Cloudflare/similar gateway) based on IP address.
- **Secrets/config:** I prefer Azure Key Vault with managed identity to access the secrets if we are on hosted on Azure. Otherwise, I would use environment variables.

---

## 6. Preferred Framework/Tooling

I chose **.NET 8 with MVC/Web API** because it provides:  
- Strong DI and middleware support for cross-cutting concerns (caching, logging, idempotency)  
- Asynchronous HTTP client support for orchestrating multiple upstream services  
- Easy API versioning and testability  
- Rich ecosystem for unit/integration testing (xUnit, Moq) and observability (Serilog, OpenTelemetry).  

This framework allows rapid development of a robust, production-ready integration gateway that meets enterprise standards.