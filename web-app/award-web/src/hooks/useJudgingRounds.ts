import { useEffect, useState } from "react";
import Api from "@/api/Api";
import { JudgingRound } from "@/types/AwardProcess";

export function useJudgingRounds(processId: number | null) {
    const [rounds, setRounds] = useState<JudgingRound[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchRounds = async () => {
        if (!processId) return;
        setLoading(true);
        setError(null);
        try {
            const result = await Api.getJudgingRoundsByAwardProcess(processId);
            setRounds(result);
        } catch (err: any) {
            setError(err.message || "Failed to load judging rounds");
        } finally {
            setLoading(false);
        }
    };

    const createJudgingRound = async (data: any) => {
        await Api.createJudgingRound(data);
        await fetchRounds();
    };

    const updateJudgingRound = async (id: number, data: any) => {
        await Api.updateJudgingRound(id, data);
        await fetchRounds();
    };

    const deleteJudgingRound = async (id: number) => {
        await Api.deleteJudgingRound(id);
        await fetchRounds();
    };

    useEffect(() => {
        fetchRounds();
    }, [processId]);

    return {
        judgingRounds: rounds,
        loading,
        error,
        refetch: fetchRounds,
        createJudgingRound,
        updateJudgingRound,
        deleteJudgingRound,
    };
}
