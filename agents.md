# Agents policy

This repository uses GitHub Copilot / AI agents.

## Mandatory review of agent-created code

Any code (or configuration, IaC, pipelines, scripts, documentation with security implications) created or modified by an AI agent **must be reviewed for compliance with**:

- `.github/copilot-instructions.md` (primary source of requirements)
- Any additional instruction files under `.github/instructions/` that apply to the changed paths
<!-- We could add more instruction e.g. running local MCP server tools
- Run `security_qa` to review security posture.
- Run `compliance_qa` to retrieve and cite relevant references for Omegapoint verifications. For compliance the most significant control is 16 Application Security.
-->
- Add all references at the end.
### Pull request expectation

PRs that contain agent-generated changes should explicitly mention that the changes were reviewed against the Copilot instructions and security requirements.
