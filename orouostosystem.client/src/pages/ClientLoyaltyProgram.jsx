import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

const API_BASE_URL = 'http://localhost:5229';

export default function ClientLoyaltyProgram() {
    const navigate = useNavigate();

    const [loyalty, setLoyalty] = useState(null);
    const [message, setMessage] = useState('');
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchLoyalty = async () => {
            try {
                const token = localStorage.getItem('token');

                const response = await fetch(`${API_BASE_URL}/api/loyalty/me`, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                if (!response.ok) {
                    if (response.status === 401) {
                        setMessage('You are not authorized. Please login again.');
                        return;
                    }
                    throw new Error('Failed to fetch loyalty data');
                }

                const data = await response.json();
                setLoyalty(data);
            } catch (error) {
                console.error(error);
                setMessage('Failed to load loyalty program data.');
            } finally {
                setLoading(false);
            }
        };

        fetchLoyalty();
    }, []);

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">Loyalty Program</h2>

            <div className="card shadow p-3" style={{ maxWidth: '600px', margin: '0 auto' }}>
                {loading && <p className="text-center">Loading...</p>}

                {!loading && message && (
                    <p className="text-center text-muted">{message}</p>
                )}

                {!loading && loyalty && (
                    <>
                        <p><strong>Loyalty Level:</strong> {loyalty.level}</p>
                        <p><strong>Points:</strong> {loyalty.points}</p>

                        <p><strong>Benefits:</strong></p>
                        <ul>
                            {loyalty.benefits.map((benefit, index) => (
                                <li key={index}>{benefit}</li>
                            ))}
                        </ul>
                    </>
                )}

                <div className="text-center mt-3">
                    <button className="btn btn-secondary" onClick={() => navigate('/home')}>
                        Back
                    </button>
                </div>
            </div>
        </div>
    );
}
