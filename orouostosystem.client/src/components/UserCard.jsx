import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function UserCard({ client }) {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.clear();
    window.location.reload();
  };

  return (
    <div className="card mb-4">
      <div className="card-header text-center">
        <h3>Client</h3>
      </div>
      <div className="card-body d-flex flex-column align-items-center">
        <button
          className="btn btn-primary mb-2 w-100"
          onClick={() => navigate('/userEdit')}
        >
          Edit my profile
        </button>

        <button
          className="btn btn-secondary mb-2 w-100"
          onClick={() => navigate('/userFlightHistory')}
        >
          See history
        </button>

        <button
          className="btn btn-success mb-2 w-100"
          onClick={() => navigate('/userLoyaltyProgram')}
        >
          See loyalty
        </button>

        <button
          className="btn btn-warning mb-2 w-100"
          onClick={() => navigate('/userFilter')}
        >
          Filter and search clients
        </button>

        {!localStorage.getItem("token") && (
          <button
            className="btn btn-info mb-2 w-100"
            onClick={() => navigate('/login')}
          >
            Login
          </button>
        )}

        {localStorage.getItem("token") && (
          <button
            className="btn btn-dark mb-2 w-100"
            onClick={handleLogout}
          >
            Logout
          </button>
        )}
      </div>
    </div>
  );
}
