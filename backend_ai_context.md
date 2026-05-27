# Momentum - Backend AI Context Document

This document is designed to provide context, architectural patterns, and strict guidelines for any AI coding assistant interacting with the Momentum (formerly Lifenote) Backend. Read this entirely before proposing or making changes.

## 1. Tech Stack
- **Framework:** .NET 10, ASP.NET Core Web API
- **Language:** C#
- **ORM:** Entity Framework Core (EF Core)
- **Database:** PostgreSQL
- **Authentication:** Firebase Admin SDK (JWT Bearer Token validation)

## 2. Architecture: Clean Architecture
The backend is strictly divided into four layers to ensure separation of concerns. Dependencies flow inwards toward the Domain.

1. **Lifenote.Domain:** (Core)
   - Contains Enterprise logic: Entities, Enums, Value Objects, and Domain exceptions.
   - *Rule:* Must have ZERO external dependencies (no EF Core, no third-party libraries).
   - *Pattern:* Entities should inherit from `BaseEntity<Guid>` or `AggregateRoot<Guid>`.

2. **Lifenote.Application:** (Use Cases)
   - Contains Application logic: Services, DTOs, and Interfaces (`Contracts`).
   - *Rule:* Can reference `Domain`, but knows nothing about the Database or API.
   - *Pattern:* Interfaces (e.g., `INoteService`) define the contract. Implementations orchestrate the fetching of data, mapping to DTOs, and returning to the caller.

3. **Lifenote.Infrastructure:** (External Concerns)
   - Contains implementations for interfaces defined in Application: EF Core DbContext (`LifenoteDbContext`), Entity Configurations, Firebase services, Cache services.
   - *Rule:* This is where database dependencies and external SDKs belong.

4. **Lifenote.API:** (Presentation)
   - Contains ASP.NET Core Controllers, Middlewares (Global Exception Handling), and request/response mapping extensions.
   - *Rule:* Controllers should be extremely thin. They inject `Application` services, parse claims via `ApiControllerBase`, and return standardized `ApiResponse<T>`.

## 3. Data & Typing Rules (CRITICAL)
- **Identifiers:** ALL identifiers (Primary Keys, Foreign Keys) are `Guid`. **Do NOT use `int` for IDs.** The database schema relies on PostgreSQL `uuid`.
- **Database Migrations:** Never attempt to generate migrations that alter `Guid` constraints manually via RAW SQL unless explicitly approved. Do not drop the database without explicit user confirmation. EF Core commands should be run against `Lifenote.Infrastructure`.

## 4. Security & Authentication
- **Current User Resolution:** All controllers inherit from `ApiControllerBase`, which provides `GetUserIdAsync() -> Task<Guid>`. Use this to enforce tenant isolation. 
- **Tenant Isolation:** Every service method fetching data must accept `Guid userId` and append `.Where(x => x.UserId == userId)` to ensure users cannot fetch other users' data.
- **Firebase Mapping:** The Firebase JWT is decoded, and a custom claim mapping maps the Firebase UID to the internal `UserInfo.Id` (Guid).

## 5. API Standards
- **Global Error Handling:** Do not wrap controller logic in `try/catch` blocks. The custom `ExceptionHandlingMiddleware` intercepts all exceptions and formats them uniformly.
- **Response Format:** All endpoints must return `ApiResponse<T>`.
  - *Success:* `return Ok(ApiResponse<T>.Success(data));`
  - *Failure:* `return NotFound(ApiResponse<T>.Fail("Message"));`

## 6. Common Pitfalls to Avoid
- **Rebranding:** The app was renamed from "Lifenote" to "Momentum". The UI says Momentum, but the namespaces remain `Lifenote.*` to avoid massive file-level renames breaking the build. Do not attempt to rename the C# namespaces unless specifically requested.
- **Async/Await:** Always use `async/await` completely through the stack. Do not use `.Result` or `.Wait()`.
- **Dto Mapping:** Map domain Entities to DTOs in the Application layer before returning them to the API layer to prevent leaking database schema details or cyclic JSON serialization issues.
