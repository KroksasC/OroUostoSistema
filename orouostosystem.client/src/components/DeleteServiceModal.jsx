import 'bootstrap/dist/css/bootstrap.min.css'

export default function DeleteServiceModal({ show, service, onClose, onDelete }) {
  if (!show || !service) return null

  const handleConfirm = () => {
    onDelete(service.id)
    onClose()
  }

  return (
    <div
      className="modal fade show"
      style={{ display: 'block', backgroundColor: 'rgba(0,0,0,0.5)' }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content shadow">
          <div className="modal-header bg-danger text-white">
            <h5 className="modal-title">Confirm Delete</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <p>
              Are you sure you want to delete <strong>{service.name}</strong>?
            </p>
            <p className="text-muted mb-0">
              This action cannot be undone.
            </p>
          </div>
          <div className="modal-footer">
            <button className="btn btn-secondary" onClick={onClose}>
              Cancel
            </button>
            <button className="btn btn-danger" onClick={handleConfirm}>
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
