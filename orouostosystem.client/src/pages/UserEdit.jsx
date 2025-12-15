import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

const API_BASE_URL = 'http://localhost:5229';

export default function UserEdit() {
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        personalID: '',
        phoneNumber: ''
    });

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const token = localStorage.getItem('token');

                const response = await fetch(`${API_BASE_URL}/api/users/me`, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to load profile');
                }

                const data = await response.json();

                setFormData({
                    firstName: data.firstName || '',
                    lastName: data.lastName || '',
                    email: data.email || '',
                    personalID: data.personalID || '',
                    phoneNumber: data.phoneNumber || ''
                });
            } catch (err) {
                console.error(err);
                setError('Failed to load profile data.');
            } finally {
                setLoading(false);
            }
        };

        fetchProfile();
    }, []);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSave = async () => {
        try {
            const token = localStorage.getItem('token');

            const response = await fetch(`${API_BASE_URL}/api/users/me`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    firstName: formData.firstName,
                    lastName: formData.lastName,
                    email: formData.email,
                    personalCode: formData.personalID, // DTO expects PersonalCode
                    phoneNumber: formData.phoneNumber
                })
            });

            if (!response.ok) {
                throw new Error('Failed to save profile');
            }

            navigate('/home');
        } catch (err) {
            console.error(err);
            setError('Failed to save profile.');
        }
    };

    return (
        <div className="container mt-5 d-flex justify-content-center">
            <div className="card shadow" style={{ maxWidth: '500px', width: '100%' }}>
                <div className="card-header text-center">
                    <h3>Edit Profile</h3>
                </div>

                <div className="card-body">
                    {loading && <p className="text-center">Loading...</p>}

                    {!loading && error && (
                        <p className="text-danger text-center">{error}</p>
                    )}

                    {!loading && !error && (
                        <form>
                            <div className="mb-3">
                                <label className="form-label">First Name</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    name="firstName"
                                    value={formData.firstName}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label">Last Name</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    name="lastName"
                                    value={formData.lastName}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label">Email</label>
                                <input
                                    type="email"
                                    className="form-control"
                                    name="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label">Personal ID</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    name="personalID"
                                    value={formData.personalID}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="mb-3">
                                <label className="form-label">Phone Number</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    name="phoneNumber"
                                    value={formData.phoneNumber}
                                    onChange={handleChange}
                                />
                            </div>

                            <div className="d-flex justify-content-end gap-2 mt-3">
                                <button
                                    type="button"
                                    className="btn btn-secondary"
                                    onClick={() => navigate('/home')}
                                >
                                    Back
                                </button>
                                <button
                                    type="button"
                                    className="btn btn-primary"
                                    onClick={handleSave}
                                >
                                    Save
                                </button>
                            </div>
                        </form>
                    )}
                </div>
            </div>
        </div>
    );
}
