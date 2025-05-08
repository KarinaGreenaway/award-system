import { useSelectedCategory } from "@/context/CategoryProvider";
import { useCategoryProfile } from "@/hooks/useCategoryProfile";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { CategoryType } from "@/types/enums/CategoryType";
import { QuestionResponseType } from "@/types/enums/QuestionResponseType.ts";

export default function CategoryProfilePage() {
    const { selectedCategoryId } = useSelectedCategory();
    const {
        category,
        loading,
        error,
        saveProfileWithVideo,
        nominationQuestions,
        setNominationQuestions,
        saveNominationQuestions,
    } = useCategoryProfile(selectedCategoryId ?? null);

    const [video, setVideo] = useState<File | null>(null);
    const [paragraph, setParagraph] = useState<string>("");
    const [status, setStatus] = useState<"draft" | "published">("draft");
    const [isSaving, setIsSaving] = useState(false);

    const userRole = localStorage.getItem("mock_role");
    const isAdmin = userRole === "Admin";

    const userId = Number(localStorage.getItem("mock_user_id"));
    const isSponsor = category?.sponsorId === userId;

    const isEditable = isAdmin || isSponsor;




    useEffect(() => {
        if (category) {
            setParagraph(category.introductionParagraph || "");
            setStatus(category.profileStatus);
            setVideo(null);
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
                awardProcessId: category.awardProcessId,
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

    const handleQuestionChange = (id: number, field: string, value: any) => {
        setNominationQuestions(prev =>
            prev.map(q => {
                if (q.id !== id) return q;
                const updated = { ...q, [field]: value };
                if (field === "optionsInput") {
                    updated.options = value
                        .split(",")
                        .map((o: string) => o.trim())
                        .filter(Boolean);
                }
                return updated;
            })
        );
    };

    const handleAddQuestion = () => {
        if (!category) return;
        setNominationQuestions(prev => [
            ...prev,
            {
                id: 0,
                categoryId: category.id,
                questionText: "",
                responseType: QuestionResponseType.Text,
                options: [],
                optionsInput: "",
                questionOrder: prev.length + 1,
            }
        ]);
    };

    if (loading) return <p className="p-6 text-gray-400">Loading category profile...</p>;
    if (error) return <p className="p-6 text-[color:var(--color-brand)]">{error}</p>;

    return (
        <div className="flex flex-col lg:flex-row h-full relative">
            {/* Left panel */}
            <div className="lg:w-1/2 xl:w-1/2 p-6 overflow-y-auto">
                <h1 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] mb-6">
                    The {category?.name} Profile
                </h1>

                {!isEditable && (
                    <div className="mb-4 p-3 rounded bg-[color:var(--color-brand-hover)] text-white rounded-xl">
                        You are in read-only mode. Only the category sponsor and admins can edit this page.
                    </div>
                )}

                {/* Upload */}
                <div className="mb-6">
                    <label className="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
                        Introduction Video
                    </label>
                    {isEditable && (
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
                            disabled={!isEditable}
                        />
                    )}


                    {/* Fixed-height container to prevent jumping */}
                    <div className="mt-2 w-full rounded border shadow overflow-hidden h-[300px] bg-black">
                        {video ? (
                            <video
                                key={video.name}
                                src={URL.createObjectURL(video)}
                                controls
                                className="w-full h-full object-contain"
                            />
                        ) : category?.introductionVideo ? (
                            <video
                                src={category.introductionVideo}
                                controls
                                className="w-full h-full object-contain"
                            />
                        ) : (
                            <div className="flex items-center justify-center h-full text-sm text-gray-500 dark:text-gray-400">
                                No video selected
                            </div>
                        )}
                    </div>
                </div>


                {/* Paragraph */}
                <div className="mb-6">
                    <label className="block mb-1 text-sm font-medium text-gray-700 dark:text-gray-300">
                        Introductory Paragraph (max 300 words)
                    </label>
                    <textarea
                        rows={6}
                        value={paragraph}
                        onChange={(e) => setParagraph(e.target.value)}
                        maxLength={300 * 6}
                        disabled={!isEditable}
                        className="w-full rounded-md p-3 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-300"
                    />
                </div>

                {/* Status Toggle */}
                <div className="space-y-2 mb-6">
                    <p className="text-sm text-gray-700 dark:text-gray-300">
                        Current Status: <span className="font-medium">{status}</span>
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
                                disabled={!isEditable || isSaving}
                            />
                            <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer dark:bg-gray-700 peer-checked:bg-[color:var(--color-brand)] peer-checked:after:translate-x-full after:content-[''] after:absolute after:left-[2px] after:top-[2px] after:bg-white after:border after:border-gray-300 after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600"></div>
                        </label>
                        <span className={`text-sm font-medium ${status === "published" ? "text-[color:var(--color-brand)]" : "text-gray-500 dark:text-gray-400"}`}>
                            Publish
                        </span>
                    </div>
                </div>

                {/* Save */}
                {isEditable && (
                    <div className="pt-4">
                        <Button onClick={handleSave} className="btn-brand" disabled={isSaving}>
                            {isSaving ? "Saving..." : "Save"}
                        </Button>
                    </div>
                )}
            </div>

            {/* Right panel - Nomination Questions */}
            <div className="right-panel p-6 space-y-6 overflow-y-auto border-l border-gray-200 dark:border-gray-700">
                <div className="flex justify-between items-center">
                    <h2 className="text-xl font-semibold text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                        Nomination Questions
                    </h2>
                    {isEditable && (
                        <Button onClick={handleAddQuestion} className="btn-brand">+ New Question</Button>
                    )}
                </div>

                {nominationQuestions
                    .sort((a, b) => (a.questionOrder ?? 0) - (b.questionOrder ?? 0))
                    .map((q) => (
                        <div key={q.id || `new-${q.questionOrder}`} className="space-y-4 shadow-md p-4 rounded-md bg-white dark:bg-gray-800">
                            <div>
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Question Text</label>
                                <input
                                    type="text"
                                    value={q.questionText}
                                    onChange={e => handleQuestionChange(q.id, "questionText", e.target.value)}
                                    disabled={!isEditable}
                                    className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Response Type</label>
                                <select
                                    value={q.responseType}
                                    onChange={e => handleQuestionChange(q.id, "responseType", Number(e.target.value))}
                                    disabled={!isEditable}
                                    className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                >
                                    <option value={QuestionResponseType.Text}>Text</option>
                                    <option value={QuestionResponseType.YesNo}>Yes/No</option>
                                    <option value={QuestionResponseType.MultipleChoice}>Multiple Choice</option>
                                </select>
                            </div>
                            {q.responseType === QuestionResponseType.MultipleChoice && (
                                <div>
                                    <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">
                                        Options (comma-separated)
                                    </label>
                                    <input
                                        type="text"
                                        value={q.optionsInput ?? ""}
                                        onChange={e => handleQuestionChange(q.id, "optionsInput", e.target.value)}
                                        disabled={!isEditable}
                                        className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                        placeholder="e.g., Option A, Option B, Option C"
                                    />
                                </div>
                            )}
                            <div>
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Order</label>
                                <input
                                    type="number"
                                    value={q.questionOrder ?? 0}
                                    onChange={e => handleQuestionChange(q.id, "questionOrder", parseInt(e.target.value))}
                                    disabled={!isEditable}
                                    className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                />
                            </div>
                        </div>
                    ))}

                {isEditable && (
                    <div className="pt-4">
                        <Button onClick={saveNominationQuestions} className="btn-brand">
                            Save Nomination Questions
                        </Button>
                    </div>
                )}
            </div>
        </div>
    );
}
