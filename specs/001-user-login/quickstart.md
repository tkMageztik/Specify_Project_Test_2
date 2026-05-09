# Quickstart: User Login

## Backend (.NET 10)

```bash
cd backend
dotnet restore
dotnet run --project src
# API available at https://localhost:5001
```

Key endpoints: `POST /api/auth/login`, `POST /api/auth/refresh`, `POST /api/auth/logout`

## Frontend (React)

```bash
cd frontend
npm install
npm run dev
# App available at http://localhost:5173
```

Login page at `/login`. Protected routes redirect to `/login` when unauthenticated.

## Environment Variables

**Backend** (`backend/appsettings.Development.json`):
- `Jwt:SecretKey` — signing key (min 32 chars)
- `Jwt:Issuer` — token issuer
- `ConnectionStrings:Default` — database connection

**Frontend** (`.env.local`):
- `VITE_API_BASE_URL` — backend base URL
