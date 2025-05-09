import { useEffect, useState } from "react";
import { useAwardEvent } from "@/hooks/useAwardEvent";
import { Button } from "@/components/ui/button";
import Api from "@/api/Api.ts";
import { useRsvpQuestions } from "@/hooks/useRsvpQuestions";
import { useFeedbackQuestions } from "@/hooks/useFeedbackQuestions";
import { QuestionResponseType } from "@/types/enums/QuestionResponseType.ts";
import { Plus } from "lucide-react";

export default function AwardEventPage() {
    const { event, activeProcessId, loading, error, refetch } = useAwardEvent();

    const [name, setName] = useState("");
    const [location, setLocation] = useState("");
    const [eventDateTime, setEventDateTime] = useState("");
    const [description, setDescription] = useState("");
    const [isSaving, setIsSaving] = useState(false);
    const [showFeedback, setShowFeedback] = useState(false);

    const {
        rsvpQuestions,
        handleQuestionChange,
        saveRsvpQuestions,
        addNewQuestion,
        isSavingRsvp,
        setRsvpQuestions
    } = useRsvpQuestions(event?.id ?? null);

    const {
        feedbackQuestions,
        handleQuestionChange: handleFeedbackChange,
        saveFeedbackQuestions,
        addNewQuestion: addNewFeedbackQuestion,
        isSavingFeedback,
        setFeedbackQuestions
    } = useFeedbackQuestions(event?.id ?? null);

    const userRole = localStorage.getItem("mock_role") as "Admin" | "User" | null;
    const isAdmin = userRole === "Admin";

    useEffect(() => {
        if (event) {
            setName(event.name);
            setLocation(event.location);
            setEventDateTime(formatDatetimeLocal(event.eventDateTime));
            setDescription(event.description ?? "");
        }
    }, [event]);

    const handleSave = async () => {
        if (!name || !location || !eventDateTime) {
            alert("Name, location, and event date/time are required.");
            return;
        }

        setIsSaving(true);
        try {
            if (event) {
                await Api.updateAwardEvent(event.id, {
                    name,
                    location,
                    eventDateTime,
                    description,
                });
            } else {
                await Api.createAwardEvent({
                    name,
                    location,
                    eventDateTime,
                    description,
                    awardProcessId: activeProcessId ?? 0,
                    directions: ""
                });
            }
            alert("Event saved successfully");
            await refetch();
        } catch (err) {
            console.error("Failed to save event", err);
            alert("Error saving event.");
        } finally {
            setIsSaving(false);
        }
    };

    function formatDatetimeLocal(datetime: string | Date | null): string {
        if (!datetime) return "";
        const dt = typeof datetime === "string" ? new Date(datetime) : datetime;
        const offset = dt.getTimezoneOffset();
        const local = new Date(dt.getTime() - offset * 60000);
        return local.toISOString().slice(0, 16);
    }

    // Function to move question up or down
    const moveQuestion = (id: number, direction: "up" | "down", isFeedback: boolean) => {
        const setQuestions = isFeedback ? setFeedbackQuestions : setRsvpQuestions;
        const questions = isFeedback ? feedbackQuestions : rsvpQuestions;

        const newQuestions = [...questions];
        const index = newQuestions.findIndex(q => q.id === id);

        if (index === -1) return;

        if (direction === "up" && index > 0) {
            [newQuestions[index], newQuestions[index - 1]] = [newQuestions[index - 1], newQuestions[index]];
        } else if (direction === "down" && index < newQuestions.length - 1) {
            [newQuestions[index], newQuestions[index + 1]] = [newQuestions[index + 1], newQuestions[index]];
        }

        // Update questionOrder after moving
        newQuestions.forEach((q, idx) => {
            q.questionOrder = idx + 1;
        });

        setQuestions(newQuestions as any);
    };

    if (loading) return <p className="p-6 text-gray-400">Loading event...</p>;
    if (error) return <p className="p-6 text-[color:var(--color-brand)]">{error}</p>;

    return (
        <div className="flex flex-col lg:flex-row h-full relative">
            {/* Left: Event Details */}
            <div className="lg:w-1/2 xl:w-1/2 p-6 overflow-y-auto">
                <h1 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] mb-6">
                    The Awards Event
                </h1>

                {!isAdmin && (
                    <div className="mb-4 p-3 rounded bg-[color:var(--color-brand-hover)] text-white rounded-xl">
                        You are in read-only mode. Only admins can edit this page.
                    </div>
                )}

                {[{ label: "Event Name *", value: name, setter: setName }, { label: "Location *", value: location, setter: setLocation }].map(
                    ({ label, value, setter }, index) => (
                        <div key={index} className="mb-6">
                            <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">{label}</label>
                            <input
                                type="text"
                                value={value}
                                onChange={(e) => setter(e.target.value)}
                                disabled={!isAdmin}
                                className="w-full rounded-md p-2 text-sm text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
                            />
                        </div>
                    )
                )}

                <div className="mb-6">
                    <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">Date & Time *</label>
                    <input
                        type="datetime-local"
                        value={eventDateTime}
                        onChange={(e) => setEventDateTime(e.target.value)}
                        disabled={!isAdmin}
                        className="w-full rounded-md p-2 text-sm text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
                    />
                </div>

                <div className="mb-6">
                    <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">Event Description</label>
                    <textarea
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        disabled={!isAdmin}
                        rows={4}
                        className="w-full rounded-md p-2 text-sm text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
                    />
                </div>

                {isAdmin && (
                    <div className="pt-4">
                        <Button onClick={handleSave} className="btn-brand" disabled={isSaving}>
                            {isSaving ? "Saving..." : "Save"}
                        </Button>
                    </div>
                )}
            </div>

            {/* Right: RSVP Questions / Feedback Questions */}
            <div className="right-panel p-6 space-y-6 overflow-y-auto border-l border-gray-200 dark:border-gray-700">
                <div className="flex justify-between items-center">
                    <h2 className="text-xl font-semibold text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                        {showFeedback ? "Feedback Questions" : "RSVP Questions"}
                    </h2>
                    <div className="flex items-center gap-4">
                        <div className="flex rounded-md overflow-hidden shadow-md">
                            <button
                                onClick={() => setShowFeedback(false)}
                                className={`px-4 py-1 text-sm font-medium border-r border-gray-300 dark:border-gray-700 ${
                                    !showFeedback
                                        ? "bg-[color:var(--color-brand)] text-white"
                                        : "bg-white dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600"
                                }`}
                            >
                                RSVP
                            </button>
                            <button
                                onClick={() => setShowFeedback(true)}
                                className={`px-4 py-1 text-sm font-medium ${
                                    showFeedback
                                        ? "bg-[color:var(--color-brand)] text-white"
                                        : "bg-white dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600"
                                }`}
                            >
                                Feedback
                            </button>
                        </div>
                    </div>
                </div>


                {(showFeedback ? feedbackQuestions : rsvpQuestions)
                    .sort((a, b) => (a.questionOrder ?? 0) - (b.questionOrder ?? 0))
                    .map((q) => (
                        <div key={q.id || `new-${q.questionOrder}`} className="space-y-4 shadow-md p-4 rounded-md bg-white dark:bg-gray-800">
                            {/* Question content */}
                            <div>
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Question Text</label>
                                <input
                                    type="text"
                                    value={q.questionText}
                                    onChange={e =>
                                        (showFeedback ? handleFeedbackChange : handleQuestionChange)(q.id, "questionText", e.target.value)
                                    }
                                    disabled={!isAdmin}
                                    className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Response Type</label>
                                <select
                                    value={q.responseType}
                                    onChange={e =>
                                        (showFeedback ? handleFeedbackChange : handleQuestionChange)(q.id, "responseType", Number(e.target.value))
                                    }
                                    disabled={!isAdmin}
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
                                        onChange={e =>
                                            (showFeedback ? handleFeedbackChange : handleQuestionChange)(q.id, "optionsInput", e.target.value)
                                        }
                                        disabled={!isAdmin}
                                        className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                        placeholder="e.g., Option A, Option B, Option C"
                                    />
                                </div>
                            )}
                            {/* Other question fields (response type, options, etc.) */}
                            <div className="flex justify-end items-center">
                                {/* Move question up/down */}
                                <div className="flex gap-2">
                                    <p className="p-2 dark:text-[color:var(--color-text-dark)]"> Q{q.questionOrder}</p>
                                    <Button
                                        onClick={() => moveQuestion(q.id, "up", showFeedback)}
                                        disabled={!isAdmin || q.questionOrder === 1}
                                        className="btn-brand p-2"
                                    >
                                        ↑
                                    </Button>
                                    <Button
                                        onClick={() => moveQuestion(q.id, "down", showFeedback)}
                                        disabled={!isAdmin || q.questionOrder === (showFeedback ? feedbackQuestions.length : rsvpQuestions.length)}
                                        className="btn-brand p-2"
                                    >
                                        ↓
                                    </Button>
                                </div>
                            </div>
                        </div>
                    ))}

                <div className="flex justify-center mt-2">
                    {isAdmin && (
                        <Button
                            onClick={() => (showFeedback ? addNewFeedbackQuestion() : addNewQuestion())}
                            className="form-add-button"
                        >
                            <Plus className="h-4 w-4" /> New Question
                        </Button>
                    )}
                </div>


                {isAdmin && (
                    <div className="flex justify-end mt-10">
                        <Button
                            onClick={showFeedback ? saveFeedbackQuestions : saveRsvpQuestions}
                            className="btn-brand"
                            disabled={showFeedback ? isSavingFeedback : isSavingRsvp}
                        >
                            {showFeedback
                                ? isSavingFeedback ? "Saving..." : "Save Feedback Questions"
                                : isSavingRsvp ? "Saving..." : "Save RSVP Questions"}
                        </Button>
                    </div>
                )}

            </div>
        </div>
    );
}
