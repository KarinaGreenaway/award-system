import {RsvpResponseType} from "@/types/enums/RsvpResponseType.ts";

export interface RsvpFormQuestionResponseDto {
    id: number;
    eventId: number;
    questionText: string;
    responseType: RsvpResponseType;
    tooltip: string;
    questionOrder: number;
    options?: string[];
}

export interface RsvpFormQuestionUpdatePayload {
    questionText: string;
    responseType: RsvpResponseType;
    tooltip: string;
    questionOrder: number;
    options?: string[];
}

export interface RsvpFormQuestionCreatePayload {
    eventId: number;
    questionText: string;
    responseType: RsvpResponseType;
    tooltip: string;
    questionOrder: number;
    options?: string[];
}