# .NET Backend Test Project

This is the **.NET (C#) developer test project** backend. It mirrors the dotnet backend's API and behavior using **ASP.NET Core minimal APIs**.

## Stack

- .NET 8.0 (SDK)
- ASP.NET Core Minimal APIs

## Project Structure

```
dotnet-backend/
├── dotnet-backend.csproj
├── Program.cs              # Minimal API setup & endpoints
├── Models/
│   ├── User.cs
│   ├── TaskItem.cs
│   └── Responses.cs
└── Data/
    └── DataStore.cs        # In-memory data store
```

## API Overview

The .NET backend exposes the same read-only endpoints as the dotnet backend:

- `GET /health` – Health check
- `GET /api/users` – Get all users
- `GET /api/users/{id}` – Get a single user by ID
- `GET /api/tasks` – Get tasks, supports filters:
  - `GET /api/tasks?status=pending`
  - `GET /api/tasks?userId=1`
  - `GET /api/tasks?status=pending&userId=1`
- `GET /api/stats` – Aggregate statistics for users and tasks

Data is stored **in-memory** with a thread-safe `DataStore`, matching the dotnet backend's sample data and behavior.

## Running the .NET Backend

From the repository root:

```bash
cd dotnet-backend
# Make sure .NET 8 SDK is installed:
#   dotnet --version

dotnet run
```

The backend will start on:

- `http://localhost:8080`

You can override the port with the `PORT` environment variable:

```bash
PORT=8081 dotnet run
```

## Endpoints (Details)

### `GET /health`
- **Response**:
  ```json
  {"status": "ok", "message": ".NET backend is running"}
  ```

### `GET /api/users`
- **Response**:
  ```json
  {
    "users": [ { "id": 1, "name": "John Doe", ... } ],
    "count": 3
  }
  ```

### `GET /api/users/{id}`
- Returns `404` with `{ "error": "User not found" }` if user is not found.

### `GET /api/tasks`
- Query params:
  - `status`: `"pending" | "in-progress" | "completed"`
  - `userId`: integer user ID

### `GET /api/stats`
- **Response**:
  ```json
  {
    "users": { "total": 3 },
    "tasks": {
      "total": 3,
      "pending": 1,
      "inProgress": 1,
      "completed": 1
    }
  }
  ```

## For .NET Candidates

As a .NET developer, you will primarily work in the `dotnet-backend/` folder.

Typical test tasks (analogous to the dotnet test) would be:
- Implement `POST /api/users` – create user
- Implement `POST /api/tasks` – create task
- Implement `PUT /api/tasks/{id}` – update task
- Add structured request logging (middleware or filters)
- (Optionally) add persistence, validation, etc.

Focus on writing clean, idiomatic C# with ASP.NET Core best practices.
