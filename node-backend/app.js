const express = require('express');
const cors = require('cors');
const config = require('./config');
const routes = require('./routes');
const { errorHandler, notFoundHandler } = require('./middleware/errorHandler');

const app = express();

// Middleware
app.use(cors());
app.use(express.json());

// Routes
app.use(routes);

// Error handling
app.use(errorHandler);
app.use(notFoundHandler);

// Start server
app.listen(config.PORT, () => {
  console.log(`Node.js backend server running on http://localhost:${config.PORT}`);
  console.log(`Connecting to dotNet backend at ${config.GO_BACKEND_URL}`);
  console.log(`Health check: http://localhost:${config.PORT}/health`);
});

module.exports = app;
