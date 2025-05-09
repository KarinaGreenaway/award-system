import { useEffect, useState } from "react";
import Api from "@/api/Api";
import {
    AwardCategoryCreatePayload,
    AwardCategoryResponseDto,
    AwardCategoryUpdatePayload
} from "@/types/AwardCategory";
import {AwardProcess} from "@/types/AwardProcess.ts";

export function useCategories(selectedProcessId: number | null) {
    const [categories, setCategories] = useState<AwardCategoryResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchCategories = async () => {
        try {
            let process: AwardProcess | null = null;
            let data: AwardCategoryResponseDto[] = [];
            if (selectedProcessId === null || selectedProcessId === undefined) {
                process = await Api.getActiveAwardProcess();
                data = await Api.getCategoriesByProcessId(process?.id as number);
            }
            else {
                data = await Api.getCategoriesByProcessId(selectedProcessId as number);
            }

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

    // Refetching categories whenever the selectedProcessId changes
    useEffect(() => {
        if (selectedProcessId !== null) {
            fetchCategories();
        }
    }, [selectedProcessId]);

    return {
        categories,
        loading,
        error,
        createCategory,
        updateCategory,
        deleteCategory
    };
}
