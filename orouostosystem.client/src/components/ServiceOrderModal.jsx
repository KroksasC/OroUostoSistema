import { useState, useEffect } from 'react'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function ServiceOrderModal({ show, service, onClose, onOrder }) {
  const [email, setEmail] = useState('')
  const [quantity, setQuantity] = useState(1)

  // Reset form when modal opens or service changes
  useEffect(() => {
    if (service) {
      setEmail('')
      setQuantity(1)
    }
  }, [service])

  if (!show || !service) return null

  const handleSubmit = (e) => {
    e.preventDefault()
    // Call parent callback with order details
    onOrder({
      serviceId: service.id,
      serviceName: service.name,
      email,
      quantity: parseInt(quantity, 10),
      totalPrice: service.price * quantity
    })
    onClose()
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
            <h5 className="modal-title">Order Service: {service.name}</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label className="form-label">Your Email</label>
                <input
                  type="email"
                  className="form-control"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
              <div className="mb-3">
                <label className="form-label">Quantity</label>
                <input
                  type="number"
                  className="form-control"
                  value={quantity}
                  min="1"
                  onChange={(e) => setQuantity(e.target.value)}
                  required
                />
              </div>
              <p className="fw-bold">
                Total Price: ${service.price * quantity}
              </p>
              <button type="submit" className="btn btn-success w-100">
                Place Order
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  )
}
