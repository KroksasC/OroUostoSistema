import 'bootstrap/dist/css/bootstrap.min.css'

export default function WeatherForecastModal({ isOpen, onClose, forecast }) {
  if (!isOpen) return null

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleString()
  }

  return (
    <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Weather Forecast</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <p><strong>Temperature:</strong> {forecast.temperature.toFixed(1)}°C</p>
            <p><strong>Humidity:</strong> {forecast.humidity.toFixed(1)}%</p>
            <p><strong>Wind Speed:</strong> {forecast.windSpeed.toFixed(1)} km/h</p>
            <p><strong>Pressure:</strong> {forecast.pressure.toFixed(1)} hPa</p>
            <p><strong>Check Time:</strong> {formatDate(forecast.checkTime)}</p>
            <div className="text-center mt-3">
              <button className="btn btn-secondary" onClick={onClose}>Close</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
