export default function FlightCardPilots({ flight, mode = 'view', onEdit, onDelete }) {
  const isForm = mode === 'form'

  return (
    <div className="card mb-3" style={{ maxWidth: '400px' }}>
      <div className="card-body">
        <p><strong>editeditedit:</strong> {flight.id}</p>
        <p><strong>Destination:</strong> {flight.destination}</p>
        <p><strong>Plane Name:</strong> {flight.planeName}</p>
        <p><strong>Runway:</strong> {flight.runAway}</p>
        <p><strong>Takeoff time:</strong> {flight.takeOffTime}</p>

        {isForm && (
          <div className="mt-3">
            <button className="btn btn-primary me-2" onClick={onEdit}>Edit</button>
          </div>
        )}
      </div>
    </div>
  )
}
