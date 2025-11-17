import api from "./axiosInstance";

export const getContactInfos = () => api.get("/contactinfo");
export const createContactInfo = (data) => api.post("/contactinfo/create", data);
export const updateContactInfo= (id, data) => api.put(`/contactinfo/update/${id}`, data);
export const deleteContactInfo = (id) => api.delete(`/contactinfo/delete/${id}`);
export const getContactInfoByEmployeeId = (id) => api.delete(`/contactinfo/getbyemployeeid/${id}`);
