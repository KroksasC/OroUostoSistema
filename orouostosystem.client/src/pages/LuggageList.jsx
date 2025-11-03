import { useState } from "react";
import { useNavigate } from "react-router-dom";
import LuggageDetailsModal from "../components/LuggageDetailsModal";
import LuggageLocationModal from "../components/LuggageLocationModal";
import "bootstrap/dist/css/bootstrap.min.css";

export default function LuggageList() {
  const navigate = useNavigate();
  const [selectedLuggage, setSelectedLuggage] = useState(null);
  const [locationLuggage, setLocationLuggage] = useState(null);

  const sampleLuggages = [
    { id: "L001", owner: "John Doe", destination: "Paris" },
    { id: "L002", owner: "Jane Smith", destination: "London" },
  ];

  return (
    <div className="container mt-5" style={{ maxWidth: "700px" }}>
      <h2 className="text-center mb-4">Luggage List</h2>

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Owner</th>
            <th>Destination</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {sampleLuggages.map((luggage) => (
            <tr key={luggage.id}>
              <td>{luggage.owner}</td>
              <td>{luggage.destination}</td>
              <td>
                <button
                  className="btn btn-primary btn-sm me-2"
                  onClick={() => setSelectedLuggage(luggage)}
                >
                  Details
                </button>
                <button
                  className="btn btn-danger btn-sm"
                  onClick={() => setLocationLuggage(luggage)}
                >
                  Location
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="text-center mt-4">
        <button className="btn btn-secondary" onClick={() => navigate("/")}>
          Back
        </button>
      </div>

      {selectedLuggage && (
        <LuggageDetailsModal
          isOpen={!!selectedLuggage}
          luggage={selectedLuggage}
          onClose={() => setSelectedLuggage(null)}
        />
      )}

      {locationLuggage && (
        <LuggageLocationModal
          isOpen={!!locationLuggage}
          luggage={locationLuggage}
          onClose={() => setLocationLuggage(null)}
        />
      )}
    </div>
  );
}
