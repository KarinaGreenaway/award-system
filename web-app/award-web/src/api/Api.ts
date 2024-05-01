import axios from "axios";
import {AwardCategoryResponseDto, AwardCategoryUpdatePayload} from "@/types/AwardCategory";
import {NomineeSummary, Nomination, NomineeSummaryUpdatePayload} from "@/types/Nominations";
import {Announcement, CreateAnnouncementPayload, UpdateAnnouncementPayload} from "@/types/Announcements";

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

    updateNomineeSummary: async (id: number, updates: NomineeSummaryUpdatePayload) => {
        const url = `${BASE_URL}/api/NomineeSummary/${id}`;
        return axios.put(url, updates);
    },

    getTeamNomination: async (id: number): Promise<Nomination> => {
        const url = `${BASE_URL}/api/Nomination/${id}`;
        const { data } = await axios.get(url);
        return data;
    },

    updateAwardCategory: async (id: number, updates: AwardCategoryUpdatePayload) => {
        const url = `${BASE_URL}/api/AwardCategory/${id}`;
        return axios.put(url, updates);
    },

    async getAnnouncementsBySponsor(selectedCategorySponsorId: number) {
        const url = `${BASE_URL}/api/Announcement/by-sponsor/${selectedCategorySponsorId}`;
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



};

export default Api;

