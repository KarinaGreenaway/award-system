import { useSelectedCategory } from "@/context/CategoryProvider";
import { useCategoryProfile } from "@/hooks/useCategoryProfile";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { CategoryType } from "@/types/enums/CategoryType";
import { QuestionResponseType } from "@/types/enums/QuestionResponseType.ts";
import {Plus, Trash2} from "lucide-react";

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
        deleteNominationQuestion,
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

    const handleQuestionChange = (id: number, field: string, value: string | string[] | QuestionResponseType) => {
        setNominationQuestions(prev =>
            prev.map(q => {
                if (q.id !== id) return q;
                const updated = { ...q, [field]: value };
                if (field === "optionsInput" && typeof value === "string") {
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
                id: Date.now(), // unique identifier for new questions
                categoryId: category.id,
                questionText: "",
                responseType: QuestionResponseType.Text,
                options: [],
                optionsInput: "",
                questionOrder: prev.length + 1,
                isNew: true, //flag to identify questions as new
            }
        ]);
    };

    const moveQuestion = (id: number, direction: "up" | "down") => {
        setNominationQuestions(prevQuestions => {
            const newQuestions = [...prevQuestions];
            const index = newQuestions.findIndex(q => q.id === id);

            if (index === -1) return prevQuestions;

            // Swapping question with the one above or below it
            if (direction === "up" && index > 0) {
                [newQuestions[index], newQuestions[index - 1]] = [newQuestions[index - 1], newQuestions[index]];
            } else if (direction === "down" && index < newQuestions.length - 1) {
                [newQuestions[index], newQuestions[index + 1]] = [newQuestions[index + 1], newQuestions[index]];
            }

            // Updating questionOrder after moving
            newQuestions.forEach((q, idx) => {
                q.questionOrder = idx + 1;
            });

            return newQuestions;
        });
    };

    const handleDeleteQuestion = (id: number) => {
        deleteNominationQuestion(id); // Call the function to delete the question from the backend and state
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
                        Current Status: <span className="font-medium">{category?.profileStatus}</span>
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
                    <div className="pt-4 flex justify-end">
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

                    <div className="flex justify-end">
                        {isEditable && (
                            <Button
                                className="btn-brand"
                                onClick={saveNominationQuestions}
                            >
                                Save
                            </Button>
                        )}
                    </div>
                </div>


                {nominationQuestions
                    .sort((a, b) => (a.questionOrder ?? 0) - (b.questionOrder ?? 0))
                    .map((q) => (
                        <div key={q.id || `new-${q.questionOrder}`} className="space-y-4 shadow-md p-4 rounded-md bg-white dark:bg-gray-800 relative">
                            {/* Delete button positioned in the top right */}
                            {q.id && (
                                <Button
                                    variant="destructive"
                                    className="cursor-pointer absolute top-2 right-2 text-gray-400 hover:text-red-600"
                                    onClick={(e) => {
                                        e.stopPropagation();

                                        if (!q.isNew && confirm("Are you sure you want to delete this question?")) {
                                            handleDeleteQuestion(q.id);
                                        } else if (q.isNew) {
                                            handleDeleteQuestion(q.id);
                                        }
                                    }}
                                >
                                    <Trash2 className="h-4 w-4" />
                                </Button>

                            )}

                            {/* Question content */}
                            <div className="pt-8">
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Question Text</label>
                                <input
                                    type="text"
                                    value={q.questionText}
                                    onChange={e => handleQuestionChange(q.id, "questionText", e.target.value)}
                                    disabled={!isEditable}
                                    className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                />
                            </div>

                            {/* Response Type */}
                            <div className="pt-2">
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

                            {/* Options Input */}
                            {q.responseType === QuestionResponseType.MultipleChoice && (
                                <div className="pt-2">
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

                            {/* Question Order */}
                            <div className="flex justify-end items-center pt-2">
                                {/* Up/Down Arrows */}
                                <div className="flex gap-2">
                                    <p className="p-2 dark:text-[color:var(--color-text-dark)]"> Q{q.questionOrder}</p>
                                    <Button
                                        onClick={() => moveQuestion(q.id, "up")}
                                        disabled={!isEditable || q.questionOrder === 1} // Disable when it's the first question
                                        className="btn-brand p-2"
                                    >
                                        ↑
                                    </Button>
                                    <Button
                                        onClick={() => moveQuestion(q.id, "down")}
                                        disabled={!isEditable || q.questionOrder === nominationQuestions.length} // Disable when it's the last question
                                        className="btn-brand p-2"
                                    >
                                        ↓
                                    </Button>
                                </div>
                            </div>
                        </div>
                    ))}

                <div className="flex justify-center mt-2">
                    {isEditable && (
                        <Button
                            className="form-add-button"
                            onClick={handleAddQuestion}
                        >
                            <Plus className="justify-center w-4 h-4 text-[color:var(--color-brand)] dark:text-white" />
                            New Question
                        </Button>
                    )}
                </div>
            </div>
        </div>
    );
}
