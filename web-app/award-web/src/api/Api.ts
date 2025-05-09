import axios from "axios";
import {AwardCategoryCreatePayload, AwardCategoryResponseDto, AwardCategoryUpdatePayload} from "@/types/AwardCategory";
import {NomineeSummary, Nomination, NomineeSummaryUpdatePayload} from "@/types/Nominations";
import {Announcement, CreateAnnouncementPayload, UpdateAnnouncementPayload} from "@/types/Announcements";
import {AwardEvent, CreateAwardEventPayload, UpdateAwardEventPayload} from "@/types/AwardEvent.ts";
import {
    RsvpFormQuestionCreatePayload,
    RsvpFormQuestionResponseDto,
    RsvpFormQuestionUpdatePayload
} from "@/types/Rsvp.ts";
import {
    NominationQuestionCreatePayload,
    NominationQuestionResponseDto,
    NominationQuestionUpdatePayload
} from "@/types/NominationQuestion.ts";
import {
    FeedbackQuestionCreatePayload,
    FeedbackQuestionsResponseDto,
    FeedbackQuestionUpdatePayload
} from "@/types/FeedbackQuestions.ts";
import {FeedbackResponseDto} from "@/types/Feedback.ts";
import {FeedbackAnalyticsResponseDto} from "@/types/FeedbackAnalytics.ts";
import {AwardProcess, CreateAwardProcessPayload, CreateJudgingRoundPayload, JudgingRound} from "@/types/AwardProcess";
import {UserResponseDto} from "@/types/User.ts";

const BASE_URL = import.meta.env.VITE_API_URL;
console.log(BASE_URL);

