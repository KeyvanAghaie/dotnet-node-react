const { makeRequest } = require('../utils/httpClient');

/**
 * Health check endpoint
 */
const getHealth = async (req, res) => {
  try {
    // Check dotNet backend health
    const goHealth = await makeRequest('/health');
    res.json({ 
      status: 'ok', 
      message: 'Node.js backend is running',
      goBackend: goHealth
    });
  } catch (error) {
    res.status(503).json({ 
      status: 'error', 
      message: 'Node.js backend is running but dotNet backend is unavailable',
      error: error.message
    });
  }
};

module.exports = {
  getHealth
};
