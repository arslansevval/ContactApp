import { Dialog, DialogTitle, DialogContent, DialogActions, Button } from "@mui/material";

// Modal component to confirm deletion actions
const DeleteModal = ({ open, onCancel, onConfirm, message }) => {
  return (
    <Dialog open={open} onClose={onCancel}>
      <DialogTitle>Delete Confirmation</DialogTitle>
      <DialogContent>{message}</DialogContent>
      <DialogActions>
        <Button onClick={onCancel}>Cancel</Button>
        <Button color="error" onClick={onConfirm}>Delete</Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeleteModal;
