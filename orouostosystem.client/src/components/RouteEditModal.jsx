import { useState, useEffect } from 'react'
import WeatherForecastModal from './WeatherForecastModal'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function RouteEditModal({ isOpen, onClose, route, onSave }) {
  const [formData, setFormData] = useState(route)
  const [originalData, setOriginalData] = useState(route)
  const [showForecast, setShowForecast] = useState(false)
  const [saving, setSaving] = useState(false)
  const [role, setRole] = useState([])
  const [hasChanges, setHasChanges] = useState(false)
  const [loadingWeather, setLoadingWeather] = useState(false)
  const [weatherData, setWeatherData] = useState(null)

  useEffect(() => { 
    setFormData(route)
    setOriginalData(route)
    setHasChanges(false)
    setWeatherData(null)
    const storedRole = JSON.parse(localStorage.getItem("role")) ?? []
    setRole(storedRole)
  }, [route])

  if (!isOpen) return null

  const hasRole = (r) => role.includes(r)
  const isWorker = hasRole("Worker")
  const isPilot = hasRole("Pilot")
  const canEdit = isWorker

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => {
      const updated = { ...prev, [name]: value }
      // Check if there are changes
      setHasChanges(
        updated.takeoffAirport !== originalData.takeoffAirport ||
        updated.landingAirport !== originalData.landingAirport ||
        parseFloat(updated.distance) !== parseFloat(originalData.distance) ||
        parseFloat(updated.duration) !== parseFloat(originalData.duration) ||
        parseFloat(updated.altitude) !== parseFloat(originalData.altitude)
      )
      return updated
    })
  }

  const formatDuration = (hours) => {
    const h = Math.floor(hours)
    const m = Math.round((hours - h) * 60)
    return `${h}h ${m}m`
  }

  const handleSave = async () => {
    if (!hasChanges) {
      onClose()
      return
    }

    setSaving(true)
    try {
      const updateData = {
        takeoffAirport: formData.takeoffAirport.trim().toUpperCase(),
        landingAirport: formData.landingAirport.trim().toUpperCase(),
        distance: parseFloat(formData.distance),
        duration: parseFloat(formData.duration),
        altitude: parseFloat(formData.altitude)
      }

      const res = await fetch(`/api/routes/${formData.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(updateData)
      })

      if (res.ok) {
        alert('Route updated successfully')
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

  const handleViewForecast = async () => {
    if (!isPilot) {
      alert('Only pilots can view weather forecasts')
      return
    }

    setLoadingWeather(true)
    try {
      const res = await fetch(`/api/routes/${formData.id}/weather`)

      if (res.ok) {
        const forecast = await res.json()
        setWeatherData(forecast)
        setShowForecast(true)
      } else {
        alert('Failed to fetch weather forecast')
      }
    } catch (error) {
      console.error('Failed to fetch weather:', error)
      alert('Failed to fetch weather forecast')
    } finally {
      setLoadingWeather(false)
    }
  }

  const handleCancel = () => {
    if (hasChanges && !window.confirm('You have unsaved changes. Are you sure you want to close?')) {
      return
    }
    onClose()
  }

  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Route Details</h5>
            <button type="button" className="btn-close" onClick={handleCancel}></button>
          </div>
          <div className="modal-body">
            <form onSubmit={(e) => { e.preventDefault(); handleSave(); }}>
              <div className="mb-3">
                <label className="form-label">Source Airfield Code</label>
                <input 
                  name="takeoffAirport" 
                  value={formData.takeoffAirport} 
                  onChange={handleChange} 
                  className="form-control" 
                  readOnly={!canEdit}
                />
              </div>
              <div className="mb-3">
                <label className="form-label">Destination Airfield Code</label>
                <input 
                  name="landingAirport" 
                  value={formData.landingAirport} 
                  onChange={handleChange} 
                  className="form-control" 
                  readOnly={!canEdit}
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
                  readOnly={!canEdit}
                  step="0.01"
                />
              </div>
              <div className="mb-3">
                <label className="form-label">Duration (hours)</label>
                <input 
                  type="number"
                  step="0.01"
                  name="duration" 
                  value={formData.duration} 
                  onChange={handleChange} 
                  className="form-control" 
                  readOnly={!canEdit}
                  placeholder={canEdit ? "e.g., 7.08 (for 7h 5m)" : ""}
                />
                {canEdit && (
                  <small className="text-muted">Enter duration in hours (e.g., 7.08 for 7h 5m). Display: {formatDuration(formData.duration || 0)}</small>
                )}
                {!canEdit && (
                  <small className="text-muted">Duration: {formatDuration(formData.duration || 0)}</small>
                )}
              </div>
              <div className="mb-3">
                <label className="form-label">Flight Altitude (km)</label>
                <input 
                  type="number"
                  name="altitude" 
                  value={formData.altitude} 
                  onChange={handleChange} 
                  className="form-control" 
                  readOnly={!canEdit}
                  step="0.1"
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
                  {isPilot && (
                    <button 
                      type="button" 
                      className="btn btn-info" 
                      onClick={handleViewForecast}
                      disabled={loadingWeather}
                    >
                      {loadingWeather ? 'Loading...' : 'View Forecast'}
                    </button>
                  )}
                </div>
                <div>
                  {canEdit ? (
                    <>
                      <button type="button" className="btn btn-secondary me-2" onClick={handleCancel}>
                        {hasChanges ? 'Cancel' : 'Close'}
                      </button>
                      <button 
                        type="submit" 
                        className="btn btn-success" 
                        disabled={saving || !hasChanges}
                      >
                        {saving ? 'Saving...' : 'Save'}
                      </button>
                    </>
                  ) : (
                    <button type="button" className="btn btn-secondary" onClick={onClose}>
                      Close
                    </button>
                  )}
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
      {showForecast && weatherData && (
        <WeatherForecastModal isOpen={showForecast} forecast={weatherData} onClose={() => setShowForecast(false)} />
      )}
    </div>
  )
}
