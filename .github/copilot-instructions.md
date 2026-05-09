<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
at `specs/001-user-login/plan.md`
<!-- SPECKIT END -->

# Copilot Instructions

This repository is a **Spec Kit** project — a specification-driven development (SDD) workflow toolkit that orchestrates GitHub Copilot through a pipeline of agent-mode slash commands.

## Architecture

Spec Kit structures development as a sequential pipeline:

```
constitution → specify → clarify → plan → tasks → implement
```

Each stage produces artifacts consumed by the next:

| Stage | Command | Output |
|-------|---------|--------|
| Constitution | `speckit.constitution` | `.specify/memory/constitution.md` |
| Specification | `speckit.specify` | `specs/NNN-feature-name/spec.md` |
| Clarification | `speckit.clarify` | Updates `spec.md` in-place |
| Planning | `speckit.plan` | `specs/NNN-feature-name/plan.md`, `research.md`, `data-model.md`, `contracts/` |
| Task generation | `speckit.tasks` | `specs/NNN-feature-name/tasks.md` |
| Implementation | `speckit.implement` | Source code per `tasks.md` |
| Analysis | `speckit.analyze` | Cross-artifact consistency report |
| Checklists | `speckit.checklist` | `specs/NNN-feature-name/checklists/` |

The **currently active feature** is tracked in `.specify/feature.json`. Downstream commands (`speckit.plan`, `speckit.tasks`, `speckit.implement`) read this file to locate the correct `specs/` directory without needing explicit arguments.

## Key Directories

- `.github/agents/` — Agent-mode prompt definitions for each `speckit.*` command
- `.github/prompts/` — Prompt-only variants of the same commands
- `.specify/templates/` — Canonical templates for `spec.md`, `plan.md`, `tasks.md`, `constitution.md`, `checklist.md`
- `.specify/scripts/powershell/` — Helper scripts invoked by agents (auto-approved in VS Code)
- `.specify/memory/constitution.md` — Project constitution (governs all planning and implementation decisions)
- `.specify/extensions.yml` — Hook configuration (before/after hooks per pipeline stage)
- `specs/` — Generated feature artifacts; one subdirectory per feature

## Spec Numbering

Features are numbered **sequentially** by default (`001-feature-name`, `002-...`) as configured in `.specify/init-options.json`. The next number is derived by scanning existing `specs/` subdirectories. Feature branch names and spec directory names are independent.

## Git Extension Hooks

The git extension runs automatically at each stage. Mandatory hooks (e.g., `before_specify`) run without prompting; optional hooks (e.g., `after_specify` auto-commit) prompt first. Hook behavior is configured in `.specify/extensions.yml`.

Branch naming follows the same sequential scheme: `NNN-feature-name`.

## Scripts

All helper scripts are PowerShell and live in `.specify/scripts/powershell/`. The key entry point used by `speckit.implement` is:

```powershell
# Run from repo root — returns JSON with FEATURE_DIR and AVAILABLE_DOCS
.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireTasks -IncludeTasks
```

VS Code auto-approves terminal execution for `.specify/scripts/` paths (see `.vscode/settings.json`).

## Task Format Conventions

Tasks in `tasks.md` use structured markers:

- `[P]` — task can run in **parallel** with other `[P]` tasks (different files, no shared dependencies)
- `[US1]`, `[US2]`, etc. — maps the task to a specific **user story** for independent delivery
- Tasks must include exact file paths
- Tests must be written and confirmed **failing** before implementation begins (TDD gate)

## Spec Quality Rules

`speckit.specify` enforces these constraints before a spec is considered ready:

- Maximum **3 `[NEEDS CLARIFICATION]`** markers; extras are resolved using informed defaults
- Specs must be **technology-agnostic** — no frameworks, languages, or implementation details
- Success criteria must be **measurable and user-facing** (not system metrics like "API response time")
- All acceptance scenarios follow Given/When/Then format

## Constitution Governance

`.specify/memory/constitution.md` is the highest-authority document in the project. Every `speckit.plan` run includes a **Constitution Check gate** that must pass before design proceeds. Violations must be documented in the plan's Complexity Tracking table with justification.

## Adding or Modifying Agents

Agent prompts live in `.github/agents/speckit.*.agent.md`. Each agent:
1. Reads `$ARGUMENTS` from the user's slash command input
2. Checks `.specify/extensions.yml` for before-hooks (mandatory hooks auto-execute; optional hooks prompt)
3. Executes its core logic
4. Checks after-hooks

When modifying an agent, keep the hook-checking structure intact — it is load-bearing for the git extension workflow.
