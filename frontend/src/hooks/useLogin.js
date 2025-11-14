import api from "../api/axiosInstance";

export const useLoginApi = () => {
  const loginFunc = async (email, password) => {
    const response = await api.post("/auth/login", { email, password });
    // Backend response: { token, user }
    return response.data;
  };

  return { loginFunc };
};
