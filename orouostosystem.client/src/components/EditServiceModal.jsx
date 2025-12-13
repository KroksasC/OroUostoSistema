import { useState, useEffect } from 'react'
import axios from 'axios'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function EditServiceModal({ show, service, onClose, onEdit }) {
  const [title, setTitle] = useState('')
  const [category, setCategory] = useState('')
  const [price, setPrice] = useState('')
  const [description, setDescription] = useState('')

  useEffect(() => {
    if (service) {
      setTitle(service.title)
      setCategory(service.category)
      setPrice(service.price)
      setDescription(service.description)
    }
  }, [service])

  if (!show || !service) return null

  const handleSubmit = async (e) => {
    e.preventDefault()

    const updatedService = {
      title,
      category,
      price: parseFloat(price),
      description
    }

    try {
      await axios.put(`/api/service/update/${service.id}`, updatedService)

      // Update local state in parent
      onEdit({ ...service, ...updatedService })
      onClose()
    } catch (error) {
      console.error("Failed to update service:", error)
      alert("Failed to update service.")
    }
  }

  return (
    <div
      className="modal fade show"
      style={{ display: 'block', backgroundColor: 'rgba(0,0,0,0.5)' }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content shadow">
          <div className="modal-header">
            <h5 className="modal-title">Edit Service</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          <div className="modal-body">
            <form onSubmit={handleSubmit}>

              <div className="mb-3">
                <label className="form-label">Title</label>
                <input
                  type="text"
                  className="form-control"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
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
                Save Changes
              </button>

            </form>
          </div>
        </div>
      </div>
    </div>
  )
}
