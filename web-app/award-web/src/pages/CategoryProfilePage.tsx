import { useSelectedCategory } from "@/context/CategoryProvider";
import { useCategoryProfile } from "@/hooks/useCategoryProfile";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { CategoryType } from "@/types/enums/CategoryType";

export default function CategoryProfilePage() {
    const { selectedCategoryId } = useSelectedCategory();
    const { category, loading, error, saveProfileWithVideo } = useCategoryProfile(selectedCategoryId ?? null);

    const [video, setVideo] = useState<File | null>(null);
    const [paragraph, setParagraph] = useState<string>("");
    const [status, setStatus] = useState<"draft" | "published">("draft");
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        if (category) {
            setParagraph(category.introductionParagraph || "");
            setStatus(category.profileStatus);
        }
    }, [category]);

    const handleSave = async () => {
        if (!category) return;

        if (status === "published" && (!video && !category.introductionVideo || paragraph.trim() === "")) {
            alert("To publish, please upload a video and write your introduction paragraph.");
            return;
        }

        setIsSaving(true);
        try {
            await saveProfileWithVideo({
                name: category.name,
                type: CategoryType[category.type],
                sponsorId: category.sponsorId,
                videoFile: video || undefined,
                introductionParagraph: paragraph,
                profileStatus: status,
            });
            alert(`Profile ${status === "published" ? "published" : "saved as draft"}`);
        } catch (error) {
            alert("Error saving profile.");
            console.error(error);
        } finally {
            setIsSaving(false);
        }
    };

    const handleStatusChange = (newStatus: boolean) => {
        setStatus(newStatus ? "published" : "draft");
    };

    if (loading) return <p className="p-6 text-gray-400">Loading category profile...</p>;
    if (error) return <p className="p-6 text-[color:var(--color-brand)]">{error}</p>;

    return (
        <div className="flex flex-col lg:flex-row h-full relative">
            {/* Left panel - Form fields */}
            <div className="lg:w-1/2 xl:w-1/2 p-6 overflow-y-auto">
                <h1 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] mb-6">
                    Hi {category?.name} Sponsor! Let's edit your profile
                </h1>

                {/* Upload Video Input */}
                <div className="space-y-2 mb-6">
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300">
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
                        className="file-input-brand"
                    />
                </div>

                {/* Paragraph */}
                <div className="mb-6">
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
                <div className="space-y-2 mb-6">
                    <p className="text-sm text-gray-700 dark:text-gray-300">
                        Current Status:{" "}
                        <span className="font-medium">
                            {category?.profileStatus}
                        </span>
                    </p>

                    <div className="flex items-center gap-3">
                        <span className={`text-sm font-medium ${status === "draft" ? "text-[color:var(--color-brand)]" : "text-gray-500 dark:text-gray-400"}`}>
                            Draft
                        </span>
                        <label className="relative inline-flex items-center cursor-pointer">
                            <input
                                type="checkbox"
                                className="sr-only peer"
                                checked={status === "published"}
                                onChange={(e) => handleStatusChange(e.target.checked)}
                                disabled={isSaving}
                            />
                            <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer dark:bg-gray-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600 peer-checked:bg-[color:var(--color-brand)]"></div>
                        </label>
                        <span className={`text-sm font-medium ${status === "published" ? "text-[color:var(--color-brand)]" : "text-gray-500 dark:text-gray-400"}`}>
                            Publish
                        </span>
                    </div>
                    <p className="text-xs text-gray-500 dark:text-gray-400">
                        Toggle to choose whether your profile should be published or in a draft state.
                    </p>
                </div>

                {/* Actions */}
                <div className="pt-4">
                    <Button
                        onClick={handleSave}
                        className="btn-brand"
                        disabled={isSaving}
                    >
                        {isSaving ? "Saving..." : "Save"}
                    </Button>
                </div>
            </div>

            {/* Right panel - Video previews */}
            <div className="right-panel">
                <div className="space-y-6">
                    {/* Current Video from DB */}
                    {category?.introductionVideo && (
                        <div className="space-y-2">
                            <h2 className="text-lg font-semibold text-gray-700 dark:text-gray-300">Current Video</h2>
                            <video
                                src={category.introductionVideo}
                                controls
                                className="w-full rounded-md border dark:border-gray-700 shadow-sm"
                            />
                        </div>
                    )}

                    {/* Selected New Video Preview */}
                    {video && (
                        <div className="space-y-2">
                            <div className="flex items-center justify-between">
                                <h2 className="text-lg font-semibold text-gray-700 dark:text-gray-300">New Video Preview</h2>
                                <button
                                    onClick={() => setVideo(null)}
                                    className="text-xs px-2 py-1 rounded bg-[color:var(--color-brand)] dark:text-white text-gray-100 hover:bg-[color:var(--color-brand-hover)] transition"
                                >
                                    Remove
                                </button>
                            </div>
                            <video
                                src={URL.createObjectURL(video)}
                                controls
                                className="w-full rounded-md border dark:border-gray-700 shadow-sm"
                            />
                        </div>
                    )}

                    {!category?.introductionVideo && !video && (
                        <div className="flex justify-between items-center mb-6">
                            <p className="text-gray-500 dark:text-gray-400">
                                Video preview will appear here after selection
                            </p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}