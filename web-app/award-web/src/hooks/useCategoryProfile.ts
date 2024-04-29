import { useEffect, useState } from "react";
import Api from "@/api/Api";
import { AwardCategoryResponseDto, AwardCategoryUpdatePayload } from "@/types/AwardCategory";

export function useCategoryProfile(categoryId: number) {
    const [category, setCategory] = useState<AwardCategoryResponseDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchProfile = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await Api.getAwardCategoryById(categoryId);
            setCategory(data);
        } catch (err: any) {
            console.error("Failed to fetch category profile", err);
            setError("Failed to fetch category profile.");
        } finally {
            setLoading(false);
        }
    };

    const updateProfile = async (updates: AwardCategoryUpdatePayload) => {
        if (!category) return;
        try {
            await Api.updateAwardCategory(category.id, updates);
            await fetchProfile(); // Refetch to get the updated state
        } catch (err) {
            console.error("Failed to update category", err);
            throw err;
        }
    };

    useEffect(() => {
        if (categoryId) fetchProfile();
    }, [categoryId]);

    return { category, loading, error, updateProfile, refetch: fetchProfile };
}
