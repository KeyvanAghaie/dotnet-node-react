# .NET Candidate Checklist

Use this checklist to track your progress through the **.NET developer** test.

## Phase 1: Setup (30 minutes)

- [ ] Forked/cloned the repository or set up `dotnet-test/`
- [ ] Installed .NET 8 SDK
- [ ] Installed Node.js 16+
- [ ] Installed dependencies (`npm install` in `node-backend` and `react-frontend`)
- [ ] Restored .NET dependencies (`dotnet restore`)
- [ ] Started .NET backend on port 8080
- [ ] Started Node.js backend on port 3000
- [ ] Started React frontend on port 5173
- [ ] Verified application works end-to-end
- [ ] Tested existing endpoints manually

## Phase 2: Core Requirements (2–3 hours)

### User Creation Endpoint (`POST /api/users`)
- [ ] Added endpoint in .NET backend
- [ ] Validates `name`, `email`, `role` fields
- [ ] Validates email format
- [ ] Generates unique ID
- [ ] Returns 201 with created user
- [ ] Returns 400 for invalid input
- [ ] New users appear in `GET /api/users`

### Task Creation Endpoint (`POST /api/tasks`)
- [ ] Added endpoint in .NET backend
- [ ] Validates `title`, `status`, `userId` fields
- [ ] Validates status enum (`pending` / `in-progress` / `completed`)
- [ ] Validates `userId` exists
- [ ] Generates unique ID
- [ ] Returns 201 with created task
- [ ] Returns 400 for invalid input
- [ ] New tasks appear in `GET /api/tasks`

### Task Update Endpoint (`PUT /api/tasks/{id}`)
- [ ] Added endpoint in .NET backend
- [ ] Supports partial updates
- [ ] Validates `status` if provided
- [ ] Validates `userId` if provided
- [ ] Returns 200 with updated task
- [ ] Returns 404 if task not found
- [ ] Returns 400 for invalid input

### Request Logging
- [ ] Added logging for all .NET backend requests
- [ ] Logs HTTP method
- [ ] Logs request path
- [ ] Logs response status code
- [ ] Logs response time/duration
- [ ] Logs errors with context
- [ ] Logs are readable and consistent

## Phase 3: Advanced Requirements (Optional)

- [ ] Data persistence (e.g., database or JSON file)
- [ ] Caching layer with expiration
- [ ] Request validation middleware
- [ ] Enhanced health check
- [ ] Other advanced features

## Phase 4: Code Quality

### Testing
- [ ] Unit tests for data store/service
- [ ] Integration tests for HTTP endpoints
- [ ] Test error cases
- [ ] Test edge cases

### Code Organization
- [ ] Follows C# and ASP.NET Core naming conventions
- [ ] Handlers/methods are focused and single-purpose
- [ ] Meaningful comments for complex logic
- [ ] No unused code

### Error Handling
- [ ] Appropriate HTTP status codes
- [ ] Meaningful error messages
- [ ] Errors logged with context
- [ ] Internal errors not exposed to clients

### Documentation
- [ ] XML doc comments on key methods/types
- [ ] API endpoint documentation
- [ ] Updated README
- [ ] Design decisions documented

## Phase 5: Bonus Tasks (Optional)

- [ ] Authentication/API keys
- [ ] Rate limiting
- [ ] Metrics/observability
- [ ] Database integration

## Submission

- [ ] All required features implemented
- [ ] Code builds and runs without errors
- [ ] All services run successfully
- [ ] Tests written and passing (if added)
- [ ] Documentation updated
- [ ] Code review notes (optional)

## Notes

Use this space to track any issues, questions, or design decisions:

```
[Your notes here]
```
