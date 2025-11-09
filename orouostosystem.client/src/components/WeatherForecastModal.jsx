import 'bootstrap/dist/css/bootstrap.min.css'

export default function WeatherForecastModal({ isOpen, onClose, forecast }) {
  if (!isOpen) return null
  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Weather Forecast</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <p><strong>Humidity:</strong> {forecast.humidity}%</p>
            <p><strong>Temperature:</strong> {forecast.temperature}°C</p>
            <p><strong>Created At:</strong> {forecast.createdAt}</p>
            <p><strong>Pressure:</strong> {forecast.pressure} hPa</p>
            <p><strong>Wind Speed:</strong> {forecast.windSpeed} km/h</p>
            <div className="text-center mt-3">
              <button className="btn btn-secondary" onClick={onClose}>Close</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
