import { useEffect, useState } from "react";
import { getEmployees, createEmployee, deleteEmployee } from "../api/employeeApi";
import EmployeeForm from "../components/EmployeeForm";
import { useApp } from "../context/AppContext";

const Employees = () => {
  const [employees, setEmployees] = useState([]);
  const { loading, setLoading } = useApp();

  const loadData = async () => {
    setLoading(true);
    const res = await getEmployees();
    setEmployees(res.data);
    setLoading(false);
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleCreate = async (data) => {
    await createEmployee(data);
    loadData();
  };

  return (
    <div>
      <h2>Employees</h2>

      {loading && <p>Loading...</p>}

      <EmployeeForm onSubmit={handleCreate} />

      <ul>
        {employees.map((e) => (
          <li key={e.id}>
            {e.name} - {e.email} ({e.position})
            <button onClick={() => deleteEmployee(e.id).then(loadData)}>
              Delete
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Employees;
