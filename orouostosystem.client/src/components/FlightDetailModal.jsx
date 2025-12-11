import 'bootstrap/dist/css/bootstrap.min.css';

export default function FlightDetailModal({ isOpen, onClose, flight }) {
  if (!isOpen || !flight) return null;

  // Format the date and time nicely
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
          <div className="modal-header">
            <h5 className="modal-title">Flight #{flight.id} Details</h5>
            <button 
              type="button" 
              className="btn-close" 
              onClick={onClose}
              aria-label="Close"
            ></button>
          </div>
          <div className="modal-body">
            <div className="mb-3">
              <p className="mb-1"><strong>Flight ID:</strong></p>
              <p className="ms-2">{flight.id}</p>
            </div>
            
            <div className="mb-3">
              <p className="mb-1"><strong>Destination:</strong></p>
              <p className="ms-2">{flight.destination}</p>
            </div>
            
            <div className="mb-3">
              <p className="mb-1"><strong>Plane Name:</strong></p>
              <p className="ms-2">{flight.planeName}</p>
            </div>
            
            <div className="mb-3">
              <p className="mb-1"><strong>Runway:</strong></p>
              <p className="ms-2">{flight.runAway}</p>
            </div>
            
            <div className="mb-3">
              <p className="mb-1"><strong>Takeoff Time:</strong></p>
              <p className="ms-2">{formatDateTime(flight.takeOffTime)}</p>
            </div>
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