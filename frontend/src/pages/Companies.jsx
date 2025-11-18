import { useEffect, useState, useRef } from "react";
import { Box, Paper, Typography, Button, IconButton, Dialog, DialogTitle, DialogContent, DialogActions, TextField, Snackbar, Alert } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { Edit, Delete, Add } from "@mui/icons-material";
import { getCompanies, createCompany, updateCompany, deleteCompany } from "../api/companyApi";
import { useModal } from "../hooks/useModal";
import { useCompanyForm } from "../hooks/useCompanyForm";
import DeleteModal from "../components/DeleteModal";

const Companies = () => {
  const [rows, setRows] = useState([]);
  const [editingRowId, setEditingRowId] = useState(null);
  const [isNewRow, setIsNewRow] = useState(false);

  const { isOpen, openModal, closeModal } = useModal();
  const { form, setField, setForm, resetForm, errors, isFormValid } = useCompanyForm();

  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState(null);
  const [warning, setWarning] = useState(""); // 🔹 backend hatalarını göstermek için

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
  // ADD / EDIT
  // -------------------
  const handleAddCompany = () => {
    resetForm();
    setEditingRowId(null);
    setIsNewRow(true);
    openModal();
  };

  const handleEditCompany = (row) => {
    setForm({ ...row });
    setEditingRowId(row.id);
    setIsNewRow(false);
    openModal();
  };

  const handleSave = async () => {
    if (!isFormValid()) return;

    try {
      if (isNewRow) {
        const res = await createCompany(form);
        // 🔹 Backend validation hatası
        if (res.data?.errors) {
          setWarning(res.data.errors[0]); // ilk hatayı göster
          return;
        }
      } else {
        const res = await updateCompany(editingRowId, form);
        if (res.data?.errors) {
          setWarning(res.data.errors[0]);
          return;
        }
      }

      await loadCompanies();
      handleCancel(); // modal ve form reset
    } catch (err) {
      console.error("Failed to save company:", err);
      setWarning("Failed to save company"); // genel hata
    }
  };

  // -------------------
  // CANCEL
  // -------------------
  const handleCancel = () => {
    resetForm();
    setEditingRowId(null);
    setIsNewRow(false);
    closeModal();
  };

  // -------------------
  // DELETE
  // -------------------
  const handleDeleteClick = (id) => {
    setDeleteId(id);
    setDeleteOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!deleteId) return;
    try {
      await deleteCompany(deleteId);
      setRows(rows.filter(r => r.id !== deleteId));
    } catch (err) {
      console.error("Failed to delete company:", err);
      setWarning("Failed to delete company");
    } finally {
      setDeleteId(null);
      setDeleteOpen(false);
    }
  };

  const handleCancelDelete = () => {
    setDeleteId(null);
    setDeleteOpen(false);
  };

  return (
    <Box className="page-container">
      <Paper className="table-card">
        <Box className="table-header">
          <Typography variant="h5">Companies</Typography>
          <Button variant="contained" color="primary" startIcon={<Add />} onClick={handleAddCompany}>Add Company</Button>
        </Box>
        <div className="data-grid-wrapper">
          <DataGrid
            rows={rows}
            columns={[
              { field: "name", headerName: "Name", flex: 1 },
              { field: "address", headerName: "Address", flex: 1 },
              { field: "phone", headerName: "Phone", flex: 1 },
              { field: "email", headerName: "Email", flex: 1 },
              {
                field: "actions",
                headerName: "Actions",
                width: 150,
                renderCell: (params) => (
                  <>
                    <IconButton color="primary" onClick={() => handleEditCompany(params.row)}><Edit /></IconButton>
                    <IconButton color="error" onClick={() => handleDeleteClick(params.row.id)}><Delete /></IconButton>
                  </>
                )
              }
            ]}
            pageSizeOptions={[5, 10]}
            pagination
            disableRowSelectionOnClick
          />
        </div>
      </Paper>

      {/* ------------------- MODAL ------------------- */}
      <Dialog open={isOpen} onClose={handleCancel} maxWidth="sm" fullWidth>
        <DialogTitle>{isNewRow ? "Add Company" : "Edit Company"}</DialogTitle>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <TextField
              label="Name"
              value={form.name}
              onChange={(e) => setField("name", e.target.value)}
              fullWidth
              error={!!errors.name}
              helperText={errors.name || ""}
            />
            <TextField
              label="Address"
              value={form.address}
              onChange={(e) => setField("address", e.target.value)}
              fullWidth
              error={!!errors.address}
              helperText={errors.address || ""}
            />
            <TextField
              label="Phone"
              value={form.phone}
              onChange={(e) => setField("phone", e.target.value)}
              error={!!errors.phone}
              helperText={errors.phone ? "Invalid phone format" : ""}
              fullWidth
            />
            <TextField
              label="Email"
              value={form.email}
              onChange={(e) => setField("email", e.target.value)}
              error={!!errors.email}
              helperText={errors.email ? "Invalid email format" : ""}
              fullWidth
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCancel}>Cancel</Button>
          <Button onClick={handleSave} variant="contained" disabled={!isFormValid()}>Save</Button>
        </DialogActions>
      </Dialog>

      {/* ------------------- DELETE MODAL ------------------- */}
      <DeleteModal
        open={deleteOpen}
        onCancel={handleCancelDelete}
        onConfirm={handleConfirmDelete}
        message="Are you sure you want to delete this company?"
      />

      {/* ------------------- WARNING SNACKBAR ------------------- */}
      <Snackbar
        open={!!warning}
        autoHideDuration={5000}
        onClose={() => setWarning("")}
        anchorOrigin={{ vertical: "top", horizontal: "center" }}
      >
        <Alert severity="warning" onClose={() => setWarning("")}>{warning}</Alert>
      </Snackbar>
    </Box>
  );
};

export default Companies;
