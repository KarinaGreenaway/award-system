import { createContext, useContext, useState, ReactNode } from "react";

interface CategoryContextType {
    selectedCategoryId: number | null;
    setSelectedCategoryId: (id: number) => void;
}

const CategoryContext = createContext<CategoryContextType | undefined>(undefined);

export const CategoryProvider = ({ children }: { children: ReactNode }) => {
    const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);

    return (
        <CategoryContext.Provider value={{ selectedCategoryId, setSelectedCategoryId }}>
            {children}
        </CategoryContext.Provider>
    );
};

export function useSelectedCategory() {
    const context = useContext(CategoryContext);
    if (!context) {
        throw new Error("useSelectedCategory must be used within a CategoryProvider");
    }
    return context;
}
