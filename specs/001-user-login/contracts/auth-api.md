# API Contract: Authentication

**Feature**: 001-user-login | **Base path**: `/api/auth`

## POST /api/auth/login

**Description**: Authenticate a user with email and password.

**Request**:
```json
{
  "email": "user@example.com",
  "password": "string"
}
```

**Response 200 OK**:
```json
{
  "accessToken": "eyJ...",
  "expiresIn": 900
}
```
Refresh token is set as `HttpOnly` cookie (`refresh_token`, 8h expiry).

**Response 401 Unauthorized**:
```json
{
  "error": "Invalid email or password."
}
```

**Response 423 Locked**:
```json
{
  "error": "Account temporarily locked. Try again in 15 minutes."
}
```

---

## POST /api/auth/refresh

**Description**: Obtain a new access token using the refresh token cookie.

**Request**: No body — refresh token read from `HttpOnly` cookie.

**Response 200 OK**:
```json
{
  "accessToken": "eyJ...",
  "expiresIn": 900
}
```

**Response 401 Unauthorized**: Cookie missing or expired.

---

## POST /api/auth/logout

**Description**: Invalidate the current session.

**Request**: No body — refresh token read from `HttpOnly` cookie.

**Response 204 No Content**: Session cleared, cookie removed.
