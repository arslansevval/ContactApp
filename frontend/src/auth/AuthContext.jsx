import { createContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";
import api from "../api/axiosInstance";

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem("user");
    if (!stored) return null;
    try {
      return JSON.parse(stored);
    } catch (err) {
      console.error("Invalid JSON in localStorage for 'user'. Clearing.", err);
      localStorage.removeItem("user");
      return null;
    }
  });

  const [token, setToken] = useState(() => localStorage.getItem("token"));
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  };

  const validateToken = (jwtToken) => {
    try {
      const decoded = jwtDecode(jwtToken);
      const now = Math.floor(Date.now() / 1000);
      if (decoded.exp && decoded.exp < now) {
        logout();
        setError("Session expired, please log in again.");
        return false;
      }
      return true;
    } catch {
      logout();
      setError("Invalid token, please log in again.");
      return false;
    }
  };

  // Token süresini 60 saniyede bir kontrol et
  useEffect(() => {
    if (!token) return;

    const interval = setInterval(() => {
      validateToken(token);
    }, 1000 * 60);

    return () => clearInterval(interval);
  }, [token]);

  const login = async (email, password) => {
    setLoading(true);
    setError(null);

    try {
      const username = email;
      const response = await api.post("/auth/login", { username, password });

      const data = response.data.data;

      const loggedUser = {
        id: data.userId,
        username: data.username,
        role: data.role,
        expiration: data.expiration
      };

      const newToken = data.token;

      setToken(newToken);
      setUser(loggedUser);
      localStorage.setItem("token", newToken);
      localStorage.setItem("user", JSON.stringify(loggedUser));

      setLoading(false);
      return { success: true };
    } catch (err) {
      const message = err.response?.data?.message || "Login failed";
      setError(message);
      setLoading(false);
      return { success: false };
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        login,
        logout,
        loading,
        error,
        isAuthenticated: !!token,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
