import './Stats.css'

function Stats({ stats }) {
  if (!stats) return null

  return (
    <div className="stats-container">
      <div className="stat-card">
        <div className="stat-icon">👥</div>
        <div className="stat-content">
          <h3>{stats.users?.total || 0}</h3>
          <p>Total Users</p>
        </div>
      </div>

      <div className="stat-card">
        <div className="stat-icon">📋</div>
        <div className="stat-content">
          <h3>{stats.tasks?.total || 0}</h3>
          <p>Total Tasks</p>
        </div>
      </div>

      <div className="stat-card pending">
        <div className="stat-icon">⏳</div>
        <div className="stat-content">
          <h3>{stats.tasks?.pending || 0}</h3>
          <p>Pending</p>
        </div>
      </div>

      <div className="stat-card in-progress">
        <div className="stat-icon">🔄</div>
        <div className="stat-content">
          <h3>{stats.tasks?.inProgress || 0}</h3>
          <p>In Progress</p>
        </div>
      </div>

      <div className="stat-card completed">
        <div className="stat-icon">✅</div>
        <div className="stat-content">
          <h3>{stats.tasks?.completed || 0}</h3>
          <p>Completed</p>
        </div>
      </div>
    </div>
  )
}

export default Stats
