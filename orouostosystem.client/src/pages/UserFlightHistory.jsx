import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function UserFlightHistory() {
    const navigate = useNavigate();

    // Placeholder flight history
    const flights = [
        { id: 'F001', date: '2025-05-12', from: 'Vilnius', to: 'Paris', status: 'Completed' },
        { id: 'F002', date: '2025-06-01', from: 'Vilnius', to: 'London', status: 'Completed' },
        { id: 'F003', date: '2025-07-15', from: 'Vilnius', to: 'Berlin', status: 'Scheduled' },
    ];

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">Flight History</h2>

            <div className="card shadow p-3" style={{ maxWidth: '700px', margin: '0 auto' }}>
                <table className="table table-striped mb-3">
                    <thead>
                        <tr>
                            <th>Flight ID</th>
                            <th>Date</th>
                            <th>From</th>
                            <th>To</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {flights.map((flight) => (
                            <tr key={flight.id}>
                                <td>{flight.id}</td>
                                <td>{flight.date}</td>
                                <td>{flight.from}</td>
                                <td>{flight.to}</td>
                                <td>{flight.status}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>

                <div className="text-center">
                    <button className="btn btn-secondary" onClick={() => navigate('/home')}>
                        Back
                    </button>
                </div>
            </div>
        </div>
    );
}
