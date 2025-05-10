import {useEffect, useState} from "react";
import {FeedbackAnalyticsResponseDto} from "@/types/FeedbackAnalytics.ts";
import Api from "@/api/Api.ts";
import {AwardEvent} from "@/types/AwardEvent.ts";

export function useFeedbackAnalytics() {
    const [analytics, setAnalytics] = useState<FeedbackAnalyticsResponseDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchAnalytics = async () => {
        setLoading(true);
        setError(null);
        try {
            const activeProcess = await Api.getActiveAwardProcess();
            if (!activeProcess) throw new Error("No active award process found");

            const event: AwardEvent = await Api.getAwardEventByProcessId(activeProcess.id);
            if (!event) throw new Error("No award event found for process");

            const data = await Api.getFeedbackAnalytics(event.id);
            setAnalytics(data);

        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : "An unexpected error occurred.";
            console.error("Failed to fetch feedback analytics", err);
            setError(errorMessage);
            setAnalytics(null);

        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchAnalytics();
    }, []);

    return {
        analytics,
        loading,
        error,
        refetch: fetchAnalytics
    };
}