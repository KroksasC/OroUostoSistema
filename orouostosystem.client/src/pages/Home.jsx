import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import SearchLuggageModal from '../components/SearchLuggageModal'
import UserCard from '../components/UserCard'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function Home() {
  const navigate = useNavigate()
  const [isModalOpen, setIsModalOpen] = useState(false)

  const user = {
    name: '[FirstName] [LastName]',
    email: '[Email]',
    loyaltyProgramLevel: '[LoyaltyLevel]'
  }

  return (
    <div className="container mt-5">
      <h1 className="text-center mb-4">Dashboard</h1>

      <div className="row justify-content-center mb-4">
        <div className="col-md-4">
          <div className="card mb-4">
            <div className="card-header text-center">
              <h3>Luggage</h3>
            </div>
            <div className="card-body d-flex flex-column align-items-center">
              <button className="btn btn-primary mb-2 w-100" onClick={() => navigate('/luggageList')}>
                Luggage List
              </button>
              <button className="btn btn-success mb-2 w-100" onClick={() => navigate('/registerLuggage')}>
                Register Luggage
              </button>
              <button className="btn btn-warning mb-2 w-100" onClick={() => setIsModalOpen(true)}>
                Search Luggage
              </button>
            </div>
          </div>
        </div>

        <div className="col-md-4">
          <div className="card">
            <div className="card-header text-center">
              <h3>Pilots</h3>
            </div>
            <div className="card-body d-flex flex-column align-items-center">
              <button
                className="btn btn-primary mb-2 w-100"
                onClick={() => navigate('/flightsListPilots')}
              >
                Flight List
              </button>
            </div>
          </div>
        </div>
      </div>

      <div className="row justify-content-center mb-4">
        <div className="col-md-4">
          <div className="card mb-4">
            <div className="card-header text-center">
              <h3>Routes</h3>
            </div>
            <div className="card-body d-flex flex-column align-items-center">
              <button className="btn btn-primary mb-2 w-100" onClick={() => navigate('/routes')}>
                Routes List
              </button>
              <button className="btn btn-success mb-2 w-100" onClick={() => navigate('/addRoute')}>
                Add Route
              </button>
            </div>
          </div>
        </div>
      </div>

      <div className="row justify-content-center mb-4">
        <div className="col-md-6">
          <UserCard client={user} mode="form" />
        </div>
      </div>

      <div className="row justify-content-center mb-4">
        <div className="col-md-4">
          <div className="card mt-4">
            <div className="card-header text-center">
              <h3>Services</h3>
            </div>
            <div className="card-body d-flex flex-column align-items-center">
              <button
                className="btn btn-primary mb-2 w-100"
                onClick={() => navigate('/servicesList')}
              >
                Go to Services
              </button>
            </div>
          </div>
        </div>
      </div>

      <SearchLuggageModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} />
    </div>
  )
}