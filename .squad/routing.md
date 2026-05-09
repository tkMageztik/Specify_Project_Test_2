# Routing Rules — 001-user-login

## Pattern → Agent Assignments

| Pattern | Agent |
|---|---|
| `/\bAPI\|endpoint\|REST\|controller\|middleware\b/i` | backend-engineer |
| `/\bC#\|\.NET\|ASP\.NET\|Entity Framework\|token\b/i` | backend-engineer |
| `/\bauth.*backend\|login.*API\|session.*server\b/i` | backend-engineer |
| `/\bNuGet\|migration\|DbContext\|repository\|DTO\b/i` | backend-engineer |
| `/\bReact\|component\|UI\|form\|frontend\|client\b/i` | frontend-engineer |
| `/\bvalidation\|redirect\|route\|hook\|state\b/i` | frontend-engineer |
| `/\baxios\|authApi\|Vite\|npm.*package\b/i` | frontend-engineer |
| `/\btest\|spec\|coverage\|QA\|TDD\|assert\b/i` | qa-engineer |
| `/\blockout\|security\|edge case\b/i` | qa-engineer |

## Tie-break Rule

When "error handling" appears in **implementation** tasks touching frontend files
(`authApi.ts`, `LoginForm.tsx`), prefer **frontend-engineer** over qa-engineer.
Reserve qa-engineer for test-writing and verification tasks only.
