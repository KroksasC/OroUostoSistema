import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function UserFilter() {
    const navigate = useNavigate();
    const [searchTerm, setSearchTerm] = useState('');

    // Placeholder user data
    const users = [
        { id: 1, firstName: 'John', lastName: 'Doe', email: 'john@example.com' },
        { id: 2, firstName: 'Jane', lastName: 'Smith', email: 'jane@example.com' },
        { id: 3, firstName: 'Alice', lastName: 'Johnson', email: 'alice@example.com' },
    ];

    // Filter users based on search term
    const filteredUsers = users.filter(
        (user) =>
            user.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            user.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            user.email.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const handleDelete = (userId) => {
        // Placeholder delete logic
        alert(`Delete user with ID: ${userId}`);
    };

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">User Management</h2>

            <div className="card shadow p-3" style={{ maxWidth: '800px', margin: '0 auto' }}>
                <div className="mb-3">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Search by first name, last name or email"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                </div>

                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>First Name</th>
                            <th>Last Name</th>
                            <th>Email</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredUsers.map((user) => (
                            <tr key={user.id}>
                                <td>{user.firstName}</td>
                                <td>{user.lastName}</td>
                                <td>{user.email}</td>
                                <td>
                                    <button
                                        className="btn btn-danger btn-sm"
                                        onClick={() => handleDelete(user.id)}
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>

                <div className="text-center mt-3">
                    <button className="btn btn-secondary" onClick={() => navigate('/home')}>
                        Back
                    </button>
                </div>
            </div>
        </div>
    );
}
