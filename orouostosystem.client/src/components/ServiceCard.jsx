export default function ServiceCard({
  service,
  onDelete,
  onViewDetails,
  onEdit,
  onOrder
}) {

  const role = localStorage.getItem("role") ? JSON.parse(localStorage.getItem("role"))[0] : null;

  let viewBtn = null;
  let editBtn = null;
  let deleteBtn = null;
  let orderBtn = null;

  if (role) {
    viewBtn = (
      <button
        className="btn btn-primary btn-sm"
        onClick={() => onViewDetails(service)}
      >
        View
      </button>
    );
  }
  console.log("ServiceCard role:", role);
  if (role === "Worker") {
    editBtn = (
      <button
        className="btn btn-warning btn-sm"
        onClick={() => onEdit(service)}
      >
        Edit
      </button>
    );
  }

  if (role === "Worker") {
    deleteBtn = (
      <button
        className="btn btn-danger btn-sm"
        onClick={() => onDelete(service)}
      >
        Delete
      </button>
    );
  }

  if (role === "Client") {
    orderBtn = (
      <button
        className="btn btn-success btn-sm"
        onClick={() => onOrder(service)}
      >
        Order
      </button>
    );
  }

  return (
    <div className="card h-100 shadow-sm">
      <div className="card-body d-flex flex-column">

        <h5 className="card-title">{service.title}</h5>

        <p><strong>Category:</strong> {service.category}</p>
        <p><strong>Price:</strong> ${service.price}</p>
        <p className="flex-grow-1">{service.description}</p>

        <div className="d-flex flex-wrap gap-2 mt-auto">
          {viewBtn}
          {editBtn}
          {deleteBtn}
          {orderBtn}
        </div>

      </div>
    </div>
  );
}
