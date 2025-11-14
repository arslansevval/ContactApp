import { useContext } from "react";
import { AuthContext } from "./AuthContext"; // ✅ artık AuthContext export edildi

export const useAuth = () => useContext(AuthContext);
