# Lifenote — Clean Architecture Migration

**Branch:** `refactor/clean-architecture-05April2026`  
**Date:** 05 April 2026  
**Author:** Ashutosh Choudhary

---

## Summary

This branch migrates the Lifenote backend from a 3-layer architecture (`Core` / `Data` / `API`) to a strict **Clean Architecture** with four layers:

```
Lifenote.Domain          ← NEW project
Lifenote.Application     ← NEW project (replaces Lifenote.Core)
Lifenote.Infrastructure  ← NEW project (replaces Lifenote.Data)
Lifenote.API             ← KEPT, cleaned up
```

---

## Old Architecture (Problems)

```
Lifenote.Core/
  Models/          ← Domain entities — CORRECT
  DTOs/            ← belonged in Application, not Core
  Interfaces/      ← mixed Domain + Application concerns

Lifenote.Data/
  Data/            ← DbContext — OK
  Repositories/    ← OK
  Services/        ← ❌ Service classes (business logic) in the Data/DB project

Lifenote.API/
  Services/        ← ❌ IFirebaseClaimService interface defined in API
  Controllers/     ← ❌ Fat try/catch blocks in every controller
  Program.cs       ← ❌ 70+ lines, every registration inline
```

### Specific violations

| File | Problem |
|------|---------|
| `Lifenote.Data/Services/HabitService.cs` | Business logic (streak calc, check-in rules) lived in the Data project — depended on EF Core transitively |
| `Lifenote.Data/Services/NoteService.cs` | Same issue |
| `Lifenote.Data/Services/UserInfoService.cs` | Same issue |
| `Lifenote.Data/Services/HabitStreakService.cs` | Streak calculation (pure business logic) lived next to EF Core repositories |
| `Lifenote.API/Services/IFirebaseClaimService.cs` | Interface defined in outermost layer — should be an abstraction in Application |
| `HabitsController.cs` | 5 separate try/catch blocks duplicating error-handling logic |
| `Program.cs` | 20+ `AddScoped` calls inline — no DI extension methods |

---

## New Architecture

### Dependency Direction

```
Lifenote.API
  └── depends on → Lifenote.Application + Lifenote.Infrastructure

Lifenote.Infrastructure
  └── depends on → Lifenote.Application + Lifenote.Domain

Lifenote.Application
  └── depends on → Lifenote.Domain

Lifenote.Domain
  └── depends on → nothing (zero external packages)
```

---

## File-by-File Migration Log

### 1. Lifenote.Domain (NEW project)

All domain entities moved from `Lifenote.Core/Models/` → `Lifenote.Domain/Entities/`.

| Old path | New path | Notes |
|----------|----------|-------|
| `Lifenote.Core/Models/Habit.cs` | `Lifenote.Domain/Entities/Habit.cs` | Namespace: `Lifenote.Core.Models` → `Lifenote.Domain.Entities` |
| `Lifenote.Core/Models/HabitLog.cs` | `Lifenote.Domain/Entities/HabitLog.cs` | Same |
| `Lifenote.Core/Models/HabitStreak.cs` | `Lifenote.Domain/Entities/HabitStreak.cs` | Same |
| `Lifenote.Core/Models/Note.cs` | `Lifenote.Domain/Entities/Note.cs` | Same |
| `Lifenote.Core/Models/UserInfo.cs` | `Lifenote.Domain/Entities/UserInfo.cs` | Same |
| `Lifenote.Core/Models/FocusSession.cs` | `Lifenote.Domain/Entities/FocusSession.cs` | Same |

**Why:** Domain entities have no external dependencies. Separating them into their own project enforces this at compile time — if someone accidentally adds `using Microsoft.EntityFrameworkCore;` here, the build fails.

---

### 2. Lifenote.Application (NEW project)

Replaces `Lifenote.Core`. Contains: interfaces (contracts), DTOs, service classes, DI registration.

#### 2a. Interfaces — moved from `Lifenote.Core/Interfaces/`

