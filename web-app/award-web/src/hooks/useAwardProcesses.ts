import { useEffect, useState } from "react";
import Api from "@/api/Api";
import { AwardProcess, CreateAwardProcessPayload } from "@/types/AwardProcess";

export function useAwardProcesses() {
    const [awardProcesses, setAwardProcesses] = useState<AwardProcess[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchProcesses = async () => {
        try {
            const data = await Api.getAwardProcesses();
            setAwardProcesses(data);
        } catch (err: any) {
            setError(err.message || "Failed to load processes");
        } finally {
            setLoading(false);
        }
    };

    const createAwardProcess = async (data: CreateAwardProcessPayload) => {
        await Api.createAwardProcess(data);
        await fetchProcesses();
    };

    const updateAwardProcess = async (id: number, data: CreateAwardProcessPayload) => {
        await Api.updateAwardProcess(id, data);
        await fetchProcesses();
    };

    const deleteAwardProcess = async (id: number) => {
        await Api.deleteAwardProcess(id);
        await fetchProcesses();
    };

    useEffect(() => {
        fetchProcesses();
    }, []);

    return {
        awardProcesses,
        loading,
        error,
        refetch: fetchProcesses,
        createAwardProcess,
        updateAwardProcess,
        deleteAwardProcess,
    };
}
