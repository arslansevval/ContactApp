import { Navigate } from "react-router-dom";
import { useAuth } from "./useAuth";
// ProtectedRoute component to restrict access based on authentication
const ProtectedRoute = ({ children }) => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

export default ProtectedRoute;
