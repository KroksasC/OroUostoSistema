export default function FlightTableRow({ 
  flight, 
  isRecommended = false, 
  isSoon = false,
  onViewDetails, 
  onEdit,
  onDelete,
  onAccept,
  onDecline
}) {
  // Format date for display
  const formattedDate = new Date(flight.takeOffTime).toLocaleDateString("en-US", {
    weekday: "short",
    year: "numeric",
    month: "short",
    day: "numeric",
  });

  // Format time for display
  const formattedTime = new Date(flight.takeOffTime).toLocaleTimeString("en-US", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });

  // Determine row styling based on isSoon
  const rowStyle = {
    fontWeight: isSoon ? "bold" : "normal",
    backgroundColor: isSoon ? "rgba(255, 0, 0, 0.1)" : "transparent"
  };

  return (
    <tr style={rowStyle}>
      <td>{flight.destination}</td>
      <td>{formattedDate}</td>
      <td>{formattedTime}</td>
      <td>
        {/* Details button - always shown */}
        <button
          className="btn btn-primary btn-sm me-2"
          onClick={() => onViewDetails(flight)}
          title="View flight details"
        >
          Details
        </button>
        
        {/* Conditional buttons based on flight type */}
        {isRecommended ? (
          // Recommended flights: Accept & Decline
          <>
            <button
              className="btn btn-success btn-sm me-2"
              onClick={() => onAccept(flight)}
              title="Accept this flight assignment"
            >
              Accept
            </button>
            <button
              className="btn btn-danger btn-sm me-2"
              onClick={() => onDecline(flight)}
              title="Decline this flight assignment"
            >
              Decline
            </button>
          </>
        ) : (
          // Regular flights: Edit & Delete
          <>
            <button
              className="btn btn-warning btn-sm me-2"
              onClick={() => onEdit(flight)}
              title="Edit flight details"
            >
              Edit
            </button>
            <button
              className="btn btn-danger btn-sm"
              onClick={() => onDelete(flight.id)}
              title="Delete this flight"
            >
              Delete
            </button>
          </>
        )}
      </td>
    </tr>
  );
}