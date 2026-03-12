const { makeRequest } = require('../utils/httpClient');

/**
 * Get all tasks with optional filters
 */
const getAllTasks = async (req, res) => {
  try {
    const { status, userId } = req.query;
    let path = '/api/tasks';
    const params = new URLSearchParams();
    if (status) params.append('status', status);
    if (userId) params.append('userId', userId);
    if (params.toString()) {
      path += '?' + params.toString();
    }
    const response = await makeRequest(path);
    res.json(response);
  } catch (error) {
    res.status(500).json({ error: error.message });
  }
};

/**
 * Create a new task
 */
const createTask = async (req, res) => {
  try {
    const response = await makeRequest('/api/tasks', {
      method: 'POST',
      body: req.body
    });
    res.status(201).json(response);
  } catch (error) {
    const statusCode = error.statusCode || (error.message.includes('400') ? 400 : 500);
    res.status(statusCode).json({ error: error.message });
  }
};

/**
 * Update a task by ID
 */
const updateTask = async (req, res) => {
  try {
    const response = await makeRequest(`/api/tasks/${req.params.id}`, {
      method: 'PUT',
      body: req.body
    });
    res.json(response);
  } catch (error) {
    const statusCode = error.statusCode || 
      (error.message.includes('404') || error.message.includes('not found') ? 404 :
       error.message.includes('400') ? 400 : 500);
    res.status(statusCode).json({ error: error.message });
  }
};

module.exports = {
  getAllTasks,
  createTask,
  updateTask
};
