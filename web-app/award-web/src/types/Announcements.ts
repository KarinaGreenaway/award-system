import {TargetAudience} from "@/types/enums/TargetAudience.ts";

export interface Announcement {
    id: number;
    title: string;
    description: string;
    imageUrl?: string;
    isPushNotification: boolean;
    scheduledTime?: string;
    status: "draft" | "published";
    audience: TargetAudience;
    createdBy: number;
    createdAt: string;
    updatedAt: string;
}

export interface CreateAnnouncementPayload {
    title: string;
    description: string;
    imageFile?: File;
    isPushNotification: boolean;
    scheduledTime?: string;
    status: "draft" | "published";
    audience: TargetAudience;
}

export interface UpdateAnnouncementPayload {
    title?: string;
    description?: string;
    imageFile?: File;
    isPushNotification?: boolean;
    scheduledTime?: string;
    status?: "draft" | "published";
    audience?: TargetAudience;
}