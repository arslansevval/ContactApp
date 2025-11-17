import api from "./axiosInstance";

export const getCompanies = () => api.get("/company/getall");
export const createCompany = (data) => api.post("/company/create", data);
export const updateCompany = (id, data) => api.put(`/company/update/${id}`, data);
export const deleteCompany = (id) => api.delete(`/company/delete/${id}`);
