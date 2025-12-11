import { useState, useEffect } from "react";
import LuggageCard from "./LuggageCard";
import "bootstrap/dist/css/bootstrap.min.css";

export default function SearchLuggageModal({ isOpen, onClose }) {
  const [luggageId, setLuggageId] = useState("");
  const [searchedLuggage, setSearchedLuggage] = useState(null);
  const [error, setError] = useState(null);

  // Reset modal on close
  useEffect(() => {
    if (!isOpen) {
      setLuggageId("");
      setSearchedLuggage(null);
      setError(null);
    }
  }, [isOpen]);

  if (!isOpen) return null;

  // ----------------------------
  // SEARCH BY ID
  // ----------------------------
  const handleSearch = async () => {
    setError(null);

    if (!luggageId.trim()) {
      setError("Please enter luggage ID.");
      return;
    }

    try {
      const res = await fetch(`/api/baggage/${luggageId}`);

      if (res.status === 404) {
        setError("Luggage not found.");
        setSearchedLuggage(null);
        return;
      }

      if (!res.ok) {
        setError("Failed to fetch luggage.");
        return;
      }

      const data = await res.json();
      setSearchedLuggage(data);

    } catch (err) {
      setError("Error communicating with the server.");
    }
  };

  // ----------------------------
  // DELETE BAGGAGE
  // ----------------------------
  const handleDelete = async () => {
    if (!searchedLuggage) return;

    if (!window.confirm("Are you sure you want to delete this luggage?")) return;

    const res = await fetch(`/api/baggage/${searchedLuggage.id}`, {
      method: "DELETE"
    });

    if (res.ok) {
      alert("Luggage deleted successfully.");
      onClose();
    } else {
      alert("Failed to delete luggage.");
    }
  };

  // ----------------------------
  // UPDATE BAGGAGE
  // ----------------------------
  const handleEdit = async (updated) => {
    const res = await fetch(`/api/baggage/${searchedLuggage.id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(updated)
    });

    if (res.ok) {
      alert("Luggage updated successfully.");
      setSearchedLuggage({ ...searchedLuggage, ...updated });
    } else {
      alert("Failed to update luggage.");
    }
  };

  return (
    <div
      className="modal d-block"
      tabIndex="-1"
      style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
    >
      <div className="modal-dialog modal-lg">
        <div className="modal-content">

          <div className="modal-header">
            <h5 className="modal-title">Search Luggage</h5>
            <button className="btn-close" onClick={onClose}></button>
          </div>

          <div className="modal-body">

            {/* SEARCH BAR */}
            {!searchedLuggage && (
              <>
                <div className="mb-3">
                  <input
                    type="text"
                    className="form-control"
                    placeholder="Enter Luggage ID"
                    value={luggageId}
                    onChange={(e) => setLuggageId(e.target.value)}
                  />
                </div>

                {error && <p className="text-danger">{error}</p>}

                <button className="btn btn-primary me-2" onClick={handleSearch}>
                  Search
                </button>

                <button className="btn btn-secondary" onClick={onClose}>
                  Cancel
                </button>
              </>
            )}

            {/* RESULT */}
            {searchedLuggage && (
              <>
                <LuggageCard
                  luggage={searchedLuggage}
                  mode="form"
                  onEdit={handleEdit}
                  onDelete={handleDelete}
                />

                <div className="text-center mt-3">
                  <button className="btn btn-secondary" onClick={onClose}>
                    Close
                  </button>
                </div>
              </>
            )}

          </div>
        </div>
      </div>
    </div>
  );
}
