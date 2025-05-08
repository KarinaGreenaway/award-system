import {useEffect, useState} from "react";
import Api from "@/api/Api.ts";
import {AwardEvent} from "@/types/AwardEvent.ts";


export function useFeedbackAISummary() {
    const [summary, setSummary] = useState<string>("Sorry! I don't currently have any feedback for you!");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchFeedbackSummary = async () => {
        setLoading(true);
        setError(null);
        try {
            const activeProcess = await Api.getActiveAwardProcess();
            if (!activeProcess) throw new Error("No active award process found");

            const event: AwardEvent = await Api.getAwardEventByProcessId(activeProcess.id);
            if (!event) throw new Error("No award event found for process");

            const summary = await Api.getFeedbackSummary(event.id);

            // setSummary(event.feedbackSummary ?? "Sorry! I don't currently have any feedback for you!");
            setSummary(summary ?? "Sorry! I don't currently have any feedback for you!");

        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : "An unexpected error occurred.";
            console.error("Failed to fetch reviews:", err);
            setError(errorMessage);
            setSummary("Sorry! I don't currently have any feedback for you!");

        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchFeedbackSummary();
    }, []);

    return {
        summary,
        loading,
        error,
        refetch: fetchFeedbackSummary
    };
}