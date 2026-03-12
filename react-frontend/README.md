# React Frontend

Modern React frontend built with Vite that consumes the dotnet backend API.

## Setup

1. Install dependencies:
```bash
npm install
```

2. Start the development server:
```bash
npm run dev
```

The frontend will run on `http://localhost:5173` by default.

## Configuration

The frontend connects to the Node.js backend at `http://localhost:3000` by default.

You can override this by creating a `.env` file:
```
VITE_API_URL=http://localhost:3000
```

**Note:** The Node.js backend then calls the dotnet backend, creating the flow: React → Node.js → dotnet

## Features

- ✅ Health status monitoring
- 👥 User list with selection
- 📋 Task list with filtering
- 📊 Statistics dashboard
- 🎨 Modern, responsive UI
- ⚡ Fast development with Vite

## Build for Production

```bash
npm run build
```

The built files will be in the `dist` directory.
