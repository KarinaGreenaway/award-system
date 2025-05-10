import { Home, List, Mail, Megaphone, Menu, Settings, Trophy } from "lucide-react";
import { Link, Outlet, useLocation } from "react-router-dom";
import { useEffect, useState } from "react";
import CategoryTabs from "@/components/categoryTabs.tsx"; // Important import!

export default function DashboardLayout() {
    const location = useLocation();
    const [collapsed, setCollapsed] = useState(false);

    const navItems = [
        { name: "Home", icon: Home, to: "/" },
        { name: "Nominations", icon: List, to: "/nominations" },
        { name: "Awards Event", icon: Trophy, to: "/events" },
        { name: "Announcements", icon: Megaphone, to: "/announcements" },
        { name: "Feedback", icon: Mail, to: "/feedback" },
        { name: "Settings", icon: Settings, to: "/settings" },
    ];

    useEffect(() => {
        const handleResize = () => {
            setCollapsed(window.innerWidth < 768);
        };
        window.addEventListener("resize", handleResize);
        handleResize();
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    return (
        <div className="flex min-h-screen bg-gray-100 dark:bg-gray-900 text-gray-900 dark:text-gray-100 transition-colors">
            {/* Sidebar */}
            <aside
                className={`fixed top-0 left-0 h-screen flex flex-col bg-white dark:bg-gray-950 ${
                    collapsed ? "w-20" : "w-64"
                } transition-all duration-300 rounded-r-2xl shadow-lg p-4`}
            >
                {/* Top Section: Logo + Nav */}
                <div className="flex flex-col flex-1 overflow-y-auto">
                    <div className="flex items-center justify-between mb-6">
                        {!collapsed && (
                            <h2 className="text-2xl font-bold tracking-tight">Awards</h2>
                        )}
                        <button
                            onClick={() => setCollapsed(!collapsed)}
                            className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition"
                        >
                            <Menu className="h-6 w-6" />
                        </button>
                    </div>

                    {/* Nav links */}
                    <nav className="flex flex-col gap-2">
                        {navItems.map((item) => {
                            const isActive = location.pathname === item.to;
                            return (
                                <Link
                                    key={item.name}
                                    to={item.to}
                                    className={`flex items-center gap-4 p-3 rounded-lg transition-colors ${
                                        isActive
                                            ? "bg-[#d33b55] text-white"
                                            : "hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-700 dark:text-gray-400"
                                    }`}
                                >
                                    <item.icon className="h-6 w-6" />
                                    {!collapsed && (
                                        <span className="text-sm font-medium">{item.name}</span>
                                    )}
                                </Link>
                            );
                        })}
                    </nav>
                </div>

                {/* Bottom Profile */}
                <div className="flex items-center gap-3 p-3 mt-4 flex-shrink-0">
                    <div className="bg-gray-300 dark:bg-gray-700 rounded-full h-8 w-8" />
                    {!collapsed && (
                        <span className="text-sm font-medium text-gray-700 dark:text-gray-400">
              John Doe
            </span>
                    )}
                </div>
            </aside>

            {/* Main Area */}
            <div className={`flex flex-col flex-1 ${collapsed ? "ml-20" : "ml-64"} transition-all duration-300`}>

                {/* Fixed Category Tabs */}
                <div className="fixed top-0 right-0 z-40 bg-gray-900 dark:bg-gray-900 flex items-center px-6 transition-all duration-300"
                    style={{ left: collapsed ? "5rem" : "16rem", height: "4rem" }}
                >
                    <div className="w-full max-w-7xl mx-auto">
                        <CategoryTabs />
                    </div>
                </div>

                {/* Main Content (under tabs) */}
                <main
                    className="flex-1 flex flex-col min-h-screen p-8 pt-24 bg-gray-900 dark:bg-gray-900 transition-all duration-300"
                >
                    <Outlet />
                </main>
            </div>
        </div>
    );
}
