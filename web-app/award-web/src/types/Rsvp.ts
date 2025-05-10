import {QuestionResponseType} from "@/types/enums/QuestionResponseType.ts";

export interface RsvpFormQuestionResponseDto {
    id: number;
    eventId: number;
    questionText: string;
    responseType: QuestionResponseType;
    tooltip: string;
    questionOrder: number;
    options?: string[];
}

export interface RsvpFormQuestionUpdatePayload {
    questionText: string;
    responseType: QuestionResponseType;
    tooltip: string;
    questionOrder: number;
    options?: string[];
}

export interface RsvpFormQuestionCreatePayload {
    eventId: number;
    questionText: string;
    responseType: QuestionResponseType;
    tooltip: string;
    questionOrder: number;
    options?: string[];
}