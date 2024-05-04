import { useEffect, useState } from "react";
import Api from "@/api/Api";
import { AwardEvent } from "@/types/AwardEvent";

export function useAwardEvent() {
    const [event, setEvent] = useState<AwardEvent | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [activeProcessId, setActiveProcess] = useState<number | null>(null);

    const fetchEvent = async () => {
        setLoading(true);
        setError(null);
        try {
            const activeProcess = await Api.getActiveAwardProcess();
            const activeProcessId = activeProcess.id;
            const event = await Api.getAwardEventByProcessId(activeProcess.id);
            setEvent(event);
            setActiveProcess(activeProcessId);
        } catch (err: any) {
            console.error("Failed to fetch award event", err);
            setError("Failed to fetch award event.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchEvent();
    }, []);

    return {
        event,
        activeProcessId,
        loading,
        error,
        refetch: fetchEvent
    };
}
