import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import LuggageDetailsModal from "../components/LuggageDetailsModal";
import LuggageLocationModal from "../components/LuggageLocationModal";
import "bootstrap/dist/css/bootstrap.min.css";

export default function LuggageList() {
  const navigate = useNavigate();

  const [allLuggage, setAllLuggage] = useState([]);
  const [luggageList, setLuggageList] = useState([]);

  const [selectedLuggage, setSelectedLuggage] = useState(null);
  const [locationLuggage, setLocationLuggage] = useState(null);

  const [role, setRole] = useState([]);

  // Filters
  const [filterFlight, setFilterFlight] = useState("");
  const [filterMinWeight, setFilterMinWeight] = useState("");
  const [filterMaxWeight, setFilterMaxWeight] = useState("");

  // Load roles
  useEffect(() => {
    const storedRole = JSON.parse(localStorage.getItem("role")) ?? [];
    setRole(storedRole);

    const allowed = ["Admin", "Worker", "Client"];
    if (!storedRole.some((r) => allowed.includes(r))) {
      navigate("/");
    }
  }, [navigate]);

  const hasRole = (r) => role.includes(r);
  const isAdmin = hasRole("Admin");
  const isWorker = hasRole("Worker");
  const isClient = hasRole("Client");

  // Load luggage
  useEffect(() => {
    const loadData = async () => {
      try {
        const res = await fetch("/api/baggage");
        const data = await res.json();

        const clientId = Number(localStorage.getItem("clientId"));
        const storedRole = JSON.parse(localStorage.getItem("role")) ?? [];

        let filtered = data;

        // FILTER BY ROLE
        if (storedRole.includes("Client") && clientId) {
          filtered = filtered.filter((b) => b.clientId === clientId);
        }

        setAllLuggage(filtered);
        setLuggageList(filtered);
      } catch (error) {
        console.error("Failed to load luggage:", error);
      }
    };
    loadData();
  }, []);

  // Apply filters
  useEffect(() => {
    let filtered = [...allLuggage];

    // Filter by flight number
    if (filterFlight.trim() !== "") {
      filtered = filtered.filter((b) =>
        b.flightNumber.toLowerCase().includes(filterFlight.toLowerCase())
      );
    }

    // Filter by min weight
    if (filterMinWeight !== "") {
      filtered = filtered.filter((b) => b.weight >= Number(filterMinWeight));
    }

    // Filter by max weight
    if (filterMaxWeight !== "") {
      filtered = filtered.filter((b) => b.weight <= Number(filterMaxWeight));
    }

    setLuggageList(filtered);
  }, [filterFlight, filterMinWeight, filterMaxWeight, allLuggage]);


  return (
    <div className="container mt-5" style={{ maxWidth: "900px" }}>
      <h2 className="text-center mb-4">Luggage List</h2>

      {/* FILTERS */}
      <div className="card p-3 mb-4 shadow-sm">
        <div className="row g-3">
          <div className="col-md-4">
            <input
              className="form-control"
              placeholder="Filter by Flight Number..."
              value={filterFlight}
              onChange={(e) => setFilterFlight(e.target.value)}
            />
          </div>

          <div className="col-md-3">
            <input
              type="number"
              className="form-control"
              placeholder="Min Weight"
              value={filterMinWeight}
              onChange={(e) => setFilterMinWeight(e.target.value)}
            />
          </div>

          <div className="col-md-3">
            <input
              type="number"
              className="form-control"
              placeholder="Max Weight"
              value={filterMaxWeight}
              onChange={(e) => setFilterMaxWeight(e.target.value)}
            />
          </div>

          <div className="col-md-2">
            <button
              className="btn btn-secondary w-100"
              onClick={() => {
                setFilterFlight("");
                setFilterMinWeight("");
                setFilterMaxWeight("");
              }}
            >
              Clear
            </button>
          </div>
        </div>
      </div>

      {/* TABLE */}
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
                  {(isAdmin || isWorker || isClient) && (
                    <button
                      className="btn btn-primary btn-sm me-2"
                      onClick={() => setSelectedLuggage(luggage)}
                    >
                      Details
                    </button>
                  )}

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

      {/* BACK BUTTON */}
      <div className="text-center mt-4">
        <button className="btn btn-secondary" onClick={() => navigate("/")}>
          Back
        </button>
      </div>

      {/* MODALS */}
      {selectedLuggage && (
        <LuggageDetailsModal
          isOpen={true}
          luggage={selectedLuggage}
          onClose={() => setSelectedLuggage(null)}
        />
      )}

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
