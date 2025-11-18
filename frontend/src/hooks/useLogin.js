import api from "../api/axiosInstance";

export const useLoginApi = () => {
  const loginFunc = async (email, password) => {
    try {
      const username = email;
      const response = await api.post("/auth/login", { username, password });

      const data = response.data.data; // 🔹 backend response'un içindeki data objesi

      return {
        success: true,
        token: data.token,
        user: {
          id: data.userId,
          username: data.username,
          role: data.role,
          expiration: data.expiration
        }
      };
    } catch (err) {
      const message = err.response?.data?.message || "Login failed";
      return { success: false, message };
    }
  };

  return { loginFunc };
};
