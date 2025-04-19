import { Home, List, Mail, Megaphone, Menu, Settings, Trophy } from "lucide-react";
import { Link, useLocation } from "react-router-dom";

interface SidebarProps {
    collapsed: boolean;
    setCollapsed: (value: boolean) => void;
}

const navItems = [
    { name: "Home", icon: Home, to: "/" },
    { name: "Nominations", icon: List, to: "/nominations" },
    { name: "Awards Event", icon: Trophy, to: "/events" },
    { name: "Announcements", icon: Megaphone, to: "/announcements" },
    { name: "Feedback", icon: Mail, to: "/feedback" },
    { name: "Settings", icon: Settings, to: "/settings" },
];

export default function Sidebar({ collapsed, setCollapsed }: SidebarProps) {
    const location = useLocation();

    return (
        <aside
            className={`fixed top-0 left-0 h-screen flex flex-col bg-[color:var(--color-sidebar-light)] dark:bg-[color:var(--color-sidebar-dark)] ${
                collapsed ? "w-20" : "w-64"
            } transition-all duration-300 rounded-r-2xl shadow-lg p-4`}
        >
            {/* Sidebar content */}
            <div className="flex flex-col flex-1 overflow-y-auto no-scrollbar">
                <div className="flex items-center justify-between mb-6">
                    {!collapsed && (
                        <h2 className="text-2xl font-bold tracking-tight text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                            Awards
                        </h2>
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
                                    <span className="text-sm font-medium">{item.name}</span>
                                )}
                            </Link>
                        );
                    })}
                </nav>
            </div>

            {/* Bottom profile */}
            <div className="flex items-center gap-3 p-3 mt-4 flex-shrink-0">
                <div className="bg-gray-300 dark:bg-gray-700 rounded-full h-8 w-8" />
                {!collapsed && (
                    <span className="text-sm font-medium text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
            John Doe
          </span>
                )}
            </div>
        </aside>
    );
}