const Api = {
    getAwardCategoryById: async (categoryId: number): Promise<AwardCategoryResponseDto> => {
        const url = `${BASE_URL}/api/AwardCategory/${categoryId}`;
        const { data } = await axios.get(url);
        return data;
    },

    getNomineeSummaries: async (categoryId: number): Promise<NomineeSummary[]> => {
        const url = `${BASE_URL}/api/NomineeSummary/category/${categoryId}`;
        const { data } = await axios.get(url);
        return data;
    },

    getTeamNominationsForCategory: async (categoryId: number): Promise<Nomination[]> => {
        const url = `${BASE_URL}/api/Nomination/category/${categoryId}`;
        const { data } = await axios.get(url);
        return data;
    },

    getNominationsByNomineeId: async (nomineeId: number): Promise<Nomination[]> => {
        const url = `${BASE_URL}/api/Nomination/nominee/${nomineeId}`;
        const { data } = await axios.get(url);
        return data;
    },
    getCategories: async (): Promise<AwardCategoryResponseDto[]> => {
        const url = `${BASE_URL}/api/AwardCategory`;
        const { data } = await axios.get(url);
        return data;
    },

    getCategoriesByProcessId: async (processId: number): Promise<AwardCategoryResponseDto[]> => {
        const url = `${BASE_URL}/api/AwardCategory/award-process/${processId}`;
        const { data } = await axios.get(url);
        return data;
    },

    createAwardCategory: async (payload: AwardCategoryCreatePayload): Promise<AwardCategoryResponseDto> => {
        const url = `${BASE_URL}/api/AwardCategory`;
        const { data } = await axios.post(url, payload);
        return data;
    },

    updateAwardCategory: async (id: number, updates: AwardCategoryUpdatePayload) => {
        const url = `${BASE_URL}/api/AwardCategory/${id}`;
        return axios.put(url, updates);
    },

    deleteAwardCategory: async (id: number) => {
        const url = `${BASE_URL}/api/AwardCategory/${id}`;
        return axios.delete(url);
    },

    updateNomineeSummary: async (id: number, updates: NomineeSummaryUpdatePayload) => {
        const url = `${BASE_URL}/api/NomineeSummary/${id}`;
        return axios.put(url, updates);
    },

    getTeamNomination: async (id: number): Promise<Nomination> => {
        const url = `${BASE_URL}/api/Nomination/${id}`;
        const { data } = await axios.get(url);
        return data;
    },

    async getAnnouncementsBySponsor(selectedCategorySponsorId: number): Promise<Announcement[]> {
        const url = `${BASE_URL}/api/Announcement/by-sponsor/${selectedCategorySponsorId}`;
        const { data } = await axios.get(url);
        return data;
    },

    async getAnnouncementById(id: number) {
        const url = `${BASE_URL}/api/Announcement/${id}`;
        const { data } = await axios.get(url);
        return data;
    },

    createAnnouncement: async (payload: CreateAnnouncementPayload): Promise<Announcement> => {
        const url = `${BASE_URL}/api/Announcement`;
        const { data } = await axios.post(url, payload);
        return data;
    },

    updateAnnouncement: async (id: number, payload: UpdateAnnouncementPayload): Promise<Announcement> => {
        const url = `${BASE_URL}/api/Announcement/${id}`;
        const { data } = await axios.put(url, payload);
        return data;
    },

    deleteAnnouncement: async (id: number): Promise<void> => {
        const url = `${BASE_URL}/api/Announcement/${id}`;
        await axios.delete(url);
    },

    uploadVideo: async (file: File): Promise<string> => {
        const url = `${BASE_URL}/api/AwardCategory/upload-video`;
        const formData = new FormData();
        formData.append("video", file);

        const response = await axios.post(url, formData, {
            headers: {
                "Content-Type": "multipart/form-data",
            },
        });

        return response.data.url; // URL returned from backend
    },

    uploadImage: async (file: File): Promise<string> => {
        const url = `${BASE_URL}/api/Announcement/upload-image`;
        const formData = new FormData();
        formData.append('image', file);

        const response = await axios.post(url, formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
        });

        return response.data.url;
    },

    async getActiveAwardProcess() {
        const url = `${BASE_URL}/api/AwardProcess/active`;
        const { data } = await axios.get(url);
        return data;
    },

    getAwardEventByProcessId: async (processId: number) => {
        const url = `${BASE_URL}/api/AwardEvent/awardProcess/${processId}`;
        const { data } = await axios.get(url);
        return data;
    },

    createAwardEvent: async (payload: CreateAwardEventPayload): Promise<AwardEvent> => {
        const url = `${BASE_URL}/api/AwardEvent`;
        const { data } = await axios.post(url, payload);
        return data;
    },

    updateAwardEvent: async (eventId: number, payload: UpdateAwardEventPayload): Promise<AwardEvent> => {
        const url = `${BASE_URL}/api/AwardEvent/${eventId}`;
        const { data } = await axios.put(url, payload);
        return data;
    },

    getRsvpFormQuestions: async (eventId: number): Promise<RsvpFormQuestionResponseDto[]> => {
        const url = `${BASE_URL}/api/Rsvp/${eventId}/questions`;
        const { data } = await axios.get(url);
        return data;
    },

    createRsvpFormQuestion: async (payload: RsvpFormQuestionCreatePayload) => {
        const url = `${BASE_URL}/api/Rsvp/question`;
        const { data } = await axios.post(url, payload);
        return data;
    },

    updateRsvpFormQuestion: async (questionId: number, payload: RsvpFormQuestionUpdatePayload) => {
        const url = `${BASE_URL}/api/Rsvp/question/${questionId}`;
        const { data } = await axios.put(url, payload);
        return data;
    },

    deleteRsvpFormQuestion: async (id: number) => {
        const url = `${BASE_URL}/api/Rsvp/question/${id}`;
        await axios.delete(url);
    },

    getNominationQuestions: async (categoryId: number): Promise<NominationQuestionResponseDto[]> => {
        const url = `${BASE_URL}/api/NominationQuestion/category/${categoryId}`;
        const { data } = await axios.get(url);
        return data;
    },

    createNominationQuestion: async (payload: NominationQuestionCreatePayload) => {
        const url = `${BASE_URL}/api/NominationQuestion`;
        const { data } = await axios.post(url, payload);
        return data;
    },

    updateNominationQuestion: async (questionId: number, payload: NominationQuestionUpdatePayload) => {
        const url = `${BASE_URL}/api/NominationQuestion/${questionId}`;
        const { data } = await axios.put(url, payload);
        return data;
    },

    deleteNominationQuestion: async (id: number) => {
        await axios.delete(`${BASE_URL}/api/NominationQuestion/${id}`);
    },

    getFeedbackFormQuestions: async (eventId: number): Promise<FeedbackQuestionsResponseDto[]> => {
        const url = `${BASE_URL}/api/Feedback/${eventId}/questions`;
        const { data } = await axios.get(url);
        return data;
    },

    createFeedbackFormQuestion: async (payload: FeedbackQuestionCreatePayload) => {
        const url = `${BASE_URL}/api/Feedback/question`;
        const { data } = await axios.post(url, payload);
        return data;
    },

    updateFeedbackFormQuestion: async (id: number, payload: FeedbackQuestionUpdatePayload) => {
        const url = `${BASE_URL}/api/Feedback/question/${id}`;
        const { data } = await axios.put(url, payload);
        return data;
    },

    deleteFeedbackFormQuestion: async (id: number) => {
        const url = `${BASE_URL}/api/Feedback/question/${id}`;
        await axios.delete(url);
    },

    getFeedbackReviews: async (eventId: number): Promise<FeedbackResponseDto[]> => {
        const url = `${BASE_URL}/api/Feedback/${eventId}`;
        const { data } = await axios.get(url);
        return data;
    },

    getFeedbackAnalytics: async (eventId: number): Promise<FeedbackAnalyticsResponseDto> => {
        const url = `${BASE_URL}/api/Feedback/analytics/${eventId}`;
        const { data } = await axios.get(url);
        return data;
    },

    getAwardProcesses: async (): Promise<AwardProcess[]> => {
        const url = `${BASE_URL}/api/AwardProcess`;
        const { data } = await axios.get(url);
        return data;
    },

    createAwardProcess: async (payload: CreateAwardProcessPayload): Promise<AwardProcess> => {
        const url = `${BASE_URL}/api/AwardProcess`;
        const { data } = await axios.post(url, payload);
        return data;
    },

    updateAwardProcess: async (id: number, payload: CreateAwardProcessPayload): Promise<void> => {
        const url = `${BASE_URL}/api/AwardProcess/${id}`;
        await axios.put(url, payload);
    },

    deleteAwardProcess: async (id: number): Promise<void> => {
        const url = `${BASE_URL}/api/AwardProcess/${id}`;
        await axios.delete(url);
    },

    getJudgingRoundsByAwardProcess: async (processId: number): Promise<JudgingRound[]> => {
        const url = `${BASE_URL}/api/JudgingRound/awardprocess/${processId}`;
        const { data } = await axios.get(url);
        return data;
    },

    createJudgingRound: async (payload: CreateJudgingRoundPayload): Promise<JudgingRound> => {
        const url = `${BASE_URL}/api/JudgingRound`;
        const { data } = await axios.post(url, payload);
        return data;
    },

    updateJudgingRound: async (id: number, payload: Omit<CreateJudgingRoundPayload, 'awardProcessId'>): Promise<void> => {
        const url = `${BASE_URL}/api/JudgingRound/${id}`;
        await axios.put(url, payload);
    },

    deleteJudgingRound: async (id: number): Promise<void> => {
        const url = `${BASE_URL}/api/JudgingRound/${id}`;
        await axios.delete(url);
    },

    getUsers: async (): Promise<UserResponseDto[]> => {
        const url = `${BASE_URL}/api/User`;
        const { data } = await axios.get(url);
        return data;
    },

    getFeedbackSummary: async (eventId: number): Promise<string> => {
        const url = `${BASE_URL}/api/Feedback/summary/${eventId}`;
        const { data } = await axios.get(url);
        return data;
    }

};

export default Api;

