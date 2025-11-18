import { createContext, useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem("user");
    if (!stored) return null;

    try {
      return JSON.parse(stored);
    } catch (err) {
      console.error("Invalid JSON stored in localStorage for 'user'. Clearing corrupted value.", err);
      localStorage.removeItem("user"); // bozuk veriyi temizle
      return null;
    }
  });


  const [token, setToken] = useState(() => localStorage.getItem("token"));
  const [loading, setLoading] = useState(false);

  const validateToken = (jwtToken) => {
    try {
      const decoded = jwtDecode(jwtToken);
      const now = Date.now() / 1000;

      if (decoded.exp && decoded.exp < now) {
        logout();
        return false;
      }
      return true;
    } catch {
      logout();
      return false;
    }
  };

  useEffect(() => {
    if (token) {
      const isValid = validateToken(token);
      if (!isValid) {
        console.warn("Token expired — auto logout.");
      }
    }
  }, []);

  const login = async (email, password, loginFunc) => {
    setLoading(true);
    try {
      const { token: newToken, user: loggedUser } = await loginFunc(email, password);

      if (!validateToken(newToken)) {
        return { success: false, message: "Token süresi geçmiş veya geçersiz." };
      }

      setToken(newToken);
      setUser(loggedUser);

      localStorage.setItem("token", newToken);
      localStorage.setItem("user", JSON.stringify(loggedUser));

      setLoading(false);
      return { success: true };
    } catch (err) {
      setLoading(false);
      return { success: false, message: err.message };
    }
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        login,
        logout,
        loading,
        isAuthenticated: !!token,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
