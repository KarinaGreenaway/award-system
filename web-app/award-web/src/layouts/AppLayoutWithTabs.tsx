import { useState, useEffect } from "react";
import { Outlet } from "react-router-dom";
import Sidebar from "@/components/sidebar.tsx";
import CategoryTabs from "@/components/categoryTabs.tsx";

export default function AppLayoutWithTabs() {
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
                {/* Category Tabs Fixed Header */}
                <div
                    className="fixed top-0 right-0 z-40 bg-[color:var(--color-content-light)] dark:bg-[color:var(--color-content-dark)] flex items-center h-16 px-6 transition-all duration-300"
                    style={{ left: collapsed ? "5rem" : "16rem" }}
                >

                <div className="w-full max-w-7xl mx-auto">
                        <CategoryTabs />
                    </div>
                </div>

                {/* Page Content */}
                <main
                    className="flex-1 flex flex-col min-h-screen p-8 pt-24 bg-[color:var(--color-content-light)] dark:bg-[color:var(--color-content-dark)] transition-all duration-300"
                >
                    <Outlet />
                </main>
            </div>
        </div>
    );
}
