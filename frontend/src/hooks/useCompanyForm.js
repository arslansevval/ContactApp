import { useState } from "react";

export const useCompanyForm = () => {
  const [form, setForm] = useState({
    name: "",
    address: "",
    phone: "",
    email: "",
    sector: "",
  });

  const setField = (field, value) =>
    setForm(prev => ({ ...prev, [field]: value }));

  const resetForm = () =>
    setForm({
      name: "",
      address: "",
      phone: "",
      email: "",
      sector: "",
    });

  return {
    form,
    setField,
    setForm,
    resetForm,
  };
};
