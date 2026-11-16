# Claude Code instructions

This repository keeps its AI agent instructions in the GitHub Copilot format. Those files are the **single source of truth** and are shared by GitHub Copilot and Claude Code. Do not copy their content into this file. Change the files under `.github/` and both agents pick up the change.

## How to read the shared instructions

- Where an instruction says **"Copilot"** (e.g. "Copilot MUST", "Copilot must verify"), it applies to **you (Claude)** in exactly the same way.
- Files under `.github/instructions/` start with `applyTo:` frontmatter, a glob that says which files the instructions cover. Apply each file's rules when you create, change or review files that match its glob:
  - `security-requirements.instructions.md`: `**/*.*` (all files)
  - `security-backend-api.instructions.md`: `**/*.cs` (.NET backend code)
  - `threat-model.instructions.md`: `**/*.*` (all files; reads the threat model in `threat-model/`)
- If you add a new `*.instructions.md` file under `.github/instructions/`, also add an import for it below. Otherwise Claude will not see it.
- When you write chat answers, review comments or pull request descriptions, cite the relevant instruction section and references, as the instruction files require.

## Agents policy

@agents.md

## Primary requirements (`.github/copilot-instructions.md`)

@.github/copilot-instructions.md

## Path-specific instructions (`.github/instructions/`)

@.github/instructions/security-requirements.instructions.md

@.github/instructions/security-backend-api.instructions.md

@.github/instructions/threat-model.instructions.md
