import { useNavigate } from 'react-router-dom'
import { useEffect, useState } from 'react'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function AddRoute() {
  const navigate = useNavigate()
  const [role, setRole] = useState([])
  const [form, setForm] = useState({
    takeoffAirport: '',
    landingAirport: '',
    distance: '',
    duration: '',
    altitude: ''
  })
  const [errors, setErrors] = useState({})
  const [saving, setSaving] = useState(false)

  // Role check - Only Workers can access
  useEffect(() => {
    const storedRole = JSON.parse(localStorage.getItem("role")) ?? []
    setRole(storedRole)

    const allowed = ["Admin", "Worker"]
    const canAccess = storedRole.some((r) => allowed.includes(r))

    if (!canAccess) {
      navigate("/")
    }
  }, [navigate])

  const handleChange = (e) => {
    const { name, value } = e.target
    setForm(prev => ({ ...prev, [name]: value }))
    // Clear error for this field when user starts typing
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }))
    }
  }

  const validate = () => {
    const newErrors = {}

    if (!form.takeoffAirport.trim()) {
      newErrors.takeoffAirport = 'Source airport code is required'
    }

    if (!form.landingAirport.trim()) {
      newErrors.landingAirport = 'Destination airport code is required'
    }

    if (!form.distance || parseFloat(form.distance) <= 0) {
      newErrors.distance = 'Distance must be greater than 0'
    }

    if (!form.duration || parseFloat(form.duration) <= 0) {
      newErrors.duration = 'Duration must be greater than 0'
    }

    if (!form.altitude || parseFloat(form.altitude) <= 0) {
      newErrors.altitude = 'Altitude must be greater than 0'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    if (!validate()) {
      return
    }

    setSaving(true)

    try {
      const routeData = {
        takeoffAirport: form.takeoffAirport.trim().toUpperCase(),
        landingAirport: form.landingAirport.trim().toUpperCase(),
        distance: parseFloat(form.distance),
        duration: parseFloat(form.duration),
        altitude: parseFloat(form.altitude)
      }

      const response = await fetch('/api/routes', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(routeData)
      })

      if (response.ok) {
        alert('Route registered successfully!')
        navigate('/routes')
      } else {
        const errorData = await response.text()
        alert('Failed to register route: ' + errorData)
      }
    } catch (error) {
      console.error('Failed to register route:', error)
      alert('Failed to register route. Please try again.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="container mt-5" style={{ maxWidth: '700px' }}>
      <h2 className="text-center mb-4">Register Route</h2>

      <div className="card p-4">
        <form onSubmit={handleSubmit}>
          <div className="row">
            <div className="col-md-12">
              <div className="mb-3">
                <label className="form-label">Source Airfield Code</label>
                <input 
                  name="takeoffAirport" 
                  className={`form-control ${errors.takeoffAirport ? 'is-invalid' : ''}`}
                  value={form.takeoffAirport}
                  onChange={handleChange}
                  placeholder="e.g., KUN"
                />
                {errors.takeoffAirport && (
                  <div className="invalid-feedback">{errors.takeoffAirport}</div>
                )}
              </div>

              <div className="mb-3">
                <label className="form-label">Destination Airfield Code</label>
                <input 
                  name="landingAirport" 
                  className={`form-control ${errors.landingAirport ? 'is-invalid' : ''}`}
                  value={form.landingAirport}
                  onChange={handleChange}
                  placeholder="e.g., VNO"
                />
                {errors.landingAirport && (
                  <div className="invalid-feedback">{errors.landingAirport}</div>
                )}
              </div>

              <div className="mb-3">
                <label className="form-label">Distance (km)</label>
                <input 
                  type="number" 
                  name="distance" 
                  className={`form-control ${errors.distance ? 'is-invalid' : ''}`}
                  value={form.distance}
                  onChange={handleChange}
                  placeholder="e.g., 5540"
                  step="0.01"
                />
                {errors.distance && (
                  <div className="invalid-feedback">{errors.distance}</div>
                )}
              </div>

              <div className="mb-3">
                <label className="form-label">Duration (hours)</label>
                <input 
                  type="number"
                  name="duration" 
                  className={`form-control ${errors.duration ? 'is-invalid' : ''}`}
                  value={form.duration}
                  onChange={handleChange}
                  placeholder="e.g., 7.08 (for 7h 5m)"
                  step="0.01"
                />
                {errors.duration && (
                  <div className="invalid-feedback">{errors.duration}</div>
                )}
                <small className="text-muted">Enter duration in decimal hours (e.g., 7.08 for 7 hours 5 minutes)</small>
              </div>

              <div className="mb-3">
                <label className="form-label">Flight Altitude (km)</label>
                <input 
                  type="number"
                  name="altitude" 
                  className={`form-control ${errors.altitude ? 'is-invalid' : ''}`}
                  value={form.altitude}
                  onChange={handleChange}
                  placeholder="e.g., 10.5"
                  step="0.1"
                />
                {errors.altitude && (
                  <div className="invalid-feedback">{errors.altitude}</div>
                )}
              </div>
            </div>
          </div>

          <div className="d-flex justify-content-between">
            <button type="button" className="btn btn-secondary" onClick={() => navigate('/')}>
              Back
            </button>
            <button type="submit" className="btn btn-success" disabled={saving}>
              {saving ? 'Registering...' : 'Register'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
