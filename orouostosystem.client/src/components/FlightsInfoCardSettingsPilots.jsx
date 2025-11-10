import FlightCard from './FlightInfoCardPilots'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function FlightInfoCardPilots({ isOpen, onClose, flight }) {
  if (!isOpen) return null

  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Flight Details</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <FlightCard flight={flight} mode="view" />
            <div className="text-center mt-3">
              <button className="btn btn-secondary" onClick={onClose}>Close</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}