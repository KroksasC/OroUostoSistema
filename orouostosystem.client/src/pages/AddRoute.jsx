import { useNavigate } from 'react-router-dom'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function AddRoute() {
  const navigate = useNavigate()

  return (
    <div className="container mt-5" style={{ maxWidth: '700px' }}>
      <h2 className="text-center mb-4">Register Route</h2>

      <div className="card p-4">
        <form onSubmit={(e) => e.preventDefault()}>
          <div className="row">
            <div className="col-md-12">
              <div className="mb-3">
                <label className="form-label">Source Airfield Code</label>
                <input name="source" className="form-control"/>
              </div>
              <div className="mb-3">
                <label className="form-label">Destination Airfield Code</label>
                <input name="destination" className="form-control"/>
              </div>
              <div className="mb-3">
                <label className="form-label">Distance (km)</label>
                <input type="number" name="distance" className="form-control"/>
              </div>
              <div className="mb-3">
                <label className="form-label">Duration</label>
                <input name="duration" className="form-control"/>
              </div>
              <div className="mb-3">
                <label className="form-label">Flight Altitude</label>
                <input name="altitude" className="form-control"/>
              </div>
            </div>
          </div>

          <div className="d-flex justify-content-between">
            <button type="button" className="btn btn-secondary" onClick={() => navigate('/')}>Back</button>
            <button type="submit" className="btn btn-success">Register</button>
          </div>
        </form>
      </div>
    </div>
  )
}
