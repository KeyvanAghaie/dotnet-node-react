# .NET Developer Test Project

This project is designed to test **.NET (C#) developers'** skills in building an ASP.NET Core backend that powers a React + Node.js application.

## 📚 Documentation

- **[Getting Started](./GETTING_STARTED.md)** – .NET test setup guide
- **[Test Requirements](./TEST_REQUIREMENTS.md)** – Complete .NET test requirements and evaluation criteria
- **[Test Summary](./TEST_SUMMARY.md)** – Quick overview of required tasks
- **[Candidate Checklist](./CANDIDATE_CHECKLIST.md)** – Track your progress

## Project Structure

```
dotnet-test/
├── dotnet-backend/   # ASP.NET Core minimal API (data source)
├── node-backend/     # Node.js Express API server (calls .NET backend)
└── react-frontend/   # React frontend (calls Node.js backend)
```

## Architecture Flow

```
React Frontend (port 5173)
    ↓
Node.js Backend (port 3000)
    ↓
.NET Backend (port 8080)
```

## Quick Start

**Important:** Start services in this order (inside `dotnet-test/`):

### 1. Start .NET Backend (Data Source)

```bash
cd dotnet-backend
update database
dotnet run
```

The .NET backend will run on `http://localhost:8080` and serves as the data source.

### 2. Start Node.js Backend (API Gateway)

```bash
cd node-backend
npm install
npm start
```

### 3. Start React Frontend

```bash
cd react-frontend
npm install
npm run dev
```

## Test Requirements

See [TEST_REQUIREMENTS.md](./TEST_REQUIREMENTS.md) for details.
