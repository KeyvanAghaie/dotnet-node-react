import './TaskList.css'

function TaskList({ tasks, onTaskEdit }) {
  if (tasks.length === 0) {
    return <div className="empty-state">No tasks found</div>
  }

  const getStatusColor = (status) => {
    switch (status) {
      case 'completed':
        return '#4caf50'
      case 'in-progress':
        return '#ff9800'
      case 'pending':
        return '#f44336'
      default:
        return '#9e9e9e'
    }
  }

  return (
    <div className="task-list">
      {tasks.map((task) => (
        <div key={task.id} className="task-card">
          <div className="task-header">
            <h3>{task.title}</h3>
            <span
              className="task-status"
              style={{ backgroundColor: getStatusColor(task.status) }}
            >
              {task.status}
            </span>
          </div>
          <div className="task-footer">
            <span className="task-id">Task #{task.id}</span>
            <span className="task-user">User ID: {task.userId}</span>
            {onTaskEdit && (
              <button 
                className="task-edit-btn"
                onClick={() => onTaskEdit(task)}
                title="Edit task"
              >
                ✏️ Edit
              </button>
            )}
          </div>
        </div>
      ))}
    </div>
  )
}

export default TaskList