| Old path | New path | Change |
|----------|----------|--------|
| `Lifenote.Core/Interfaces/IHabitRepository.cs` | `Lifenote.Application/Contracts/IHabitRepository.cs` | Namespace updated |
| `Lifenote.Core/Interfaces/IHabitStreakRepository.cs` | `Lifenote.Application/Contracts/IHabitStreakRepository.cs` | Namespace updated |
| `Lifenote.Core/Interfaces/INoteRepository.cs` | `Lifenote.Application/Contracts/INoteRepository.cs` | Namespace updated |
| `Lifenote.Core/Interfaces/IUserInfoRepository.cs` | `Lifenote.Application/Contracts/IUserInfoRepository.cs` | Namespace updated |
| `Lifenote.Core/Interfaces/IUnitOfWork.cs` | `Lifenote.Application/Contracts/IUnitOfWork.cs` | Added `HabitStreaks` property |
| `Lifenote.Core/Interfaces/IHabitService.cs` | `Lifenote.Application/Contracts/IHabitService.cs` | Namespace updated |
| `Lifenote.Core/Interfaces/IHabitStreakService.cs` | `Lifenote.Application/Contracts/IHabitStreakService.cs` | Namespace updated |
| `Lifenote.Core/Interfaces/INoteService.cs` | `Lifenote.Application/Contracts/INoteService.cs` | Namespace updated |
| `Lifenote.Core/Interfaces/IUserInfoService.cs` | `Lifenote.Application/Contracts/IUserInfoService.cs` | Namespace updated |
| `Lifenote.Core/Interfaces/ICurrentUserService.cs` | `Lifenote.Application/Contracts/ICurrentUserService.cs` | Namespace updated |
| `Lifenote.API/Services/IFirebaseClaimService.cs` | `Lifenote.Application/Contracts/IFirebaseClaimService.cs` | **Moved from API → Application** |

**Key change — IFirebaseClaimService:** The interface was defined in `Lifenote.API/Services/`, meaning the outermost layer owned an abstraction. Moved to `Application.Contracts` so the Application layer can use it without depending on the API project.

#### 2b. DTOs — moved from `Lifenote.Core/DTOs/`

| Old path | New path |
|----------|----------|
| `Lifenote.Core/DTOs/Habit/` | `Lifenote.Application/DTOs/Habit/HabitDto.cs` (consolidated) |
| `Lifenote.Core/DTOs/Note/` | `Lifenote.Application/DTOs/Note/NoteDto.cs` (consolidated) |
| `Lifenote.Core/DTOs/UserInfo/` | `Lifenote.Application/DTOs/UserInfo/UserInfoDtos.cs` (consolidated) |

#### 2c. Services — moved from `Lifenote.Data/Services/`

This is the most important structural fix. Service classes (business logic) were in the `Data` project. They now live in `Application`.

| Old path | New path | Why |
|----------|----------|-----|
| `Lifenote.Data/Services/HabitService.cs` | `Lifenote.Application/Services/HabitService.cs` | Business logic: check-in rules, streak delegation, mapping — not DB concerns |
| `Lifenote.Data/Services/HabitStreakService.cs` | `Lifenote.Application/Services/HabitStreakService.cs` | Streak calculation is pure business logic |
| `Lifenote.Data/Services/NoteService.cs` | `Lifenote.Application/Services/NoteService.cs` | Note pinning/archiving logic |
| `Lifenote.Data/Services/UserInfoService.cs` | `Lifenote.Application/Services/UserInfoService.cs` | User profile management |

**Logic preserved 100%.** All service method signatures, validation rules, streak calculation, check-in logic, and weekly calendar calculation are identical — only the namespace and project changed.

#### 2d. DI Extension Method (NEW)

`Lifenote.Application/DependencyInjection.cs` — `AddApplication()` extension method registers all service classes. Called from `Program.cs`.

---

### 3. Lifenote.Infrastructure (NEW project — replaces Lifenote.Data)

All EF Core code, repositories, and external service implementations live here.

#### 3a. Persistence

| Old path | New path | Change |
|----------|----------|--------|
| `Lifenote.Data/Data/LifenoteDbContext.cs` | `Lifenote.Infrastructure/Persistence/LifenoteDbContext.cs` | Namespace updated |
| *(EF config was inline in Core entities)* | `Lifenote.Infrastructure/Persistence/Configurations/*.cs` | **NEW** — `IEntityTypeConfiguration<T>` classes per entity. EF annotations removed from Domain entities. |

**Why separate configuration classes?** Previously, EF Core annotations (column names, constraints) were mixed into the domain models in `Lifenote.Core/Models/`. This violates the Dependency Rule — Domain should not know about EF Core. Now each entity has a dedicated `XxxConfiguration : IEntityTypeConfiguration<Xxx>` class in Infrastructure, and `modelBuilder.ApplyConfigurationsFromAssembly()` auto-discovers them.

#### 3b. Repositories

| Old path | New path |
|----------|----------|
| `Lifenote.Data/Repositories/HabitRepository.cs` | `Lifenote.Infrastructure/Repositories/HabitRepository.cs` |
| `Lifenote.Data/Repositories/HabitStreakRepository.cs` | `Lifenote.Infrastructure/Repositories/HabitStreakRepository.cs` |
| `Lifenote.Data/Repositories/NoteRepository.cs` | `Lifenote.Infrastructure/Repositories/NoteRepository.cs` |
| `Lifenote.Data/Repositories/UserInfoRepository.cs` | `Lifenote.Infrastructure/Repositories/UserInfoRepository.cs` |
| `Lifenote.Data/Repositories/UnitOfWork.cs` | `Lifenote.Infrastructure/Repositories/UnitOfWork.cs` |

