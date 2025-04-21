export interface AwardCategoryResponseDto {
  id: number;
  name: string;
  type: "individual" | "team";
  sponsorId: number;
  introductionVideo?: string;
  introductionParagraph?: string;
  profileStatus: "draft" | "published";
  createdAt: string;
  updatedAt: string;
}
