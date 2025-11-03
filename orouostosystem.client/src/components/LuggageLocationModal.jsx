import "bootstrap/dist/css/bootstrap.min.css";

export default function LuggageLocationModal({ isOpen, onClose, luggage }) {
  if (!isOpen) return null;

  const currentLocation = "Warehouse A - Section 3";
  const lastUpdated = "2025-11-03 14:30";

  return (
    <div
      className="modal d-block"
      tabIndex="-1"
      style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
    >
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Luggage Location</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <p><strong>Current Location:</strong> {currentLocation}</p>
            <p><strong>A big map</strong></p>
            <div className="text-center mt-3">
              <button className="btn btn-secondary" onClick={onClose}>
                Close
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
