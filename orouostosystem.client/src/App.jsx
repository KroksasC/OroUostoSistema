import { Routes, Route } from 'react-router-dom'
import Home from './pages/Home'
import LuggageList from './pages/LuggageList'
import RegisterLuggage from './pages/RegisterLuggage'
import ServicesList from './pages/ServicesList'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/luggageList" element={<LuggageList />} />
      <Route path="/registerLuggage" element={<RegisterLuggage />} />
      <Route path="/servicesList" element={<ServicesList />} />
    </Routes>
  )
}
