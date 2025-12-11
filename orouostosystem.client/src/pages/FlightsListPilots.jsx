import { useState } from "react";
import { useNavigate } from "react-router-dom";
import FlightDetailModal from "../components/FlightDetailModal";
import FlightEditModal from "../components/FlightEditModal";
import FlightTableRow from "../components/FlightTableRow";
import "bootstrap/dist/css/bootstrap.min.css";

export default function FlightsListPilots() {
  const navigate = useNavigate();
  const [selectedFlight, setSelectedFlight] = useState(null);
  const [editingFlight, setEditingFlight] = useState(null);
  
  // Flight data state
  const [flights, setFlights] = useState([
    { id: "F001", destination: "Paris", takeOffTime: "2024-01-15T12:13", runAway: "17B", planeName: "Boyeng17" },
    { id: "F002", destination: "New York", takeOffTime: "2025-11-10T12:13", runAway: "17B", planeName: "Boyeng17" },
  ]);

  const [recommendedFlights, setRecommendedFlights] = useState([
    { id: "F003", destination: "Kaunas", takeOffTime: "2024-01-15T12:13", runAway: "17B", planeName: "Boyeng17" },
  ]);

  // Helper function to check if flight is soon
  const isFlightSoon = (takeOffTime) => {
    const now = new Date();
    const flightTime = new Date(takeOffTime);
    const diffInMs = flightTime - now;
    const diffInDays = diffInMs / (1000 * 60 * 60 * 24);
    return diffInDays < 2 && diffInDays >= 0;
  };

  // Handler: Update flight details
  const handleUpdateFlight = (updatedFlight) => {
    const isRecommended = recommendedFlights.some(f => f.id === updatedFlight.id);
    
    if (isRecommended) {
      setRecommendedFlights(prev => 
        prev.map(f => f.id === updatedFlight.id ? updatedFlight : f)
      );
    } else {
      setFlights(prev => 
        prev.map(f => f.id === updatedFlight.id ? updatedFlight : f)
      );
    }
    setEditingFlight(null);
  };

  // Handler: Accept recommended flight
  const handleAcceptFlight = (flight) => {
    // Move from recommended to main flights
    setFlights(prev => [...prev, flight]);
    setRecommendedFlights(prev => prev.filter(f => f.id !== flight.id));
  };

  // Handler: Decline recommended flight
  const handleDeclineFlight = (flight) => {
    setRecommendedFlights(prev => prev.filter(f => f.id !== flight.id));
  };

  // Handler: Delete flight (if needed)
  const handleDeleteFlight = (flightId) => {
    setFlights(prev => prev.filter(f => f.id !== flightId));
  };

return (
    <div className="container mt-5" style={{ maxWidth: "700px", paddingBottom: recommendedFlights.length > 0 ? "250px" : "0" }}>
      <h2 className="text-center mb-4">Flights Timetable</h2>

      {/* Main Flights Table */}
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
          {flights.map((flight) => (
            <FlightTableRow
              key={flight.id}
              flight={flight}
              isRecommended={false}
              isSoon={isFlightSoon(flight.takeOffTime)}
              onViewDetails={() => setSelectedFlight(flight)}
              onEdit={() => setEditingFlight(flight)}
              onDelete={() => handleDeleteFlight(flight.id)}
            />
          ))}
        </tbody>
      </table>

      {/* Back Button */}
      <div className="text-center mt-4">
        <button className="btn btn-secondary" onClick={() => navigate("/")}>
          Back
        </button>
      </div>

      {/* Modals */}
      {selectedFlight && (
        <FlightDetailModal
          isOpen={!!selectedFlight}
          flight={selectedFlight}
          onClose={() => setSelectedFlight(null)}
        />
      )}

      {editingFlight && (
        <FlightEditModal
          isOpen={!!editingFlight}
          flight={editingFlight}
          onClose={() => setEditingFlight(null)}
          onSave={handleUpdateFlight}
        />
      )}

      {/* Recommended Flights Section - FIXED TO BOTTOM OF VIEWPORT */}
      {recommendedFlights.length > 0 && (
        <div style={{ 
          position: 'fixed', 
          bottom: 0, 
          left: 0, 
          right: 0,
          backgroundColor: 'white',
          padding: '20px 0',
          borderTop: '2px solid #dee2e6',
          zIndex: 1000,
          boxShadow: '0 -2px 10px rgba(0,0,0,0.1)'
        }}>
          <div className="container" style={{ maxWidth: "700px" }}>
            <h3 className="text-center mb-3">Recommended Flights</h3>
            <table className="table table-striped mb-0">
              <thead>
                <tr>
                  <th>Destination</th>
                  <th>Date</th>
                  <th>Time</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {recommendedFlights.map((flight) => (
                  <FlightTableRow
                    key={flight.id}
                    flight={flight}
                    isRecommended={true}
                    isSoon={isFlightSoon(flight.takeOffTime)}
                    onViewDetails={() => setSelectedFlight(flight)}
                    onAccept={() => handleAcceptFlight(flight)}
                    onDecline={() => handleDeclineFlight(flight)}
                  />
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}