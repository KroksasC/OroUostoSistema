import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import FlightDetailModal from "../components/FlightDetailModal";
import FlightEditModal from "../components/FlightEditModal";
import "bootstrap/dist/css/bootstrap.min.css";

export default function FlightsListPilots() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [flights, setFlights] = useState([]);
  const [recommendedFlights, setRecommendedFlights] = useState([]);
  const [selectedFlight, setSelectedFlight] = useState(null);
  const [editingFlight, setEditingFlight] = useState(null);
  const [decliningFlight, setDecliningFlight] = useState(null);
  const [pilotProfileId, setPilotProfileId] = useState(null);
  const [userId, setUserId] = useState(null);

  useEffect(() => {
    initializeAndLoadData();
  }, []);

  const initializeAndLoadData = async () => {
    const storedUserId = localStorage.getItem("userId");
    if (!storedUserId) {
      alert("Please login first");
      navigate("/login");
      return;
    }
    
    setUserId(storedUserId);

    try {
      const response = await fetch(`/api/flight/pilot/profile/${storedUserId}`);
      
      if (response.ok) {
        const data = await response.json();
        setPilotProfileId(data.id);
        await loadFlights(data.id);
        await loadRecommendedFlights();
      } else {
        console.warn("No pilot profile found");
        alert("Pilot profile not found. Please contact administrator.");
      }
    } catch (error) {
      console.error("Error initializing:", error);
      alert("Failed to load pilot data");
    } finally {
      setLoading(false);
    }
  };

  const loadFlights = async (pilotId) => {
    try {
      const response = await fetch(`/api/flight/pilot/${pilotId}`);
      
      if (response.ok) {
        const data = await response.json();
        setFlights(data.flights || []);
      }
    } catch (error) {
      console.error("Error loading flights:", error);
    }
  };

  const loadRecommendedFlights = async () => {
    try {
      const response = await fetch(`/api/flight/unassigned`);
      
      if (response.ok) {
        const data = await response.json();
        setRecommendedFlights(data.flights || []);
      }
    } catch (error) {
      console.error("Error loading recommended flights:", error);
    }
  };

  const handleAcceptFlight = async (flight) => {
    if (!confirm(`Accept flight ${flight.flightId}?`)) return;

    try {
      const response = await fetch(`/api/flight/accept/${flight.id}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId, role: "co-pilot" })
      });

      if (response.ok) {
        alert('Flight accepted!');
        await loadFlights(pilotProfileId);
        await loadRecommendedFlights();
      } else {
        const data = await response.json();
        alert(data.message || 'Failed to accept flight');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Failed to accept flight');
    }
  };

  const handleSaveFlight = async (updatedFlight) => {
    try {
      const response = await fetch(`/api/flight/${updatedFlight.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          aircraft: updatedFlight.planeName,
          startingAirport: updatedFlight.startingAirport
        })
      });

      if (response.ok) {
        alert('Flight updated!');
        await loadFlights(pilotProfileId);
      } else {
        throw new Error('Update failed');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Failed to update flight');
      throw error;
    }
  };

  const handleDeclineFlight = async (flight, declineType) => {
    try {
      const response = await fetch(`/api/flight/decline/${flight.id}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId, declineType })
      });

      if (response.ok) {
        const data = await response.json();
        alert(data.message);
        setDecliningFlight(null);
        await loadFlights(pilotProfileId);
        await loadRecommendedFlights();
      } else {
        const data = await response.json();
        alert(data.message || 'Failed to decline flight');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Failed to decline flight');
    }
  };

  const formatDateTime = (dateString) => {
    const date = new Date(dateString);
    return {
      date: date.toLocaleDateString(),
      time: date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    };
  };

  const getStatusBadge = (status, isSoon) => {
    if (isSoon) return <span className="badge bg-warning text-dark">⚠️ Soon</span>;
    const colors = {
      'Scheduled': 'bg-primary',
      'Boarding': 'bg-info',
      'Departed': 'bg-success',
      'Delayed': 'bg-danger'
    };
    return <span className={`badge ${colors[status] || 'bg-secondary'}`}>{status}</span>;
  };

  if (loading) {
    return (
      <div className="container mt-5 text-center">
        <div className="spinner-border text-primary"></div>
        <p className="mt-3">Loading...</p>
      </div>
    );
  }

  return (
    <div className="container mt-5" style={{ maxWidth: "1200px" }}>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>✈️ Flight Schedule</h2>
        <button className="btn btn-secondary" onClick={() => navigate("/")}>← Back</button>
      </div>

      {/* My Flights */}
      <div className="card shadow mb-4">
        <div className="card-header bg-primary text-white">
          <div className="d-flex justify-content-between align-items-center">
            <h5 className="mb-0">My Assigned Flights ({flights.length})</h5>
            <button className="btn btn-sm btn-light" onClick={() => loadFlights(pilotProfileId)}>
              🔄 Refresh
            </button>
          </div>
        </div>
        <div className="card-body p-0">
          {flights.length === 0 ? (
            <div className="text-center py-5 text-muted">
              <p>📭 No flights assigned</p>
            </div>
          ) : (
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>Flight #</th>
                  <th>Departure</th>
                  <th>Destination</th>
                  <th>Date</th>
                  <th>Time</th>
                  <th>Aircraft</th>
                  <th>Status</th>
                  <th>Role</th>
                  <th>Type</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {flights.map((flight) => {
                  const { date, time } = formatDateTime(flight.takeOffTime);
                  const isMain = flight.assignedMainPilotId === pilotProfileId;
                  const isCo = flight.assignedPilotId === pilotProfileId;
                  const isRepeating = flight.repeatIntervalHours != null;
                  
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
                        {isMain && <span className="badge bg-primary">Main</span>}
                        {isCo && <span className="badge bg-info">Co-Pilot</span>}
                      </td>
                      <td>
                        {isRepeating ? (
                          <span className="badge bg-success" title={`Repeats every ${flight.repeatIntervalHours} hours`}>
                            🔄 Repeating
                          </span>
                        ) : (
                          <span className="badge bg-secondary">One-time</span>
                        )}
                      </td>
                      <td>
                        <button className="btn btn-sm btn-info me-2" onClick={() => setSelectedFlight(flight)}>
                          View
                        </button>
                        <button className="btn btn-sm btn-warning me-2" onClick={() => setEditingFlight(flight)}>
                          Edit
                        </button>
                        <button className="btn btn-sm btn-danger" onClick={() => setDecliningFlight(flight)}>
                          Decline
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
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
              <p>💡 No recommended flights</p>
            </div>
          ) : (
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>Flight #</th>
                  <th>Departure</th>
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
                        <button className="btn btn-sm btn-info me-2" onClick={() => setSelectedFlight(flight)}>
                          View
                        </button>
                        <button className="btn btn-sm btn-success" onClick={() => handleAcceptFlight(flight)}>
                          Accept
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
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

      {decliningFlight && (
        <DeclineFlightModal
          isOpen={!!decliningFlight}
          flight={decliningFlight}
          onClose={() => setDecliningFlight(null)}
          onDecline={handleDeclineFlight}
        />
      )}
    </div>
  );
}

// Decline Flight Modal Component
function DeclineFlightModal({ isOpen, flight, onClose, onDecline }) {
  if (!isOpen || !flight) return null;

  const isRepeating = flight.repeatIntervalHours != null;

  const handleDecline = (type) => {
    let confirmMsg;
    
    if (type === 'once') {
      const nextDate = new Date(flight.takeOffTime);
      nextDate.setHours(nextDate.getHours() + flight.repeatIntervalHours);
      confirmMsg = `Decline the NEXT occurrence (${nextDate.toLocaleString()}) of flight ${flight.flightId}?\n\nYou will remain assigned to all other occurrences.`;
    } else if (isRepeating) {
      confirmMsg = `Permanently decline ALL future occurrences of repeating flight ${flight.flightId}?\n\nThis cannot be undone.`;
    } else {
      confirmMsg = `Decline flight ${flight.flightId}?\n\nThis will remove you from this one-time flight.`;
    }
    
    if (confirm(confirmMsg)) {
      onDecline(flight, type);
    }
  };

  const getRepeatText = () => {
    if (!isRepeating) return null;
    const hours = flight.repeatIntervalHours;
    if (hours === 24) return "Daily";
    if (hours === 168) return "Weekly";  
    if (hours === 720) return "Monthly";
    return `Every ${hours} hours`;
  };

  const getNextOccurrence = () => {
    if (!isRepeating) return null;
    const nextDate = new Date(flight.takeOffTime);
    nextDate.setHours(nextDate.getHours() + flight.repeatIntervalHours);
    return nextDate;
  };

  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header bg-danger text-white">
            <h5 className="modal-title">⚠️ Decline Flight - {flight.flightId}</h5>
            <button 
              type="button" 
              className="btn-close btn-close-white" 
              onClick={onClose}
              aria-label="Close"
            ></button>
          </div>
          <div className="modal-body">
            <div className="alert alert-warning">
              <strong>⚠️ Emergency Decline:</strong> Use this only in case of emergency or unforeseen circumstances.
            </div>

            <div className="mb-3">
              <p><strong>Flight:</strong> {flight.flightId}</p>
              <p><strong>Destination:</strong> {flight.destination}</p>
              <p><strong>Current Date:</strong> {new Date(flight.takeOffTime).toLocaleString()}</p>
              {isRepeating && (
                <>
                  <p><strong>Repeating:</strong> <span className="badge bg-success">{getRepeatText()}</span></p>
                  <p><strong>Next Occurrence:</strong> {getNextOccurrence()?.toLocaleString()}</p>
                </>
              )}
            </div>

            <h6 className="mb-3">Choose decline option:</h6>

            {isRepeating ? (
              <>semester                <button 
                  className="btn btn-warning w-100 mb-3"
                  onClick={() => handleDecline('once')}
                >
                  <div className="fw-bold">📅 Decline Next Occurrence Only</div>
                  <div className="small mt-1">
                    Skip: {getNextOccurrence()?.toLocaleDateString()} at {getNextOccurrence()?.toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'})}
                  </div>
                  <div className="small text-muted">You remain assigned to all other occurrences</div>
                </button>
                <button 
                  className="btn btn-danger w-100"
                  onClick={() => handleDecline('permanent')}
                >
                  <div className="fw-bold">🚫 Permanently Decline All Occurrences</div>
                  <div className="small mt-1">Remove yourself from this repeating flight completely</div>
                </button>
              </>
            ) : (
              <button 
                className="btn btn-danger w-100"
                onClick={() => handleDecline(null)}
              >
                <div className="fw-bold">🚫 Decline This Flight</div>
                <div className="small mt-1">Remove yourself from this one-time flight</div>
              </button>
            )}
          </div>
          <div className="modal-footer">
            <button className="btn btn-secondary" onClick={onClose}>
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}