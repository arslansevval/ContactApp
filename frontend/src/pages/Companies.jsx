import { useState } from "react";
import { Box, Paper, Typography, Button, IconButton } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { Edit, Delete, Save, Add } from "@mui/icons-material";

const Companies = () => {
  const [rows, setRows] = useState([
    { id: 1, name: "Apple", address: "Cupertino, CA", phone: "123-456-7890", email: "contact@apple.com", sector: "Technology" },
    { id: 2, name: "Google", address: "Mountain View, CA", phone: "234-567-8901", email: "contact@google.com", sector: "Technology" },
  ]);

  const [editingRowId, setEditingRowId] = useState(null);

  const handleEdit = (id) => setEditingRowId(id);

  const handleDelete = (id) => {
    if (window.confirm("Are you sure you want to delete this company?")) {
      setRows(rows.filter(r => r.id !== id));
    }
  };

  const handleSave = (id) => setEditingRowId(null);

  const handleAddCompany = () => {
    const newRow = {
      id: Date.now(),
      name: "",
      address: "",
      phone: "",
      email: "",
      sector: "",
    };
    setRows(prev => [...prev, newRow]);
    setEditingRowId(newRow.id);
  };

  const handleRowChange = (id, field, value) => {
    setRows(prev => prev.map(r => r.id === id ? { ...r, [field]: value } : r));
  };

  const columns = [
    { 
      field: "name", 
      headerName: "Name", 
      flex: 1,
      renderCell: (params) => editingRowId === params.row.id ? (
        <input 
          className="grid-input"
          value={params.row.name} 
          onChange={(e) => handleRowChange(params.row.id, "name", e.target.value)} 
        />
      ) : <span>{params.row.name}</span>
    },
    { 
      field: "address", 
      headerName: "Address", 
      flex: 1,
      renderCell: (params) => editingRowId === params.row.id ? (
        <input 
          className="grid-input"
          value={params.row.address} 
          onChange={(e) => handleRowChange(params.row.id, "address", e.target.value)} 
        />
      ) : <span>{params.row.address}</span>
    },
    { 
      field: "phone", 
      headerName: "Phone", 
      flex: 1,
      renderCell: (params) => editingRowId === params.row.id ? (
        <input 
          className="grid-input"
          value={params.row.phone} 
          onChange={(e) => handleRowChange(params.row.id, "phone", e.target.value)} 
        />
      ) : <span>{params.row.phone}</span>
    },
    { 
      field: "email", 
      headerName: "Email", 
      flex: 1,
      renderCell: (params) => editingRowId === params.row.id ? (
        <input 
          className="grid-input"
          value={params.row.email} 
          onChange={(e) => handleRowChange(params.row.id, "email", e.target.value)} 
        />
      ) : <span>{params.row.email}</span>
    },
    { 
      field: "sector", 
      headerName: "Sector", 
      flex: 1,
      renderCell: (params) => editingRowId === params.row.id ? (
        <input 
          className="grid-input"
          value={params.row.sector} 
          onChange={(e) => handleRowChange(params.row.id, "sector", e.target.value)} 
        />
      ) : <span>{params.row.sector}</span>
    },
    {
      field: "actions",
      headerName: "Actions",
      width: 180,
      renderCell: (params) => editingRowId === params.row.id ? (
        <IconButton className="save-btn" color="primary" onClick={() => handleSave(params.row.id)}>
          <Save />
        </IconButton>
      ) : (
        <>
          <IconButton className="edit-btn" color="primary" onClick={() => handleEdit(params.row.id)}>
            <Edit />
          </IconButton>
          <IconButton className="delete-btn" color="error" onClick={() => handleDelete(params.row.id)}>
            <Delete />
          </IconButton>
        </>
      )
    }
  ];

  return (
    <Box className="page-container">
      <Paper className="table-card">
        <Box className="table-header">
            <Typography variant="h5" className="table-title">Companies</Typography>
            <Button className="new-btn" variant="contained" color="primary" startIcon={<Add />} onClick={handleAddCompany}>
                Add Company
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
    </Box>
  );
};

export default Companies;
