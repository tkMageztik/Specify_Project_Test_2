# Task Routing — 001-user-login

Generated: 2026-05-08  
Strategy: capability-match  
Source: `specs/001-user-login/tasks.md`

## Routing Table

```
Task Routing Summary
───────────────────────────────────────────────────────────────────────────────
Task                                                  Agent                Tier
───────────────────────────────────────────────────────────────────────────────
PHASE 1: SETUP
T01  Create backend project (dotnet new webapi)       backend-engineer     complex
T02  Create frontend project (Vite + React TS)        frontend-engineer    simple
T03  Configure backend NuGet packages                 backend-engineer     simple
T04  Configure frontend npm packages                  frontend-engineer    simple
T05  Configure JWT settings in appsettings.json       backend-engineer     standard
T06  Configure EF Core DbContext + entities           backend-engineer     standard
T07  Run EF Core migrations                           backend-engineer     simple

PHASE 2: FOUNDATION
T08  Create User entity model                         backend-engineer     standard
T09  Create RefreshToken entity model                 backend-engineer     standard
T10  Create UserRepository                            backend-engineer     standard
T11  Create LoginRequest DTO                          backend-engineer     simple
T12  Create LoginResponse DTO                         backend-engineer     simple
T13  Create IAuthService interface                    backend-engineer     standard
T14  Create JwtTokenService (generate/validate JWT)   backend-engineer     complex
T15  Register services in Program.cs (DI/middleware)  backend-engineer     standard
T16  Create useAuth React hook skeleton               frontend-engineer    standard
T17  Create authApi.ts (axios instance)               frontend-engineer    standard

PHASE 3: US1 — LOGIN EXITOSO
T18  Write failing test: valid credentials → token    qa-engineer          standard
T19  Write failing test: login resets FailedAttempts  qa-engineer          standard
T20  Write failing test: submit form → API + redirect qa-engineer          standard
T21  Implement AuthService.LoginAsync() (BCrypt+JWT)  backend-engineer     complex
T22  Implement POST /api/auth/login controller        backend-engineer     standard
T23  Implement POST /api/auth/refresh (token rotate)  backend-engineer     complex
T24  Implement POST /api/auth/logout                  backend-engineer     standard
T25  Build LoginForm.tsx component                    frontend-engineer    standard
T26  Build LoginPage.tsx component                    frontend-engineer    standard
T27  Implement session persistence in useAuth.ts      frontend-engineer    complex
T28  Build ProtectedRoute.tsx (redirect + preserve)   frontend-engineer    standard
T29  Configure React Router routes in App.tsx         frontend-engineer    standard
T30  Verify T18, T19, T20 pass                        qa-engineer          simple

PHASE 4: US2 — CREDENCIALES INCORRECTAS
T31  Write failing test: wrong password → generic err qa-engineer          standard
T32  Write failing test: 5th attempt → lockout 15min  qa-engineer          standard
T33  Write failing test: 401 shows error on form      qa-engineer          standard
T34  Extend AuthService: FailedAttempts + lockout     backend-engineer     complex
T35  Extend AuthController: return 423 Locked         backend-engineer     standard
T36  Handle 401/423 in authApi.ts + LoginForm.tsx     frontend-engineer    standard
T37  Verify T31, T32, T33 pass                        qa-engineer          simple

PHASE 5: US3 — VALIDACIÓN FORMULARIO
T38  Write failing test: empty submit → field errors  qa-engineer          standard
T39  Write failing test: invalid email format error   qa-engineer          standard
T40  Add react-hook-form validation to LoginForm.tsx  frontend-engineer    standard
T41  Add WCAG 2.1 AA error messages (aria-describedby)frontend-engineer    standard
T42  Verify T38, T39 pass                             qa-engineer          simple
───────────────────────────────────────────────────────────────────────────────
Routed: 42 / 42   Unrouted: 0
```

## Coverage by Agent

| Agent               | Tasks | Complex | Standard | Simple |
|---------------------|-------|---------|----------|--------|
| backend-engineer    | 19    | 4       | 12       | 3      |
| frontend-engineer   | 12    | 1       | 9        | 2      |
| qa-engineer         | 11    | 0       | 8        | 3      |
| **Total**           | **42**| **5**   | **29**   | **8**  |

## Complex Tasks (require senior reasoning)

| Task | Agent | Why Complex |
|------|-------|-------------|
| T01  | backend-engineer | Initial project architecture decisions |
| T14  | backend-engineer | JWT signing algorithm, key rotation design |
| T21  | backend-engineer | BCrypt verify + JWT + refresh token + cookie — multi-step auth flow |
| T23  | backend-engineer | Refresh token rotation with revocation strategy |
| T27  | frontend-engineer | Token lifecycle management across React renders |
| T34  | backend-engineer | Lockout state machine + race condition risk |

## Notes

- All 42 tasks routed via **capability-match**
- No unrouted tasks — all patterns covered by `.squad/routing.md` rules
- QA gate tasks (T30, T37, T42) are `simple` tier — just test execution verification
- T36 routed to **frontend-engineer** (implementation in authApi.ts/LoginForm) despite "error handling" keyword — keyword context is frontend code
