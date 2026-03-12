const { makeRequest } = require('../utils/httpClient');

/**
 * Get statistics
 */
const getStats = async (req, res) => {
  try {
    const stats = await makeRequest('/api/stats');
    res.json(stats);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

module.exports = {
  getStats
};
