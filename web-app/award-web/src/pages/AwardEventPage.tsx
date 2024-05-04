
import { useEffect, useState } from "react";
import { useAwardEvent } from "@/hooks/useAwardEvent";
import { Button } from "@/components/ui/button";
import Api from "@/api/Api.ts";
import "react-datepicker/dist/react-datepicker.css";
import {
    RsvpFormQuestionResponseDto,
    RsvpFormQuestionUpdatePayload,
    RsvpFormQuestionCreatePayload
} from "@/types/RsvpTypes";
import { RsvpResponseType } from "@/types/enums/RsvpResponseType.ts";

export default function AwardEventPage() {
    const { event, activeProcessId, loading, error, refetch } = useAwardEvent();

    const [name, setName] = useState("");
    const [location, setLocation] = useState("");
    const [eventDateTime, setEventDateTime] = useState("");
    const [description, setDescription] = useState("");
    const [status, setStatus] = useState<"draft" | "published">("draft");
    const [isSaving, setIsSaving] = useState(false);

    type EditableRsvpQuestion = RsvpFormQuestionResponseDto & { optionsInput?: string };
    const [rsvpQuestions, setRsvpQuestions] = useState<EditableRsvpQuestion[]>([]);

    const [isSavingRsvp, setIsSavingRsvp] = useState(false);

    const userRole = localStorage.getItem("mock_role") as "Admin" | "User" | null;
    const isAdmin = userRole === "Admin";

    useEffect(() => {
        const fetchQuestions = async () => {
            if (!event?.id) return;
            try {
                const questions = await Api.getRsvpFormQuestions(event.id);
                setRsvpQuestions(
                    questions.map(q => ({
                        ...q,
                        options: q.options ?? [],
                        optionsInput: (q.options ?? []).join(", ")
                    }))
                );
            } catch (err) {
                console.error("Failed to fetch RSVP questions", err);
            }
        };

        fetchQuestions();
    }, [event]);

    const handleQuestionChange = (id: number, field: keyof RsvpFormQuestionResponseDto | "optionsInput", value: any) => {
        setRsvpQuestions(prev =>
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

    const handleSaveRsvp = async () => {
        if (!event?.id) {
            alert("Please create and save the event first.");
            return;
        }

        setIsSavingRsvp(true);
        try {
            for (const question of rsvpQuestions) {
                const options = question.optionsInput?.split(",").map(o => o.trim()).filter(Boolean) ?? [];

                const payload = {
                    questionText: question.questionText,
                    responseType: question.responseType,
                    tooltip: question.tooltip,
                    questionOrder: question.questionOrder,
                    options: options ?? []
                };

                if (
                    question.responseType === RsvpResponseType.MultipleChoice &&
                    (!options || options.length === 0)
                ) {
                    alert("Multiple Choice questions must have at least one option.");
                    return;
                }

                if (question.id === 0) {
                    await Api.createRsvpFormQuestion({
                        ...payload,
                        eventId: event.id
                    } as RsvpFormQuestionCreatePayload);
                } else {
                    await Api.updateRsvpFormQuestion(question.id, payload as RsvpFormQuestionUpdatePayload);
                }
            }
            alert("RSVP questions saved successfully.");
            const updated = await Api.getRsvpFormQuestions(event.id);
            setRsvpQuestions(updated);
        } catch (err) {
            console.error("Failed to save RSVP questions", err);
            alert("Error saving RSVP questions.");
        } finally {
            setIsSavingRsvp(false);
        }
    };

    useEffect(() => {
        if (event) {
            setName(event.name);
            setLocation(event.location);
            setEventDateTime(formatDatetimeLocal(event.eventDateTime));
            setDescription(event.description ?? "");
            setStatus(event.status ?? "draft");
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
                    status
                });
            } else {
                await Api.createAwardEvent({
                    name,
                    location,
                    eventDateTime,
                    description,
                    status,
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

    const handleStatusChange = (newStatus: boolean) => {
        setStatus(newStatus ? "published" : "draft");
    };

    const handleAddQuestion = () => {
        if (!event?.id) {
            alert("Please save the event before adding questions.");
            return;
        }

        setRsvpQuestions(prev => [
            ...prev,
            {
                id: 0,
                eventId: event.id,
                questionText: "",
                responseType: RsvpResponseType.Text,
                tooltip: "",
                questionOrder: prev.length + 1,
                options: [],
                optionsInput: ""
            }
        ]);
    };

    function formatDatetimeLocal(datetime: string | Date | null): string {
        if (!datetime) return "";
        const dt = typeof datetime === "string" ? new Date(datetime) : datetime;
        const offset = dt.getTimezoneOffset();
        const local = new Date(dt.getTime() - offset * 60000);
        return local.toISOString().slice(0, 16);
    }

    if (loading) return <p className="p-6 text-gray-400">Loading event...</p>;
    if (error) return <p className="p-6 text-[color:var(--color-brand)]">{error}</p>;

    return (
        <div className="flex flex-col lg:flex-row h-full relative">
            <div className="lg:w-1/2 xl:w-1/2 p-6 overflow-y-auto">
                <h1 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] mb-6">
                    The Awards Event
                </h1>

                {!isAdmin && (
                    <div className="mb-4 p-3 rounded bg-[color:var(--color-brand-hover)] text-white rounded-xl">
                        You are in read-only mode. Only admins can edit this page.
                    </div>
                )}

                {([["Event Name *", name, setName], ["Location *", location, setLocation]] as const).map(([label, val, setter]) => (
                    <div key={label} className="mb-6">
                        <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">{label}</label>
                        <input
                            type="text"
                            value={val}
                            onChange={(e) => setter(e.target.value)}
                            disabled={!isAdmin}
                            className="w-full rounded-md p-2 text-sm text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
                        />
                    </div>
                ))}

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
                                disabled={!isAdmin || isSaving}
                            />
                            <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer dark:bg-gray-700 peer-checked:bg-[color:var(--color-brand)] peer-checked:after:translate-x-full after:content-[''] after:absolute after:left-[2px] after:top-[2px] after:bg-white after:border after:border-gray-300 after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600"></div>
                        </label>
                        <span className={`text-sm font-medium ${status === "published" ? "text-[color:var(--color-brand)]" : "text-gray-500 dark:text-gray-400"}`}>
                            Publish
                        </span>
                    </div>
                </div>

                {isAdmin && (
                    <div className="pt-4">
                        <Button onClick={handleSave} className="btn-brand" disabled={isSaving}>
                            {isSaving ? "Saving..." : "Save"}
                        </Button>
                    </div>
                )}
            </div>

            <div className="right-panel p-6 space-y-6 overflow-y-auto border-l border-gray-200 dark:border-gray-700">
                <div className="flex justify-between items-center">
                    <h2 className="text-xl font-semibold text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                        RSVP Questions
                    </h2>
                    {isAdmin && (
                        <Button onClick={handleAddQuestion} className="btn-brand">+ New Question</Button>
                    )}
                </div>

                {[...rsvpQuestions]
                    .sort((a, b) => (a.questionOrder ?? 0) - (b.questionOrder ?? 0))
                    .map((q) => (
                        <div key={q.id || `new-${q.questionOrder}`} className="space-y-4 shadow-md p-4 rounded-md bg-white dark:bg-gray-800">
                            <div>
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Question Text</label>
                                <input
                                    type="text"
                                    value={q.questionText}
                                    onChange={e => handleQuestionChange(q.id, "questionText", e.target.value)}
                                    disabled={!isAdmin}
                                    className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Response Type</label>
                                <select
                                    value={q.responseType}
                                    onChange={e => handleQuestionChange(q.id, "responseType", Number(e.target.value))}
                                    disabled={!isAdmin}
                                    className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                >
                                    <option value={RsvpResponseType.Text}>Text</option>
                                    <option value={RsvpResponseType.YesNo}>Yes/No</option>
                                    <option value={RsvpResponseType.MultipleChoice}>Multiple Choice</option>
                                </select>
                            </div>
                            {q.responseType === RsvpResponseType.MultipleChoice && (
                                <div>
                                    <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">
                                        Options (comma-separated)
                                    </label>
                                    <input
                                        type="text"
                                        value={q.optionsInput ?? ""}
                                        onChange={e => handleQuestionChange(q.id, "optionsInput", e.target.value)}
                                        disabled={!isAdmin}
                                        className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                        placeholder="e.g., Option A, Option B, Option C"
                                    />
                                </div>
                            )}
                            <div>
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Tooltip</label>
                                <input
                                    type="text"
                                    value={q.tooltip ?? ""}
                                    onChange={e => handleQuestionChange(q.id, "tooltip", e.target.value)}
                                    disabled={!isAdmin}
                                    className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1 text-gray-700 dark:text-gray-300">Order</label>
                                <input
                                    type="number"
                                    value={q.questionOrder ?? 0}
                                    onChange={e => handleQuestionChange(q.id, "questionOrder", parseInt(e.target.value))}
                                    disabled={!isAdmin}
                                    className="w-full rounded-md p-2 border text-sm border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-900 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                />
                            </div>
                        </div>
                    ))}

                {isAdmin && (
                    <div className="pt-4">
                        <Button onClick={handleSaveRsvp} className="btn-brand" disabled={isSavingRsvp}>
                            {isSavingRsvp ? "Saving..." : "Save RSVP Questions"}
                        </Button>
                    </div>
                )}
            </div>
        </div>
    );
}
