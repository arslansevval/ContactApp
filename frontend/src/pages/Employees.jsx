import { useState } from "react";
import {
  Box,
  Paper,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  IconButton,
  Checkbox,
  FormControlLabel
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { Edit, Delete, Add, Remove } from "@mui/icons-material";

const Employees = () => {
  const [rows, setRows] = useState([
    {
      id: 1,
      name: "John Doe",
      emails: [
        { email: "john@mail.com", isPrimary: true },
        { email: "john.alt@mail.com", isPrimary: false }
      ],
      phones: [
        { number: "123-456-7890", isPrimary: true },
        { number: "111-222-3333", isPrimary: false }
      ],
      position: "Engineer",
      company: "Apple",
    },
    {
      id: 2,
      name: "Sarah Smith",
      emails: [
        { email: "sarah@mail.com", isPrimary: true }
      ],
      phones: [
        { number: "234-567-8901", isPrimary: true }
      ],
      position: "Manager",
      company: "Google",
    },
  ]);

  const [openEdit, setOpenEdit] = useState(false);
  const [currentEmployee, setCurrentEmployee] = useState(null);
  const [isNew, setIsNew] = useState(false);

  const handleEdit = (employee) => {
    setCurrentEmployee({
      ...employee,
      emails: employee.emails ?? [],
      phones: employee.phones ?? [],
    });
    setIsNew(false);
    setOpenEdit(true);
  };

  const handleAddEmployee = () => {
    setCurrentEmployee({
      id: Date.now(),
      name: "",
      position: "",
      company: "",
      emails: [{ email: "", isPrimary: true }],
      phones: [{ number: "", isPrimary: true }]
    });
    setIsNew(true);
    setOpenEdit(true);
  };

  const handleClose = () => {
    setOpenEdit(false);
    setCurrentEmployee(null);
    setIsNew(false);
  };

  const handleDelete = (id) => {
    if (window.confirm("Are you sure you want to delete this employee?")) {
      setRows(rows.filter(r => r.id !== id));
    }
  };

  const handleSave = () => {
    if (isNew) {
      setRows([...rows, currentEmployee]);
    } else {
      setRows(rows.map(r => (r.id === currentEmployee.id ? currentEmployee : r)));
    }
    handleClose();
  };

  const handleEmailChange = (index, value) => {
    const newEmails = [...currentEmployee.emails];
    newEmails[index].email = value;
    setCurrentEmployee({ ...currentEmployee, emails: newEmails });
  };

  const handlePhoneChange = (index, value) => {
    const newPhones = [...currentEmployee.phones];
    newPhones[index].number = value;
    setCurrentEmployee({ ...currentEmployee, phones: newPhones });
  };

  const handlePrimaryEmail = (index) => {
    const newEmails = currentEmployee.emails.map((e, i) => ({ ...e, isPrimary: i === index }));
    setCurrentEmployee({ ...currentEmployee, emails: newEmails });
  };

  const handlePrimaryPhone = (index) => {
    const newPhones = currentEmployee.phones.map((p, i) => ({ ...p, isPrimary: i === index }));
    setCurrentEmployee({ ...currentEmployee, phones: newPhones });
  };

  const handleAddEmail = () => {
    setCurrentEmployee({
      ...currentEmployee,
      emails: [...currentEmployee.emails, { email: "", isPrimary: false }]
    });
  };

  const handleAddPhone = () => {
    setCurrentEmployee({
      ...currentEmployee,
      phones: [...currentEmployee.phones, { number: "", isPrimary: false }]
    });
  };

  const handleRemoveEmail = (index) => {
    const newEmails = [...currentEmployee.emails];
    newEmails.splice(index, 1);
    setCurrentEmployee({ ...currentEmployee, emails: newEmails });
  };

  const handleRemovePhone = (index) => {
    const newPhones = [...currentEmployee.phones];
    newPhones.splice(index, 1);
    setCurrentEmployee({ ...currentEmployee, phones: newPhones });
  };

  const columns = [
    { field: "id", headerName: "ID", width: 80 },
    { field: "name", headerName: "Name", flex: 1 },
    {
      field: "emails",
      headerName: "Primary Email",
      flex: 1,
      valueGetter: (emails) => {
        if (!emails || !Array.isArray(emails)) return "";
        const primary = emails.find(e => e.isPrimary);
        return primary?.email || "";
      },
    },
    {
      field: "phones",
      headerName: "Primary Phone",
      flex: 1,
      valueGetter: (phones) => {
        if (!phones || !Array.isArray(phones)) return "";
        const primary = phones.find(p => p.isPrimary);
        return primary?.number || "";
      },
    },
    { field: "position", headerName: "Position", flex: 1 },
    { field: "company", headerName: "Company", flex: 1 },
    {
      field: "actions",
      headerName: "Actions",
      width: 160,
      renderCell: (params) => (
        <>
          <IconButton color="primary" onClick={() => handleEdit(params.row)}>
            <Edit />
          </IconButton>
          <IconButton color="error" onClick={() => handleDelete(params.row.id)}>
            <Delete />
          </IconButton>
        </>
      ),
    },
  ];

  return (
    <Box className="page-container">
      <Paper className="table-card">
        <Box className="table-header">
            <Typography variant="h5" className="table-title">Employees</Typography>
            <Button 
                className="new-btn"
                variant="contained" 
                color="primary" 
                startIcon={<Add />} 
                onClick={handleAddEmployee}
            >
                Add Employee
            </Button>
        </Box>

        <div className="data-grid-wrapper">
          <DataGrid
            rows={rows}
            columns={columns}
            pageSizeOptions={[5, 10]}
            pagination
            disableRowSelectionOnClick
          />
        </div>
      </Paper>

      {currentEmployee && (
        <Dialog open={openEdit} onClose={handleClose} maxWidth="sm" fullWidth>
          <DialogTitle>{isNew ? "Add Employee" : "Edit Employee"}</DialogTitle>
          <DialogContent>
            <TextField
              label="Name"
              fullWidth
              margin="dense"
              value={currentEmployee.name}
              onChange={(e) => setCurrentEmployee({ ...currentEmployee, name: e.target.value })}
            />
            <TextField
              label="Position"
              fullWidth
              margin="dense"
              value={currentEmployee.position}
              onChange={(e) => setCurrentEmployee({ ...currentEmployee, position: e.target.value })}
            />
            <TextField
              label="Company"
              fullWidth
              margin="dense"
              value={currentEmployee.company}
              onChange={(e) => setCurrentEmployee({ ...currentEmployee, company: e.target.value })}
            />

            <Typography mt={2}>Emails</Typography>
            {currentEmployee.emails.map((emailObj, idx) => (
              <Box key={idx} display="flex" alignItems="center" mb={1}>
                <TextField
                  label={`Email ${idx + 1}`}
                  fullWidth
                  value={emailObj.email}
                  onChange={(e) => handleEmailChange(idx, e.target.value)}
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={emailObj.isPrimary}
                      onChange={() => handlePrimaryEmail(idx)}
                    />
                  }
                  label="Primary"
                  sx={{ ml: 1 }}
                />
                <IconButton onClick={() => handleRemoveEmail(idx)} color="error">
                  <Remove />
                </IconButton>
              </Box>
            ))}
            <Button startIcon={<Add />} onClick={handleAddEmail}>Add Email</Button>

            <Typography mt={2}>Phones</Typography>
            {currentEmployee.phones.map((phoneObj, idx) => (
              <Box key={idx} display="flex" alignItems="center" mb={1}>
                <TextField
                  label={`Phone ${idx + 1}`}
                  fullWidth
                  value={phoneObj.number}
                  onChange={(e) => handlePhoneChange(idx, e.target.value)}
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={phoneObj.isPrimary}
                      onChange={() => handlePrimaryPhone(idx)}
                    />
                  }
                  label="Primary"
                  sx={{ ml: 1 }}
                />
                <IconButton onClick={() => handleRemovePhone(idx)} color="error">
                  <Remove />
                </IconButton>
              </Box>
            ))}
            <Button startIcon={<Add />} onClick={handleAddPhone}>Add Phone</Button>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleClose}>Cancel</Button>
            <Button variant="contained" onClick={handleSave}>{isNew ? "Add" : "Save"}</Button>
          </DialogActions>
        </Dialog>
      )}
    </Box>
  );
};

export default Employees;
