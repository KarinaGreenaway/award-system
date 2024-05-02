import { useEffect, useState } from "react";
import Api from "@/api/Api";
import { Announcement, CreateAnnouncementPayload } from "@/types/Announcements";

export function useAnnouncements(sponsorId: number | null) {
    const [announcements, setAnnouncements] = useState<Announcement[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchAnnouncements = async () => {
        if (!sponsorId || sponsorId <= 0) return; // prevent bad fetch
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

    useEffect(() => {
        fetchAnnouncements();
    }, [sponsorId]);

    return {
        announcements,
        loading,
        error,
        createAnnouncement: async (payload: CreateAnnouncementPayload) => {
            const newAnnouncement = await Api.createAnnouncement(payload);
            setAnnouncements(prev => [newAnnouncement, ...prev]);
            return newAnnouncement;
        },
        updateAnnouncement: async (id: number, payload: Partial<CreateAnnouncementPayload>) => {
            const updated = await Api.updateAnnouncement(id, payload);
            setAnnouncements(prev => prev.map(a => a.id === id ? updated : a));
            return updated;
        },
        deleteAnnouncement: async (id: number) => {
            await Api.deleteAnnouncement(id);
            setAnnouncements(prev => prev.filter(a => a.id !== id));
        },
        handleImageUpload: async (file: File) => {
            try {
                return await Api.uploadImage(file);
            } catch {
                return "";
            }
        },
        refetch: fetchAnnouncements,
    };
}
