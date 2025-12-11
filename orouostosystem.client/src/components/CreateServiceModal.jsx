import { useState } from 'react';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function CreateServiceModal({ show, onClose, onCreate }) {
  const [name, setName] = useState('');
  const [category, setCategory] = useState('');
  const [price, setPrice] = useState('');
  const [description, setDescription] = useState('');

  if (!show) return null;

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!name || !category || !price) return;

    // TEMP employeeId (you can replace later with dropdown from backend)
    const employeeId = 1;

    const dto = {
      title: name,
      category,
      price: parseFloat(price),
      description: description,
      employeeId: employeeId
    };

    try {
      const response = await axios.post(
        "/api/service/create",
        dto,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("token")}`
          }
        }
      );

      onCreate(response.data);

      setName('');
      setCategory('');
      setPrice('');
      setDescription('');

      onClose();
      alert("Service created successfully!");

      window.location.reload();

    } catch (error) {
      console.error("Error creating service:", error);
      alert("Failed to create service.");
    }
  };

  return (
    <div
      className="modal fade show"
      style={{ display: 'block', backgroundColor: 'rgba(0,0,0,0.5)' }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content shadow">
          <div className="modal-header">
            <h5 className="modal-title">Create New Service</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          <div className="modal-body">
            <form onSubmit={handleSubmit}>

              <div className="mb-3">
                <label className="form-label">Name</label>
                <input
                  type="text"
                  className="form-control"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  required
                />
              </div>

              <div className="mb-3">
                <label className="form-label">Category</label>
                <input
                  type="text"
                  className="form-control"
                  value={category}
                  onChange={(e) => setCategory(e.target.value)}
                  required
                />
              </div>

              <div className="mb-3">
                <label className="form-label">Price</label>
                <input
                  type="number"
                  className="form-control"
                  value={price}
                  onChange={(e) => setPrice(e.target.value)}
                  required
                />
              </div>

              <div className="mb-3">
                <label className="form-label">Description</label>
                <textarea
                  className="form-control"
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                />
              </div>

              <button type="submit" className="btn btn-primary w-100">
                Create
              </button>

            </form>
          </div>

        </div>
      </div>
    </div>
  );
}
