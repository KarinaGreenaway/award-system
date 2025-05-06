import {Calendar, ClipboardEdit, Home, Megaphone, Menu, MessageCircle, Trophy, Shield } from "lucide-react";
import { Link, useLocation } from "react-router-dom";
import ThemeToggle from "@/components/ui/themeToggle";
import LogoDark from "@/assets/logo-white.png";
import LogoLight from "@/assets/logo-black.png";


interface SidebarProps {
    collapsed: boolean;
    setCollapsed: (value: boolean) => void;
}

export default function Sidebar({ collapsed, setCollapsed }: SidebarProps) {
    const location = useLocation();
    const userRole = localStorage.getItem("mock_role");
    const isAdmin = userRole === "Admin";

    const navItems = [
        { name: "Home", icon: Home, to: "/" },
        { name: "Nominations", icon: Trophy, to: "/nominations" },
        { name: "Awards Event", icon: Calendar, to: "/events" },
        { name: "Announcements", icon: Megaphone, to: "/announcements" },
        { name: "Category Profile", icon: ClipboardEdit, to: "/category-profile" },
        { name: "Feedback", icon: MessageCircle, to: "/feedback" },
        ...(isAdmin ? [{ name: "Awards Management", icon: Shield, to: "/awards-management" }] : []),
        // { name: "Settings", icon: Settings, to: "/settings" }
    ];

    return (
        <aside
            className={`fixed top-0 left-0 h-screen flex flex-col bg-[color:var(--color-sidebar-light)] dark:bg-[color:var(--color-sidebar-dark)] ${
                collapsed ? "w-20" : "w-64"
            } transition-all duration-300  shadow-lg p-4`}
        >
            {/* Sidebar content */}
            <div className="flex flex-col flex-1 overflow-y-auto no-scrollbar">
                <div className="flex items-center justify-between mb-6">
                    {!collapsed && (
                        <div className="flex items-center gap-2">
                            {/* Dark mode logo */}
                            <img
                                src={LogoDark}
                                alt="Logo Dark"
                                className="h-8 w-auto object-contain hidden dark:block"
                            />
                            {/* Light mode logo */}
                            <img
                                src={LogoLight}
                                alt="Logo Light"
                                className="h-8 w-auto object-contain block dark:hidden"
                            />

                            <h2 className="text-2xl tracking-tight text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                                Awards
                            </h2>
                        </div>
                    )}
                    <button
                        onClick={() => setCollapsed(!collapsed)}
                        className="p-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-700 transition"
                    >
                        <Menu className="h-6 w-6 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]" />
                    </button>
                </div>

                <nav className="flex flex-col gap-2">
                    {navItems.map((item) => {
                        const isActive = location.pathname === item.to;
                        return (
                            <Link
                                key={item.name}
                                to={item.to}
                                className={`flex items-center gap-4 p-3 rounded-lg transition-all duration-200 ease-out transform hover:scale-[1.01] active:scale-[0.98] ${
                                    isActive
                                        ? "bg-[color:var(--color-brand)] text-white"
                                        : "hover:bg-gray-200 dark:hover:bg-gray-700 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                }`}
                            >
                                <item.icon className="h-6 w-6" />
                                {!collapsed && (
                                    <span className="text-sm">{item.name}</span>
                                )}
                            </Link>
                        );
                    })}
                </nav>
            </div>

            {/* Bottom profile */}
            {collapsed ? (
                // COLLAPSED MODE: icon below avatar
                <div className="flex flex-col items-center gap-2 p-3">
                    <div className="bg-gray-300 dark:bg-gray-700 rounded-full h-8 w-8" />
                    <ThemeToggle collapsed />
                </div>
            ) : (
                // EXPANDED MODE: inline row
                <div className="flex items-center justify-between gap-3 p-3 mt-4 flex-shrink-0">
                    <div className="flex items-center gap-3">
                        <div className="bg-gray-300 dark:bg-gray-700 rounded-full h-8 w-8" />
                        <span className="text-sm text-text-light dark:text-text-dark">
                            John
                        </span>
                    </div>
                    <ThemeToggle />
                </div>
            )}
        </aside>
    );
}
