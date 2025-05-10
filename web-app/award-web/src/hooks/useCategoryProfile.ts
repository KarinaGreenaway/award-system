import { useEffect, useState } from "react";
import Api from "@/api/Api";
import {
    AwardCategoryResponseDto,
    AwardCategoryUpdatePayload,
} from "@/types/AwardCategory";
import { QuestionResponseType } from "@/types/enums/QuestionResponseType.ts";

type EditableNominationQuestion = {
    isNew: boolean;
    id: number;
    categoryId: number;
    questionText: string;
    responseType: QuestionResponseType;
    questionOrder: number;
    options?: string[];
    optionsInput?: string;
};

export function useCategoryProfile(categoryId: number | null) {
    const [category, setCategory] = useState<AwardCategoryResponseDto | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [nominationQuestions, setNominationQuestions] = useState<EditableNominationQuestion[]>([]);

    const fetchProfile = async () => {
        if (!categoryId || categoryId <= 0) return;
        setLoading(true);
        setError(null);
        try {
            const data = await Api.getAwardCategoryById(categoryId);
            setCategory(data);

            const questions = await Api.getNominationQuestions(categoryId);
            setNominationQuestions(
                questions.map((q: any) => ({
                    ...q,
                    responseType: Number(q.responseType), // ensures number
                    options: q.options ?? [],
                    optionsInput: (q.options ?? []).join(", ")
                }))
            );
        } catch (err: any) {
            console.error("Failed to fetch category profile", err);
            setError("Failed to fetch category profile.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchProfile();
    }, [categoryId]);

    const updateProfile = async (updates: AwardCategoryUpdatePayload) => {
        if (!category) return;
        await Api.updateAwardCategory(category.id, updates);
        await fetchProfile();
    };

    const saveProfileWithVideo = async (
        updates: Omit<AwardCategoryUpdatePayload, "introductionVideo"> & { videoFile?: File }
    ) => {
        if (!category) return;

        let videoUrl = category.introductionVideo;
        if (updates.videoFile) {
            videoUrl = await Api.uploadVideo(updates.videoFile);
        }

        await updateProfile({
            ...updates,
            introductionVideo: videoUrl,
        });
    };

    const saveNominationQuestions = async () => {
        if (!category) return;

        for (const question of nominationQuestions) {
            const payload = {
                questionText: question.questionText,
                responseType: Number(question.responseType),
                questionOrder: question.questionOrder,
                options: question.options ?? [],
            };

            if (
                question.responseType === QuestionResponseType.MultipleChoice &&
                (!payload.options || payload.options.length === 0)
            ) {
                alert("Multiple Choice questions must have at least one option.");
                return;
            }

            if (question.isNew) {
                await Api.createNominationQuestion({
                    ...payload,
                    categoryId: category.id,
                });
            } else {
                await Api.updateNominationQuestion(question.id, payload);
            }
        }

        alert("Nomination questions saved.");
        await fetchProfile();
    };

    const deleteNominationQuestion = async (id: number) => {
        try {
            const questionToDelete = nominationQuestions.find(q => q.id === id);

            if (questionToDelete?.isNew) {
                setNominationQuestions(prevQuestions =>
                    prevQuestions.filter(q => q.id !== id)
                );
            } else {
                await Api.deleteNominationQuestion(id);
                setNominationQuestions(prevQuestions =>
                    prevQuestions.filter(q => q.id !== id)
                );
                alert("Question deleted successfully.");
            }
        } catch (error) {
            alert("Error deleting question.");
            console.error(error);
        }
    };


    return {
        category,
        loading,
        error,
        updateProfile,
        saveProfileWithVideo,
        nominationQuestions,
        setNominationQuestions,
        saveNominationQuestions,
        deleteNominationQuestion,
        refetch: fetchProfile,
    };
}
