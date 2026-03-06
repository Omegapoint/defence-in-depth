---
applyTo: "**/*.cs"
---
# .NET Backend API Security Instructions
This document defines security instructions for .NET backend REST APIs. 
Copilot must verify that the code adheres to these instructions when generating code or reviewing pull requests.

Copilot must always add a reference to the relevant section in any instruction in this document when writing chat answers or pull request comments.

## 1. Identity & Authentication
_Reference: Identity Modelling, Clients & Sessions_

Copilot must verify:

### 1.1 Identity
- Identity must be based on the JWT claim sub or, when sub is not present, client_id.
- If the act claim is present it must be used to represent the acting party on behalf of whom the request is made.  

### 1.2 Authentication
- All requests must be authenticated using trusted JWTs from the Authorization header.
- Any endpoints allowing anonymous access must be explicitly defined with AllowAnonymous attribute for the specific endpoint, NEVER allow AllowAnonymous at class level.
- JWTs must be validated, in particular (but not limited to): Signature (only accept secure methods according to OWASP ASVS), issuer, audience, expiration.
- If API request authentication (JWT validation) errors occur they must result in a 401 response code.

---

## 2. Authorization & Claims-Based Access Control
_Reference: Claims-Based Access Control, Secure APIs, Secure APIs by Design_

Copilot must verify:

### 2.1 Authorization

- Ensure least-privilege: Components should only access what they strictly need according to access model requirements (defined in the file security-requirements.instructions.md).
- Authorization decisions must be based on permissions from the permissions service or claims in the verified JWT. NEVER trust user-supplied data (like hard to guess routes or object ids).   
- Authorization decisions must be explicit and occur close to protected resources, at a trusted service layer.


### 2.2 Transform from claims to permissions
- Assert that claims, in the ClaimsPrincipal object, are transformed to permissions according to applicable access model implemented by the permission service.
- Assert that when sub, client_id and scope claims are present they are input to the permission service logic.  

### 2.3 Authorize access to the operation
- Assert that access to the operation is verified both at the API endpoint, using .NET authorization attribute with named policies, and in the domain service layer, mitigating any BFLA vulnerabilities.
- If authorization to the operation fails it must result in a 403 response.

### 2.4 Authorize access to the data object
- Assert that access to the requested data object is verified, mitigating any BOLA/IDOR vulnerabilities.
- If authorization to the data object fails it must result in a 404. 

### 2.5 Authorize access to the data object field
- Assert that access to the requested data object field is verified, mitigating any BOPLA vulnerabilities. This is typically impleneted by using data transfer objects (DTOs) that only include fields the caller is authorized to access (also know as request-response models or data contracts). 

---
## 3. Input Validation and Output Encoding
_Reference: Secure APIs, Secure APIs by Design_

Copilot must verify:

### 3.1 Input validation
- All endpoints must any validate input in API requests, this includes all route, headers and body fields that are processed by the API in any way.
- Apply the .NET FromBody, FromRoute, FromQuery, and FromHeader attributes to explicitly define the source of input parameters.
- Input must be validated according to strict schemas (length, type, format).
- Input must be validated according to domain logic using domain primitives.
- Input validation must must be applied according to trust boundaries, as early as possible, both at the API boundary and when entering the domain service layer.  
- Prefer whitelisting over blacklisting, for example any input URL must be validated using exact string matching against a whitelist.
- All strings must be validated against expected formats and lengths
- No dynamic or unbounded deserialization.
- If input validation errors occur they must result in a 400 response code without reflecting the raw input value (mitigating reflected XSS).   

### 3.2 Output encoding
- Ensure that any data returned to clients is properly encoded according to trust boundaries and applicable output context to prevent injection attacks.
- Always use response data contracts, never return domain entities or database models directly.  
- Never return sensitive identifiers or internal state, no leakage of internal implementation details in error messages.

---

## 4. Infrastructure & Data Security
_Reference: Infrastructure & Data Storage_

Copilot must verify:

### 4.1 Protocol and transport
- Enforce HTTPS/TLS and reject plaintext protocols, HTTP must not be supported.
- If the cnf claim is present in the JWT it must be validated according to the applicable proof-of-possession method (MTLS, DPoP).


### 4.2 Secrets & configuration
- Always use Entra Managed Identity when supported.
- Azure Key Vault MUST be used for all secrets needed by the application.
- Secrets MUST never be stored in repository files, Git history, or any build artifacts etc.
- JWKS endpoints used for JWT validation must be from trusted identity providers (https) and configured in application settings, never hardcoded in code.
- Any secrets that needs to be created must be created with sufficient entropy according to current best practices (using cryptographically secure .NET methods).  

### 4.3 Storage
- Warn if sensitive data such as personal data, tokens or credentials are stored.
- If sensitive data is stored, assert that it is protected according to applicable regulations and best practices (hashing etc).

### 4.4 Logging
- Assert that all logs are written using the configured logging service.
- Assert that any unexpected errors are treated as exceptions and logged as errors.
- Assert that any security realted policy violation is logged as warnings, using the AuditLoggingService.
- Assert that any security realted operation that succeeds is logged as information, using the AuditLoggingService.
- Warn if logs contain sensitive data such as personal data, tokens or credentials.

---

## 5. Browser & Client-Side Security
_Reference: Web Browsers_

Copilot must verify:

- Use HTTP security headers according to OWASP REST Security Cheat Sheet recommendations (e.g. HSTS, X-Content-Type-Options, cache-control).  

---

## 6. Test-Driven Application Security
_Reference: Test-Driven AppSec_

Copilot must enforce that:

- Critical application logic paths are covered with unit, integration and system tests.
- There are tests that verify that authorization rules behave as intended (positive + negative tests).  
- There are tests that verify that input validation works correctly and that malformed/malicious payloads are rejected.  

---

## 7. Defensive Coding Requirements
_Reference: Defense-in-Depth, Secure Architecture_

Copilot must enforce:

- Apply a defense-in-depth model: do not rely on a single control. For example, configure the DefaultPolicy and FallbackPolicy to require authenticated users, apply an authorization attribute on each endpoint, and enforce authorization checks in the domain service layer. Never assume that authentication, authorization or important validation logic is done only by an upstream component. 
- Fail securely: default should deny access and never leak internal information such as serialized exception objects.  
- Validate assumptions explicitly in code (argument checks, null checks, type checks).  
- Reject needless complexity that increases attack surface.

---

References:
- Omegapoint Secure Design Principles: https://securityblog.omegapoint.se/en/cis-control-verifications-cloud-native-applications 
- Omegapoint Secure APIs by Design: https://securityblog.omegapoint.se/en/secure-apis-by-design
- Omegapoint Test-Driven AppSec: https://securityblog.omegapoint.se/en/test-driven-application-security
- Omegapoint Defense-in-Depth: https://securityblog.omegapoint.se/en/defense-in-depth
- OWASP ASVS v5.0.0: https://owasp.org/www-project-application-security-verification-standard/
- OWASP REST Security Cheat Sheet: https://cheatsheetseries.owasp.org/cheatsheets/REST_Security_Cheat_Sheet.html
