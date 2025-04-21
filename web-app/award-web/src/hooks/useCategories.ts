import { useEffect, useState } from "react";
import Api from "@/api/Api";
import { AwardCategoryResponseDto } from "@/types/AwardCategory";

export function useCategories() {
    const [categories, setCategories] = useState<AwardCategoryResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const data = await Api.getCategories();
                setCategories(data);
            } catch (err: any) {
                console.error("Error loading categories", err);
                setError(err.message || "Failed to load categories.");
            } finally {
                setLoading(false);
            }
        };

        fetchCategories();
    }, []);

    return { categories, loading, error };
}
