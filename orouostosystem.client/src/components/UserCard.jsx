import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function UserCard() {
  const navigate = useNavigate();

  const token = localStorage.getItem('token');
  const rawRole = localStorage.getItem('role');
  const role = rawRole ? JSON.parse(rawRole)[0] : null;


  const handleLogout = () => {
    localStorage.clear();
    window.location.reload();
  };

  return (
    <div className="card mb-4">
      <div className="card-header text-center">
        <h3>{role ?? 'Guest'}</h3>
      </div>

      <div className="card-body d-flex flex-column align-items-center">

        {token && (
          <button
            className="btn btn-primary mb-2 w-100"
            onClick={() => navigate('/userEdit')}
          >
            Edit my profile
          </button>
        )}

        {token && role === 'Client' && (
          <>
            <button
              className="btn btn-secondary mb-2 w-100"
              onClick={() => navigate('/clientServiceHistory')}
            >
              Service history
            </button>

            <button
              className="btn btn-success mb-2 w-100"
              onClick={() => navigate('/clientLoyaltyProgram')}
            >
              See loyalty
            </button>
          </>
        )}

        {token && role === 'Worker' && (
          <button
            className="btn btn-warning mb-2 w-100"
            onClick={() => navigate('/userFilter')}
          >
            Filter and search users
          </button>
        )}

        {!token && (
          <button
            className="btn btn-info mb-2 w-100"
            onClick={() => navigate('/login')}
          >
            Login
          </button>
        )}

        {token && (
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
