export interface AwardEvent {
    id: number;
    awardProcessId: number;
    name: string;
    location: string;
    eventDateTime: string;
    description?: string;
    directions: string;
    feedbackSummary?: string;
    createdAt: string;
    updatedAt: string;
}

export interface CreateAwardEventPayload {
    awardProcessId: number;
    name: string;
    location: string;
    eventDateTime: string;
    description?: string;
    directions: string;
    feedbackSummary?: string;
}

export interface UpdateAwardEventPayload {
    name?: string;
    location?: string;
    eventDateTime?: string;
    description?: string;
    directions?: string;
}
