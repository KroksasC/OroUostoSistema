import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

const API_BASE_URL = 'http://localhost:5229';

export default function UserFilter() {
    const navigate = useNavigate();
    const [searchTerm, setSearchTerm] = useState('');
    const [users, setUsers] = useState([]);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const token = localStorage.getItem('token');

                const response = await fetch(`${API_BASE_URL}/api/users`, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to load users');
                }

                const data = await response.json();
                setUsers(data);
            } catch (err) {
                console.error(err);
                setError('Failed to load users.');
            }
        };

        fetchUsers();
    }, []);

    const filteredUsers = users.filter(
        (u) =>
            u.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            u.lastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            u.email.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const handleDelete = async (userId) => {
        if (!window.confirm('Delete this user?')) return;

        try {
            const token = localStorage.getItem('token');

            const response = await fetch(`${API_BASE_URL}/api/users/${userId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error('Delete failed');
            }

            setUsers(users.filter(u => u.id !== userId));
        } catch (err) {
            console.error(err);
            alert('Failed to delete user');
        }
    };

    return (
        <div className="container mt-5">
            <h2 className="text-center mb-4">User Management</h2>

            <div className="card shadow p-3" style={{ maxWidth: '800px', margin: '0 auto' }}>
                {error && <p className="text-danger text-center">{error}</p>}

                <input
                    className="form-control mb-3"
                    placeholder="Search by name or email"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                />

                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>First name</th>
                            <th>Last name</th>
                            <th>Email</th>
                            <th />
                        </tr>
                    </thead>
                    <tbody>
                        {filteredUsers.map(u => (
                            <tr key={u.id}>
                                <td>{u.firstName}</td>
                                <td>{u.lastName}</td>
                                <td>{u.email}</td>
                                <td>
                                    <button
                                        className="btn btn-danger btn-sm"
                                        onClick={() => handleDelete(u.id)}
                                    >
                                        Delete
                                    </button>
                                </td>
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
