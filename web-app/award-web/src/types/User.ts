export interface UserResponseDto {
    id: number;
    workEmail: string;
    role: string;
    createdAt: string;
    updatedAt: string;
    displayName?: string;
    firstName?: string;
    lastName?: string;
}