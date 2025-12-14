import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";

const API_BASE_URL = "http://localhost:5229";

export default function UserServiceHistory() {
  const navigate = useNavigate();
  const [services, setServices] = useState([]);
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchServiceHistory = async () => {
      try {
        const token = localStorage.getItem("token");
        console.log("TOKEN:", token);

        const response = await fetch(`${API_BASE_URL}/api/history/services`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!response.ok) {
          if (response.status === 401) {
            setMessage("You are not authorized. Please login again.");
            return;
          }
          throw new Error("Request failed");
        }

        const data = await response.json();

        if (data.message) {
          setMessage(data.message);
        } else {
          setServices(data);
        }
      } catch (error) {
        console.error(error);
        setMessage("Failed to load service history.");
      } finally {
        setLoading(false);
      }
    };

    fetchServiceHistory();
  }, []);

  return (
    <div className="container mt-5">
      <h2 className="text-center mb-4">Service History</h2>

      <div
        className="card shadow p-3"
        style={{ maxWidth: "900px", margin: "0 auto" }}
      >
        {loading && <p className="text-center">Loading...</p>}

        {!loading && message && (
          <p className="text-center text-muted">{message}</p>
        )}

        {!loading && services.length > 0 && (
          <table className="table table-striped mb-3">
            <thead>
              <tr>
                <th>Service</th>
                <th>Category</th>
                <th>Order Date</th>
                <th>Quantity</th>
                <th>Total Price (€)</th>
              </tr>
            </thead>
            <tbody>
              {services.map((order, index) => (
                <tr key={index}>
                  <td>{order.serviceName}</td>
                  <td>{order.category}</td>
                  <td>{order.orderDate}</td>
                  <td>{order.quantity}</td>
                  <td>{order.totalPrice.toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}

        <div className="text-center">
          <button
            className="btn btn-secondary"
            onClick={() => navigate("/home")}
          >
            Back
          </button>
        </div>
      </div>
    </div>
  );
}
