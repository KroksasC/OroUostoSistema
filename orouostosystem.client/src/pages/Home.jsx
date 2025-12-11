import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import SearchLuggageModal from "../components/SearchLuggageModal";
import UserCard from "../components/UserCard";
import "bootstrap/dist/css/bootstrap.min.css";

export default function Home() {
  const navigate = useNavigate();
  const [isModalOpen, setIsModalOpen] = useState(false);

  const [user, setUser] = useState({
    name: "",
    email: "",
    loyaltyProgramLevel: "",
    role: [],
  });

  useEffect(() => {
    const username = localStorage.getItem("username");
    const role = JSON.parse(localStorage.getItem("role"));

    setUser({
      name: username,
      email: username,
      loyaltyProgramLevel: "",
      role: role ?? [],
    });
  }, []);

  const userId = localStorage.getItem("userId");

  if (userId) {
    fetch(`/api/client/byUser/${userId}`)
      .then(res => res.json())
      .then(clientId => localStorage.setItem("clientId", clientId))
      .catch(() => console.warn("This user has no client profile."));
  }


  const hasRole = (r) => user.role.includes(r);
  const isWorker = hasRole("Worker");
  const isAdmin = hasRole("Admin");
  const isClient = hasRole("Client");

  return (
    <div className="container mt-5">
      {/* Logged user info */}
      <div className="alert alert-info text-center">
        Logged in as <b>{user.email}</b> | Role: <b>{user.role.join(", ")}</b>
      </div>

      <h1 className="text-center mb-4">Dashboard</h1>

      <div className="row justify-content-center mb-4">
        <div className="col-md-4">
          <div className="card mb-4">
            <div className="card-header text-center">
              <h3>Luggage</h3>
            </div>
            <div className="card-body d-flex flex-column align-items-center">
              {/* LUGGAGE LIST — Worker, Client, Admin */}
              {(isWorker || isClient || isAdmin) && (
                <button className="btn btn-primary mb-2 w-100" onClick={() => navigate('/luggageList')}>
                  Luggage List
                </button>
              )}

              {/* REGISTER LUGGAGE — Worker, Admin */}
              {(isWorker || isAdmin) && (
                <button className="btn btn-success mb-2 w-100" onClick={() => navigate('/registerLuggage')}>
                  Register Luggage
                </button>
              )}

              {/* SEARCH LUGGAGE — Worker, Admin */}
              {(isWorker || isAdmin) && (
                <button className="btn btn-warning mb-2 w-100" onClick={() => setIsModalOpen(true)}>
                  Search Luggage
                </button>
              )}
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
                onClick={() => navigate("/flightsListPilots")}
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
              <button
                className="btn btn-primary mb-2 w-100"
                onClick={() => navigate("/routes")}
              >
                Routes List
              </button>
              <button
                className="btn btn-success mb-2 w-100"
                onClick={() => navigate("/addRoute")}
              >
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
                onClick={() => navigate("/servicesList")}
              >
                Go to Services
              </button>
            </div>
          </div>
        </div>
      </div>

      <SearchLuggageModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
      />
    </div>
  );
}
