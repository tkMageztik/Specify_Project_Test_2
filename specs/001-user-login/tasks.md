# Tasks: User Login

**Input**: Design documents from `/specs/001-user-login/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: User story (US1=Login exitoso, US2=Credenciales incorrectas, US3=Validación formulario)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization — must complete before all other phases.

- **T01** Create backend project structure: `dotnet new webapi -n LoginApi` in `backend/src/`
- **T02** Create frontend project: `npm create vite@latest frontend -- --template react-ts` in `frontend/`
- **T03** [P] Configure backend NuGet packages: `BCrypt.Net-Next`, `Microsoft.AspNetCore.Authentication.JwtBearer`, EF Core
- **T04** [P] Configure frontend packages: `react-router-dom`, `axios`, `react-hook-form`
- **T05** Configure JWT settings in `backend/src/appsettings.json` (Issuer, SecretKey, expiry)
- **T06** Configure EF Core DbContext and User/RefreshToken entities in `backend/src/Data/AppDbContext.cs`
- **T07** Run EF Core migrations to create database schema

---

## Phase 2: Foundation (Shared — blocks all US)

**Purpose**: Core auth infrastructure used by all user stories.

- **T08** [P] Create `User` entity model in `backend/src/Users/Models/User.cs` per data-model.md
- **T09** [P] Create `RefreshToken` entity model in `backend/src/Users/Models/RefreshToken.cs`
- **T10** Create `UserRepository` in `backend/src/Users/Repositories/UserRepository.cs` (find by email, update)
- **T11** [P] Create `LoginRequest` DTO in `backend/src/Auth/Models/LoginRequest.cs`
- **T12** [P] Create `LoginResponse` DTO in `backend/src/Auth/Models/LoginResponse.cs`
- **T13** Create `IAuthService` interface in `backend/src/Auth/Services/IAuthService.cs`
- **T14** Create `JwtTokenService` in `backend/src/Auth/Services/JwtTokenService.cs` (generate/validate JWT)
- **T15** [P] Register services in `backend/src/Program.cs` (DI, JWT middleware, CORS)
- **T16** [P] Create `useAuth` React hook skeleton in `frontend/src/features/auth/useAuth.ts`
- **T17** [P] Create `authApi.ts` in `frontend/src/api/authApi.ts` (axios instance, base URL from env)

---

## Phase 3: US1 — Login Exitoso (P1)

**Goal**: User can log in with valid credentials and session persists across reloads.

### Tests first (TDD gate — must fail before implementation)

- **T18** Write failing test: `AuthServiceTests.cs` — valid credentials returns access token
- **T19** Write failing test: `AuthServiceTests.cs` — successful login resets FailedAttempts to 0
- **T20** [P] Write failing test: `LoginForm.test.tsx` — submit valid form calls API and redirects

### Implementation

- **T21** Implement `AuthService.LoginAsync()` in `backend/src/Auth/Services/AuthService.cs`:
  - Verify email exists, BCrypt verify password, generate JWT + refresh token, set HttpOnly cookie
- **T22** Implement `POST /api/auth/login` in `backend/src/Auth/Controllers/AuthController.cs`
- **T23** Implement `POST /api/auth/refresh` endpoint (refresh token rotation)
- **T24** Implement `POST /api/auth/logout` endpoint (revoke refresh token, clear cookie)
- **T25** [P] Build `LoginForm.tsx` in `frontend/src/features/auth/LoginForm.tsx` (email + password fields, submit)
- **T26** [P] Build `LoginPage.tsx` in `frontend/src/features/auth/LoginPage.tsx` (wraps LoginForm)
- **T27** Implement session persistence in `useAuth.ts` (check token on mount, auto-refresh)
- **T28** [P] Build `ProtectedRoute.tsx` in `frontend/src/features/auth/ProtectedRoute.tsx` (redirect to /login if unauthenticated, preserve destination URL)
- **T29** Configure React Router routes in `frontend/src/App.tsx` (public /login, protected /*)

### Verify tests pass

- **T30** Confirm T18, T19, T20 now pass

---

## Phase 4: US2 — Credenciales Incorrectas (P2)

**Goal**: Failed logins show generic error; 5 failures lock account for 15 min.

### Tests first (TDD gate)

- **T31** Write failing test: `AuthServiceTests.cs` — wrong password returns generic error, no field hint
- **T32** Write failing test: `AuthServiceTests.cs` — 5th failed attempt sets LockoutUntil + 15 min
- **T33** [P] Write failing test: `LoginForm.test.tsx` — API 401 shows generic error message on form

### Implementation

- **T34** Extend `AuthService.LoginAsync()`: increment FailedAttempts on failure, lock on 5th attempt
- **T35** Extend `AuthController`: return 423 Locked with lockout message when account is locked
- **T36** [P] Handle 401/423 responses in `authApi.ts` and surface error in `LoginForm.tsx`

### Verify tests pass

- **T37** Confirm T31, T32, T33 now pass

---

## Phase 5: US3 — Validación de Formulario (P3)

**Goal**: Form validates fields client-side before hitting the API.

### Tests first (TDD gate)

- **T38** Write failing test: `LoginForm.test.tsx` — empty submit shows required field errors
- **T39** [P] Write failing test: `LoginForm.test.tsx` — invalid email format shows format error

### Implementation

- **T40** Add `react-hook-form` validation rules to `LoginForm.tsx` (required, email format)
- **T41** Add accessible error message display per field (WCAG 2.1 AA, aria-describedby)

### Verify tests pass

- **T42** Confirm T38, T39 now pass

---

## Dependency Graph

```
T01-T02 → T03-T07 → T08-T17 → T18-T30 (US1) → T31-T37 (US2) → T38-T42 (US3)
```

US1 must complete before US2 (lockout logic builds on login flow).  
US3 is independent of US2 but shares the LoginForm component.
