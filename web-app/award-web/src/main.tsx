import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import App from "./App";
import "./index.css";
import {FeatureFlagsProvider} from "@/context/FeatureFlagsProvider.tsx";

ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
        <BrowserRouter>
            <FeatureFlagsProvider>
                <App />
            </FeatureFlagsProvider>
        </BrowserRouter>
    </React.StrictMode>
);
