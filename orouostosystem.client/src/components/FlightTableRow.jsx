// Update to handle real API data structure
const FlightTableRow = ({ 
  flight, 
  isRecommended = false, 
  isSoon = false,
  onViewDetails, 
  onEdit,
  onDelete,
  onAccept,
  onDecline
}) => {
  // Format date from API (might already be string or Date object)
  const formatDate = (dateString) => {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString("en-US", {
        weekday: "short",
        year: "numeric",
        month: "short",
        day: "numeric",
      });
    } catch {
      return "Invalid Date";
    }
  };

  // Format time
  const formatTime = (dateString) => {
    try {
      const date = new Date(dateString);
      return date.toLocaleTimeString("en-US", {
        hour: "2-digit",
        minute: "2-digit",
        hour12: false,
      });
    } catch {
      return "Invalid Time";
    }
  };

  const rowStyle = {
    fontWeight: isSoon ? "bold" : "normal",
    backgroundColor: isSoon ? "rgba(255, 0, 0, 0.1)" : "transparent"
  };

  return (
    <tr style={rowStyle}>
      <td>{flight.destination || "Unknown"}</td>
      <td>{formatDate(flight.takeOffTime)}</td>
      <td>{formatTime(flight.takeOffTime)}</td>
      <td>{flight.status || "Scheduled"}</td>
      <td>
        <button
          className="btn btn-primary btn-sm me-2"
          onClick={() => onViewDetails(flight)}
          title="View flight details"
        >
          Details
        </button>
        
        {isRecommended ? (
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
};

export { FlightTableRow };
