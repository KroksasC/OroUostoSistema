import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import FlightDetailModal from "../components/FlightDetailModal";
import FlightEditModal from "../components/FlightEditModal";
import "bootstrap/dist/css/bootstrap.min.css";

const API_BASE_URL = "http://localhost:5229";

export default function FlightsListPilots() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [flights, setFlights] = useState([]);
  const [recommendedFlights, setRecommendedFlights] = useState([]);
  const [selectedFlight, setSelectedFlight] = useState(null);
  const [editingFlight, setEditingFlight] = useState(null);

  useEffect(() => {
    loadFlights();
    loadRecommendedFlights();
  }, []);

  const loadFlights = async () => {
    setLoading(true);
    setError("");
    
    try {
      const response = await fetch(`${API_BASE_URL}/api/flight/pilot-flights`);
      
      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }
      
      const data = await response.json();
      
      if (data.success && data.flights) {
        const processedFlights = data.flights.map(flight => ({
          id: flight.id,
          flightId: flight.flightId,
          destination: flight.destination || "Unknown",
          takeOffTime: flight.takeOffTime,
          planeName: flight.planeName || "Unknown",
          pilotName: flight.pilotName || "Not Assigned",
          status: flight.status || "Scheduled",
          isSoon: flight.isSoon,
          hoursUntil: Math.round(flight.hoursUntil * 10) / 10,
          routesCount: flight.routesCount || 0,
          startingAirport: flight.startingAirport || "TBD"
        }));
        
        setFlights(processedFlights);
      } else {
        setFlights([]);
      }
      
    } catch (err) {
      console.error("Error loading flights:", err);
      setError(`Failed to load flights: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const loadRecommendedFlights = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/api/flight/unassigned`);
      
      if (!response.ok) {
        console.warn("Failed to load recommended flights");
        return;
      }
      
      const data = await response.json();
      
      if (data.success && data.flights) {
        const processedFlights = data.flights.map(flight => ({
          id: flight.id,
          flightId: flight.flightId,
          destination: flight.destination || "Unknown",
          takeOffTime: flight.takeOffTime,
          planeName: flight.planeName || "Unknown",
          pilotName: "Unassigned",
          status: flight.status || "Scheduled",
          isSoon: flight.isSoon,
          hoursUntil: Math.round(flight.hoursUntil * 10) / 10,
          routesCount: flight.routesCount || 0,
          startingAirport: flight.startingAirport || "TBD"
        }));
        
        setRecommendedFlights(processedFlights);
      }
    } catch (err) {
      console.error("Error loading recommended flights:", err);
    }
  };

  const handleSaveFlight = async (updatedFlight) => {
    try {
      const response = await fetch(`${API_BASE_URL}/api/flight/${updatedFlight.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          aircraft: updatedFlight.planeName,
          startingAirport: parseFloat(updatedFlight.startingAirport) || 0
        })
      });

      if (!response.ok) {
        throw new Error('Failed to update flight');
      }

      const data = await response.json();
      
      if (data.success) {
        // Update local state
        setFlights(prev => prev.map(f => 
          f.id === updatedFlight.id ? updatedFlight : f
        ));
        alert('Flight updated successfully!');
      } else {
        throw new Error(data.message || 'Update failed');
      }
    } catch (error) {
      console.error('Error saving flight:', error);
      alert(`Failed to save: ${error.message}`);
      throw error;
    }
  };

  const handleAcceptFlight = async (flight) => {
    // TODO: Get actual userId from authentication context
    const userId = "31f4a4ce-6e8d-438e-af02-01c3ae28acdc"; // This should come from your auth system
    
    if (!confirm(`Accept flight ${flight.flightId} to ${flight.destination}?`)) {
      return;
    }

    try {
      const response = await fetch(`${API_BASE_URL}/api/flight/accept/${flight.id}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userId })
      });

      if (!response.ok) {
        throw new Error('Failed to accept flight');
      }

      const data = await response.json();
      
      if (data.success) {
        alert('Flight accepted successfully!');
        // Reload both lists
        await loadFlights();
        await loadRecommendedFlights();
      } else {
        throw new Error(data.message || 'Accept failed');
      }
    } catch (error) {
      console.error('Error accepting flight:', error);
      alert(`Failed to accept flight: ${error.message}`);
    }
  };

  const handleDeclineFlight = async (flight) => {
    if (!confirm(`Decline flight ${flight.flightId}? This will keep it in the unassigned list.`)) {
      return;
    }

    // For now, just remove from recommended list locally
    // In a real app, you might want to track declined flights
    setRecommendedFlights(prev => prev.filter(f => f.id !== flight.id));
    alert('Flight declined. It will remain available for other pilots.');
  };

  const formatDateTime = (dateString) => {
    const date = new Date(dateString);
    return {
      date: date.toLocaleDateString(),
      time: date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    };
  };

  const getStatusBadge = (status, isSoon) => {
    if (isSoon) {
      return <span className="badge bg-warning text-dark">⚠️ Departing Soon</span>;
    }
    
    const statusColors = {
      'Scheduled': 'bg-primary',
      'Boarding': 'bg-info',
      'Departed': 'bg-success',
      'Arrived': 'bg-secondary',
      'Delayed': 'bg-danger',
      'Cancelled': 'bg-dark'
    };
    
    const color = statusColors[status] || 'bg-secondary';
    return <span className={`badge ${color}`}>{status}</span>;
  };

  if (loading) {
    return (
      <div className="container mt-5 text-center">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
        <p className="mt-3">Loading flight schedule...</p>
      </div>
    );
  }

  return (
    <div className="container mt-5" style={{ maxWidth: "1200px" }}>
      {/* Header */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>✈️ Flight Schedule</h2>
        <button className="btn btn-secondary" onClick={() => navigate("/")}>
          ← Back
        </button>
      </div>

      {/* Error Display */}
      {error && (
        <div className="alert alert-danger mb-4">
          <strong>❌ Error:</strong> {error}
          <button 
            className="btn btn-sm btn-outline-danger ms-3"
            onClick={loadFlights}
          >
            🔄 Retry
          </button>
        </div>
      )}

      {/* My Assigned Flights */}
      <div className="card shadow mb-4">
        <div className="card-header bg-primary text-white">
          <div className="d-flex justify-content-between align-items-center">
            <h5 className="mb-0">My Assigned Flights ({flights.length})</h5>
            <button 
              className="btn btn-sm btn-light"
              onClick={loadFlights}
            >
              🔄 Refresh
            </button>
          </div>
        </div>
        <div className="card-body p-0">
          {flights.length === 0 ? (
            <div className="text-center py-5 text-muted">
              <p className="mb-3">📭 No flights assigned</p>
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover mb-0">
                <thead className="table-light">
                  <tr>
                    <th>Flight #</th>
                    <th>Starting Airport</th>
                    <th>Destination</th>
                    <th>Date</th>
                    <th>Time</th>
                    <th>Aircraft</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {flights.map((flight) => {
                    const { date, time } = formatDateTime(flight.takeOffTime);
                    return (
                      <tr key={flight.id}>
                        <td className="fw-bold">{flight.flightId}</td>
                        <td>{flight.startingAirport}</td>
                        <td>{flight.destination}</td>
                        <td>{date}</td>
                        <td>{time}</td>
                        <td>{flight.planeName}</td>
                        <td>{getStatusBadge(flight.status, flight.isSoon)}</td>
                        <td>
                          <button 
                            className="btn btn-sm btn-info me-2"
                            onClick={() => setSelectedFlight(flight)}
                          >
                            View
                          </button>
                          <button 
                            className="btn btn-sm btn-warning"
                            onClick={() => setEditingFlight(flight)}
                          >
                            Edit
                          </button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {/* Recommended Flights */}
      <div className="card shadow">
        <div className="card-header bg-success text-white">
          <h5 className="mb-0">Recommended Flights ({recommendedFlights.length})</h5>
        </div>
        <div className="card-body p-0">
          {recommendedFlights.length === 0 ? (
            <div className="text-center py-5 text-muted">
              <p className="mb-3">💡 No recommended flights at this time</p>
              <p className="small">Check back later for flight recommendations</p>
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover mb-0">
                <thead className="table-light">
                  <tr>
                    <th>Flight #</th>
                    <th>Starting Airport</th>
                    <th>Destination</th>
                    <th>Date</th>
                    <th>Time</th>
                    <th>Aircraft</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {recommendedFlights.map((flight) => {
                    const { date, time } = formatDateTime(flight.takeOffTime);
                    return (
                      <tr key={flight.id}>
                        <td className="fw-bold">{flight.flightId}</td>
                        <td>{flight.startingAirport}</td>
                        <td>{flight.destination}</td>
                        <td>{date}</td>
                        <td>{time}</td>
                        <td>{flight.planeName}</td>
                        <td>{getStatusBadge(flight.status, false)}</td>
                        <td>
                          <button 
                            className="btn btn-sm btn-info me-2"
                            onClick={() => setSelectedFlight(flight)}
                          >
                            View
                          </button>
                          <button 
                            className="btn btn-sm btn-success me-2"
                            onClick={() => handleAcceptFlight(flight)}
                          >
                            Accept
                          </button>
                          <button 
                            className="btn btn-sm btn-danger"
                            onClick={() => handleDeclineFlight(flight)}
                          >
                            Decline
                          </button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          )}
        </div>
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
          onSave={handleSaveFlight}
        />
      )}
    </div>
  );
}