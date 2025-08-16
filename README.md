# BaseStarterPack (.NET 8) – Clean Architecture + JWT & Refresh Tokens + CRUD

**Runs with ZERO edits** (EF InMemory). Switch to SQL Server by setting `ConnectionStrings:DefaultConnection`.

### Auth
- `/api/auth/register`
- `/api/auth/login`
- `/api/auth/refresh-token`
- `/api/auth/forgot-password` (stub; returns 200)
- `/api/auth/me` (requires Bearer token)

### CRUD (Clinics)
- `/api/clinics` (GET/POST/PUT/DELETE)

### Default creds (seeded)
- email: `admin@base.local`
- password: `Admin123!`

### Run
Open the solution in Visual Studio 2022+, set **BaseStarterPack.API** as startup, F5, open `/` for Swagger.
