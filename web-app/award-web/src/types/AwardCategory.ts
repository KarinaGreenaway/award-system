import {CategoryType} from "@/types/enums/CategoryType.ts";

export interface AwardCategoryResponseDto {
  id: number;
  name: string;
  type: CategoryType;
  sponsorId: number;
  introductionVideo?: string;
  introductionParagraph?: string;
  profileStatus: "draft" | "published";
  createdAt: string;
  updatedAt: string;
}

export interface AwardCategoryUpdatePayload {
  name: string;
  type: CategoryType;
  sponsorId: number;
  introductionVideo?: string;
  introductionParagraph?: string;
  profileStatus: string;
}
