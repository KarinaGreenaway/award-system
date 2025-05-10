import {QuestionResponseType} from "@/types/enums/QuestionResponseType.ts";

export interface FeedbackQuestionsResponseDto {
    id: number;
    eventId: number;
    questionText: string;
    responseType: QuestionResponseType;
    options?: string[];
    questionOrder: number;
}

export interface FeedbackQuestionUpdatePayload {
    questionText: string;
    responseType: QuestionResponseType;
    options?: string[];
    questionOrder: number;
}

export interface FeedbackQuestionCreatePayload {
    eventId: number;
    questionText: string;
    responseType: QuestionResponseType;
    options?: string[];
    questionOrder: number;
}