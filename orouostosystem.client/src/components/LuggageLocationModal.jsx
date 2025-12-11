import { useEffect, useState } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";
import "bootstrap/dist/css/bootstrap.min.css";
import "leaflet/dist/leaflet.css";

// Fix for missing marker icons in Leaflet
const markerIcon = new L.Icon({
  iconUrl: "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png",
  shadowUrl:
    "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png",
  iconSize: [25, 41],
  iconAnchor: [12, 41],
});

export default function LuggageLocationModal({ isOpen, onClose, luggage }) {
  const [location, setLocation] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  if (!isOpen || !luggage) return null;

  // ================================
  // FETCH LIVE LOCATION FROM BACKEND
  // ================================
  useEffect(() => {
    async function fetchLocation() {
      try {
        const res = await fetch(`/api/baggage/${luggage.id}/location`);
        if (!res.ok) throw new Error("Failed to fetch live location");

        const data = await res.json();
        setLocation(data);
      } catch (err) {
        setError(err.message);
      }
      setLoading(false);
    }

    fetchLocation();
  }, [luggage.id]);

  return (
    <div
      className="modal d-block"
      tabIndex="-1"
      style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
    >
      <div className="modal-dialog modal-lg">
        <div className="modal-content">
          {/* HEADER */}
          <div className="modal-header">
            <h5 className="modal-title">Luggage Location</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          {/* BODY */}
          <div className="modal-body">

            {loading && <p>Loading live location...</p>}

            {error && <p className="text-danger">{error}</p>}

            {location && (
              <>
                <p>
                  <strong>Latitude:</strong> {location.latitude}
                </p>
                <p>
                  <strong>Longitude:</strong> {location.longitude}
                </p>
                <p>
                  <strong>Last updated:</strong>{" "}
                  {new Date(location.updatedAt).toLocaleString()}
                </p>

                {/* MAP */}
                <div style={{ height: "350px", width: "100%" }}>
                  <MapContainer
                    center={[location.latitude, location.longitude]}
                    zoom={7}
                    style={{ height: "100%", width: "100%" }}
                  >
                    <TileLayer
                      url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                      attribution="© OpenStreetMap contributors"
                    />

                    <Marker
                      position={[location.latitude, location.longitude]}
                      icon={markerIcon}
                    >
                      <Popup>
                        Luggage Location <br />
                        {luggage.clientName}
                      </Popup>
                    </Marker>
                  </MapContainer>
                </div>
              </>
            )}

            <div className="text-center mt-3">
              <button className="btn btn-secondary" onClick={onClose}>
                Close
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
