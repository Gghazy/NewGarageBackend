# Cashif – .NET 9 Clean Architecture (DDD) + SQL Server + JWT (v8) + JSON Localization

## Quick Start
1. افتح `Cashif.sln` في Visual Studio 2022.
2. عدّل `src/Cashif.Api/appsettings.json` (ConnectionStrings + Jwt:Key).
3. من Package Manager Console:
   - Default project = `Cashif.Infrastructure`
   - `Add-Migration Init -StartupProject Cashif.Api`
   - `Update-Database -StartupProject Cashif.Api`
4. Run **Cashif.Api** وادخل Swagger.

### حسابات تجريبية
- Admin: `admin@cashif.local` / `Admin#12345`
- User : `user@cashif.local` / `User#12345`

## Localization (JSON)
- ملفات `ar.json` و `en.json` في `src/Cashif.Api/Localization/Resources`.
- غيّر اللغة بـ `?culture=en` أو هيدر `Accept-Language`.

## Endpoints
- `POST /api/auth/register`, `POST /api/auth/login`
- `GET /api/users` (users.read)
- `/api/branches` CRUD مع صلاحيات `branches.*`
