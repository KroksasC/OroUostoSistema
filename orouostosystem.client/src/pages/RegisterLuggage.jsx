import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";

export default function RegisterLuggage() {
  const navigate = useNavigate();

  const [clients, setClients] = useState([]);
  const [flights, setFlights] = useState([]);

  const [role, setRole] = useState([]);

  const [form, setForm] = useState({
    weight: "",
    size: "",
    comment: "",
    clientId: "",
    flightId: ""
  });

  const [errors, setErrors] = useState({});
  const [saving, setSaving] = useState(false);

  // ---------------------------------------
  // ROLE CHECK → Only Admin + Worker
  // ---------------------------------------
  useEffect(() => {
    const storedRole = JSON.parse(localStorage.getItem("role")) ?? [];
    setRole(storedRole);

    const allowed = ["Admin", "Worker"];
    const canAccess = storedRole.some((r) => allowed.includes(r));

    if (!canAccess) {
      navigate("/"); // redirect home
    }
  }, [navigate]);


  // Load clients + flights for dropdowns
  useEffect(() => {
    const load = async () => {
      try {
        const c = await fetch("/api/client");
        const f = await fetch("/api/flights");

        setClients(await c.json());
        setFlights(await f.json());
      } catch (err) {
        console.error("Failed to load dropdown data:", err);
      }
    };
    load();
  }, []);

  // Validator
  const validate = () => {
    let e = {};

    if (!form.weight || form.weight <= 0) e.weight = "Weight must be positive.";
    if (!form.size.trim()) e.size = "Size is required.";
    if (!form.clientId) e.clientId = "Client must be selected.";
    if (!form.flightId) e.flightId = "Flight must be selected.";

    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    setSaving(true);

    const response = await fetch("/api/baggage", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(form)
    });

    setSaving(false);

    if (response.ok) {
      alert("Luggage registered successfully!");
      navigate("/luggageList");
    } else {
      alert("Failed to register luggage.");
    }
  };

  return (
    <div className="container mt-5" style={{ maxWidth: "550px" }}>
      <h2 className="text-center mb-4">Register Luggage</h2>

      <div className="card p-4 shadow-sm">
        <form onSubmit={handleSubmit}>
          {/* Weight */}
          <div className="mb-3">
            <label className="form-label">Weight (kg)</label>
            <input
              type="number"
              className={`form-control ${errors.weight ? "is-invalid" : ""}`}
              value={form.weight}
              onChange={(e) => setForm({ ...form, weight: e.target.value })}
            />
            {errors.weight && <div className="invalid-feedback">{errors.weight}</div>}
          </div>

          {/* Size */}
          <div className="mb-3">
            <label className="form-label">Size</label>
            <input
              type="text"
              className={`form-control ${errors.size ? "is-invalid" : ""}`}
              value={form.size}
              onChange={(e) => setForm({ ...form, size: e.target.value })}
            />
            {errors.size && <div className="invalid-feedback">{errors.size}</div>}
          </div>

          {/* Comment optional */}
          <div className="mb-3">
            <label className="form-label">Comment (optional)</label>
            <textarea
              className="form-control"
              value={form.comment}
              onChange={(e) => setForm({ ...form, comment: e.target.value })}
            />
          </div>

          {/* Client dropdown */}
          <div className="mb-3">
            <label className="form-label">Client</label>
            <select
              className={`form-select ${errors.clientId ? "is-invalid" : ""}`}
              value={form.clientId}
              onChange={(e) => setForm({ ...form, clientId: e.target.value })}
            >
              <option value="">-- Select Client --</option>
              {clients.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.firstName} {c.lastName}
                </option>
              ))}
            </select>
            {errors.clientId && <div className="invalid-feedback">{errors.clientId}</div>}
          </div>

          {/* Flight dropdown */}
          <div className="mb-3">
            <label className="form-label">Flight</label>
            <select
              className={`form-select ${errors.flightId ? "is-invalid" : ""}`}
              value={form.flightId}
              onChange={(e) => setForm({ ...form, flightId: e.target.value })}
            >
              <option value="">-- Select Flight --</option>
              {flights.map((f) => (
                <option key={f.id} value={f.id}>
                  {f.flightNumber} ({new Date(f.flightDate).toLocaleDateString()})
                </option>
              ))}
            </select>
            {errors.flightId && <div className="invalid-feedback">{errors.flightId}</div>}
          </div>

          {/* Buttons */}
          <div className="d-flex justify-content-between">
            <button type="button" className="btn btn-secondary" onClick={() => navigate("/")}>
              Back
            </button>

            <button type="submit" className="btn btn-success" disabled={saving}>
              {saving ? "Saving..." : "Register"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
