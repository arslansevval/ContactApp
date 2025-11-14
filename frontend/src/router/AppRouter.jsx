import { Routes, Route, Navigate, Outlet } from "react-router-dom";
import Home from "../pages/Home";
import Employees from "../pages/Employees";
import Companies from "../pages/Companies";
import Login from "../pages/Login";
import NotFound from "../pages/NotFound";
import Navbar from "../components/Navbar";
import { useAuth } from "../auth/useAuth";

// ProtectedRoute bileşeni
const ProtectedRoute = ({ redirectPath = "/login" }) => {
  const { isAuthenticated } = useAuth();
  if (!isAuthenticated) return <Navigate to={redirectPath} replace />;
  return <Outlet />;
};

const AppRouter = () => {
  const { isAuthenticated } = useAuth();

  return (
    <>
      {isAuthenticated && <Navbar />}
      <Routes>
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<Login />} />
        <Route element={<ProtectedRoute />}>
          <Route path="/home" element={<Home />} />
          <Route path="/employees" element={<Employees />} />
          <Route path="/companies" element={<Companies />} />
        </Route>
        <Route path="*" element={<NotFound />} />
      </Routes>
    </>
  );
};

export default AppRouter;
