import { Link } from 'react-router-dom'

function DashboardPage() {
  return (
    <div>
      <h1>Dashboard</h1>
      <Link to="/login">Ir al login</Link>
      <br />
      <Link to="/register">Ir al register</Link>
      <br />
      <Link to="/ingredients">Ir a ingredientes</Link>
    </div>
  )
}

export default DashboardPage