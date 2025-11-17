import { useState } from "react";

export const useEmployeeForm = (initialValues = null) => {
  const [employee, setEmployee] = useState(initialValues);
  const [isNew, setIsNew] = useState(false);

  const initForm = (emp = null, newEmployee = false) => {
    if (emp) {
      setEmployee({
        ...emp,
        emails: emp.emails ?? [],
        phones: emp.phones ?? [],
      });
      setIsNew(newEmployee);
    } else {
      setEmployee({
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
  };

  const resetEmployee = () => {
    setEmployee(null);
    setIsNew(false);
  };

  const handleChange = (field, value) => {
    setEmployee(prev => ({ ...prev, [field]: value }));
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
    const newEmails = employee.emails.map((e, i) => ({ ...e, isPrimary: i === index }));
    setEmployee({ ...employee, emails: newEmails });
  };

  const handlePrimaryPhone = (index) => {
    const newPhones = employee.phones.map((p, i) => ({ ...p, isPrimary: i === index }));
    setEmployee({ ...employee, phones: newPhones });
  };

  const handleAddEmail = () => {
    setEmployee({
      ...employee,
      emails: [...employee.emails, { email: "", isPrimary: false }]
    });
  };

  const handleAddPhone = () => {
    setEmployee({
      ...employee,
      phones: [...employee.phones, { number: "", isPrimary: false }]
    });
  };

  const handleRemoveEmail = (index) => {
    const newEmails = employee.emails.filter((_, i) => i !== index);
    setEmployee({ ...employee, emails: newEmails });
  };

  const handleRemovePhone = (index) => {
    const newPhones = employee.phones.filter((_, i) => i !== index);
    setEmployee({ ...employee, phones: newPhones });
  };

  return {
    employee,
    setEmployee,
    isNew,
    setIsNew,
    initForm,
    resetEmployee,
    handleChange,
    handleEmailChange,
    handlePhoneChange,
    handlePrimaryEmail,
    handlePrimaryPhone,
    handleAddEmail,
    handleAddPhone,
    handleRemoveEmail,
    handleRemovePhone,
  };
};
