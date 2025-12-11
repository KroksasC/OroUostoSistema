import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import LuggageDetailsModal from "../components/LuggageDetailsModal";
import LuggageLocationModal from "../components/LuggageLocationModal";
import "bootstrap/dist/css/bootstrap.min.css";

export default function LuggageList() {
  const navigate = useNavigate();

  const [luggageList, setLuggageList] = useState([]);
  const [selectedLuggage, setSelectedLuggage] = useState(null);
  const [locationLuggage, setLocationLuggage] = useState(null);

  const [role, setRole] = useState([]);

  // Load user role from localStorage
  useEffect(() => {
    const storedRole = JSON.parse(localStorage.getItem("role")) ?? [];
    setRole(storedRole);

    // Redirect if unauthorized
    const allowed = ["Admin", "Worker", "Client"];
    if (!storedRole.some((r) => allowed.includes(r))) {
      navigate("/"); // redirect to home
    }
  }, [navigate]);

  // Helper functions
  const hasRole = (r) => role.includes(r);
  const isAdmin = hasRole("Admin");
  const isWorker = hasRole("Worker");
  const isClient = hasRole("Client");

  // Load luggage from API
  useEffect(() => {
    const loadData = async () => {
      try {
        const res = await fetch("/api/baggage");
        const data = await res.json();
        setLuggageList(data);
      } catch (error) {
        console.error("Failed to load luggage:", error);
      }
    };
    loadData();
  }, []);

  return (
    <div className="container mt-5" style={{ maxWidth: "800px" }}>
      <h2 className="text-center mb-4">Luggage List</h2>

      <table className="table table-striped align-middle">
        <thead>
          <tr>
            <th>Owner</th>
            <th>Flight</th>
            <th>Weight</th>
            <th>Size</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          {luggageList.length === 0 ? (
            <tr>
              <td colSpan="5" className="text-center text-muted">
                No luggage found.
              </td>
            </tr>
          ) : (
            luggageList.map((luggage) => (
              <tr key={luggage.id}>
                <td>{luggage.clientName}</td>
                <td>{luggage.flightNumber}</td>
                <td>{luggage.weight} kg</td>
                <td>{luggage.size}</td>

                <td>
                  {/* DETAILS — visible to Admin, Worker, Client */}
                  {(isAdmin || isWorker || isClient) && (
                    <button
                      className="btn btn-primary btn-sm me-2"
                      onClick={() => setSelectedLuggage(luggage)}
                    >
                      Details
                    </button>
                  )}

                  {/* LOCATION — visible to Admin, Client */}
                  {(isAdmin || isClient) && (
                    <button
                      className="btn btn-danger btn-sm"
                      onClick={() => setLocationLuggage(luggage)}
                    >
                      Location
                    </button>
                  )}
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      <div className="text-center mt-4">
        <button className="btn btn-secondary" onClick={() => navigate("/")}>
          Back
        </button>
      </div>

      {/* DETAILS MODAL */}
      {selectedLuggage && (
        <LuggageDetailsModal
          isOpen={true}
          luggage={selectedLuggage}
          onClose={() => setSelectedLuggage(null)}
        />
      )}

      {/* LOCATION MODAL */}
      {locationLuggage && (
        <LuggageLocationModal
          isOpen={true}
          luggage={locationLuggage}
          onClose={() => setLocationLuggage(null)}
        />
      )}
    </div>
  );
}
