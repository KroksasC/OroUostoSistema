import { Routes, Route } from 'react-router-dom'
import Home from './pages/Home'
import LuggageList from './pages/LuggageList'
import RegisterLuggage from './pages/RegisterLuggage'
import RoutesList from './pages/RoutesList'
import AddRoute from './pages/AddRoute'

export default function App() {
  return (
    <>
      <Routes>
        <Route path="/" element={<Home />}/>
        <Route path="/luggageList" element={<LuggageList />}/>
        <Route path="/registerLuggage" element={<RegisterLuggage />}/>
        <Route path="/routes" element={<RoutesList />}/>
        <Route path="/addRoute" element={<AddRoute />}/>
      </Routes>
    </>
  )
}