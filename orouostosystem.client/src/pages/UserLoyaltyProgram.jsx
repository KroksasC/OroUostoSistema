import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function UserLoyaltyProgram() {
    const navigate = useNavigate();

    // Placeholder loyalty data
    const loyalty = {
        level: 'Gold',
        points: 1250,
        benefits: [
            'Priority boarding',
            'Extra baggage allowance',
            'Discounts on flights'
        ]
    };

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">Loyalty Program</h2>

            <div className="card shadow p-3" style={{ maxWidth: '600px', margin: '0 auto' }}>
                <p><strong>Loyalty Level:</strong> {loyalty.level}</p>
                <p><strong>Points:</strong> {loyalty.points}</p>
                <p><strong>Benefits:</strong></p>
                <ul>
                    {loyalty.benefits.map((benefit, index) => (
                        <li key={index}>{benefit}</li>
                    ))}
                </ul>

                <div className="text-center mt-3">
                    <button className="btn btn-secondary" onClick={() => navigate('/home')}>
                        Back
                    </button>
                </div>
            </div>
        </div>
    );
}
