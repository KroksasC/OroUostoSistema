import { Routes, Route } from 'react-router-dom'
import Home from './pages/Home'
import LuggageList from './pages/LuggageList'
import RegisterLuggage from './pages/RegisterLuggage'
import FlightsListPilots from './pages/FlightsListPilots'

export default function App() {
  return (
    <>
      <Routes>
        <Route path="/" element={<Home />}/>
        <Route path="/luggageList" element={<LuggageList />}/>
        <Route path="/registerLuggage" element={<RegisterLuggage />}/>

        <Route path="/flightsListPilots" element={<FlightsListPilots />}/>
      </Routes>
    </>
  )
}