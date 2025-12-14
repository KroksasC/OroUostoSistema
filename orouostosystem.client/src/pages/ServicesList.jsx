import { useState, useEffect } from 'react'
import axios from 'axios'

import ServiceCard from '../components/ServiceCard'
import CreateServiceModal from '../components/CreateServiceModal'
import DeleteServiceModal from '../components/DeleteServiceModal'
import EditServiceModal from '../components/EditServiceModal'
import ServiceOrderModal from '../components/ServiceOrderModal'
import ServiceDetailModal from '../components/ServiceDetailModal'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function ServicesList() {
  const [services, setServices] = useState([])
  const role = localStorage.getItem("role") ? JSON.parse(localStorage.getItem("role"))[0] : null;
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false)
  const [isViewModalOpen, setIsViewModalOpen] = useState(false)
  const [isEditModalOpen, setIsEditModalOpen] = useState(false)
  const [isOrderModalOpen, setIsOrderModalOpen] = useState(false)

  const [selectedService, setSelectedService] = useState(null)
  const [selectedEditService, setSelectedEditService] = useState(null)
  const [selectedViewService, setSelectedViewService] = useState(null)
  const [selectedOrderService, setSelectedOrderService] = useState(null)

  useEffect(() => {
    loadServices()
  }, [])

  const loadServices = async () => {
    try {
      const response = await axios.get("/api/service")
      setServices(response.data)
    } catch (error) {
      console.error("Failed to load services:", error)
    }
  }

  const handleCreateService = (newService) => {
    setServices(prev => [...prev, newService])
  }

  const handleDeleteClick = (service) => {
    setSelectedService(service)
    setIsDeleteModalOpen(true)
  }

  const handleDeleteService = (id) => {
    setServices(services.filter(s => s.id !== id))
  }

  const handleViewDetailsClick = (service) => {
    setSelectedViewService(service)
    setIsViewModalOpen(true)
  }

  const handleEditClick = (service) => {
    setSelectedEditService(service)
    setIsEditModalOpen(true)
  }

  const handleEditService = (updatedService) => {
    setServices(
      services.map(s => s.id === updatedService.id ? updatedService : s)
    )
  }

  const handleOrderClick = (service) => {
    setSelectedOrderService(service)
    setIsOrderModalOpen(true)
  }

  const handlePlaceOrder = (order) => {
    console.log("Order placed:", order)
  }

  return (
    <div className="container mt-5" style={{ maxWidth: "900px" }}>

      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="text-center flex-grow-1">Service List</h2>

        {(role === "Worker") && (
          <button
            className="btn btn-success ms-3"
            onClick={() => setIsCreateModalOpen(true)}
          >
            + Create
          </button>
        )}

      </div>

      {/* Services grid */}
      <div className="row">
        {services.map(service => (
          <div key={service.id} className="col-md-6 mb-4">
            <ServiceCard
              service={service}
              onDelete={handleDeleteClick}
              onViewDetails={handleViewDetailsClick}
              onEdit={handleEditClick}
              onOrder={handleOrderClick}
            />
          </div>
        ))}
      </div>

      {/* Modals */}
      <CreateServiceModal
        show={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onCreate={handleCreateService}
      />

      <DeleteServiceModal
        show={isDeleteModalOpen}
        service={selectedService}
        onClose={() => setIsDeleteModalOpen(false)}
        onDelete={handleDeleteService}
      />

      <ServiceDetailModal
        show={isViewModalOpen}
        service={selectedViewService}
        onClose={() => setIsViewModalOpen(false)}
      />

      <EditServiceModal
        show={isEditModalOpen}
        service={selectedEditService}
        onClose={() => setIsEditModalOpen(false)}
        onEdit={handleEditService}
      />

      <ServiceOrderModal
        show={isOrderModalOpen}
        service={selectedOrderService}
        onClose={() => setIsOrderModalOpen(false)}
        onOrder={handlePlaceOrder}
      />
    </div>
  )
}
