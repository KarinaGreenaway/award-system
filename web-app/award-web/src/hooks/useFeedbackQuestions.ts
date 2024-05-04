import { useEffect, useState } from "react";
import Api from "@/api/Api";
import {
    FeedbackQuestionsResponseDto,
    FeedbackQuestionCreatePayload,
    FeedbackQuestionUpdatePayload
} from "@/types/FeedbackQuestions";
import { QuestionResponseType } from "@/types/enums/QuestionResponseType.ts";

type EditableFeedbackQuestion = FeedbackQuestionsResponseDto & { optionsInput?: string };

export function useFeedbackQuestions(eventId: number | null) {
    const [feedbackQuestions, setFeedbackQuestions] = useState<EditableFeedbackQuestion[]>([]);
    const [isSaving, setIsSaving] = useState(false);

    const mapQuestions = (questions: FeedbackQuestionsResponseDto[]): EditableFeedbackQuestion[] => {
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
                const questions = await Api.getFeedbackFormQuestions(eventId);
                setFeedbackQuestions(mapQuestions(questions));
            } catch (err) {
                console.error("Failed to fetch feedback questions", err);
            }
        };

        fetchQuestions();
    }, [eventId]);

    const handleQuestionChange = (
        id: number,
        field: keyof FeedbackQuestionsResponseDto | "optionsInput",
        value: any
    ) => {
        setFeedbackQuestions(prev =>
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

    const saveFeedbackQuestions = async () => {
        if (!eventId) {
            alert("Please create and save the event first.");
            return;
        }

        setIsSaving(true);
        try {
            for (const question of feedbackQuestions) {
                const options = question.optionsInput?.split(",").map(o => o.trim()).filter(Boolean) ?? [];

                const payload = {
                    eventId,
                    questionText: question.questionText,
                    responseType: question.responseType,
                    options,
                    questionOrder: question.questionOrder
                };

                if (
                    question.responseType === QuestionResponseType.MultipleChoice &&
                    options.length === 0
                ) {
                    alert("Multiple Choice questions must have at least one option.");
                    return;
                }

                if (question.id === 0) {
                    await Api.createFeedbackFormQuestion({
                        ...payload
                    } as FeedbackQuestionCreatePayload);
                } else {
                    await Api.updateFeedbackFormQuestion(question.id, payload as FeedbackQuestionUpdatePayload);
                }
            }

            alert("Feedback questions saved successfully.");
            const updated = await Api.getFeedbackFormQuestions(eventId);
            setFeedbackQuestions(mapQuestions(updated));
        } catch (err) {
            console.error("Failed to save feedback questions", err);
            alert("Error saving feedback questions.");
        } finally {
            setIsSaving(false);
        }
    };

    const addNewQuestion = () => {
        if (!eventId) {
            alert("Please save the event before adding questions.");
            return;
        }

        setFeedbackQuestions(prev => [
            ...prev,
            {
                id: 0,
                eventId,
                questionText: "",
                responseType: QuestionResponseType.Text,
                questionOrder: prev.length + 1,
                options: [],
                optionsInput: ""
            }
        ]);
    };

    return {
        feedbackQuestions,
        setFeedbackQuestions,
        isSavingFeedback: isSaving,
        handleQuestionChange,
        saveFeedbackQuestions,
        addNewQuestion
    };
}
