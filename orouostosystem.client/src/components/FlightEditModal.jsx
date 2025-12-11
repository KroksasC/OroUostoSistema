import { useState, useEffect } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function FlightEditModal({ isOpen, onClose, flight, onSave }) {
  const [editableFlight, setEditableFlight] = useState(flight || {});
  const [isSaving, setIsSaving] = useState(false);
  
  if (!isOpen || !flight) return null;

  // Update local state when flight prop changes
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
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancel = () => {
    onClose();
  };

  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Edit Flight #{flight.id}</h5>
            <button 
              type="button" 
              className="btn-close" 
              onClick={handleCancel}
              disabled={isSaving}
              aria-label="Close"
            ></button>
          </div>
          <div className="modal-body">
            {/* Read-only fields */}
            <div className="mb-3">
              <label className="form-label"><strong>Flight ID:</strong></label>
              <input 
                type="text" 
                className="form-control" 
                value={flight.id} 
                disabled 
              />
            </div>
            
            <div className="mb-3">
              <label className="form-label"><strong>Destination:</strong></label>
              <input 
                type="text" 
                className="form-control" 
                value={flight.destination} 
                disabled 
              />
            </div>
            
            {/* Editable fields */}
            <div className="mb-3">
              <label className="form-label"><strong>Plane Name:</strong></label>
              <input
                type="text"
                className="form-control"
                value={editableFlight.planeName || ''}
                onChange={(e) => handleInputChange('planeName', e.target.value)}
                placeholder="Enter plane name"
                disabled={isSaving}
              />
            </div>
            
            <div className="mb-3">
              <label className="form-label"><strong>Runway:</strong></label>
              <input
                type="text"
                className="form-control"
                value={editableFlight.runAway || ''}
                onChange={(e) => handleInputChange('runAway', e.target.value)}
                placeholder="Enter runway number"
                disabled={isSaving}
              />
            </div>
            
            <div className="mb-3">
              <label className="form-label"><strong>Takeoff Time:</strong></label>
              <input
                type="datetime-local"
                className="form-control"
                value={editableFlight.takeOffTime ? 
                  editableFlight.takeOffTime.slice(0, 16) : ''}
                onChange={(e) => handleInputChange('takeOffTime', e.target.value)}
                disabled={isSaving}
              />
              <div className="form-text">
                Current: {new Date(flight.takeOffTime).toLocaleString()}
              </div>
            </div>
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
              {isSaving ? 'Saving...' : 'Save Changes'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}