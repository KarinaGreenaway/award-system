import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { useState } from "react";

export default function LoginPage() {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [role, setRole] = useState("Admin");

    const handleMockLogin = () => {
        setLoading(true);

        // Simulate login delay
        setTimeout(() => {
            // Mock login storing fake token in session/localStorage
            localStorage.setItem("mock_token", "fake-jwt-token");
            localStorage.setItem("mock_role", role);
            navigate("/");
        }, 1000);
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-100 dark:bg-gray-900">
            <div className="bg-white dark:bg-gray-800 p-8 rounded-lg shadow-md w-full max-w-md">
                <h1 className="text-2xl font-bold mb-6 text-center text-gray-900 dark:text-white">
                    Log in to Award System
                </h1>

                <div className="mb-4">
                    <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">Select Role</label>
                    <select
                        value={role}
                        onChange={(e) => setRole(e.target.value)}
                        className="w-full p-2 rounded-md border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-800 dark:text-gray-100"
                    >
                        <option value="Admin">Admin</option>
                        <option value="Sponsor">Sponsor</option>
                    </select>
                </div>

                <Button
                    onClick={handleMockLogin}
                    className="w-full text-gray-800 dark:text-gray-100"
                    disabled={loading}
                >
                    {loading ? "Logging in..." : "Login with Mock User"}
                </Button>
            </div>
        </div>
    );
}