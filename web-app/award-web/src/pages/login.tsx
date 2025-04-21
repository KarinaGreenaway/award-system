import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { useState } from "react";

export default function LoginPage() {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const handleMockLogin = () => {
        setLoading(true);

        // Simulate login delay
        setTimeout(() => {
            // Mock login storing fake token in session/localStorage
            localStorage.setItem("mock_token", "fake-jwt-token");
            navigate("/");
        }, 1000);
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-100 dark:bg-gray-900">
            <div className="bg-white dark:bg-gray-800 p-8 rounded-lg shadow-md w-full max-w-md">
                <h1 className="text-2xl font-bold mb-6 text-center text-gray-900 dark:text-white">
                    Log in to Award System
                </h1>
                <Button
                    onClick={handleMockLogin}
                    className="w-full"
                    disabled={loading}
                >
                    {loading ? "Logging in..." : "Login with Mock User"}
                </Button>
            </div>
        </div>
    );
}
