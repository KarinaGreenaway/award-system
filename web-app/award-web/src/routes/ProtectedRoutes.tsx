import { Navigate, Outlet } from "react-router-dom";

export default function ProtectedRoute() {
    const isAuthenticated = !!localStorage.getItem("mock_token");

    return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
}
