import { useContext } from "react";
import { FeatureFlagsContext } from "@/context/FeatureFlagsProvider";

export function useFeatureFlags() {
    const context = useContext(FeatureFlagsContext);
    if (!context) {
        throw new Error("useFeatureFlags must be used within a FeatureFlagsProvider");
    }
    return context;
}