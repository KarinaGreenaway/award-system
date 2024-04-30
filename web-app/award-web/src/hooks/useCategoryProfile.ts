import { useEffect, useState } from "react";
import Api from "@/api/Api";
import { AwardCategoryResponseDto, AwardCategoryUpdatePayload } from "@/types/AwardCategory";

export function useCategoryProfile(categoryId: number) {
    const [category, setCategory] = useState<AwardCategoryResponseDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchProfile = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await Api.getAwardCategoryById(categoryId);
            setCategory(data);
        } catch (err: any) {
            console.error("Failed to fetch category profile", err);
            setError("Failed to fetch category profile.");
        } finally {
            setLoading(false);
        }
    };

    const updateProfile = async (updates: AwardCategoryUpdatePayload) => {
        if (!category) return;
        try {
            await Api.updateAwardCategory(category.id, updates);
            await fetchProfile(); // Refresh
        } catch (err) {
            console.error("Failed to update category", err);
            throw err;
        }
    };

    const saveProfileWithVideo = async (updates: Omit<AwardCategoryUpdatePayload, "introductionVideo"> & { videoFile?: File }) => {
        if (!category) return;

        let videoUrl = category.introductionVideo;
        if (updates.videoFile) {
            videoUrl = await Api.uploadVideo(updates.videoFile); // POST video
        }

        await updateProfile({
            ...updates,
            introductionVideo: videoUrl,
        });
    };

    useEffect(() => {
        if (categoryId) fetchProfile();
    }, [categoryId]);

    return {
        category,
        loading,
        error,
        updateProfile,
        saveProfileWithVideo,
        refetch: fetchProfile,
    };
}
