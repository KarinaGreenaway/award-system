import { createContext, ReactNode, useState } from "react";

interface FeatureFlags {
    enableAI: boolean;
}

interface FeatureFlagsContextType {
    features: FeatureFlags;
    setFeatures: (flags: FeatureFlags) => void;
}

export const FeatureFlagsContext = createContext<FeatureFlagsContextType | undefined>(undefined);

export function FeatureFlagsProvider({ children }: { children: ReactNode }) {
    const [features, setFeatures] = useState<FeatureFlags>({
        enableAI: true
    });

    return (
        <FeatureFlagsContext.Provider value={{ features, setFeatures }}>
            {children}
        </FeatureFlagsContext.Provider>
    );
}