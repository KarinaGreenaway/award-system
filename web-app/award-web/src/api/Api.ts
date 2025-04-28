import axios from "axios";
import { AwardCategoryResponseDto } from "@/types/AwardCategory";
import {NomineeSummary, Nomination, NomineeSummaryUpdatePayload} from "@/types/Nominations";

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

};

export default Api;

