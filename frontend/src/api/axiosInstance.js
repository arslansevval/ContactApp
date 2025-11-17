import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:5001/api",
});

// Request interceptor: Her isteğe token ekler
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor: 401 veya diğer hataları yakalar
api.interceptors.response.use(
  (response) => response,
  (error) => {
    return Promise.reject(error);
  }
);

export default api;
