import {QuestionResponseType} from "@/types/enums/QuestionResponseType.ts";

export interface NominationQuestionResponseDto {
    id: number;
    categoryId: number;
    questionText: string;
    responseType: QuestionResponseType;
    options?: string[];
    questionOrder: number;
}

export interface NominationQuestionUpdatePayload {
    questionText: string;
    responseType: QuestionResponseType;
    options?: string[];
    questionOrder: number;
}

export interface NominationQuestionCreatePayload {
    categoryId: number;
    questionText: string;
    responseType: QuestionResponseType;
    options?: string[];
    questionOrder: number;
}