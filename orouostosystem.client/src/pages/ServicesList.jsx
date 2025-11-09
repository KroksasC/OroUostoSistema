import { useState } from 'react'
import ServiceCard from '../components/ServiceCard'
import CreateServiceModal from '../components/CreateServiceModal'
import DeleteServiceModal from '../components/DeleteServiceModal'
import EditServiceModal from '../components/EditServiceModal'
import ServiceOrderModal from '../components/ServiceOrderModal'
import ServiceDetailModal from '../components/ServiceDetailModal'
import 'bootstrap/dist/css/bootstrap.min.css'

export default function ServicesList() {
  const [services, setServices] = useState([
    { id: 1, name: 'Luggage Storage', category: 'Convenience', description: 'Store your luggage securely for the day.', price: 10 },
    { id: 2, name: 'Fast Track', category: 'Priority', description: 'Skip the waiting lines at security and save time.', price: 25 },
    { id: 3, name: 'VIP Lounge', category: 'Comfort', description: 'Relax and enjoy free snacks, drinks, and WiFi.', price: 40 },
  ])


  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)


  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false)
  const [selectedService, setSelectedService] = useState(null)


  const [isViewModalOpen, setIsViewModalOpen] = useState(false)
  const [selectedViewService, setSelectedViewService] = useState(null)

  const [isEditModalOpen, setIsEditModalOpen] = useState(false)
  const [selectedEditService, setSelectedEditService] = useState(null)

  const [isOrderModalOpen, setIsOrderModalOpen] = useState(false)
  const [selectedOrderService, setSelectedOrderService] = useState(null)

  const handleOrderClick = (service) => {
    setSelectedOrderService(service)
    setIsOrderModalOpen(true)
  }

  const handlePlaceOrder = (order) => {
    console.log('Order placed:', order)
  }

  const handleEditClick = (service) => {
    setSelectedEditService(service)
    setIsEditModalOpen(true)
  }

  const handleCreateService = (newService) => {
    setServices([...services, newService])
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

  const handleEditService = (updatedService) => {
    setServices(services.map(s => s.id === updatedService.id ? updatedService : s))
  }

  return (
    <div className="container mt-5" style={{ maxWidth: "900px" }}>
      {/* Header + Create button */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="text-center flex-grow-1">Service List</h2>
        <button className="btn btn-success ms-3" onClick={() => setIsCreateModalOpen(true)}>
          + Create
        </button>
      </div>

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
