import { useState, useEffect } from "react";
import { Outlet } from "react-router-dom";
import Sidebar from "@/components/sidebar.tsx";
import CategoryTabs from "@/components/categoryTabs.tsx";
import {CategoryProvider} from "@/context/CategoryProvider.tsx";
import VitalityLogo from "@/assets/vitality-pink-logo.png";

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
        <CategoryProvider>
            <div className="flex min-h-screen bg-[color:var(--color-content-light)] dark:bg-[color:var(--color-content-dark)] transition-colors">
                {/* Sidebar */}
                <Sidebar collapsed={collapsed} setCollapsed={setCollapsed} />

                {/* Main Content */}
                <div
                    className={`flex flex-col flex-1 ${collapsed ? "ml-20" : "ml-64"} transition-all duration-300`}
                >
                    {/* Category Tabs Fixed Header */}
                    <div
                        className="fixed top-0 right-0 z-40 flex items-center h-16 px-6 transition-all duration-300  bg-[color:var(--color-content-light)] dark:bg-[color:var(--color-content-dark)] rounded-lg"
                        style={{ left: collapsed ? "5rem" : "16rem" }}
                    >
                        <div className="flex-1 z-50 overflow-x-auto">
                            <CategoryTabs />
                        </div>

                        {/* Image in Top Right Corner */}
                        <div className="z-40">
                            <img src={VitalityLogo} alt="logo" className="w-12 h-12" />
                        </div>
                    </div>

                    {/* Page Content */}
                    <main
                        className="relative flex-1 flex flex-col min-h-screen p-8 pt-24 bg-[color:var(--color-content-light)] dark:bg-[color:var(--color-content-dark)] transition-all duration-300"
                    >
                        <Outlet />
                    </main>
                </div>
            </div>
        </CategoryProvider>
    );
}
