import 'bootstrap/dist/css/bootstrap.min.css';

export default function FlightDetailModal({ isOpen, onClose, flight }) {
  if (!isOpen || !flight) return null;

  const formatDateTime = (dateTimeString) => {
    const date = new Date(dateTimeString);
    return date.toLocaleString('en-US', {
      weekday: 'short',
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    });
  };

  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header bg-primary text-white">
            <h5 className="modal-title">Flight Details - {flight.flightId}</h5>
            <button 
              type="button" 
              className="btn-close btn-close-white" 
              onClick={onClose}
              aria-label="Close"
            ></button>
          </div>
          <div className="modal-body">
            <div className="mb-3">
              <p className="mb-1 text-muted"><strong>Flight Number:</strong></p>
              <p className="ms-2 fs-5">{flight.flightId}</p>
            </div>
            
            <div className="mb-3">
              <p className="mb-1 text-muted"><strong>Starting Airport:</strong></p>
              <p className="ms-2">{flight.startingAirport}</p>
            </div>
            
            <div className="mb-3">
              <p className="mb-1 text-muted"><strong>Destination:</strong></p>
              <p className="ms-2">{flight.destination}</p>
            </div>
            
            <div className="mb-3">
              <p className="mb-1 text-muted"><strong>Aircraft:</strong></p>
              <p className="ms-2">{flight.planeName}</p>
            </div>
            
            <div className="mb-3">
              <p className="mb-1 text-muted"><strong>Takeoff Time:</strong></p>
              <p className="ms-2">{formatDateTime(flight.takeOffTime)}</p>
            </div>

            <div className="mb-3">
              <p className="mb-1 text-muted"><strong>Status:</strong></p>
              <p className="ms-2">
                <span className="badge bg-primary">{flight.status}</span>
              </p>
            </div>

            <div className="mb-3">
              <p className="mb-1 text-muted"><strong>Pilot:</strong></p>
              <p className="ms-2">{flight.pilotName}</p>
            </div>

            {flight.isSoon && (
              <div className="alert alert-warning">
                <strong>⚠️ Attention:</strong> This flight is departing soon!
              </div>
            )}
          </div>
          <div className="modal-footer">
            <button 
              className="btn btn-secondary" 
              onClick={onClose}
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}