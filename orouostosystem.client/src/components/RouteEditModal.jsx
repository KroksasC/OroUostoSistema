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

  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Edit Route</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <form>
              <div className="mb-3">
                <label className="form-label">Source Airfield Code</label>
                <input name="source" value={formData.source} onChange={handleChange} className="form-control" required />
              </div>
              <div className="mb-3">
                <label className="form-label">Destination Airfield Code</label>
                <input name="destination" value={formData.destination} onChange={handleChange} className="form-control" required />
              </div>
              <div className="mb-3">
                <label className="form-label">Distance (km)</label>
                <input type="number" name="distance" value={formData.distance} onChange={handleChange} className="form-control" required />
              </div>
              <div className="mb-3">
                <label className="form-label">Duration</label>
                <input name="duration" value={formData.duration} onChange={handleChange} className="form-control" placeholder="e.g. 7h 05m" required />
              </div>
              <div className="mb-3">
                <label className="form-label">Flight Altitude</label>
                <input name="altitude" value={formData.altitude} onChange={handleChange} className="form-control" required />
              </div>
              <div className="d-flex justify-content-between mt-3">
                <button type="button" className="btn btn-info" onClick={() => setShowForecast(true)}>View Forecast</button>
                <div>
                  <button type="button" className="btn btn-secondary me-2" onClick={onClose}>Cancel</button>
                  <button type="button" className="btn btn-success">Save</button>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
      {showForecast && (
        <WeatherForecastModal isOpen={showForecast} forecast={formData.forecast} onClose={() => setShowForecast(false)} />
      )}
    </div>
  )
}
