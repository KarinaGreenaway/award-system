import React from "react";
import { Navigate } from "react-router-dom";

interface AdminGuardProps {
    children: React.ReactNode;
}

const AdminGuard: React.FC<AdminGuardProps> = ({ children }) => {
    const role = localStorage.getItem("mock_role");

    if (role !== "Admin") {
        return <Navigate to="/unauthorised" replace />;
    }

    return <>{children}</>;
};

export default AdminGuard;
