# Implementation Plan: User Login

**Branch**: `001-user-login` | **Date**: 2026-05-08 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-user-login/spec.md`

## Summary

Implement email/password authentication for a web application using a .NET 10 ASP.NET Core REST API backend and a React frontend. The feature covers login form, client-side validation, session management via secure tokens, account lockout after 5 failed attempts, and protected route redirection.

## Technical Context

**Language/Version**: C# / .NET 10 (constitution-mandated)
**Primary Dependencies**: ASP.NET Core, Entity Framework Core, React 19+, React Router
**Storage**: SQL Server or PostgreSQL (relational — user accounts, session tokens)
**Testing**: xUnit + Moq (backend), Jest + React Testing Library (frontend)
**Target Platform**: Web application (.NET 10 backend + React frontend)
**Project Type**: Web service (REST API) + Single Page Application
**Performance Goals**: Login response < 2 seconds under normal load
**Constraints**: No sensitive info in error messages; 5-attempt lockout; 8h session expiry
**Scale/Scope**: Standard web app — single user store, stateless API

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|---|---|---|
| I. Code Quality | ✅ Pass | Single-responsibility controllers and services; code review required |
| II. Testing Standards | ✅ Pass | TDD enforced; xUnit + Jest; 80% coverage gate |
| III. UX Consistency | ✅ Pass | Form validation, accessible error messages, WCAG 2.1 AA |
| IV. Performance Requirements | ✅ Pass | SC-002 defines < 2s response; benchmark before optimizing |
| Technical Constraints | ✅ Pass | C# / .NET 10 backend, React frontend — constitution-mandated |

**All gates pass. Proceeding to Phase 0.**

## Project Structure

### Documentation (this feature)

```text
specs/001-user-login/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   └── auth-api.md
└── tasks.md             # Phase 2 output (/speckit.tasks)
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── Auth/
│   │   ├── Controllers/AuthController.cs
│   │   ├── Services/AuthService.cs
│   │   ├── Models/LoginRequest.cs
│   │   └── Models/LoginResponse.cs
│   └── Users/
│       ├── Models/User.cs
│       └── Repositories/UserRepository.cs
└── tests/
    └── Auth/
        ├── AuthControllerTests.cs
        └── AuthServiceTests.cs

frontend/
├── src/
│   ├── features/auth/
│   │   ├── LoginForm.tsx
│   │   ├── LoginPage.tsx
│   │   ├── authSlice.ts (or useAuth hook)
│   │   └── ProtectedRoute.tsx
│   └── api/
│       └── authApi.ts
└── src/__tests__/auth/
    ├── LoginForm.test.tsx
    └── ProtectedRoute.test.tsx
```

## Phase 0: Research

*See [research.md](./research.md) for full findings.*

### Key Decisions

| Decision | Choice | Rationale |
|---|---|---|
| Token strategy | JWT (short-lived access + refresh token) | Stateless, scalable, standard for .NET + React |
| Password hashing | BCrypt via `BCrypt.Net-Next` | Industry standard, resistant to brute-force |
| Account lockout | In-database counter + timestamp | Simple, reliable, no external dependency |
| Session persistence | HttpOnly cookie (refresh) + memory (access) | Prevents XSS on refresh token |
| Frontend state | React Context or Redux Toolkit `authSlice` | Lightweight for single feature |
| Protected routes | React Router `<ProtectedRoute>` wrapper | Standard SPA pattern |

## Phase 1: Design & Contracts

*See [data-model.md](./data-model.md) and [contracts/auth-api.md](./contracts/auth-api.md).*

### Complexity Tracking

| Item | Complexity | Justification |
|---|---|---|
| JWT refresh token rotation | Medium | Required for secure long-lived sessions |
| Account lockout logic | Low | Simple counter + timestamp in DB |
| Protected route redirect | Low | Standard React Router pattern |
| BCrypt hashing | Low | Library handles complexity |
