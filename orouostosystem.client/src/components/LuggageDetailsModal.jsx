import "bootstrap/dist/css/bootstrap.min.css";

export default function LuggageDetailsModal({ isOpen, onClose, luggage }) {
  if (!isOpen || !luggage) return null;

  return (
    <div
      className="modal d-block"
      tabIndex="-1"
      style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
    >
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          {/* HEADER */}
          <div className="modal-header">
            <h5 className="modal-title">Luggage Details</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          {/* BODY */}
          <div className="modal-body">
            <div className="container">
              
              <div className="row mb-2">
                <div className="col-4 fw-bold">Owner:</div>
                <div className="col-8">{luggage.clientName}</div>
              </div>

              <div className="row mb-2">
                <div className="col-4 fw-bold">Flight:</div>
                <div className="col-8">{luggage.flightNumber}</div>
              </div>

              <div className="row mb-2">
                <div className="col-4 fw-bold">Weight:</div>
                <div className="col-8">{luggage.weight} kg</div>
              </div>

              <div className="row mb-2">
                <div className="col-4 fw-bold">Size:</div>
                <div className="col-8">{luggage.size}</div>
              </div>

              <div className="row mb-2">
                <div className="col-4 fw-bold">Comment:</div>
                <div className="col-8">{luggage.comment}</div>
              </div>

              <div className="row mb-2">
                <div className="col-4 fw-bold">Registered:</div>
                <div className="col-8">
                  {new Date(luggage.registrationDate).toLocaleString()}
                </div>
              </div>
            </div>

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
