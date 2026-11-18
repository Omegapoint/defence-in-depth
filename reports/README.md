# Security review reports

This folder holds security review reports written by AI agents (GitHub Copilot and Claude Code) or by people. A review follows the instructions in `.github/copilot-instructions.md` and `.github/instructions/`, and uses the local threat model in [`threat-model/`](../threat-model/README.md).

Reports in this folder are **not committed** (see `.gitignore`); only this README is tracked. A report describes weaknesses and how to exploit them, so share it only where access is controlled. Fix the findings in pull requests and refer to them by finding ID (for example F1), without copying report details into public pull requests or issues.

Reports are drafts. A person must verify the findings before anyone relies on them.

## Naming

Use `security-review-<yyyy-mm-dd>.md`. Add a short suffix if more than one review is done on the same day, for example `security-review-2026-11-16-identity.md`.

## Content

A report should contain:

- **Scope:** what was reviewed (components, branch, commit) and what was not.
- **Instructions and threat model used:** the instruction files and threat model files that were applied, or a statement that no threat model was found.
- **Summary:** overall posture and number of findings per severity.
- **Findings:** ordered by severity, each with an ID, location (file and line), the instruction section and threat IDs it relates to, the problem, a fix and the tests needed.
- **Threat model coverage:** the relevant threats and whether each one is mitigated, partly mitigated or not mitigated.
- **Suggested threat model updates.**
- **Security test gaps.**
- **References.**

To ask an agent for a review, for example: *"Do a security review using the agent instructions and threat model, and save the result as a report in the reports folder."*
