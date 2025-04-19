import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";
import tailwind from "@tailwindcss/vite";
import path from "path";

export default defineConfig({
  plugins: [
    tailwind(),
    react(),
  ],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
});
