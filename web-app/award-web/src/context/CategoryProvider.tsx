import React, { createContext, useContext, useState, useEffect } from "react";
import { useCategories } from "@/hooks/useCategories";
import { AwardCategoryResponseDto } from "@/types/AwardCategory";

interface CategoryContextType {
    selectedCategoryId: number | null;
    setSelectedCategoryId: (id: number) => void;
    selectedCategory: AwardCategoryResponseDto | null;
}

const CategoryContext = createContext<CategoryContextType>({
    selectedCategoryId: null,
    setSelectedCategoryId: () => {},
    selectedCategory: null,
});

export const CategoryProvider = ({ children }: { children: React.ReactNode }) => {
    const { categories, loading } = useCategories();
    const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);

    // Default to first category once loaded
    useEffect(() => {
        if (!loading && categories.length > 0 && !selectedCategoryId) {
            setSelectedCategoryId(categories[0].id);
        }
    }, [loading, categories, selectedCategoryId]);

    const selectedCategory = Array.isArray(categories)
        ? categories.find(cat => cat.id === selectedCategoryId) ?? null
        : null;

    return (
        <CategoryContext.Provider value={{ selectedCategoryId, setSelectedCategoryId, selectedCategory }}>
            {children}
        </CategoryContext.Provider>
    );
};

export const useSelectedCategory = () => useContext(CategoryContext);
