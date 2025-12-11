import { useState } from "react";

export default function LuggageCard({ luggage, mode = "view", onEdit, onDelete }) {
  const isForm = mode === "form";

  const [formData, setFormData] = useState({
    weight: luggage.weight,
    size: luggage.size,
    comment: luggage.comment ?? "",
    clientId: luggage.clientId,
    flightId: luggage.flightId,
  });

  const [errors, setErrors] = useState({});

  // ===========================
  // VALIDATION (same as RegisterLuggage)
  // ===========================
  const validate = () => {
    const newErrors = {};

    if (!formData.weight || formData.weight <= 0) {
      newErrors.weight = "Weight must be positive.";
    }

    if (!formData.size.trim()) {
      newErrors.size = "Size is required.";
    }

    if (!formData.clientId) {
      newErrors.clientId = "Client must be selected.";
    }

    if (!formData.flightId) {
      newErrors.flightId = "Flight must be selected.";
    }

    // COMMENT is optional → NO validation here

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = () => {
    if (!validate()) return;
    onEdit(formData); // send validated data back to parent modal
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  return (
    <div className="card mb-3" style={{ maxWidth: "500px" }}>
      <div className="card-body">

        <p><strong>Owner:</strong> {luggage.clientName}</p>
        <p><strong>Flight:</strong> {luggage.flightNumber}</p>

        {/* WEIGHT */}
        {!isForm ? (
          <p><strong>Weight:</strong> {luggage.weight} kg</p>
        ) : (
          <div className="mb-2">
            <label className="form-label">Weight (kg)</label>
            <input
              type="number"
              name="weight"
              className={`form-control ${errors.weight ? "is-invalid" : ""}`}
              value={formData.weight}
              onChange={handleChange}
            />
            {errors.weight && <div className="invalid-feedback">{errors.weight}</div>}
          </div>
        )}

        {/* SIZE */}
        {!isForm ? (
          <p><strong>Size:</strong> {luggage.size}</p>
        ) : (
          <div className="mb-2">
            <label className="form-label">Size</label>
            <input
              type="text"
              name="size"
              className={`form-control ${errors.size ? "is-invalid" : ""}`}
              value={formData.size}
              onChange={handleChange}
            />
            {errors.size && <div className="invalid-feedback">{errors.size}</div>}
          </div>
        )}

        {/* COMMENT (optional) */}
        {!isForm ? (
          <p><strong>Comment:</strong> {luggage.comment || "—"}</p>
        ) : (
          <div className="mb-2">
            <label className="form-label">Comment (optional)</label>
            <textarea
              name="comment"
              className="form-control"
              value={formData.comment}
              onChange={handleChange}
            />
          </div>
        )}

        {/* REGISTERED DATE */}
        <p>
          <strong>Registered:</strong>{" "}
          {new Date(luggage.registrationDate).toLocaleString()}
        </p>

        {/* BUTTONS */}
        {isForm && (
          <div className="mt-3 d-flex justify-content-between">
            <button className="btn btn-primary" onClick={handleSave}>
              Save Changes
            </button>
            <button className="btn btn-danger" onClick={onDelete}>
              Delete
            </button>
          </div>
        )}

      </div>
    </div>
  );
}
