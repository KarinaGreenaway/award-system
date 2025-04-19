import { Routes, Route } from "react-router-dom";
import HomePage from "@/pages/HomePage.tsx";
import AppLayoutWithTabs from "@/layouts/AppLayoutWithTabs.tsx"; // ← import layout

export default function App() {
    return (
        <Routes>
            {/* Sponsor and Admin Routes */}
            <Route path="/login" element={
                <div className="flex items-center justify-center min-h-screen">
                    <h1 className="text-5xl font-bold text-blue-600">Login Page Works!</h1>
                    <h1 className="text-4xl font-bold text-blue-500 dark:text-red-500">Test Heading</h1>
                </div>
            } />

            {/* Dashboard Route */}
            <Route path="/" element={<AppLayoutWithTabs />}>
                <Route index element={<HomePage />} />
            </Route>
        </Routes>
    );
}