All repository logic preserved. `AsNoTracking()` added on all read-only queries for performance.

#### 3c. Services

| Old path | New path | Why |
|----------|----------|-----|
| `Lifenote.API/Services/CurrentUserService.cs` | `Lifenote.Infrastructure/Services/CurrentUserService.cs` | Depends on `IHttpContextAccessor` + `IMemoryCache` + DB — these are infrastructure concerns |
| `Lifenote.API/Services/FirebaseClaimService.cs` | `Lifenote.Infrastructure/Services/FirebaseClaimService.cs` | Firebase Admin SDK is an external dependency — belongs in Infrastructure |

#### 3d. DI Extension Method (NEW)

`Lifenote.Infrastructure/DependencyInjection.cs` — `AddInfrastructure(configuration)` extension method registers DbContext, all repositories, UoW, and infrastructure services.

---

### 4. Lifenote.API (cleaned up)

| Change | Detail |
|--------|--------|
| `Program.cs` reduced from 70+ lines to ~50 lines | Replaced 15+ `AddScoped` calls with `builder.Services.AddApplication()` and `builder.Services.AddInfrastructure(builder.Configuration)` |
| `ExceptionHandlingMiddleware` enabled | Was commented out (`//app.UseMiddleware<ExceptionHandlingMiddleware>()`). Now registered as the first middleware. |
| All try/catch removed from `HabitsController.cs` | 5 try/catch blocks removed. `KeyNotFoundException`, `ArgumentException`, `InvalidOperationException` all handled by the global middleware. |
| All try/catch removed from `NoteController.cs` | Same. |
| Controller namespaces updated | `Lifenote.Core.DTOs.Habit` → `Lifenote.Application.DTOs.Habit`, etc. |
| `IFirebaseClaimService` injected from `Application.Contracts` | No longer referencing `Lifenote.API.Services` namespace |

---

## Solution File Update Required

You need to update `backend/Lifenote.sln` locally to replace the old projects with the new ones:

```bash
# Remove old projects
dotnet sln backend/Lifenote.sln remove backend/Lifenote.Core/Lifenote.Core.csproj
dotnet sln backend/Lifenote.sln remove backend/Lifenote.Data/Lifenote.Data.csproj

# Add new projects
dotnet sln backend/Lifenote.sln add backend/Lifenote.Domain/Lifenote.Domain.csproj
dotnet sln backend/Lifenote.sln add backend/Lifenote.Application/Lifenote.Application.csproj
dotnet sln backend/Lifenote.sln add backend/Lifenote.Infrastructure/Lifenote.Infrastructure.csproj
```

---

## What Was NOT Changed

- All **HTTP routes** remain identical — Angular frontend requires no changes
- All **business logic** — check-in rules, streak calculation, weekly calendar — is preserved exactly
- All **database queries** — EF Core queries in repositories are identical
- `appsettings.json` and `appsettings.Development.json` — unchanged
- `SettingsController.cs` and `TimerController.cs` — kept as-is (stub endpoints)
- Firebase authentication middleware — unchanged
- CORS policy — unchanged
- The PostgreSQL schema — unchanged (no model property changes, only namespace moves)

---

## How to Run After Migration

```bash
cd backend

# Restore packages
dotnet restore

# Build
dotnet build

# Run API
dotnet run --project Lifenote.API/Lifenote.API.csproj
```

Migrations remain in `Lifenote.Data` (old project) for now. Once `Lifenote.Core` and `Lifenote.Data` are removed from the solution, re-point migrations:

```bash
dotnet ef migrations add PostRefactor \
  --project backend/Lifenote.Infrastructure \
  --startup-project backend/Lifenote.API
```

---

## Architecture Diagram (After)

```
┌────────────────────────────────────────────────────┐
│                  Lifenote.API                      │
│  Controllers, Program.cs, Middleware               │
│  → AddApplication() + AddInfrastructure()          │
└──────────────┬────────────────┬───────────────────┘
               │                │
               ▼                ▼
┌─────────────────────┐  ┌─────────────────────────┐
│ Lifenote.Application│  │ Lifenote.Infrastructure  │
│ Contracts/          │  │ Persistence/DbContext     │
│ Services/           │  │ Repositories/             │
│ DTOs/               │  │ Services/ (Firebase,      │
│ DependencyInjection │  │   CurrentUser)            │
└──────────┬──────────┘  │ DependencyInjection       │
           │              └──────────┬────────────────┘
           │                         │
           └──────────┬──────────────┘
                      ▼
           ┌───────────────────┐
           │  Lifenote.Domain  │
           │  Entities/        │
           │  (zero deps)      │
           └───────────────────┘
```
