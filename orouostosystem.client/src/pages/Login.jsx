import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';

export default function LoginWindow() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = async () => {
    try {
      const response = await axios.post("/api/account/login", {
        email,
        password
      });

      const { token, userId, username, role } = response.data;

      localStorage.setItem("token", token);
      localStorage.setItem("username", username);
      localStorage.setItem("userId", userId)
      localStorage.setItem("email", email);
      localStorage.setItem("role", JSON.stringify(role));

      navigate("/");

    } catch (e) {
      alert("Invalid email or password");
    }
  }

  return (
    <div className="d-flex justify-content-center align-items-center vh-100 bg-light">
      <div className="card p-4 shadow-sm" style={{ width: '350px' }}>
        <h4 className="mb-3 text-center">Login</h4>

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

        <button className="btn btn-primary w-100" onClick={handleLogin}>Login</button>

        <div className="text-center mt-3">
          <span>Don't have an account? </span>
          <button className="btn btn-link p-0" onClick={() => navigate('/register')}>Register</button>
          <button className="btn btn-link p-0" onClick={() => navigate('/')}>Return to home</button>
        </div>
      </div>
    </div>
  );
}
