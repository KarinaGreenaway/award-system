import { useEffect, useState } from "react";
import Api from "@/api/Api";
import {
    AwardCategoryCreatePayload,
    AwardCategoryResponseDto,
    AwardCategoryUpdatePayload
} from "@/types/AwardCategory";

export function useCategories() {
    const [categories, setCategories] = useState<AwardCategoryResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchCategories = async () => {
        try {
            const activeProcess = await Api.getActiveAwardProcess();
            const data = await Api.getCategoriesByProcessId(activeProcess.id);
            setCategories(data);
        } catch (err: any) {
            console.error("Error loading categories", err);
            setError(err.message || "Failed to load categories.");
        } finally {
            setLoading(false);
        }
    };

    const createCategory = async (category: AwardCategoryCreatePayload) => {
        await Api.createAwardCategory(category);
        await fetchCategories();

        alert("Category saved successfully");
    };

    const updateCategory = async (id: number, data: AwardCategoryUpdatePayload) => {
        await Api.updateAwardCategory(id, data);
        await fetchCategories();

        alert("Category saved successfully");
    };

    const deleteCategory = async (id: number) => {
        await Api.deleteAwardCategory(id);
        await fetchCategories();
    };

    useEffect(() => {
        fetchCategories();
    }, []);

    return {
        categories,
        loading,
        error,
        createCategory,
        updateCategory,
        deleteCategory
    };
}
