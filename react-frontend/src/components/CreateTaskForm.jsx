import { useState } from 'react'
import './CreateTaskForm.css'

function CreateTaskForm({ users, onSubmit, onCancel }) {
  const [formData, setFormData] = useState({
    title: '',
    status: 'pending',
    userId: ''
  })
  const [errors, setErrors] = useState({})

  const validate = () => {
    const newErrors = {}
    if (!formData.title.trim()) {
      newErrors.title = 'Title is required'
    }
    if (!formData.status) {
      newErrors.status = 'Status is required'
    } else if (!['pending', 'in-progress', 'completed'].includes(formData.status)) {
      newErrors.status = 'Status must be pending, in-progress, or completed'
    }
    if (!formData.userId) {
      newErrors.userId = 'User is required'
    }
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    if (validate()) {
      onSubmit({
        ...formData,
        userId: parseInt(formData.userId)
      })
      setFormData({ title: '', status: 'pending', userId: '' })
      setErrors({})
    }
  }

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }))
    }
  }

  return (
    <div className="create-task-form-overlay">
      <div className="create-task-form">
        <h2>Create New Task</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="title">Title *</label>
            <input
              type="text"
              id="title"
              name="title"
              value={formData.title}
              onChange={handleChange}
              className={errors.title ? 'error' : ''}
            />
            {errors.title && <span className="error-message">{errors.title}</span>}
          </div>

          <div className="form-group">
            <label htmlFor="status">Status *</label>
            <select
              id="status"
              name="status"
              value={formData.status}
              onChange={handleChange}
              className={errors.status ? 'error' : ''}
            >
              <option value="pending">Pending</option>
              <option value="in-progress">In Progress</option>
              <option value="completed">Completed</option>
            </select>
            {errors.status && <span className="error-message">{errors.status}</span>}
          </div>

          <div className="form-group">
            <label htmlFor="userId">User *</label>
            <select
              id="userId"
              name="userId"
              value={formData.userId}
              onChange={handleChange}
              className={errors.userId ? 'error' : ''}
            >
              <option value="">Select a user</option>
              {users.map(user => (
                <option key={user.id} value={user.id}>
                  {user.name} ({user.email})
                </option>
              ))}
            </select>
            {errors.userId && <span className="error-message">{errors.userId}</span>}
          </div>

          <div className="form-actions">
            <button type="submit" className="btn-primary">Create Task</button>
            <button type="button" onClick={onCancel} className="btn-secondary">Cancel</button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default CreateTaskForm
