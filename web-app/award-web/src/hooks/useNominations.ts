import { useEffect, useState } from "react";
import Api from "@/api/Api.ts";
import { AwardCategoryResponseDto } from "@/types/AwardCategory";
import { Nomination, NomineeSummary } from "@/types/Nominations";

export function useNominations(categoryId: number) {
    const [category, setCategory] = useState<AwardCategoryResponseDto | null>(null);
    const [data, setData] = useState<NomineeSummary[] | Nomination[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        async function fetchData() {
            if (!categoryId) return;
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
        }

        fetchData();
    }, [categoryId]);

    return { category, data, loading, error };
}
