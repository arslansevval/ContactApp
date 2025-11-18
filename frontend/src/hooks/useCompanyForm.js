import { useState } from "react";
// Custom hook to manage company form state and validation
export const useCompanyForm = () => {
  const [form, setForm] = useState({
    name: "",
    address: "",
    phone: "",
    email: "",
    sector: "",
  });

  const [errors, setErrors] = useState({
    phone: false,
    email: false,
  });
// email and phone validation regex
  const phoneRegex = /^\+?[0-9]{7,15}$/;
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  const setField = (field, value) => {
    setForm(prev => ({ ...prev, [field]: value }));

    // Validation
    if (field === "phone") setErrors(prev => ({ ...prev, phone: value && !phoneRegex.test(value) }));
    if (field === "email") setErrors(prev => ({ ...prev, email: value && !emailRegex.test(value) }));
  };

  const resetForm = () => {
    setForm({
      name: "",
      address: "",
      phone: "",
      email: "",
      sector: "",
    });
    setErrors({ phone: false, email: false });
  };

  const isFormValid = () => {
    return (
      form.name.trim() &&
      form.address.trim() &&
      !errors.phone &&
      !errors.email
    );
  };

  return {
    form,
    setField,
    setForm,
    resetForm,
    errors,
    isFormValid
  };
};
