import { useState, useEffect } from 'react'
import './UpdateTaskForm.css'

function UpdateTaskForm({ task, users, onSubmit, onCancel }) {
  const [formData, setFormData] = useState({
    title: task.title || '',
    status: task.status || 'pending',
    userId: task.userId || ''
  })
  const [errors, setErrors] = useState({})

  useEffect(() => {
    setFormData({
      title: task.title || '',
      status: task.status || 'pending',
      userId: task.userId || ''
    })
  }, [task])

  const validate = () => {
    const newErrors = {}
    if (formData.title !== undefined && !formData.title.trim()) {
      newErrors.title = 'Title cannot be empty'
    }
    if (formData.status && !['pending', 'in-progress', 'completed'].includes(formData.status)) {
      newErrors.status = 'Status must be pending, in-progress, or completed'
    }
    if (formData.userId && !users.find(u => u.id === parseInt(formData.userId))) {
      newErrors.userId = 'User does not exist'
    }
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    if (validate()) {
      const updateData = {}
      if (formData.title !== undefined) updateData.title = formData.title
      if (formData.status !== undefined) updateData.status = formData.status
      if (formData.userId !== undefined && formData.userId !== '') {
        updateData.userId = parseInt(formData.userId)
      }
      onSubmit(task.id, updateData)
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
    <div className="update-task-form-overlay">
      <div className="update-task-form">
        <h2>Update Task #{task.id}</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="title">Title</label>
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
            <label htmlFor="status">Status</label>
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
            <label htmlFor="userId">User</label>
            <select
              id="userId"
              name="userId"
              value={formData.userId}
              onChange={handleChange}
              className={errors.userId ? 'error' : ''}
            >
              <option value="">Keep current user</option>
              {users.map(user => (
                <option key={user.id} value={user.id}>
                  {user.name} ({user.email})
                </option>
              ))}
            </select>
            {errors.userId && <span className="error-message">{errors.userId}</span>}
          </div>

          <div className="form-actions">
            <button type="submit" className="btn-primary">Update Task</button>
            <button type="button" onClick={onCancel} className="btn-secondary">Cancel</button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default UpdateTaskForm
