import {
    Calendar,
    ClipboardEdit,
    // Home,
    Megaphone,
    Menu,
    MessageCircle,
    Trophy,
    Shield,
    User
} from "lucide-react";
import {Link, useNavigate, useLocation} from "react-router-dom";
import ThemeToggle from "@/components/ui/themeToggle";
import LogoDark from "@/assets/logo-white.png";
import LogoLight from "@/assets/logo-black.png";
import {useEffect, useRef, useState} from "react";
import {createPortal} from "react-dom";


interface SidebarProps {
    collapsed: boolean;
    setCollapsed: (value: boolean) => void;
}

export default function Sidebar({ collapsed, setCollapsed }: SidebarProps) {
    const location = useLocation();
    const [profileMenuOpen, setProfileMenuOpen] = useState(false);
    const [menuPosition, setMenuPosition] = useState<{ x: number; y: number } | null>(null);
    const profileMenuRef = useRef<HTMLDivElement | null>(null);
    const navigate = useNavigate();


    const userRole = localStorage.getItem("mock_role");
    const isAdmin = userRole === "Admin";

    const navItems = [
        // { name: "Home", icon: Home, to: "/" },
        { name: "Nominations", icon: Trophy, to: "/nominations" },
        { name: "Announcements", icon: Megaphone, to: "/announcements" },
        { name: "Category Profile", icon: ClipboardEdit, to: "/category-profile" },
        { name: "Awards Event", icon: Calendar, to: "/events" },
        { name: "Feedback", icon: MessageCircle, to: "/feedback" },
        ...(isAdmin ? [{ name: "Awards Management", icon: Shield, to: "admin/awards-management" }] : []),
    ];

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (
                profileMenuRef.current &&
                !profileMenuRef.current.contains(event.target as Node)
            ) {
                setProfileMenuOpen(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const handleProfileClick = (e: React.MouseEvent) => {
        e.preventDefault();
        setProfileMenuOpen(true);
        setMenuPosition({ x: e.pageX, y: e.pageY });
    };



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
                <div className="flex flex-col items-center gap-2 p-3" onClick={handleProfileClick}>
                    <div className="card-icon-wrap">
                        <User className="h-6 w-6 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]" />
                    </div>
                    <ThemeToggle collapsed />
                </div>
            ) : (
                <div>
                    <div className="flex items-center justify-between gap-3 p-1 mt-4 flex-shrink-0" onClick={handleProfileClick}>
                        <div className="flex items-center gap-3">
                            <div className="card-icon-wrap">
                                <User className="h-6 w-6 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]" />
                            </div>
                            <span className="text-sm text-text-light dark:text-text-dark">John</span>
                            <div className="absolute right-7">
                                <ThemeToggle />
                            </div>
                        </div>
                    </div>
                    {/*<div className="flex p-1 text-xs">*/}
                    {/*    <button*/}
                    {/*        className="text-sx text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300"*/}
                    {/*        onClick={() => {*/}
                    {/*            localStorage.removeItem("mock_role");*/}
                    {/*            navigate("/login");*/}
                    {/*        }}*/}
                    {/*    >*/}
                    {/*        Log out*/}
                    {/*    </button>*/}
                    {/*</div>*/}

                </div>
            )}

            {/* Context Menu — Always Rendered */}
            {profileMenuOpen && menuPosition &&
                createPortal(
                    <div
                        ref={profileMenuRef}
                        className="fixed z-50 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 shadow-md rounded-md overflow-hidden"
                        style={{ top: menuPosition.y, left: menuPosition.x }}
                    >
                        <button
                            className="w-full px-4 py-2 text-left text-sm hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-700 dark:text-gray-200"
                            onClick={() => {
                                localStorage.removeItem("mock_role");
                                setProfileMenuOpen(false);
                                navigate("/login");
                            }}
                        >
                            Log out
                        </button>
                    </div>,
                    document.body
                )
            }
        </aside>
    );
}
