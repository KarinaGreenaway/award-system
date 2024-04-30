import { useEffect, useState } from "react";
import Api from "@/api/Api";
import {Announcement, CreateAnnouncementPayload} from "@/types/Announcements";

export function useAnnouncements(sponsorId: number | null) {
    const [announcements, setAnnouncements] = useState<Announcement[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchAnnouncements = async () => {
        if (!sponsorId) return;

        setLoading(true);
        setError(null);
        try {
            const data = await Api.getAnnouncementsBySponsor(sponsorId);
            setAnnouncements(data);
        } catch (err: any) {
            console.error("Failed to fetch announcements", err);
            setError(err.message || "Failed to fetch announcements");
        } finally {
            setLoading(false);
        }
    };

    const createAnnouncement = async (payload: CreateAnnouncementPayload) => {
        try {
            const newAnnouncement = await Api.createAnnouncement(payload);
            setAnnouncements(prev => [newAnnouncement, ...prev]);
            return newAnnouncement;
        } catch (err: any) {
            console.error("Failed to create announcement", err);
            throw err;
        }
    };

    const updateAnnouncement = async (id: number, payload: Partial<CreateAnnouncementPayload>) => {
        try {
            const updatedAnnouncement = await Api.updateAnnouncement(id, payload);
            setAnnouncements(prev =>
                prev.map(ann => ann.id === id ? updatedAnnouncement : ann)
            );
            return updatedAnnouncement;
        } catch (err: any) {
            console.error("Failed to update announcement", err);
            throw err;
        }
    };

    const deleteAnnouncement = async (id: number) => {
        try {
            await Api.deleteAnnouncement(id);
            setAnnouncements(prev => prev.filter(ann => ann.id !== id));
        } catch (err: any) {
            console.error("Failed to delete announcement", err);
            throw err;
        }
    };

    useEffect(() => {
        fetchAnnouncements();
    }, [sponsorId]);

    return {
        announcements,
        loading,
        error,
        createAnnouncement,
        updateAnnouncement,
        deleteAnnouncement,
        refetch: fetchAnnouncements
    };
}