import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:3000'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor for logging
apiClient.interceptors.request.use(
  (config) => {
    console.log(`Making ${config.method.toUpperCase()} request to ${config.url}`)
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      // Server responded with error status
      throw new Error(
        error.response.data?.error || 
        error.response.data?.message || 
        `Request failed with status ${error.response.status}`
      )
    } else if (error.request) {
      // Request was made but no response received
      throw new Error('No response from server. Is the Node.js backend running?')
    } else {
      // Something else happened
      throw new Error(error.message || 'An unexpected error occurred')
    }
  }
)

export const checkHealth = async () => {
  try {
    const response = await apiClient.get('/health')
    return response.data
  } catch (error) {
    throw new Error(`Health check failed: ${error.message}`)
  }
}

export const getUsers = async () => {
  const response = await apiClient.get('/api/users')
  return response.data
}

export const getUserById = async (id) => {
  const response = await apiClient.get(`/api/users/${id}`)
  return response.data
}

export const getTasks = async (status = '', userId = '') => {
  const params = {}
  if (status) params.status = status
  if (userId) params.userId = userId
  
  const response = await apiClient.get('/api/tasks', { params })
  return response.data
}

export const getTaskById = async (id) => {
  const response = await apiClient.get(`/api/tasks/${id}`)
  return response.data
}

export const getStats = async () => {
  const response = await apiClient.get('/api/stats')
  return response.data
}

export const createUser = async (userData) => {
  const response = await apiClient.post('/api/users', userData)
  return response.data
}

export const createTask = async (taskData) => {
  const response = await apiClient.post('/api/tasks', taskData)
  return response.data
}

export const updateTask = async (id, taskData) => {
  const response = await apiClient.put(`/api/tasks/${id}`, taskData)
  return response.data
}
