export interface AwardProcess {
    id: number;
    awardsName: string;
    startDate: string;
    endDate: string;
    status: string;
    createdAt: string;
}

export interface CreateAwardProcessPayload {
    awardsName: string;
    startDate: string;
    endDate: string;
    status: string;
}

export interface JudgingRound {
    id: number;
    awardProcessId: number;
    roundName: string;
    startDate: string;
    deadline: string;
    candidateCount: number;
    createdAt: string;
    updatedAt: string;
}

export interface CreateJudgingRoundPayload {
    awardProcessId: number;
    roundName: string;
    startDate: string;
    deadline: string;
    candidateCount: number;
}
