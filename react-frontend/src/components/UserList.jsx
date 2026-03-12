import './UserList.css'

function UserList({ users, selectedUserId, onUserSelect }) {
  if (users.length === 0) {
    return <div className="empty-state">No users found</div>
  }

  return (
    <div className="user-list">
      {users.map((user) => (
        <div
          key={user.id}
          className={`user-card ${selectedUserId === user.id ? 'selected' : ''}`}
          onClick={() => onUserSelect(user.id)}
        >
          <div className="user-avatar">
            {user.name.charAt(0).toUpperCase()}
          </div>
          <div className="user-info">
            <h3>{user.name}</h3>
            <p className="user-email">{user.email}</p>
            <span className="user-role">{user.role}</span>
          </div>
        </div>
      ))}
    </div>
  )
}

export default UserList
