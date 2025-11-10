import { useState } from "react";
import { useNavigate } from "react-router-dom";
import FlightDetailInfo from "../components/FlightsInfoCardSettingsPilots";
import FlightEdit from "../components/FlightEditCardSettingsPilots";
import "bootstrap/dist/css/bootstrap.min.css";

export default function FlightsListPilots() {
  const navigate = useNavigate();
  const [selectedFlight, setSelectedFlight] = useState(null);
  const [editFlight, setEditFlight] = useState(null);

  const sampleFlights = [
    { id: "F001", destination: "Paris", takeOffTime: "2024-01-15T12:13:00", runAway: "17B", planeName: "Boyeng17" },
    { id: "F002", destination: "New York", takeOffTime: "2025-11-10T12:13:00", runAway: "17B", planeName: "Boyeng17" },
  ];

  const recomendedFlights = [
    { id: "F003", destination: "Kaunas", takeOffTime: "2024-01-15T12:13:00", runAway: "17B", planeName: "Boyeng17" },
  ];

  const isFlightSoon = (takeOffTime) => {
    const now = new Date();
    const flightTime = new Date(takeOffTime);
    const diffInMs = flightTime - now;
    const diffInDays = diffInMs / (1000 * 60 * 60 * 24);
    return diffInDays < 2 && diffInDays >= 0;
  };

  return (
    <div className="container mt-5" style={{ maxWidth: "700px" }}>
      <h2 className="text-center mb-4">Flights Timetable</h2>

      <table className="table table-striped">
        <thead>
          <tr>
            <th>Destination</th>
            <th>Date</th>
            <th>Time</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {sampleFlights.map((flight) => {
            const soon = isFlightSoon(flight.takeOffTime);
            return (
              <tr
                key={flight.id}
                style={{
                  fontWeight: soon ? "bold" : "normal", // css dadeti spalvos pakeitima
                }}
              >
                <td>{flight.destination}</td>
                <td>
                  {new Date(flight.takeOffTime).toLocaleDateString("en-US", {
                    weekday: "short",
                    year: "numeric",
                    month: "short",
                    day: "numeric",
                  })}
                </td>
                <td>
                  {new Date(flight.takeOffTime).toLocaleTimeString("en-US", {
                    hour: "2-digit",
                    minute: "2-digit",
                    hour12: false,
                  })}
                </td>
                <td>
                  <button
                    className="btn btn-primary btn-sm me-2"
                    onClick={() => setSelectedFlight(flight)}
                  >
                    Details
                  </button>
                  <button
                    className="btn btn-primary btn-sm me-3"
                    onClick={() => setEditFlight(flight)}
                  >
                    Edit
                  </button>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>

      <div className="text-center mt-4">
        <button className="btn btn-secondary" onClick={() => navigate("/")}>
          Back
        </button>
      </div>

      {recomendedFlights.length > 0 && (
        <div className="container mt-5 d-flex flex-column" style={{ minHeight: "60vh", maxWidth: "700px" }}>
          <table className="table table-striped mt-auto">
            <thead>
              <tr>
                <th>Destination</th>
                <th>Date</th>
                <th>Time</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {recomendedFlights.map((flight) => {
                const soon = isFlightSoon(flight.takeOffTime);
                return (
                  <tr
                    key={flight.id}
                    style={{
                      backgroundColor: soon ? "rgba(255, 0, 0, 0.6) !important" : "transparent", // css dadeti spalvos pakeitima
                      fontWeight: soon ? "bold" : "normal",
                    }}
                  >
                    <td>{flight.destination}</td>
                    <td>
                      {new Date(flight.takeOffTime).toLocaleDateString("en-US", {
                        weekday: "short",
                        year: "numeric",
                        month: "short",
                        day: "numeric",
                      })}
                    </td>
                    <td>
                      {new Date(flight.takeOffTime).toLocaleTimeString("en-US", {
                        hour: "2-digit",
                        minute: "2-digit",
                        hour12: false,
                      })}
                    </td>
                    <td>
                      <button
                        className="btn btn-primary btn-sm me-2"
                        onClick={() => setSelectedFlight(flight)}
                      >
                        Details
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}

      {selectedFlight && (
        <FlightDetailInfo
          isOpen={!!selectedFlight}
          flight={selectedFlight}
          onClose={() => setSelectedFlight(null)}
        />
      )}

      {editFlight && (
        <FlightEdit
          isOpen={!!editFlight}
          flight={editFlight}
          onClose={() => setEditFlight(null)}
        />
      )}
    </div>
  );
}
