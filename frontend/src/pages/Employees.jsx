import { 
  Box, Paper, Typography, Button, Dialog, DialogTitle, DialogContent, DialogActions, 
  TextField, IconButton, Checkbox, FormControlLabel, Select, MenuItem, FormControl, InputLabel,
  Snackbar, Alert 
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { Edit, Delete, Add, Remove } from "@mui/icons-material";
import { useState, useEffect, useRef } from "react";
import { useModal } from "../hooks/useModal";
import { useEmployeeForm } from "../hooks/useEmployeeForm";
import { getEmployeesWithContactInfo, createEmployee, updateEmployee, deleteEmployee } from "../api/employeeApi";
import { getCompanies } from "../api/companyApi";
import DeleteModal from "../components/DeleteModal";

const Employees = () => {
  const [rows, setRows] = useState([]);
  const [companies, setCompanies] = useState([]);
  const { isOpen, openModal, closeModal } = useModal();
  const { 
    initForm, 
    resetEmployee, 
    emailErrors, 
    phoneErrors, 
    isFormValid 
  } = useEmployeeForm();

  const [draftEmployee, setDraftEmployee] = useState(null);
  const isMounted = useRef(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState(null);
  const [warningMsg, setWarningMsg] = useState(""); // 🔹 Warning mesaj state

  // Regex
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  const phoneRegex = /^\+?[0-9]{7,15}$/;

  useEffect(() => {
    if (isMounted.current) return;
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
    setDraftEmployee(newEmployee);
    openModal();
  };

  const handleEdit = (emp) => {
    setDraftEmployee(JSON.parse(JSON.stringify(emp))); // deep copy
    openModal();
  };

  const handleSave = async () => {
    if (!isFormValidDraft()) return;

    try {
      const contactInfos = [
        ...draftEmployee.emails.map(e => ({ id: e.id, employeeId: draftEmployee.id, type: "Email", value: e.email, isPrimary: e.isPrimary })),
        ...draftEmployee.phones.map(p => ({ id: p.id, employeeId: draftEmployee.id, type: "Phone", value: p.number, isPrimary: p.isPrimary }))
      ];

      const payload = {
        id: draftEmployee.id,
        firstName: draftEmployee.firstname,
        lastName: draftEmployee.lastname,
        position: draftEmployee.position,
        companyId: draftEmployee.companyId,
        companyName: draftEmployee.company,
        contactInfos
      };

      if (draftEmployee.id === 0) {
        const res = await createEmployee(payload);
        const newEmployee = {
          id: res.data.id,
          firstname: res.data.firstName,
          lastname: res.data.lastName,
          position: res.data.position,
          companyId: res.data.companyId,
          company: res.data.companyName,
          createdAt: formatDate(res.data.createdAt),
          emails: res.data.contactInfos.filter(c => c.type === "Email").map(c => ({ id: c.id, email: c.value, isPrimary: c.isPrimary })),
          phones: res.data.contactInfos.filter(c => c.type === "Phone").map(c => ({ id: c.id, number: c.value, isPrimary: c.isPrimary }))
        };
        setRows([...rows, newEmployee]);
      } else {
        await updateEmployee(draftEmployee.id, payload);
        setRows(rows.map(r => r.id === draftEmployee.id ? draftEmployee : r));
      }

      setDraftEmployee(null);
      resetEmployee();
      closeModal();

    } catch (err) {
      console.error("Failed to save employee:", err);

      // 🔹 Backend validation hatalarını yakala
      if (err.response?.data && Array.isArray(err.response.data)) {
        setWarningMsg(err.response.data.join("\n"));
      } else if (err.response?.data?.message) {
        setWarningMsg(err.response.data.message);
      } else {
        setWarningMsg("Failed to save employee");
      }
    }
  };

  const handleCancel = () => {
    setDraftEmployee(null);
    resetEmployee();
    closeModal();
  };

  const handleDeleteClick = (id) => {
    setDeleteId(id);
    setDeleteOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!deleteId) return;
    try {
      await deleteEmployee(deleteId);
      setRows(rows.filter(r => r.id !== deleteId));
    } catch (err) {
      console.error("Failed to delete employee:", err);
    } finally {
      setDeleteId(null);
      setDeleteOpen(false);
    }
  };

  const handleCancelDelete = () => {
    setDeleteId(null);
    setDeleteOpen(false);
  };

  // Draft input handlers
  const handleDraftEmailChange = (index, value) => {
    const newEmails = [...draftEmployee.emails];
    newEmails[index].email = value;
    setDraftEmployee({...draftEmployee, emails: newEmails});
  };

  const handleDraftPhoneChange = (index, value) => {
    const newPhones = [...draftEmployee.phones];
    newPhones[index].number = value;
    setDraftEmployee({...draftEmployee, phones: newPhones});
  };

  const handleDraftPrimaryEmail = (index) => {
    setDraftEmployee({...draftEmployee, emails: draftEmployee.emails.map((e,i) => ({...e, isPrimary: i===index}))});
  };

  const handleDraftPrimaryPhone = (index) => {
    setDraftEmployee({...draftEmployee, phones: draftEmployee.phones.map((p,i) => ({...p, isPrimary: i===index}))});
  };

  const handleDraftAddEmail = () => {
    setDraftEmployee({...draftEmployee, emails: [...draftEmployee.emails, {email:"", isPrimary:false}]});
  };

  const handleDraftAddPhone = () => {
    setDraftEmployee({...draftEmployee, phones: [...draftEmployee.phones, {number:"", isPrimary:false}]});
  };

  const handleDraftRemoveEmail = (index) => {
    setDraftEmployee({...draftEmployee, emails: draftEmployee.emails.filter((_,i)=>i!==index)});
  };

  const handleDraftRemovePhone = (index) => {
    setDraftEmployee({...draftEmployee, phones: draftEmployee.phones.filter((_,i)=>i!==index)});
  };

  // Validation
  const isFormValidDraft = () => {
    if (!draftEmployee) return false;
    const invalidEmail = draftEmployee.emails.some(e => !e.email || !emailRegex.test(e.email));
    const invalidPhone = draftEmployee.phones.some(p => !p.number || !phoneRegex.test(p.number));
    return !invalidEmail && !invalidPhone && draftEmployee.firstname && draftEmployee.lastname && draftEmployee.position && draftEmployee.companyId;
  };

  const formatDate = (isoDate) => new Date(isoDate).toLocaleDateString("tr-TR");

  const columns = [
    { field: "id", headerName: "ID", width: 80 },
    { field: "firstname", headerName: "Firstname", flex: 1 },
    { field: "lastname", headerName: "Lastname", flex: 1 },
    { field: "emails", headerName: "Primary Email", flex: 1, valueGetter: (params) => params.find(e=>e.isPrimary)?.email||"" },
    { field: "phones", headerName: "Primary Phone", flex: 1, valueGetter: (params) => params.find(p=>p.isPrimary)?.number||"" },
    { field: "position", headerName: "Position", flex: 1 },
    { field: "company", headerName: "Company", flex: 1 },
    { field: "createdAt", headerName: "Created At", flex: 1 },
    {
      field: "actions",
      headerName: "Actions",
      width: 160,
      renderCell: (params) => (
        <>
          <IconButton color="primary" onClick={()=>handleEdit(params.row)}><Edit /></IconButton>
          <IconButton color="error" onClick={()=>handleDeleteClick(params.row.id)}><Delete /></IconButton>
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
          <DataGrid rows={rows} columns={columns} pageSizeOptions={[5,10]} pagination disableRowSelectionOnClick />
        </div>
      </Paper>

      {draftEmployee && (
        <Dialog open={isOpen} onClose={handleCancel} maxWidth="sm" fullWidth>
          <DialogTitle>{draftEmployee.id===0 ? "Add Employee" : "Edit Employee"}</DialogTitle>
          <DialogContent>
            <Box display="flex" gap={2} mb={2} mt={2}>
              <TextField label="Firstname" fullWidth value={draftEmployee.firstname} onChange={(e)=>setDraftEmployee({...draftEmployee, firstname:e.target.value})}/>
              <TextField label="Lastname" fullWidth value={draftEmployee.lastname} onChange={(e)=>setDraftEmployee({...draftEmployee, lastname:e.target.value})}/>
            </Box>
            <Box display="flex" gap={2} mb={2}>
              <TextField label="Position" fullWidth value={draftEmployee.position} onChange={(e)=>setDraftEmployee({...draftEmployee, position:e.target.value})}/>
              <FormControl fullWidth>
                <InputLabel>Company</InputLabel>
                <Select
                  value={draftEmployee.companyId || ""}
                  label="Company"
                  onChange={(e)=>{
                    const selected = companies.find(c=>c.id===e.target.value);
                    setDraftEmployee({...draftEmployee, companyId:selected.id, company:selected.name});
                  }}
                >
                  {companies.map(c=><MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>)}
                </Select>
              </FormControl>
            </Box>

            <Typography mt={2}>Emails</Typography>
            {draftEmployee.emails.map((email, idx)=>(
              <Box key={idx} display="flex" alignItems="center" mb={1}>
                <TextField
                  label={`Email ${idx+1}`}
                  fullWidth
                  value={email.email}
                  onChange={(e)=>handleDraftEmailChange(idx, e.target.value)}
                  error={email.email && !emailRegex.test(email.email)} // boşsa error false
                  helperText={email.email && !emailRegex.test(email.email) ? "Invalid email" : ""}
                />
                <FormControlLabel control={<Checkbox checked={email.isPrimary} onChange={()=>handleDraftPrimaryEmail(idx)}/>} label="Primary" sx={{ml:1}}/>
                <IconButton onClick={()=>handleDraftRemoveEmail(idx)} color="error"><Remove/></IconButton>
              </Box>
            ))}
            <Button startIcon={<Add />} onClick={handleDraftAddEmail}>Add Email</Button>

            <Typography mt={2}>Phones</Typography>
            {draftEmployee.phones.map((phone, idx)=>(
              <Box key={idx} display="flex" alignItems="center" mb={1}>
                <TextField
                  label={`Phone ${idx+1}`}
                  fullWidth
                  value={phone.number}
                  onChange={(e)=>handleDraftPhoneChange(idx, e.target.value)}
                  error={phone.number && !phoneRegex.test(phone.number)} // boşsa error false
                  helperText={phone.number && !phoneRegex.test(phone.number) ? "Invalid phone" : ""}
                />
                <FormControlLabel control={<Checkbox checked={phone.isPrimary} onChange={()=>handleDraftPrimaryPhone(idx)}/>} label="Primary" sx={{ml:1}}/>
                <IconButton onClick={()=>handleDraftRemovePhone(idx)} color="error"><Remove/></IconButton>
              </Box>
            ))}
            <Button startIcon={<Add />} onClick={handleDraftAddPhone}>Add Phone</Button>

          </DialogContent>

          <DialogActions>
            <Button onClick={handleCancel}>Cancel</Button>
            <Button variant="contained" onClick={handleSave} disabled={!isFormValidDraft()}>{draftEmployee.id===0 ? "Add" : "Save"}</Button>
          </DialogActions>
        </Dialog>
      )}

      <DeleteModal
        open={deleteOpen}
        onCancel={handleCancelDelete}
        onConfirm={handleConfirmDelete}
        message="Are you sure you want to delete this employee?"
      />

      {/* 🔹 Warning Snackbar */}
      <Snackbar
        open={!!warningMsg}
        autoHideDuration={5000}
        onClose={() => setWarningMsg("")}
        anchorOrigin={{ vertical: "top", horizontal: "center" }}
      >
        <Alert onClose={() => setWarningMsg("")} severity="warning" sx={{ width: '100%' }}>
          {warningMsg}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default Employees;
