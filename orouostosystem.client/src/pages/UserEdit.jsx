import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function UserEdit() {
    const navigate = useNavigate();

    // 
    const [formData] = useState({
        firstName: '[FirstName]',
        lastName: '[LastName]',
        email: '[Email]',
        personalID: '[PersonalID]',
        phoneNumber: '[PhoneNumber]'
    });

    return (
        <div className="container mt-5 d-flex justify-content-center">
            <div className="card shadow" style={{ maxWidth: '500px', width: '100%' }}>
                <div className="card-header text-center">
                    <h3>Edit Profile</h3>
                </div>
                <div className="card-body">
                    <form>
                        <div className="mb-3">
                            <label className="form-label">First Name</label>
                            <input type="text" className="form-control" value={formData.firstName} readOnly />
                        </div>
                        <div className="mb-3">
                            <label className="form-label">Last Name</label>
                            <input type="text" className="form-control" value={formData.lastName} readOnly />
                        </div>
                        <div className="mb-3">
                            <label className="form-label">Email</label>
                            <input type="email" className="form-control" value={formData.email} readOnly />
                        </div>
                        <div className="mb-3">
                            <label className="form-label">Personal ID</label>
                            <input type="text" className="form-control" value={formData.personalID} readOnly />
                        </div>
                        <div className="mb-3">
                            <label className="form-label">Phone Number</label>
                            <input type="text" className="form-control" value={formData.phoneNumber} readOnly />
                        </div>

                        <div className="d-flex justify-content-end gap-2 mt-3">
                            <button type="button" className="btn btn-secondary" onClick={() => navigate('/home')}>
                                Back
                            </button>
                            <button type="button" className="btn btn-primary" onClick={() => navigate('/home')}>
                                Save
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}
