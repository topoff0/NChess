import { Navigate, Outlet } from "react-router-dom";
import { routes } from "./routes";
import { useAuth } from "../../features/auth/AuthProvider";

export const ProtectedRoute = () => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to={routes.auth} replace />;
  }

  return <Outlet />;
};
