# Node.js Backend API

This is a simple Node.js Express API server that provides endpoints for users and tasks management.

## Setup

1. Install dependencies:
```bash
npm install
```

2. Start the server:
```bash
npm start
```

Or for development with auto-reload:
```bash
npm run dev
```

The server will run on `http://localhost:3000` by default.

## API Endpoints

### Health Check
- `GET /health` - Check if the server is running

### Users
- `GET /api/users` - Get all users
- `GET /api/users/:id` - Get user by ID
- `POST /api/users` - Create a new user
  - Body: `{ "name": "string", "email": "string", "role": "string" }`

### Tasks
- `GET /api/tasks` - Get all tasks (supports query params: `status`, `userId`)
- `GET /api/tasks/:id` - Get task by ID
- `POST /api/tasks` - Create a new task
  - Body: `{ "title": "string", "status": "string", "userId": number }`

### Statistics
- `GET /api/stats` - Get statistics about users and tasks

## Example Requests

```bash
# Health check
curl http://localhost:3000/health

# Get all users
curl http://localhost:3000/api/users

# Get user by ID
curl http://localhost:3000/api/users/1

# Get tasks by status
curl http://localhost:3000/api/tasks?status=pending

# Get statistics
curl http://localhost:3000/api/stats
```
