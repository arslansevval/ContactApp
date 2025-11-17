import { useEffect, useState, useRef } from "react";
import { Box, Paper, Typography, Button, IconButton } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { Edit, Delete, Save, Add, Close } from "@mui/icons-material";
import { getCompanies, createCompany, updateCompany, deleteCompany } from "../api/companyApi";
import { useModal } from "../hooks/useModal";
import { useCompanyForm } from "../hooks/useCompanyForm";

const Companies = () => {
  const [rows, setRows] = useState([]);
  const [editingRowId, setEditingRowId] = useState(null);
  const [isNewRow, setIsNewRow] = useState(false);

  const { isOpen, openModal, closeModal } = useModal();
  const { form, setField, setForm, resetForm } = useCompanyForm();

  const isMounted = useRef(false);

  useEffect(() => {
    if (isMounted.current) return;
    isMounted.current = true;
    loadCompanies();
  }, []);

  const loadCompanies = async () => {
    try {
      const res = await getCompanies();
      setRows(res.data);
    } catch (err) {
      console.error("Failed to fetch companies:", err);
    }
  };

  // -------------------
  // ADD NEW COMPANY
  // -------------------
  const handleAddCompany = () => {
    const tempId = Date.now();
    const newRow = { id: tempId, ...form };
    setRows(prev => [...prev, newRow]);
    setEditingRowId(tempId);
    setIsNewRow(true);
    openModal(); // Modal aç
  };

  // -------------------
  // SAVE
  // -------------------
  const handleSave = async () => {
    const row = rows.find(r => r.id === editingRowId);
    if (!row) return;

    try {
      if (isNewRow) {
        await createCompany(row);
      } else {
        await updateCompany(row.id, row);
      }
      loadCompanies();
    } catch (err) {
      console.error("Failed to save company:", err);
    }

    setEditingRowId(null);
    setIsNewRow(false);
    resetForm();
    closeModal();
  };

  // -------------------
  // DELETE
  // -------------------
  const handleDelete = async (id) => {
    if (!window.confirm("Delete this company?")) return;

    try {
      await deleteCompany(id);
      loadCompanies();
    } catch (err) {
      console.error("Failed to delete company:", err);
    }
  };

  // -------------------
  // CANCEL
  // -------------------
  const handleCancel = () => {
    if (isNewRow) {
      setRows(prev => prev.filter(r => r.id !== editingRowId));
    }
    setEditingRowId(null);
    setIsNewRow(false);
    resetForm();
    closeModal();
  };

  // -------------------
  // INLINE EDIT
  // -------------------
  const handleRowChange = (id, field, value) => {
    setRows(prev =>
      prev.map(r => (r.id === id ? { ...r, [field]: value } : r))
    );
  };

  const editableCell = (params, field) =>
    editingRowId === params.row.id ? (
      <input
        className="grid-input"
        value={params.row[field] || ""}
        onChange={(e) => handleRowChange(params.row.id, field, e.target.value)}
      />
    ) : (
      <span>{params.row[field]}</span>
    );

  const columns = [
    { field: "name", headerName: "Name", flex: 1, renderCell: (p) => editableCell(p, "name") },
    { field: "address", headerName: "Address", flex: 1, renderCell: (p) => editableCell(p, "address") },
    { field: "phone", headerName: "Phone", flex: 1, renderCell: (p) => editableCell(p, "phone") },
    { field: "email", headerName: "Email", flex: 1, renderCell: (p) => editableCell(p, "email") },
    {
      field: "actions",
      headerName: "Actions",
      width: 200,
      renderCell: (params) =>
        editingRowId === params.row.id ? (
          <>
            <IconButton color="primary" onClick={handleSave}><Save /></IconButton>
            <IconButton color="error" onClick={handleCancel}><Close /></IconButton>
          </>
        ) : (
          <>
            <IconButton color="primary" onClick={() => { setEditingRowId(params.row.id); openModal(); }}><Edit /></IconButton>
            <IconButton color="error" onClick={() => handleDelete(params.row.id)}><Delete /></IconButton>
          </>
        ),
    },
  ];

  return (
    <Box className="page-container">
      <Paper className="table-card">
        <Box className="table-header">
          <Typography variant="h5">Companies</Typography>
          <Button variant="contained" color="primary" startIcon={<Add />} onClick={handleAddCompany}>
            Add Company
          </Button>
        </Box>

        <div className="data-grid-wrapper">
          <DataGrid
            rows={rows}
            columns={columns}
            initialState={{ pagination: { paginationModel: { pageSize: 5, page: 0 } } }}
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
