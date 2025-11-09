import 'bootstrap/dist/css/bootstrap.min.css'

export default function ServiceDetailsModal({ show, service, onClose }) {
  if (!show || !service) return null

  return (
    <div
      className="modal fade show"
      style={{ display: 'block', backgroundColor: 'rgba(0,0,0,0.5)' }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content shadow">
          <div className="modal-header">
            <h5 className="modal-title">{service.name}</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <p><strong>Category:</strong> {service.category}</p>
            <p><strong>Price:</strong> ${service.price}</p>
            <p><strong>Description:</strong></p>
            <p>{service.description}</p>
          </div>
          <div className="modal-footer">
            <button className="btn btn-secondary" onClick={onClose}>
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}