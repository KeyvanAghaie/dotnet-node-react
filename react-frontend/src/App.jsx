import { useState, useEffect } from 'react'
import './App.css'
import { getUsers, getUserById, getTasks, getStats, checkHealth, createUser, createTask, updateTask } from './services/api'
import UserList from './components/UserList'
import TaskList from './components/TaskList'
import Stats from './components/Stats'
import HealthStatus from './components/HealthStatus'
import CreateUserForm from './components/CreateUserForm'
import CreateTaskForm from './components/CreateTaskForm'
import UpdateTaskForm from './components/UpdateTaskForm'

function App() {
  const [users, setUsers] = useState([])
  const [tasks, setTasks] = useState([])
  const [stats, setStats] = useState(null)
  const [health, setHealth] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const [selectedUserId, setSelectedUserId] = useState(null)
  const [selectedUser, setSelectedUser] = useState(null)
  const [taskFilter, setTaskFilter] = useState('')
  const [showCreateUser, setShowCreateUser] = useState(false)
  const [showCreateTask, setShowCreateTask] = useState(false)
  const [taskToEdit, setTaskToEdit] = useState(null)

  useEffect(() => {
    loadInitialData()
  }, [])

  const loadInitialData = async () => {
    setLoading(true)
    setError(null)
    try {
      // Check health first
      const healthData = await checkHealth()
      setHealth(healthData)

      // Load data in parallel
      const [usersData, tasksData, statsData] = await Promise.all([
        getUsers(),
        getTasks(),
        getStats()
      ])

      setUsers(usersData.users || [])
      setTasks(tasksData.tasks || [])
      setStats(statsData)
    } catch (err) {
      setError(err.message || 'Failed to load data')
      console.error('Error loading data:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleUserSelect = async (userId) => {
    setSelectedUserId(userId)
    setLoading(true)
    setError(null)
    try {
      const user = await getUserById(userId)
      setSelectedUser(user)
      // Also filter tasks for this user
      const userTasks = await getTasks('', userId.toString())
      setTasks(userTasks.tasks || [])
    } catch (err) {
      setError(err.message || 'Failed to load user details')
      console.error('Error loading user:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleTaskFilter = async (status) => {
    setTaskFilter(status)
    setLoading(true)
    setError(null)
    try {
      const tasksData = await getTasks(status, '')
      setTasks(tasksData.tasks || [])
    } catch (err) {
      setError(err.message || 'Failed to filter tasks')
      console.error('Error filtering tasks:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleRefresh = () => {
    setSelectedUserId(null)
    setSelectedUser(null)
    setTaskFilter('')
    loadInitialData()
  }

  const handleCreateUser = async (userData) => {
    setLoading(true)
    setError(null)
    try {
      await createUser(userData)
      setShowCreateUser(false)
      await loadInitialData()
    } catch (err) {
      setError(err.message || 'Failed to create user')
      console.error('Error creating user:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleCreateTask = async (taskData) => {
    setLoading(true)
    setError(null)
    try {
      await createTask(taskData)
      setShowCreateTask(false)
      await loadInitialData()
    } catch (err) {
      setError(err.message || 'Failed to create task')
      console.error('Error creating task:', err)
    } finally {
      setLoading(false)
    }
  }

  const handleUpdateTask = async (taskId, taskData) => {
    setLoading(true)
    setError(null)
    try {
      await updateTask(taskId, taskData)
      setTaskToEdit(null)
      await loadInitialData()
    } catch (err) {
      setError(err.message || 'Failed to update task')
      console.error('Error updating task:', err)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="app">
      <header className="app-header">
        <h1>dotnet Developer Test Project</h1>
        <p>React Frontend → Node.js Backend → dotnet Backend</p>
      </header>

      <HealthStatus health={health} />

      {error && (
        <div className="error-banner">
          <span>⚠️ {error}</span>
          <button onClick={handleRefresh}>Retry</button>
        </div>
      )}

      <div className="main-content">
        <div className="stats-section">
          {stats && <Stats stats={stats} />}
        </div>

        <div className="data-section">
          <div className="panel">
            <div className="panel-header">
              <h2>Users</h2>
              <button 
                className="btn-create"
                onClick={() => setShowCreateUser(true)}
              >
                + Create User
              </button>
            </div>
            {loading && !users.length ? (
              <div className="loading">Loading users...</div>
            ) : (
              <UserList
                users={users}
                selectedUserId={selectedUserId}
                onUserSelect={handleUserSelect}
              />
            )}
            {selectedUser && (
              <div className="user-details">
                <h3>Selected User Details</h3>
                <div className="detail-card">
                  <p><strong>Name:</strong> {selectedUser.name}</p>
                  <p><strong>Email:</strong> {selectedUser.email}</p>
                  <p><strong>Role:</strong> {selectedUser.role}</p>
                </div>
              </div>
            )}
          </div>

          <div className="panel">
            <div className="panel-header">
              <h2>Tasks</h2>
              <div className="panel-header-actions">
                <div className="filter-buttons">
                  <button
                    className={taskFilter === '' ? 'active' : ''}
                    onClick={() => handleTaskFilter('')}
                  >
                    All
                  </button>
                  <button
                    className={taskFilter === 'pending' ? 'active' : ''}
                    onClick={() => handleTaskFilter('pending')}
                  >
                    Pending
                  </button>
                  <button
                    className={taskFilter === 'in-progress' ? 'active' : ''}
                    onClick={() => handleTaskFilter('in-progress')}
                  >
                    In Progress
                  </button>
                  <button
                    className={taskFilter === 'completed' ? 'active' : ''}
                    onClick={() => handleTaskFilter('completed')}
                  >
                    Completed
                  </button>
                </div>
                <button 
                  className="btn-create"
                  onClick={() => setShowCreateTask(true)}
                >
                  + Create Task
                </button>
              </div>
            </div>
            {loading && !tasks.length ? (
              <div className="loading">Loading tasks...</div>
            ) : (
              <TaskList 
                tasks={tasks} 
                onTaskEdit={setTaskToEdit}
              />
            )}
          </div>
        </div>
      </div>

      <footer className="app-footer">
        <button onClick={handleRefresh} className="refresh-btn">
          🔄 Refresh All Data
        </button>
      </footer>

      {showCreateUser && (
        <CreateUserForm
          onSubmit={handleCreateUser}
          onCancel={() => setShowCreateUser(false)}
        />
      )}

      {showCreateTask && (
        <CreateTaskForm
          users={users}
          onSubmit={handleCreateTask}
          onCancel={() => setShowCreateTask(false)}
        />
      )}

      {taskToEdit && (
        <UpdateTaskForm
          task={taskToEdit}
          users={users}
          onSubmit={handleUpdateTask}
          onCancel={() => setTaskToEdit(null)}
        />
      )}
    </div>
  )
}

export default App
