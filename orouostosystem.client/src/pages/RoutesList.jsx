import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import RouteEditModal from '../components/RouteEditModal'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function RoutesList() {
  const routes = [
    { id: '1', source: 'KUN', destination: 'VNO', distance: 5540, duration: '7h 5m', altitude: 35000, forecast: { humidity: 60, temperature: 12, createdAt: '2025-11-01', pressure: 1015, windSpeed: 25 } },
    { id: '2', source: 'KUN', destination: 'PLQ', distance: 8770, duration: '11h 30m', altitude: 36000, forecast: { humidity: 55, temperature: 18, createdAt: '2025-11-02', pressure: 1012, windSpeed: 30 } }
  ]
  const [editingRoute, setEditingRoute] = useState(null)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const navigate = useNavigate()

  const openEditModal = (route) => {
    setEditingRoute(route)
    setIsModalOpen(true)
  }

  const closeModal = () => {
    setIsModalOpen(false)
    setEditingRoute(null)
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
          {routes.map(route => (
            <tr key={route.id}>
              <td>{route.id}</td>
              <td>{route.source}</td>
              <td>{route.destination}</td>
              <td>{route.distance}</td>
              <td>{route.duration}</td>
              <td>{route.altitude}</td>
              <td>
                <button className="btn btn-primary btn-sm me-2" onClick={() => openEditModal(route)}>Edit</button>
                <button className="btn btn-danger btn-sm">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div className="text-center mt-4">
        <button className="btn btn-secondary" onClick={() => navigate('/')}>Back</button>
      </div>
      {isModalOpen && editingRoute && (
        <RouteEditModal isOpen={isModalOpen} onClose={closeModal} route={editingRoute} />
      )}
    </div>
  )
}
