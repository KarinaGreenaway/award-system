import { useEffect, useState } from "react";
import Api from "@/api/Api";
import {
    RsvpFormQuestionResponseDto,
    RsvpFormQuestionCreatePayload,
    RsvpFormQuestionUpdatePayload
} from "@/types/Rsvp.ts";
import { QuestionResponseType } from "@/types/enums/QuestionResponseType.ts";

type EditableRsvpQuestion = RsvpFormQuestionResponseDto & { optionsInput?: string, isNew?: boolean };

export function useRsvpQuestions(eventId: number | null) {
    const [rsvpQuestions, setRsvpQuestions] = useState<EditableRsvpQuestion[]>([]);
    const [isSaving, setIsSaving] = useState(false);

    const mapQuestions = (questions: RsvpFormQuestionResponseDto[]): EditableRsvpQuestion[] => {
        return questions.map(q => ({
            ...q,
            options: q.options ?? [],
            optionsInput: (q.options ?? []).join(", ")
        }));
    };

    useEffect(() => {
        if (!eventId) return;

        const fetchQuestions = async () => {
            try {
                const questions = await Api.getRsvpFormQuestions(eventId);
                setRsvpQuestions(mapQuestions(questions));
            } catch (err) {
                console.error("Failed to fetch RSVP questions", err);
            }
        };

        fetchQuestions();
    }, [eventId]);

    const handleQuestionChange = (
        id: number,
        field: keyof RsvpFormQuestionResponseDto | "optionsInput",
        value: any
    ) => {
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

    const saveRsvpQuestions = async () => {
        if (!eventId) {
            alert("Please create and save the event first.");
            return;
        }

        setIsSaving(true);
        try {
            for (const question of rsvpQuestions) {
                const options = question.optionsInput?.split(",").map(o => o.trim()).filter(Boolean) ?? [];

                const payload = {
                    questionText: question.questionText,
                    responseType: question.responseType,
                    tooltip: question.tooltip,
                    questionOrder: question.questionOrder,
                    options
                };

                if (
                    question.responseType === QuestionResponseType.MultipleChoice &&
                    options.length === 0
                ) {
                    alert("Multiple Choice questions must have at least one option.");
                    return;
                }

                if (question.isNew) {
                    await Api.createRsvpFormQuestion({
                        ...payload,
                        eventId
                    } as RsvpFormQuestionCreatePayload);
                } else {
                    await Api.updateRsvpFormQuestion(question.id, payload as RsvpFormQuestionUpdatePayload);
                }
            }

            alert("RSVP questions saved successfully.");
            const updated = await Api.getRsvpFormQuestions(eventId);
            setRsvpQuestions(mapQuestions(updated));
        } catch (err) {
            console.error("Failed to save RSVP questions", err);
            alert("Error saving RSVP questions.");
        } finally {
            setIsSaving(false);
        }
    };

    const addNewQuestion = () => {
        if (!eventId) {
            alert("Please save the event before adding questions.");
            return;
        }

        setRsvpQuestions(prev => [
            ...prev,
            {
                id: Date.now(),
                eventId,
                questionText: "",
                responseType: QuestionResponseType.Text,
                tooltip: "",
                questionOrder: prev.length + 1,
                options: [],
                optionsInput: "",
                isNew: true
            }
        ]);
    };

    const deleteRsvpQuestion = async (id: number) => {
        try {
            await Api.deleteRsvpFormQuestion(id);

            setRsvpQuestions(prevQuestions =>
                prevQuestions.filter(q => q.id !== id)
            );
            alert("Question deleted successfully.");
        } catch (error) {
            alert("Error deleting question.");
            console.error(error);
        }
    };

    return {
        rsvpQuestions,
        setRsvpQuestions,
        isSavingRsvp: isSaving,
        handleQuestionChange,
        saveRsvpQuestions,
        deleteRsvpQuestion,
        addNewQuestion
    };
}
