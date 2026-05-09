# Data Model: User Login

**Feature**: 001-user-login | **Date**: 2026-05-08

## Entities

### User

| Field | Type | Constraints | Notes |
|---|---|---|---|
| Id | GUID | PK | Auto-generated |
| Email | string | Unique, Required, max 254 | Normalized to lowercase |
| PasswordHash | string | Required | BCrypt hash |
| Status | enum | Active / Locked | Default: Active |
| FailedAttempts | int | >= 0 | Reset on successful login |
| LockoutUntil | DateTime? | Nullable | Set when locked, null when active |
| CreatedAt | DateTime | Required | UTC |
| UpdatedAt | DateTime | Required | UTC, auto-updated |

### RefreshToken

| Field | Type | Constraints | Notes |
|---|---|---|---|
| Id | GUID | PK | Auto-generated |
| UserId | GUID | FK → User.Id | Cascade delete |
| Token | string | Required, Unique | Cryptographically random |
| ExpiresAt | DateTime | Required | UTC, 8h from creation |
| RevokedAt | DateTime? | Nullable | Set on logout or rotation |

## State Transitions

```
User.Status:
  Active → Locked   (5th failed login attempt)
  Locked → Active   (LockoutUntil passed + successful login)
```

## Validation Rules

- Email: valid format, max 254 chars, unique in system
- Password: min 8 chars (enforced at registration, not login)
- FailedAttempts: reset to 0 on successful login
- LockoutUntil: set to `now + 15 minutes` on 5th failed attempt
