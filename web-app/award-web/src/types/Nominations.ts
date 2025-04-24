export interface NomineeSummary {
    id: number;
    nomineeId?: number;
    nomineeName?: string; // will be used for individuals
    teamNominationId?: number;
    teamName?: string; // for teams
    location?: string; // optional
    categoryId: number;
    totalNominations?: number;
    isPinned: boolean;
    isShortlisted: boolean;
    isWinner: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface Nomination {
    id: number;
    creatorId: number;
    categoryId: number;
    nomineeId?: number;
    teamName?: string;
    aiSummary?: string;
    location?: string;
    createdAt: string;
    updatedAt: string;
    answers: NominationAnswer[];
    teamMembers?: TeamMember[];
}

export interface NominationAnswer {
    question: string;
    answer: string;
}

export interface TeamMember {
    id: number;
    nominationId: number;
    userId: number;
}
