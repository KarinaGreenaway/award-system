import { useEffect, useState } from "react";
import Api from "@/api/Api.ts";
import { AwardCategoryResponseDto } from "@/types/AwardCategory";
import { Nomination, NomineeSummary } from "@/types/Nominations";

export function useNominations(categoryId: number | null) {
    const [category, setCategory] = useState<AwardCategoryResponseDto | null>(null);
    const [data, setData] = useState<NomineeSummary[] | Nomination[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchData = async () => {
        if (!categoryId || categoryId <= 0) return; // preventing invalid call
        setLoading(true);
        setError(null);
        try {
            const category = await Api.getAwardCategoryById(categoryId);
            setCategory(category);
            const summaries = await Api.getNomineeSummaries(categoryId);
            setData(summaries);
        } catch (err: any) {
            setError(err.message || "Something went wrong while fetching nominations.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, [categoryId]);

    return { category, data, loading, error, refetch: fetchData };
}
