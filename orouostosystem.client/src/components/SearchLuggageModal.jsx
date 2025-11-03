import { useState, useEffect } from "react";
import LuggageCard from "./LuggageCard";
import "bootstrap/dist/css/bootstrap.min.css";

export default function SearchLuggageModal({ isOpen, onClose }) {
  const [luggageId, setLuggageId] = useState("");
  const [searchedLuggage, setSearchedLuggage] = useState(null);

  useEffect(() => {
    if (!isOpen) {
      setLuggageId("");
      setSearchedLuggage(null);
    }
  }, [isOpen]);

  if (!isOpen) return null;

  const handleSearch = () => {
    const found = { id: luggageId, owner: "John Doe", destination: "Paris" };
    setSearchedLuggage(found);
  };

  return (
    <div
      className="modal d-block"
      tabIndex="-1"
      style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
    >
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Search Luggage</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            {!searchedLuggage ? (
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
                <button className="btn btn-primary me-2" onClick={handleSearch}>
                  Search
                </button>
                <button className="btn btn-secondary" onClick={onClose}>
                  Cancel
                </button>
              </>
            ) : (
              <>
                <LuggageCard
                  luggage={searchedLuggage}
                  mode="form"
                  onEdit={() => console.log("Edit clicked")}
                  onDelete={() => console.log("Delete clicked")}
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
