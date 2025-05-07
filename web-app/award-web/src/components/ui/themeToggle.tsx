"use client";

import { Moon, Sun } from "lucide-react";
import { useTheme } from "@/hooks/useTheme.ts";
import { motion } from "framer-motion";

interface ThemeToggleProps {
    collapsed?: boolean;
}

export default function ThemeToggle({ collapsed = false }: ThemeToggleProps) {
    const { theme, toggleTheme } = useTheme();
    const isDark = theme === "dark";

    // collapsed = just icon
    if (collapsed) {
        return (
            <button
                onClick={toggleTheme}
                className="w-10 h-10 flex items-center justify-center rounded-full hover:bg-gray-200 dark:hover:bg-gray-700 transition"
                aria-label="Toggle theme"
            >
                {isDark ? (
                    <Moon className="h-5 w-5 text-gray-500" />
                ) : (
                    <Sun className="h-5 w-5 text-gray-400" />
                )}
            </button>
        );
    }

    // full switch layout
    return (
        <button
            onClick={toggleTheme}
            aria-label="Toggle theme"
            className="relative w-14 h-8 rounded-full bg-gray-200 dark:bg-gray-600 transition-colors duration-300 flex items-center px-1"
        >
            <Sun className="h-4 w-4 text-gray-700 absolute left-2 z-10" />
            <Moon className="h-4 w-4 text-gray-300 absolute right-2 z-10" />

            <motion.div
                className="w-6 h-6 rounded-full bg-white shadow-md z-20"
                layout
                initial={false}
                animate={{ x: isDark ? 0 : 24 }}
                transition={{ type: "spring", stiffness: 500, damping: 30 }}
            />
        </button>
    );
}
