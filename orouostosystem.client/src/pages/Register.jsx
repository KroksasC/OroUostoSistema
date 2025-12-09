import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function RegisterWindow() {
  const navigate = useNavigate();
  const [Name, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const handleRegister = async () => {
    if (password !== confirmPassword) {
      alert("Passwords do not match!");
      return;
    }
    try {
      await axios.post("api/account/register", {
        Name,
        email,
        password,
        confirmPassword
      });

      alert("Registration successful!");
      navigate("/login");
    } catch (e) {
      alert("Registration failed.");
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
      <div className="card p-4 shadow-sm" style={{ width: '350px' }}>
        <h4 className="mb-3 text-center">Register</h4>

        <div className="mb-3">
          <label className="form-label">Username</label>
          <input
            type="text"
            className="form-control"
            value={Name}
            onChange={e => setUsername(e.target.value)}
            placeholder="Your name"
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Email</label>
          <input
            type="email"
            className="form-control"
            value={email}
            onChange={e => setEmail(e.target.value)}
            placeholder="example@mail.com"
          />
        </div>

        <div className="mb-3">
          <label className="form-label">Password</label>
          <input
            type="password"
            className="form-control"
            value={password}
            onChange={e => setPassword(e.target.value)}
            placeholder="Password"
          />
        </div>
        <div className="mb-3">
          <label className="form-label">Confirm Password</label>
          <input
            type="password"
            className="form-control"
            value={confirmPassword}
            onChange={e => setConfirmPassword(e.target.value)}
            placeholder="confirmPasssword"
          />
        </div>

        <button className="btn btn-success w-100" onClick={handleRegister}>Register</button>

        <div className="text-center mt-3">
          <span>Already have an account? </span>
          <button className="btn btn-link p-0" onClick={() => navigate('/login')}>Login</button>
        </div>
      </div>
    </div>
  );
}
