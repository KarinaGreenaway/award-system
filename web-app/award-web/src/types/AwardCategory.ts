import {CategoryType} from "@/types/enums/CategoryType.ts";

export interface AwardCategoryResponseDto {
  id: number;
  name: string;
  type: CategoryType;
  sponsorId: number;
  sponsorName: string;
  awardProcessId: number;
  introductionVideo?: string;
  introductionParagraph?: string;
  profileStatus: "draft" | "published";
  createdAt: string;
  updatedAt: string;
}

export interface AwardCategoryUpdatePayload {
  name: string;
  type: string;
  sponsorId: number;
  awardProcessId: number;
  introductionVideo: string | undefined;
  introductionParagraph: string;
  profileStatus: "draft" | "published"
}

export interface AwardCategoryCreatePayload {
  name: string;
  type: string;
  sponsorId: number;
  awardProcessId: number;
  introductionVideo: string | undefined;
  introductionParagraph: string;
  profileStatus: "draft" | "published"
}
