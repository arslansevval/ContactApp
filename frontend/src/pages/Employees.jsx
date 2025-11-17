import { Box, Paper, Typography, Button, Dialog, DialogTitle, DialogContent, DialogActions, TextField, IconButton, Checkbox, FormControlLabel, Select, MenuItem, FormControl, InputLabel } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { Edit, Delete, Add, Remove } from "@mui/icons-material";
import { useState, useEffect, useRef } from "react";
import { useModal } from "../hooks/useModal";
import { useEmployeeForm } from "../hooks/useEmployeeForm";
import { getEmployeesWithContactInfo, createEmployee, updateEmployee, deleteEmployee } from "../api/employeeApi";
import { getCompanies } from "../api/companyApi";

const Employees = () => {
  const [rows, setRows] = useState([]);
  const [companies, setCompanies] = useState([]);
  const { isOpen, openModal, closeModal } = useModal();
  const { employee, setEmployee, resetEmployee } = useEmployeeForm();

  const isMounted = useRef(false); // Strict Mode için

  useEffect(() => {
    if (isMounted.current) return; // ikinci mount'u engelle
    isMounted.current = true;

    const fetchData = async () => {
      try {
        const [empRes, compRes] = await Promise.all([getEmployeesWithContactInfo(), getCompanies()]);
        const transformedEmployees = empRes.data.map(emp => ({
          id: emp.id,
          firstname: emp.firstName,
          lastname: emp.lastName,
          position: emp.position,
          company: emp.companyName,
          companyId: emp.companyId,
          createdAt: formatDate(emp.createdAt),
          emails: emp.contactInfos.filter(c => c.type === "Email").map(c => ({ id: c.id, email: c.value, isPrimary: c.isPrimary })),
          phones: emp.contactInfos.filter(c => c.type === "Phone").map(c => ({ id: c.id, number: c.value, isPrimary: c.isPrimary })),
        }));
        setRows(transformedEmployees);
        setCompanies(compRes.data);
      } catch (err) {
        console.error("Failed to fetch data:", err);
      }
    };

    fetchData();
  }, []);

  const handleAddEmployee = () => {
    const newEmployee = {
      id: 0,
      firstname: "",
      lastname: "",
      position: "",
      company: "",
      companyId: null,
      emails: [{ email: "", isPrimary: true }],
      phones: [{ number: "", isPrimary: true }],
    };
    setEmployee(newEmployee);
    openModal();
  };

  const handleCreateEmployee = async () => {
    try {
      const contactInfos = [
        ...employee.emails.map(e => ({ id: 0, employeeId: 0, type: "Email", value: e.email, isPrimary: e.isPrimary })),
        ...employee.phones.map(p => ({ id: 0, employeeId: 0, type: "Phone", value: p.number, isPrimary: p.isPrimary }))
      ];

      const payload = {
        id: 0,
        firstName: employee.firstname,
        lastName: employee.lastname,
        position: employee.position,
        companyId: employee.companyId,
        companyName: employee.company,
        contactInfos
      };

      const res = await createEmployee(payload);   // ✔ backend sonucu

      // backend’den gelen doğru employee (createdAt dahil)
      const newEmployee = {
        id: res.data.id,
        firstname: res.data.firstName,
        lastname: res.data.lastName,
        position: res.data.position,
        companyId: res.data.companyId,
        company: res.data.companyName,
        createdAt: formatDate(res.data.createdAt),   // ✔ artık doğru tarih
        emails: res.data.contactInfos
          .filter(c => c.type === "Email")
          .map(c => ({ id: c.id, email: c.value, isPrimary: c.isPrimary })),
        phones: res.data.contactInfos
          .filter(c => c.type === "Phone")
          .map(c => ({ id: c.id, number: c.value, isPrimary: c.isPrimary }))
      };

      setRows([...rows, newEmployee]); // ✔ doğru veriyi ekledik

      resetEmployee();
      closeModal();
    } catch (err) {
      console.error("Failed to create employee:", err);
    }
  };


  const handleEdit = (emp) => {
    setEmployee({ ...emp });
    openModal();
  };

  const handleSave = async () => {
    try {
      const contactInfos = [
        ...employee.emails.map(e => ({ id: e.id, employeeId: employee.id, type: "Email", value: e.email, isPrimary: e.isPrimary })),
        ...employee.phones.map(p => ({ id: p.id, employeeId: employee.id, type: "Phone", value: p.number, isPrimary: p.isPrimary }))
      ];
      const payload = {
        id: employee.id,
        firstName: employee.firstname,
        lastName: employee.lastname,
        position: employee.position,
        companyId: employee.companyId,
        companyName: employee.company,
        contactInfos
      };
      await updateEmployee(employee.id, payload);
      setRows(rows.map(r => (r.id === employee.id ? employee : r)));
      resetEmployee();
      closeModal();
    } catch (err) {
      console.error("Failed to update employee:", err);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this employee?")) return;
    try {
      await deleteEmployee(id);
      setRows(rows.filter(r => r.id !== id));
    } catch (err) {
      console.error("Failed to delete employee:", err);
    }
  };

  const handleEmailChange = (index, value) => {
    const newEmails = [...employee.emails];
    newEmails[index].email = value;
    setEmployee({ ...employee, emails: newEmails });
  };

  const handlePhoneChange = (index, value) => {
    const newPhones = [...employee.phones];
    newPhones[index].number = value;
    setEmployee({ ...employee, phones: newPhones });
  };

  const handlePrimaryEmail = (index) => {
    setEmployee({ ...employee, emails: employee.emails.map((e, i) => ({ ...e, isPrimary: i === index })) });
  };

  const handlePrimaryPhone = (index) => {
    setEmployee({ ...employee, phones: employee.phones.map((p, i) => ({ ...p, isPrimary: i === index })) });
  };

  const handleAddEmail = () => setEmployee({ ...employee, emails: [...employee.emails, { email: "", isPrimary: false }] });
  const handleAddPhone = () => setEmployee({ ...employee, phones: [...employee.phones, { number: "", isPrimary: false }] });
  const handleRemoveEmail = (index) => setEmployee({ ...employee, emails: employee.emails.filter((_, i) => i !== index) });
  const handleRemovePhone = (index) => setEmployee({ ...employee, phones: employee.phones.filter((_, i) => i !== index) });

  const formatDate = (isoDate) => {
    const date = new Date(isoDate);
    return date.toLocaleDateString("tr-TR");
  };

  const columns = [
    { field: "id", headerName: "ID", width: 80 },
    { field: "firstname", headerName: "Firstname", flex: 1 },
    { field: "lastname", headerName: "Lastname", flex: 1 },
    { field: "emails", headerName: "Primary Email", flex: 1, valueGetter: (params) => params.find(e => e.isPrimary)?.email || "" },
    { field: "phones", headerName: "Primary Phone", flex: 1, valueGetter: (params) => params.find(p => p.isPrimary)?.number || "" },
    { field: "position", headerName: "Position", flex: 1 },
    { field: "company", headerName: "Company", flex: 1 },
    { field: "createdAt", headerName: "Created At", flex: 1 },
    {
      field: "actions",
      headerName: "Actions",
      width: 160,
      renderCell: (params) => (
        <>
          <IconButton color="primary" onClick={() => handleEdit(params.row)}><Edit /></IconButton>
          <IconButton color="error" onClick={() => handleDelete(params.row.id)}><Delete /></IconButton>
        </>
      )
    }
  ];

  return (
    <Box className="page-container">
      <Paper className="table-card">
        <Box className="table-header">
          <Typography variant="h5" className="table-title">Employees</Typography>
          <Button variant="contained" color="primary" startIcon={<Add />} onClick={handleAddEmployee}>Add Employee</Button>
        </Box>
        <div className="data-grid-wrapper">
          <DataGrid rows={rows} columns={columns} pageSizeOptions={[5, 10]} pagination disableRowSelectionOnClick />
        </div>
      </Paper>

      {employee && (
        <Dialog open={isOpen} onClose={() => { resetEmployee(); closeModal(); }} maxWidth="sm" fullWidth>
          <DialogTitle>{employee.id ? "Edit Employee" : "Add Employee"}</DialogTitle>
          <DialogContent>
            {/* Firstname & Lastname */}
            <Box display="flex" gap={2} mb={2} mt={2}>
              <TextField label="Firstname" fullWidth value={employee.firstname} onChange={(e) => setEmployee({ ...employee, firstname: e.target.value })} />
              <TextField label="Lastname" fullWidth value={employee.lastname} onChange={(e) => setEmployee({ ...employee, lastname: e.target.value })} />
            </Box>

            {/* Position & Company */}
            <Box display="flex" gap={2} mb={2}>
              <TextField label="Position" fullWidth value={employee.position} onChange={(e) => setEmployee({ ...employee, position: e.target.value })} />
              <FormControl fullWidth>
                <InputLabel>Company</InputLabel>
                <Select
                  value={employee.companyId || ""}
                  label="Company"
                  onChange={(e) => {
                    const selected = companies.find(c => c.id === e.target.value);
                    setEmployee({ ...employee, companyId: selected.id, company: selected.name });
                  }}
                >
                  {companies.map(c => (
                    <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Box>

            <Typography mt={2}>Emails</Typography>
            {employee.emails.map((email, idx) => (
              <Box key={idx} display="flex" alignItems="center" mb={1}>
                <TextField label={`Email ${idx + 1}`} fullWidth value={email.email} onChange={(e) => handleEmailChange(idx, e.target.value)} />
                <FormControlLabel control={<Checkbox checked={email.isPrimary} onChange={() => handlePrimaryEmail(idx)} />} label="Primary" sx={{ ml: 1 }} />
                <IconButton onClick={() => handleRemoveEmail(idx)} color="error"><Remove /></IconButton>
              </Box>
            ))}
            <Button startIcon={<Add />} onClick={handleAddEmail}>Add Email</Button>

            <Typography mt={2}>Phones</Typography>
            {employee.phones.map((phone, idx) => (
              <Box key={idx} display="flex" alignItems="center" mb={1}>
                <TextField label={`Phone ${idx + 1}`} fullWidth value={phone.number} onChange={(e) => handlePhoneChange(idx, e.target.value)} />
                <FormControlLabel control={<Checkbox checked={phone.isPrimary} onChange={() => handlePrimaryPhone(idx)} />} label="Primary" sx={{ ml: 1 }} />
                <IconButton onClick={() => handleRemovePhone(idx)} color="error"><Remove /></IconButton>
              </Box>
            ))}
            <Button startIcon={<Add />} onClick={handleAddPhone}>Add Phone</Button>
          </DialogContent>

          <DialogActions>
            <Button onClick={() => { resetEmployee(); closeModal(); }}>Cancel</Button>
            {employee.id ? (
              <Button variant="contained" onClick={handleSave}>Save</Button>
            ) : (
              <Button variant="contained" onClick={handleCreateEmployee}>Add</Button>
            )}
          </DialogActions>
        </Dialog>
      )}
    </Box>
  );
};

export default Employees;
