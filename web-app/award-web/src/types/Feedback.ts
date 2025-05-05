export interface FeedbackResponseDto {
    id: number;
    userId: number;
    eventId: number;
    submittedAt: string;
    answers: FeedbackAnswer[];
}

export interface FeedbackAnswer {
    question: string;
    answer: string;
}

