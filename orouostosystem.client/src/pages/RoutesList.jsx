import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import RouteEditModal from '../components/RouteEditModal'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function RoutesList() {
  const [routes, setRoutes] = useState([])
  const [editingRoute, setEditingRoute] = useState(null)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [role, setRole] = useState([])
  const navigate = useNavigate()

  // Load roles
  useEffect(() => {
    const storedRole = JSON.parse(localStorage.getItem("role")) ?? []
    setRole(storedRole)
  }, [])

  const hasRole = (r) => role.includes(r)
  const isWorker = hasRole("Worker")
  const isPilot = hasRole("Pilot")

  // Load routes from API
  const loadRoutes = async () => {
    try {
      const res = await fetch('/api/routes')
      const data = await res.json()
      setRoutes(data)
    } catch (error) {
      console.error('Failed to load routes:', error)
    }
  }

  useEffect(() => {
    loadRoutes()
  }, [])

  const openEditModal = (route) => {
    setEditingRoute(route)
    setIsModalOpen(true)
  }

  const closeModal = () => {
    setIsModalOpen(false)
    setEditingRoute(null)
  }

  const handleSave = () => {
    loadRoutes()
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this route?')) {
      return
    }

    try {
      const res = await fetch(`/api/routes/${id}`, {
        method: 'DELETE'
      })

      if (res.ok) {
        setRoutes(routes.filter(r => r.id !== id))
        alert('Route deleted successfully')
      } else {
        alert('Failed to delete route')
      }
    } catch (error) {
      console.error('Failed to delete route:', error)
      alert('Failed to delete route')
    }
  }

  const formatDuration = (hours) => {
    const h = Math.floor(hours)
    const m = Math.round((hours - h) * 60)
    return `${h}h ${m}m`
  }

  return (
    <div className="container mt-5" style={{ maxWidth: '900px' }}>
      <h2 className="text-center mb-4">Routes</h2>
      <table className="table table-striped">
        <thead>
          <tr>
            <th>ID</th>
            <th>Source</th>
            <th>Destination</th>
            <th>Distance (km)</th>
            <th>Duration</th>
            <th>Altitude</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {routes.length === 0 ? (
            <tr>
              <td colSpan="7" className="text-center text-muted">
                No routes found.
              </td>
            </tr>
          ) : (
            routes.map(route => (
              <tr key={route.id}>
                <td>{route.id}</td>
                <td>{route.takeoffAirport}</td>
                <td>{route.landingAirport}</td>
                <td>{route.distance}</td>
                <td>{formatDuration(route.duration)}</td>
                <td>{route.altitude}</td>
                <td>
                  {(isWorker || isPilot) && (
                    <button className="btn btn-primary btn-sm me-2" onClick={() => openEditModal(route)}>Details</button>
                  )}
                  {isWorker && (
                    <button className="btn btn-danger btn-sm" onClick={() => handleDelete(route.id)}>Delete</button>
                  )}
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
      <div className="text-center mt-4">
        <button className="btn btn-secondary" onClick={() => navigate('/')}>Back</button>
      </div>
      {isModalOpen && editingRoute && (
        <RouteEditModal 
          isOpen={isModalOpen} 
          onClose={closeModal} 
          route={editingRoute}
          onSave={handleSave}
        />
      )}
    </div>
  )
}
