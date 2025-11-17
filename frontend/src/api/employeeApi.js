import api from "./axiosInstance";

export const getEmployeesWithContactInfo = () => api.get("/employee/GetAllWithContactInfos");
export const createEmployee = (data) => api.post("/employee/create", data);
export const updateEmployee = (id, data) => api.put(`/employee/update/${id}`, data);
export const deleteEmployee = (id) => api.delete(`/employee/delete/${id}`);
