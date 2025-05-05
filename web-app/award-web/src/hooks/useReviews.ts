import {useEffect, useState} from "react";
import Api from "@/api/Api.ts";
import {FeedbackResponseDto} from "@/types/Feedback.ts";


export function useReviews() {
    const [reviews, setReviews] = useState<FeedbackResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [eventId, setEventId] = useState<number | null>(null)

    const fetchReviews = async () => {
        setLoading(true);
        setError(null);
        try {
            const activeProcess = await Api.getActiveAwardProcess();
            if (!activeProcess) throw new Error("No active award process found");

            const event = await Api.getAwardEventByProcessId(activeProcess.id);
            if (!event) throw new Error("No award event found for process");

            const fetchedReviews = await Api.getFeedbackReviews(event.id);
            setReviews(fetchedReviews);
            setEventId(event.id);

        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : "An unexpected error occurred.";
            console.error("Failed to fetch reviews:", err);
            setError(errorMessage);
            setReviews([]);

        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchReviews();
    }, []);

    return {
        reviews,
        eventId,
        loading,
        error,
        refetch: fetchReviews
    };
}