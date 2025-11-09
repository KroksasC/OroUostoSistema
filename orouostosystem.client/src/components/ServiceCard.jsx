export default function ServiceCard({ service, onDelete, onViewDetails, onEdit, onOrder }) {
  return (
    <div className="card h-100 shadow-sm">
      <div className="card-body d-flex flex-column">
        <h5 className="card-title">{service.name}</h5>
        <p className="text-secondary mb-2">
          <strong>Category:</strong> {service.category}
        </p>
        <p className="card-text text-muted flex-grow-1">{service.description}</p>

        <div className="d-flex justify-content-between align-items-center mt-auto">

          <div className="d-flex flex-wrap gap-2">
            <button
              className="btn btn-primary btn-sm"
              onClick={() => onViewDetails(service)}
            >
              View
            </button>
            <button
              className="btn btn-warning btn-sm"
              onClick={() => onEdit(service)}
            >
              Edit
            </button>
            <button
              className="btn btn-danger btn-sm"
              onClick={() => onDelete(service)}
            >
              Delete
            </button>
            <button
              className="btn btn-success btn-sm"
              onClick={() => onOrder(service)}
            >
              Order
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}
