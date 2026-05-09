# Specify Project Constitution

## Core Principles

### I. Code Quality (NON-NEGOTIABLE)

Every piece of code MUST be readable, maintainable, and purposeful.

- Code MUST be reviewed before merging — no self-merges on shared branches.
- Functions and modules MUST have a single, clear responsibility (Single Responsibility Principle).
- Dead code, unused imports, and commented-out blocks MUST be removed before merging.
- Naming MUST be descriptive and consistent with the existing codebase conventions.
- Complexity MUST be justified; simpler solutions are preferred when functionally equivalent.

**Rationale**: Unmaintainable code accumulates technical debt that compounds over time,
slowing delivery and increasing defect rates. Quality is enforced at the source, not patched later.

### II. Testing Standards (NON-NEGOTIABLE)

Tests are a first-class deliverable, not an afterthought.

- TDD is mandatory: tests MUST be written and confirmed failing before implementation begins.
- Every feature MUST have unit tests covering primary flows and edge cases.
- Integration tests MUST cover cross-module boundaries and external contracts.
- A minimum of 80% code coverage is required for new code; coverage MUST NOT regress.
- Tests MUST be deterministic — flaky tests MUST be fixed or removed immediately.
- Test names MUST describe behavior, not implementation (e.g., "user cannot checkout with empty cart").

**Rationale**: Tests are the specification in executable form. They protect against regression,
document intent, and enable confident refactoring. The TDD gate enforces design discipline.

### III. User Experience Consistency

Every user-facing surface MUST follow established interaction patterns and design language.

- UI components MUST reuse existing design system elements before introducing new ones.
- Error messages MUST be actionable, human-readable, and free of technical jargon.
- User flows MUST be validated against acceptance scenarios defined in the spec before shipping.
- Accessibility (WCAG 2.1 AA minimum) MUST be verified for all new UI surfaces.
- Behavioral changes visible to users MUST be documented in release notes.

**Rationale**: Inconsistent UX erodes user trust and increases support burden.
Standardizing interactions reduces cognitive load and accelerates user adoption.

### IV. Performance Requirements

Performance is a feature and MUST be considered from the design phase.

- Every feature MUST define measurable, user-facing performance criteria in its spec.
- Regressions in response time or resource consumption MUST be flagged and justified before merging.
- Performance-critical paths MUST be profiled; optimizations MUST be data-driven, not speculative.
- Features MUST degrade gracefully under load — no hard failures without user feedback.
- Baseline benchmarks MUST be established before optimizing and verified after.

**Rationale**: Performance issues discovered late are expensive to fix and damage user experience.
Defining targets upfront aligns design, implementation, and testing around measurable outcomes.

## Quality Gates

All work MUST pass the following gates before merging:

- **Code review**: At least one peer review approval required.
- **TDD gate**: Tests written, confirmed failing, then implementation follows.
- **Coverage gate**: New code meets 80% coverage threshold.
- **Performance gate**: No regressions against established baselines.
- **UX gate**: User flows validated against spec acceptance scenarios.
- **Constitution check**: Plan explicitly documents compliance or justified deviation
  for each applicable principle.

## Governance

This constitution supersedes all other project practices. Amendments require:

1. A documented rationale explaining the change.
2. A review of impact on existing features and pipelines.
3. A migration plan if existing code must be updated for compliance.
4. A version bump following semantic versioning:
   - **MAJOR**: Principle removed, redefined, or made incompatible with prior work.
   - **MINOR**: New principle or section added.
   - **PATCH**: Clarification, wording fix, or non-semantic refinement.

All planning sessions (`/speckit.plan`) MUST include a Constitution Check gate that explicitly
confirms compliance or documents justified deviation for each principle.

**Version**: 1.0.0 | **Ratified**: 2026-05-08 | **Last Amended**: 2026-05-08
