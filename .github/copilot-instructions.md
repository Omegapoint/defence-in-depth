For any new or changed code, Copilot MUST:

1. Detect violations of Omegapoint’s secure design principles, including relevant verifications in OWASP ASVS v5.0.0. 
2. Suggest corrections that strengthen defense-in-depth, such as the 6-step model for secure API design or other relevant reference architecture.  
3. Require tests for all security-relevant logic. In particular negative tests concering authorization logic and input validation.

Copilot should also warn and explain when security posture is weakened. 
Thus, the following questions (when applicable) should be investigated and explained:

- Is authentication and authorization enforced according to the principles of least privilege, zero trust and defense in depth?
- Is input validation applied according to trust boundaries and domain logic?
- Is output encoding applied according to trust boundaries and context?
- Are there any outdated or vulnerable dependencies?
- Are there any unnecessary or high-risk dependencies (e.g. without reliable ownership) that can be replaced?
- Does the code or configuration contain or expose sensitive data, such as personal data, tokens or credentials?
- Can business logic be abused, e.g. for denial-of-service attacks?
- Are security-relevant events, errors and exceptions logged?
- Are errors and exceptions handled securely without leaking internal implementation details?
- Are there any tests with focus on security, e.g. negative tests for authorization logic and input validation?

---

References:
- Omegapoint Secure Design Principles: https://securityblog.omegapoint.se/en/cis-control-verifications-cloud-native-applications 
- Omegapoint Secure APIs by Design: https://securityblog.omegapoint.se/en/secure-apis-by-design
- Omegapoint Test-Driven AppSec: https://securityblog.omegapoint.se/en/test-driven-application-security
- Omegapoint Defense-in-Depth: https://securityblog.omegapoint.se/en/defense-in-depth
- OWASP ASVS v5.0.0: https://owasp.org/www-project-application-security-verification-standard/