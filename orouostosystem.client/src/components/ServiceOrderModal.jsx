import { useState, useEffect } from 'react'
import axios from 'axios'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function ServiceOrderModal({ show, service, onClose, onOrder }) {
  const [email, setEmail] = useState('')
  const [quantity, setQuantity] = useState(1)

  const userId = localStorage.getItem("userId")
  const storedEmail = localStorage.getItem("email")

  useEffect(() => {
    if (service) {
      setEmail(storedEmail || "")
      setQuantity(1)
    }
  }, [service])

  if (!show || !service) return null

  const handleSubmit = async (e) => {
    e.preventDefault()

    const orderDto = {
      serviceId: service.id,
      serviceName: service.title,
      email: email,
      UserId: userId,
      quantity: quantity,
      totalPrice: service.price * quantity,
      orderDate: new Date()
    }

    console.log(orderDto)

    try {
      await axios.post("/api/service/OrderService", orderDto)
      alert("Order placed successfully!")
      onOrder(orderDto)
      onClose()
    } catch (error) {
      console.error("Order failed:", error)
      alert("Failed to place order.")
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
            <h5 className="modal-title">Order Service: {service.title}</h5>
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
                Total Price: {service.price * quantity}
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
