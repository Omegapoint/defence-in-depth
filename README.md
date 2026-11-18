# Defence in depth implementation

Source code samples for "Defence in Depth" articles and presentations.

This is an accompanying implementation of a secure REST API in .NET 10
that follows the series of articles found at:

https://securityblog.omegapoint.se

Presentations on YouTube based on this material are linked from

https://securityblog.omegapoint.se/en/defense-in-depth

Articles that cover these concepts exactly can be found at:

https://securityblog.omegapoint.se/en/secure-apis-by-design

https://securityblog.omegapoint.se/en/test-driven-appsec

https://securityblog.omegapoint.se/en/agent-driven-appsec


The approach follows that of the book "Secure by Design":

https://www.manning.com/books/secure-by-design

The repo also contains a token service for demo and education purposes built with Duende Identity Server:

https://github.com/DuendeSoftware/IdentityServer


There is also a Java repo at 

https://github.com/Omegapoint/defence-in-depth-java


## AI Agent Instructions

This repo contains instructions for reviewing the security posture, both using GitHub Pull Request review agents and from the prompt.

Agent instructions can be added in many ways, this is just a POC to show that we can use agents to identify security issues in this repo, according to implemented secure by design principles, not for any code.

The instructions are kept in the GitHub Copilot format and are shared by GitHub Copilot and Claude Code (through `CLAUDE.md`):

- `.github/copilot-instructions.md`: primary requirements for all new or changed code
- `.github/instructions/security-requirements.instructions.md`: the access model for the Product API
- `.github/instructions/security-backend-api.instructions.md`: security requirements for the .NET backend API
- `.github/instructions/threat-model.instructions.md`: how agents use the threat model in security reviews
- `agents.md`: policy for review of agent-created code

### Threat model

Place a local copy of the threat model in the `threat-model/` folder. The files are ignored by git and never committed, only `threat-model/README.md` is tracked. Agents read the threat model before a security review and use it to:

- map changes to relevant threats and verify that their mitigations and tests are in place
- warn when a change weakens a mitigation, or adds assets, entry points, data flows or trust boundaries that the threat model does not cover
- prioritize findings by the risk ratings in the threat model

If there is no threat model, agents warn about it and review against the other instructions. See `threat-model/README.md` for the expected content and a threat template. When a change affects the threat model, update the master copy where it is stored.

### Security review reports

Agents can save security review reports in the `reports/` folder. Like the threat model, reports are ignored by git and never committed, only `reports/README.md` is tracked. See `reports/README.md` for naming and expected content.

Since the threat model is never pushed, only agents running locally (e.g. Claude Code or Copilot in the IDE) can use it. Pull request review agents on GitHub will state that the threat model is not available, so do threat-model-based reviews locally. Pull requests only refer to threat and finding IDs, never to threat model or report details.

### Pre-commit hook

A pre-commit hook in `.githooks/pre-commit` blocks commits of any file in `threat-model/` or `reports/` other than `README.md` and `.gitignore`, also if the file was added with `git add -f`. Enable it once per clone:

```
git config core.hooksPath .githooks
```

Other agent instructions can be found at e.g.

https://github.com/OWASP/secure-agent-playbook

https://github.com/github/awesome-copilot  


