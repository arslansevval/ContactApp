import api from "../api/axiosInstance";

export const useLoginApi = () => {
  const loginFunc = async (email, password) => {
    const username = email;
    const response = await api.post("/auth/login", { username, password });
    // Backend response: { token, user }
    return response.data;
  };

  return { loginFunc };
};
