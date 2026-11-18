# Threat model

This folder holds the threat model for the system. AI agents (GitHub Copilot and Claude Code) read every file here before a security review, as described in [threat-model.instructions.md](../.github/instructions/threat-model.instructions.md).

Threat model files in this folder are **not committed** (see `.gitignore`); only this README is tracked. Each developer places a local copy of the threat model here, for example taken from the team's document store. A threat model describes weaknesses and attack paths, so keep it where access is controlled, not in the repository.

If a review warns that a change is not covered by the threat model, update the master copy where it is stored and refresh the local copy here.

## What to put here

Add one or more files with:

- **Scope:** the system, its components and what is out of scope.
- **Assets:** what we protect, for example product data per market, tokens and audit logs.
- **Actors:** legitimate clients, users and threat actors.
- **Trust boundaries, entry points and data flows:** for example client → API gateway → Product API → database, and the identity provider that issues tokens.
- **Threats:** each threat with a unique ID, a description, the affected assets and entry points, a risk rating and the mitigations.
- **Assumptions:** what the model assumes about the environment, for example that TLS is terminated by a trusted component.

Supported formats: Markdown (`.md`), text, `.json`, `.yaml`, Microsoft Threat Modeling Tool (`.tm7`) and diagrams as text (Mermaid `.mmd`, PlantUML `.puml`). Prefer text formats so agents and reviewers can read and diff them.

Do not store secrets, tokens, credentials or personal data in this folder.

## Suggested layout

```
threat-model/
├── README.md          This file
├── threat-model.md    Scope, assets, actors, assumptions and the threat list
└── data-flow.mmd      Data flow diagram with trust boundaries
```

## Threat entry template

Use a unique, stable ID so that reviews and pull requests can refer to the threat.

The entry below is an **example of the format only**. Its mitigations describe the target design, and its status reflects the current code, where the market is still hard-coded in the permission service. Record the actual status of your system, never copy a status from an example.

```markdown
### TM-001 (example): Client reads products from another market

- **Category (STRIDE):** Information disclosure / Elevation of privilege
- **Assets:** Product data
- **Entry point:** GET /api/products/{id}
- **Threat actor:** Authenticated client with access to a different market
- **Risk:** High (likelihood: medium, impact: high)
- **Mitigations:**
  - Market permission is derived from verified JWT claims by the permission service
  - Data access is checked in the domain service layer and returns 404 on failure (BOLA/IDOR)
- **Tests:** Negative test that a client for market A gets 404 for a product in market B (missing)
- **Status:** Partly mitigated (the 404 check exists, but the market is hard-coded and not derived from claims)
```

## References

- OWASP Threat Modeling Cheat Sheet: https://cheatsheetseries.owasp.org/cheatsheets/Threat_Modeling_Cheat_Sheet.html
- Omegapoint Secure Design Principles: https://securityblog.omegapoint.se/en/cis-control-verifications-cloud-native-applications
- Omegapoint Defense-in-Depth: https://securityblog.omegapoint.se/en/defense-in-depth
