import { useState, useEffect } from 'react'
import WeatherForecastModal from './WeatherForecastModal'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function RouteEditModal({ isOpen, onClose, route, onSave }) {
  const [formData, setFormData] = useState(route)
  const [showForecast, setShowForecast] = useState(false)
  const [isEditing, setIsEditing] = useState(false)
  const [saving, setSaving] = useState(false)
  const [role, setRole] = useState([])

  useEffect(() => { 
    setFormData(route)
    const storedRole = JSON.parse(localStorage.getItem("role")) ?? []
    setRole(storedRole)
  }, [route])

  if (!isOpen) return null

  const hasRole = (r) => role.includes(r)
  const isWorker = hasRole("Worker")

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))
  }

  const formatDuration = (hours) => {
    const h = Math.floor(hours)
    const m = Math.round((hours - h) * 60)
    return `${h}h ${m}m`
  }

  const parseDuration = (durationStr) => {
    // Convert "7h 5m" format back to hours (decimal)
    const match = durationStr.match(/(\d+)h\s*(\d+)m/)
    if (match) {
      const hours = parseInt(match[1])
      const minutes = parseInt(match[2])
      return hours + (minutes / 60)
    }
    return parseFloat(durationStr) || 0
  }

  const handleSave = async () => {
    setSaving(true)
    try {
      const updateData = {
        takeoffAirport: formData.takeoffAirport,
        landingAirport: formData.landingAirport,
        distance: parseFloat(formData.distance),
        duration: typeof formData.duration === 'string' && formData.duration.includes('h') 
          ? parseDuration(formData.duration) 
          : parseFloat(formData.duration),
        altitude: parseFloat(formData.altitude)
      }

      const res = await fetch(`/api/routes/${formData.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(updateData)
      })

      if (res.ok) {
        alert('Route updated successfully')
        setIsEditing(false)
        if (onSave) onSave()
        onClose()
      } else {
        alert('Failed to update route')
      }
    } catch (error) {
      console.error('Failed to update route:', error)
      alert('Failed to update route')
    } finally {
      setSaving(false)
    }
  }

  const handleCancel = () => {
    setFormData(route)
    setIsEditing(false)
  }

  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Route Details</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <form>
              <div className="mb-3">
                <label className="form-label">Source Airfield Code</label>
                <input 
                  name="takeoffAirport" 
                  value={formData.takeoffAirport} 
                  onChange={handleChange} 
                  className="form-control" 
                  readOnly={!isEditing}
                />
              </div>
              <div className="mb-3">
                <label className="form-label">Destination Airfield Code</label>
                <input 
                  name="landingAirport" 
                  value={formData.landingAirport} 
                  onChange={handleChange} 
                  className="form-control" 
                  readOnly={!isEditing}
                />
              </div>
              <div className="mb-3">
                <label className="form-label">Distance (km)</label>
                <input 
                  type="number" 
                  name="distance" 
                  value={formData.distance} 
                  onChange={handleChange} 
                  className="form-control" 
                  readOnly={!isEditing}
                />
              </div>
              <div className="mb-3">
                <label className="form-label">Duration (hours)</label>
                <input 
                  type="number"
                  step="0.01"
                  name="duration" 
                  value={isEditing ? formData.duration : formatDuration(formData.duration)} 
                  onChange={handleChange} 
                  className="form-control" 
                  readOnly={!isEditing}
                  placeholder={isEditing ? "e.g., 7.08 (for 7h 5m)" : ""}
                />
                {isEditing && (
                  <small className="text-muted">Enter duration in hours (e.g., 7.08 for 7h 5m)</small>
                )}
              </div>
              <div className="mb-3">
                <label className="form-label">Flight Altitude</label>
                <input 
                  type="number"
                  name="altitude" 
                  value={formData.altitude} 
                  onChange={handleChange} 
                  className="form-control" 
                  readOnly={!isEditing}
                />
              </div>
              <div className="mb-3">
                <label className="form-label">Flights</label>
                {formData.flightNumbers && formData.flightNumbers.length > 0 ? (
                  <ul className="list-group">
                    {formData.flightNumbers.map((flightNumber, index) => (
                      <li key={index} className="list-group-item">{flightNumber}</li>
                    ))}
                  </ul>
                ) : (
                  <input value="No flights assigned" className="form-control" readOnly />
                )}
              </div>
              <div className="d-flex justify-content-between mt-3">
                <div>
                  {formData.latestForecast && (
                    <button type="button" className="btn btn-info" onClick={() => setShowForecast(true)}>
                      View Forecast
                    </button>
                  )}
                </div>
                <div>
                  {!isEditing ? (
                    <>
                      {isWorker && (
                        <button type="button" className="btn btn-primary me-2" onClick={() => setIsEditing(true)}>
                          Edit
                        </button>
                      )}
                      <button type="button" className="btn btn-secondary" onClick={onClose}>
                        Close
                      </button>
                    </>
                  ) : (
                    <>
                      <button type="button" className="btn btn-secondary me-2" onClick={handleCancel}>
                        Cancel
                      </button>
                      <button 
                        type="button" 
                        className="btn btn-success" 
                        onClick={handleSave}
                        disabled={saving}
                      >
                        {saving ? 'Saving...' : 'Save'}
                      </button>
                    </>
                  )}
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
      {showForecast && formData.latestForecast && (
        <WeatherForecastModal isOpen={showForecast} forecast={formData.latestForecast} onClose={() => setShowForecast(false)} />
      )}
    </div>
  )
}
