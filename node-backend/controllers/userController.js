const { makeRequest } = require('../utils/httpClient');

/**
 * Get all users
 */
const getAllUsers = async (req, res) => {
  try {
    const response = await makeRequest('/api/users');
    res.json(response);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

/**
 * Get user by ID
 */
const getUserById = async (req, res) => {
  try {
    const user = await makeRequest(`/api/users/${req.params.id}`);
    res.json(user);
  } catch (error) {
    if (error.message.includes('404') || error.message.includes('not found')) {
      res.status(404).json({ error: 'User not found' });
    } else {
      res.status(500).json({ error: error.message });
    }
  }
};

/**
 * Create a new user
 */
const createUser = async (req, res) => {
  try {
    const response = await makeRequest('/api/users', {
      method: 'POST',
      body: req.body
    });
    res.status(201).json(response);
  } catch (error) {
    const statusCode = error.statusCode || (error.message.includes('400') ? 400 : 500);
    res.status(statusCode).json({ error: error.message });
  }
};

module.exports = {
  getAllUsers,
  getUserById,
  createUser
};
