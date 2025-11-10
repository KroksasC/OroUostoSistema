import { Routes, Route } from 'react-router-dom'
import Login from "./pages/Login";
import Register from "./pages/Register";
import Home from './pages/Home'
import LuggageList from './pages/LuggageList'
import RegisterLuggage from './pages/RegisterLuggage'
import RoutesList from './pages/RoutesList'
import AddRoute from './pages/AddRoute'
import ServicesList from './pages/ServicesList'
import UserEdit from './pages/UserEdit'
import UserFlightHistory from './pages/UserFlightHistory'
import UserLoyaltyProgram from './pages/UserLoyaltyProgram'
import UserFilter from './pages/UserFilter'
import FlightsListPilots from './pages/FlightsListPilots';

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/home" element={<Home />} />
      <Route path="/luggageList" element={<LuggageList />} />
      <Route path="/flightsListPilots" element={<FlightsListPilots />}/>

      <Route path="/registerLuggage" element={<RegisterLuggage />} />
      <Route path="/routes" element={<RoutesList />} />
      <Route path="/addRoute" element={<AddRoute />} />
      <Route path="/servicesList" element={<ServicesList />} />

    {/* User pages */}
      <Route path="/userEdit" element={<UserEdit />} />
      <Route path="/userFlightHistory" element={<UserFlightHistory />} />
      <Route path="/userLoyaltyProgram" element={<UserLoyaltyProgram />} />
      <Route path="/userFilter" element={<UserFilter />} />
    </Routes>
  )
}

