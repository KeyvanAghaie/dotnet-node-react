import './HealthStatus.css'

function HealthStatus({ health }) {
  if (!health) {
    return (
      <div className="health-status unknown">
        <span className="status-indicator">⏳</span>
        <span>Checking backend status...</span>
      </div>
    )
  }

  const isHealthy = health.status === 'ok'

  return (
    <div className={`health-status ${isHealthy ? 'healthy' : 'unhealthy'}`}>
      <span className="status-indicator">
        {isHealthy ? '✅' : '❌'}
      </span>
      <span>
        {health.message || `Backend is ${isHealthy ? 'healthy' : 'unhealthy'}`}
      </span>
    </div>
  )
}

export default HealthStatus
