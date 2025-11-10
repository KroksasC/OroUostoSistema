export default function LuggageCard({ luggage, mode = 'view', onEdit, onDelete }) {
  const isForm = mode === 'form'

  return (
    <div className="card mb-3" style={{ maxWidth: '400px' }}>
      <div className="card-body">
        <p><strong>Luggage ID:</strong> {luggage.id}</p>
        <p><strong>Owner:</strong> {luggage.owner}</p>
        <p><strong>Destination:</strong> {luggage.destination}</p>

        {isForm && (
          <div className="mt-3">
            <button className="btn btn-primary me-2" onClick={onEdit}>Edit</button>
            <button className="btn btn-danger" onClick={onDelete}>Delete</button>
          </div>
        )}
      </div>
    </div>
  )
}
