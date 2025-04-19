import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";
import Sidebar from "@/components/sidebar.tsx";

export default function AppLayoutWithoutTabs() {
    const [collapsed, setCollapsed] = useState(false);

    useEffect(() => {
        const handleResize = () => {
            setCollapsed(window.innerWidth < 768);
        };
        window.addEventListener("resize", handleResize);
        handleResize();
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    return (
        <div className="flex min-h-screen bg-[color:var(--color-content-light)] dark:bg-[color:var(--color-content-dark)] transition-colors">
            {/* Sidebar */}
            <Sidebar collapsed={collapsed} setCollapsed={setCollapsed} />

            {/* Main Content */}
            <div
                className={`flex flex-col flex-1 ${collapsed ? "ml-20" : "ml-64"} transition-all duration-300`}
            >
                <main className="flex-1 flex flex-col min-h-screen p-8 pt-8 bg-[color:var(--color-content-light)] dark:bg-[color:var(--color-content-dark)] transition-all duration-300">
                    <Outlet />
                </main>
            </div>
        </div>
    );
}
