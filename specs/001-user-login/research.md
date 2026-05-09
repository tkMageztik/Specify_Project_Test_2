# Research: User Login

**Feature**: 001-user-login | **Date**: 2026-05-08

## Token Strategy

- **Decision**: JWT access token (15 min) + refresh token (8h) stored in HttpOnly cookie
- **Rationale**: Stateless API, XSS-safe refresh token, standard .NET + React pattern
- **Alternatives**: Session cookies (stateful, harder to scale), localStorage JWT (XSS risk)

## Password Hashing

- **Decision**: BCrypt via `BCrypt.Net-Next` NuGet package
- **Rationale**: Adaptive cost factor, widely adopted, resistant to rainbow tables
- **Alternatives**: Argon2 (more modern but less ecosystem support in .NET), PBKDF2 (built-in but weaker)

## Account Lockout

- **Decision**: Track `FailedAttempts` + `LockoutUntil` columns on User entity in database
- **Rationale**: Simple, no external dependency, persists across server restarts
- **Alternatives**: Redis distributed lock (overkill for this scale), ASP.NET Identity built-in lockout (viable but ties to Identity framework)

## Frontend State Management

- **Decision**: React Context + `useAuth` custom hook
- **Rationale**: Lightweight for single auth feature; avoids Redux overhead for v1
- **Alternatives**: Redux Toolkit `authSlice` (better for large apps with many auth-aware components)

## Protected Routes

- **Decision**: `<ProtectedRoute>` wrapper component using React Router v6
- **Rationale**: Declarative, composable, standard SPA pattern
- **URL preservation**: Pass `location` state through redirect to restore after login

## Error Message Strategy

- **Decision**: Generic "Invalid email or password" for all auth failures
- **Rationale**: FR-003 — must not reveal which field is incorrect (security requirement)
- **Lockout message**: "Account temporarily locked. Try again in X minutes." (does not confirm email existence in isolation)
