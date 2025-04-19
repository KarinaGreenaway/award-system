/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: "class",
    theme: {
        extend: {
            colors: {
                brand: {
                    DEFAULT: "#e84b67",
                    hover: "#f26c85",
                },
                sidebar: {
                    light: "#f9fafb",
                    dark: "#111827",
                },
                content: {
                    light: "#f3f4f6",
                    dark: "#1f2937",
                },
                tabs: {
                    light: "#e5e7eb",
                    dark: "#374151",
                },
                text: {
                    light: "#111827",
                    dark: "#d1d5db",
                },
            },
            fontFamily: {
                sans: ["Inter", "sans-serif"],
            },
        },
    },
    plugins: [],
};
