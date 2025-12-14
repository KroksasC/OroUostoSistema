import { useState, useEffect } from 'react'
import WeatherForecastModal from './WeatherForecastModal'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function RouteEditModal({ isOpen, onClose, route }) {
  const [formData, setFormData] = useState(route)
  const [showForecast, setShowForecast] = useState(false)

  useEffect(() => { setFormData(route) }, [route])

  if (!isOpen) return null

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))
  }

  const formatDuration = (hours) => {
    const h = Math.floor(hours)
    const m = Math.round((hours - h) * 60)
    return `${h}h ${m}m`
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
                <input name="takeoffAirport" value={formData.takeoffAirport} onChange={handleChange} className="form-control" readOnly />
              </div>
              <div className="mb-3">
                <label className="form-label">Destination Airfield Code</label>
                <input name="landingAirport" value={formData.landingAirport} onChange={handleChange} className="form-control" readOnly />
              </div>
              <div className="mb-3">
                <label className="form-label">Distance (km)</label>
                <input type="number" name="distance" value={formData.distance} onChange={handleChange} className="form-control" readOnly />
              </div>
              <div className="mb-3">
                <label className="form-label">Duration</label>
                <input name="duration" value={formatDuration(formData.duration)} onChange={handleChange} className="form-control" readOnly />
              </div>
              <div className="mb-3">
                <label className="form-label">Flight Altitude</label>
                <input name="altitude" value={formData.altitude} onChange={handleChange} className="form-control" readOnly />
              </div>
              <div className="mb-3">
                <label className="form-label">Flight Number</label>
                <input name="flightNumber" value={formData.flightNumber} onChange={handleChange} className="form-control" readOnly />
              </div>
              <div className="d-flex justify-content-between mt-3">
                {formData.latestForecast && (
                  <button type="button" className="btn btn-info" onClick={() => setShowForecast(true)}>View Forecast</button>
                )}
                <button type="button" className="btn btn-secondary ms-auto" onClick={onClose}>Close</button>
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
