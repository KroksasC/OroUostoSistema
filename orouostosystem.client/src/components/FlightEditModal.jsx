import { useState, useEffect } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function FlightEditModal({ isOpen, onClose, flight, onSave }) {
  const [editableFlight, setEditableFlight] = useState(flight || {});
  const [isSaving, setIsSaving] = useState(false);
  
  if (!isOpen || !flight) return null;

  useEffect(() => {
    setEditableFlight(flight || {});
  }, [flight]);

  const handleInputChange = (field, value) => {
    setEditableFlight(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleSave = async () => {
    setIsSaving(true);
    try {
      if (onSave) {
        await onSave(editableFlight);
      }
      onClose();
    } catch (error) {
      console.error('Failed to save flight:', error);
      alert('Failed to save changes. Please try again.');
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancel = () => {
    setEditableFlight(flight);
    onClose();
  };

  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header bg-warning">
            <h5 className="modal-title">Edit Flight - {flight.flightId}</h5>
            <button 
              type="button" 
              className="btn-close" 
              onClick={handleCancel}
              disabled={isSaving}
              aria-label="Close"
            ></button>
          </div>
          <div className="modal-body">
            {/* Read-only Information */}
            <div className="mb-4">
              <h6 className="text-muted mb-3">Flight Information (Read-Only)</h6>
              
              <div className="mb-3">
                <label className="form-label fw-bold">Flight Number:</label>
                <input 
                  type="text" 
                  className="form-control" 
                  value={flight.flightId} 
                  disabled 
                />
              </div>
              
              <div className="mb-3">
                <label className="form-label fw-bold">Destination:</label>
                <input 
                  type="text" 
                  className="form-control" 
                  value={flight.destination} 
                  disabled 
                />
              </div>

              <div className="mb-3">
                <label className="form-label fw-bold">Takeoff Time:</label>
                <input 
                  type="text" 
                  className="form-control" 
                  value={new Date(flight.takeOffTime).toLocaleString()} 
                  disabled 
                />
              </div>
            </div>

            <hr />

            {/* Editable Fields */}
            <div className="mb-4">
              <h6 className="text-success mb-3">Editable Fields</h6>
              
              <div className="mb-3">
                <label className="form-label fw-bold">
                  Starting Airport (Runway): <span className="text-danger">*</span>
                </label>
                <input
                  type="text"
                  className="form-control"
                  value={editableFlight.startingAirport || ''}
                  onChange={(e) => handleInputChange('startingAirport', e.target.value)}
                  placeholder="e.g., 09, 27, 33"
                  disabled={isSaving}
                />
                <div className="form-text">
                  Enter the runway number for takeoff (numeric value)
                </div>
              </div>
              
              <div className="mb-3">
                <label className="form-label fw-bold">
                  Aircraft: <span className="text-danger">*</span>
                </label>
                <input
                  type="text"
                  className="form-control"
                  value={editableFlight.planeName || ''}
                  onChange={(e) => handleInputChange('planeName', e.target.value)}
                  placeholder="e.g., Boeing 737, Airbus A320"
                  disabled={isSaving}
                />
                <div className="form-text">
                  Specify the aircraft model
                </div>
              </div>
            </div>

            {flight.isSoon && (
              <div className="alert alert-warning">
                <strong>⚠️ Warning:</strong> This flight is departing soon. Please ensure all changes are correct.
              </div>
            )}
          </div>
          <div className="modal-footer">
            <button 
              className="btn btn-secondary" 
              onClick={handleCancel}
              disabled={isSaving}
            >
              Cancel
            </button>
            <button 
              className="btn btn-primary" 
              onClick={handleSave}
              disabled={isSaving}
            >
              {isSaving ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                  Saving...
                </>
              ) : (
                'Save Changes'
              )}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}