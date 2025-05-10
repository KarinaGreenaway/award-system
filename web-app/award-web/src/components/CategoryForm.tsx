import React, {useEffect, useState} from "react";
import { AwardCategoryCreatePayload, AwardCategoryResponseDto } from "@/types/AwardCategory";
import { useSponsors } from "@/hooks/useSponsors";
import {CategoryType} from "@/types/enums/CategoryType.ts";

interface CategoryFormProps {
    awardProcessId: number;
    initialData?: AwardCategoryResponseDto;
    isEditing?: boolean;
    onSubmit: (data: AwardCategoryCreatePayload) => void;
    onClose: () => void;
}

const CategoryForm: React.FC<CategoryFormProps> = ({
                                                       awardProcessId,
                                                       initialData,
                                                       isEditing = false,
                                                       onSubmit,
                                                       onClose
                                                   }) => {
    const [name, setName] = useState(initialData?.name ?? "");
    const [type, setType] = useState(
        initialData?.type === CategoryType.Team ? "Team" : "Individual"
    );
    const [sponsorId, setSponsorId] = useState(initialData?.sponsorId ?? 0);
    const [introVideo] = useState(initialData?.introductionVideo ?? "");
    const [introParagraph, setIntroParagraph] = useState(initialData?.introductionParagraph ?? "");
    const [profileStatus, setProfileStatus] = useState(initialData?.profileStatus ?? "draft");

    const { sponsors, loading: loadingSponsors } = useSponsors();
    const [sponsorSearch, setSponsorSearch] = useState("");

    useEffect(() => {
        if (initialData && initialData.sponsorName) {
            setSponsorSearch(initialData.sponsorName);
        }
    }, [initialData]);

    useEffect(() => {
        if (sponsorId && sponsors.length > 0) {
            const match = sponsors.find(s => s.id === sponsorId);
            if (match) setSponsorSearch(match.displayName ?? "");
        }
    }, [sponsorId, sponsors]);


    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSubmit({
            name,
            type: type === "Individual" ? "individual" : "team",
            sponsorId,
            awardProcessId,
            introductionVideo: introVideo,
            introductionParagraph: introParagraph,
            profileStatus: profileStatus === "draft" ? "draft" : "published"
        });
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/30">
            <form
                onSubmit={handleSubmit}
                className="bg-white dark:bg-gray-900 p-6 rounded-lg shadow-md w-full max-w-md space-y-4"
            >
                <h2 className="text-xl font-bold text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                    {isEditing ? "Edit Category" : "Create Category"}
                </h2>

                <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Name</label>
                    <input
                        type="text"
                        className="input w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        required
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Type</label>
                    <select
                        value={type}
                        onChange={(e) => setType(e.target.value)}
                        className="input w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                    >
                        <option value="Individual">Individual</option>
                        <option value="Team">Team</option>
                    </select>
                </div>

                <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Sponsor</label>
                    <input
                        type="text"
                        placeholder="Search sponsor..."
                        value={sponsorSearch}
                        onChange={(e) => {
                            const val = e.target.value;
                            setSponsorSearch(val);
                            if (val.trim() === "") setSponsorId(0); // clearing selection if blank
                        }}

                        className="input p-1 mb-1 mr-1 w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                    />
                    {loadingSponsors ? (
                        <div className="px-3 py-2 text-sm text-gray-500 dark:text-gray-400">
                            Loading sponsors...
                        </div>
                    ) : sponsorSearch.trim() !== "" && (
                        <div className="max-h-40 overflow-y-auto mt-1 bg-white dark:text-gray-300 dark:bg-gray-900 border border-gray-300 dark:border-gray-600 rounded-md">
                            {sponsors
                                .filter(s => s.displayName?.toLowerCase().includes(sponsorSearch.toLowerCase()))
                                .map(sponsor => (
                                    <div
                                        key={sponsor.id}
                                        onClick={() => {
                                            setSponsorId(sponsor.id);
                                            setSponsorSearch(sponsor.displayName ?? "");
                                        }}
                                        className={`cursor-pointer px-3 py-2 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 ${
                                            sponsorId === sponsor.id ? "dark:text-gray-300 bg-gray-100 dark:bg-gray-700 font-medium" : ""
                                        }`}
                                    >
                                        {sponsor.displayName}
                                    </div>
                                ))}
                        </div>
                    )}

                    {typeof sponsorId === "number" && sponsorId > 0 && (
                        <p className="text-xs text-gray-500 mt-1">Selected Sponsor ID: {sponsorId}</p>
                    )}

                </div>


                <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Introduction Paragraph</label>
                    <textarea
                        value={introParagraph}
                        onChange={(e) => setIntroParagraph(e.target.value)}
                        className="input w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                        rows={3}
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Profile Status</label>
                    <select
                        value={profileStatus}
                        onChange={(e) => setProfileStatus(e.target.value as "draft" | "published")}
                        className="input w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                    >
                        <option value="draft">Draft</option>
                        <option value="published">Published</option>
                    </select>
                </div>

                <div className="flex justify-end gap-3 pt-4">
                    <button type="button" onClick={onClose} className="btn-secondary px-4 py-2 rounded text-sm font-medium">
                        Cancel
                    </button>
                    <button type="submit" className="btn-brand px-4 py-2 rounded text-sm font-medium">
                        {isEditing ? "Update Category" : "Create Category"}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default CategoryForm;
