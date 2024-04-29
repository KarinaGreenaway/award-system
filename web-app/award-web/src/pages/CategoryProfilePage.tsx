import { useSelectedCategory } from "@/context/CategoryProvider";
import { useCategoryProfile } from "@/hooks/useCategoryProfile";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import {CategoryType} from "@/types/enums/CategoryType.ts";

export default function CategoryProfilePage() {
    const { selectedCategoryId } = useSelectedCategory();
    const { category, loading, error, updateProfile } = useCategoryProfile(selectedCategoryId ?? 0);

    const [video, setVideo] = useState<File | null>(null);
    const [paragraph, setParagraph] = useState<string>("");
    const [status, setStatus] = useState<"draft" | "published">("draft");
    const [isToggling, setIsToggling] = useState(false);

    useEffect(() => {
        if (category) {
            setParagraph(category.introductionParagraph || "");
            setStatus(category.profileStatus);
        }
    }, [category]);

    const handleSave = async () => {
        if (!category) return;

        // If trying to publish but requirements aren't met
        if (status === "published" && (!video || paragraph.trim() === "")) {
            alert("To publish, please upload a video and write your introduction paragraph.");
            return;
        }

        setIsToggling(true);
        try {
            const payload = {
                name: category.name,
                type: CategoryType[category.type],
                sponsorId: category.sponsorId,
                introductionVideo: video ? video.name : category.introductionVideo,
                introductionParagraph: paragraph,
                profileStatus: status,
            };
            await updateProfile(payload);
            alert(`Profile ${status === "published" ? "published" : "saved as draft"}`);
        } catch (error) {
            alert("Error saving profile");
            console.error(error);
        } finally {
            setIsToggling(false);
        }
    };

    const handleStatusChange = (newStatus: boolean) => {
        setStatus(newStatus ? "published" : "draft");
    };

    if (loading) return <p className="p-6 text-gray-400">Loading category profile...</p>;
    if (error) return <p className="p-6 text-red-500">{error}</p>;

    return (
        <div className="p-6 space-y-6 max-w-3xl">
            <h1 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                Hi {category?.name} Sponsor! Let's edit your profile
            </h1>

            {/* Upload Video */}
            <div>
                <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                    Upload Introduction Video
                </label>
                <input
                    type="file"
                    accept="video/*"
                    onChange={(e) => {
                        const file = e.target.files?.[0];
                        if (file && file.type.startsWith("video/")) {
                            setVideo(file);
                        } else {
                            setVideo(null);
                            alert("Please select a valid video file.");
                        }
                    }}
                    className="w-full text-sm text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]
            file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0
            file:bg-[color:var(--color-brand)] file:text-white
            dark:file:bg-[color:var(--color-brand)] dark:file:text-white
            rounded-md"
                />
                {category?.introductionVideo && !video && (
                    <p className="text-sm text-gray-500 mt-2">Current: {category.introductionVideo}</p>
                )}
                {video && (
                    <p className="text-sm text-gray-500 mt-2">Selected: {video.name}</p>
                )}
            </div>

            {/* Paragraph */}
            <div>
                <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                    Introductory Paragraph (max 300 words)
                </label>
                <textarea
                    maxLength={300 * 6}
                    rows={6}
                    value={paragraph}
                    onChange={(e) => setParagraph(e.target.value)}
                    placeholder="Write up your intro..."
                    className="w-full rounded-md p-3 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                />
            </div>

            {/* Status Toggle */}
            <div className="space-y-2">
                <p className="text-sm text-gray-700 dark:text-gray-300">
                    Current Status:{" "}
                    <span className="font-medium">
                        {category?.profileStatus}
                    </span>
                </p>

                <div className="flex items-center gap-3">
                    <span className={`text-sm font-medium ${
                        status === "draft"
                            ? "text-[color:var(--color-brand)]"
                            : "text-gray-500 dark:text-gray-400"
                    }`}>
                        Draft
                    </span>
                    <label className="relative inline-flex items-center cursor-pointer">
                        <input
                            type="checkbox"
                            className="sr-only peer"
                            checked={status === "published"}
                            onChange={(e) => handleStatusChange(e.target.checked)}
                            disabled={isToggling}
                        />
                        <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer dark:bg-gray-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600 peer-checked:bg-[color:var(--color-brand)]"></div>
                    </label>
                    <span className={`text-sm font-medium ${
                        status === "published"
                            ? "text-[color:var(--color-brand)]"
                            : "text-gray-500 dark:text-gray-400"
                    }`}>
                        Publish
                    </span>
                </div>
                <p className="text-xs text-gray-500 dark:text-gray-400">
                    Toggle to choose whether your profile should be published or in a draft state
                </p>
            </div>

            {/* Actions */}
            <div className="pt-4">
                <Button
                    onClick={handleSave}
                    className="bg-[color:var(--color-brand)] text-white hover:bg-[color:var(--color-brand-hover)] hover:scale-[1.01] active:scale-[0.98]"
                    disabled={isToggling}
                >
                    {isToggling ? "Saving..." : "Save"}
                </Button>
            </div>
        </div>
    );
}