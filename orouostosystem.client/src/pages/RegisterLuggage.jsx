import { useNavigate } from 'react-router-dom'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function RegisterLuggage() {
  const navigate = useNavigate()

  return (
    <div className="container mt-5" style={{ maxWidth: '500px' }}>
      <h2 className="text-center mb-4">Register Luggage</h2>

      <div className="card p-4">
        <form onSubmit={(e) => e.preventDefault()}>
          <div className="mb-3">
            <label htmlFor="ownerName" className="form-label">Owner Name</label>
            <input type="text" className="form-control" id="ownerName" placeholder="Enter owner name" />
          </div>

          <div className="mb-3">
            <label htmlFor="destination" className="form-label">Destination</label>
            <input type="text" className="form-control" id="destination" placeholder="Enter destination" />
          </div>

          <div className="mb-3">
            <label htmlFor="weight" className="form-label">Weight (kg)</label>
            <input type="number" className="form-control" id="weight" placeholder="Enter weight" />
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
