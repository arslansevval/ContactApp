import { useState } from "react";
// Custom hook to manage employee form state and validation
export const useEmployeeForm = () => {
  const [employee, setEmployee] = useState(null); // tablo verisi
  const [draftEmployee, setDraftEmployee] = useState(null); // modalda düzenlenen geçici veri
  const [isNew, setIsNew] = useState(false);
  const [emailErrors, setEmailErrors] = useState([]);
  const [phoneErrors, setPhoneErrors] = useState([]);
  
// email and phone validation regex
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  const phoneRegex = /^\+?[0-9]{7,15}$/;

  const initForm = (emp = null, newEmployee = false) => {
    if (emp) {
      setDraftEmployee({
        ...emp,
        emails: emp.emails ?? [],
        phones: emp.phones ?? [],
      });
      setIsNew(newEmployee);
    } else {
      setDraftEmployee({
        id: Date.now(),
        firstname: "",
        lastname: "",
        position: "",
        company: "",
        companyId: 0,
        emails: [{ email: "", isPrimary: true }],
        phones: [{ number: "", isPrimary: true }],
      });
      setIsNew(true);
    }
    setEmailErrors([]);
    setPhoneErrors([]);
  };

  const resetEmployee = () => {
    setDraftEmployee(null);
    setIsNew(false);
    setEmailErrors([]);
    setPhoneErrors([]);
  };

  const handleEmailChange = (index, value) => {
    const newEmails = [...draftEmployee.emails];
    newEmails[index].email = value;
    setDraftEmployee({ ...draftEmployee, emails: newEmails });

    const errors = [...emailErrors];
    errors[index] = value && !emailRegex.test(value);
    setEmailErrors(errors);
  };

  const handlePhoneChange = (index, value) => {
    const newPhones = [...draftEmployee.phones];
    newPhones[index].number = value;
    setDraftEmployee({ ...draftEmployee, phones: newPhones });

    const errors = [...phoneErrors];
    errors[index] = value && !phoneRegex.test(value);
    setPhoneErrors(errors);
  };

  const handlePrimaryEmail = (index) => {
    setDraftEmployee({ ...draftEmployee, emails: draftEmployee.emails.map((e, i) => ({ ...e, isPrimary: i === index })) });
  };

  const handlePrimaryPhone = (index) => {
    setDraftEmployee({ ...draftEmployee, phones: draftEmployee.phones.map((p, i) => ({ ...p, isPrimary: i === index })) });
  };

  const handleAddEmail = () => setDraftEmployee({ ...draftEmployee, emails: [...draftEmployee.emails, { email: "", isPrimary: false }] });
  const handleAddPhone = () => setDraftEmployee({ ...draftEmployee, phones: [...draftEmployee.phones, { number: "", isPrimary: false }] });
  const handleRemoveEmail = (index) => {
    setDraftEmployee({ ...draftEmployee, emails: draftEmployee.emails.filter((_, i) => i !== index) });
    setEmailErrors(emailErrors.filter((_, i) => i !== index));
  };
  const handleRemovePhone = (index) => {
    setDraftEmployee({ ...draftEmployee, phones: draftEmployee.phones.filter((_, i) => i !== index) });
    setPhoneErrors(phoneErrors.filter((_, i) => i !== index));
  };

  const isFormValid = () => {
    if (!draftEmployee) return false;
    const hasInvalidEmail = draftEmployee.emails.some((e, i) => !e.email || emailErrors[i]);
    const hasInvalidPhone = draftEmployee.phones.some((p, i) => !p.number || phoneErrors[i]);
    return !hasInvalidEmail && !hasInvalidPhone && draftEmployee.firstname && draftEmployee.lastname && draftEmployee.position && draftEmployee.companyId;
  };

  return {
    employee,
    setEmployee,
    draftEmployee,
    setDraftEmployee,
    isNew,
    setIsNew,
    initForm,
    resetEmployee,
    handleEmailChange,
    handlePhoneChange,
    handlePrimaryEmail,
    handlePrimaryPhone,
    handleAddEmail,
    handleAddPhone,
    handleRemoveEmail,
    handleRemovePhone,
    emailErrors,
    phoneErrors,
    isFormValid
  };
};
