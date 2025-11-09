export default function RouteCard({ route, mode = 'view', onEdit, onDelete, onForecast }) {
  const isForm = mode === 'form'

  return (
    <div className="card mb-3" style={{ maxWidth: '500px' }}>
      <div className="card-body">
        <p><strong>Route ID:</strong> {route.id || '(new)'}</p>
        <p><strong>Source:</strong> {route.source}</p>
        <p><strong>Destination:</strong> {route.destination}</p>
        <p><strong>Distance (km):</strong> {route.distance}</p>
        <p><strong>Duration:</strong> {route.duration}</p>
        <p><strong>Altitude:</strong> {route.altitude}</p>

        {isForm && (
          <div className="mt-3">
            <button className="btn btn-primary me-2" onClick={onEdit}>Edit</button>
            <button className="btn btn-info me-2" onClick={onForecast}>Weather</button>
            <button className="btn btn-danger" onClick={onDelete}>Delete</button>
          </div>
        )}
      </div>
    </div>
  )
}
