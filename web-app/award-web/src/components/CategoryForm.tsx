import React, { useState } from "react";
import { AwardCategoryCreatePayload, AwardCategoryResponseDto } from "@/types/AwardCategory";

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
    const [type, setType] = useState(initialData?.type ?? "Individual");
    const [sponsorId, setSponsorId] = useState(initialData?.sponsorId ?? 0);
    const [introVideo, setIntroVideo] = useState(initialData?.introductionVideo ?? "");
    const [introParagraph, setIntroParagraph] = useState(initialData?.introductionParagraph ?? "");
    const [profileStatus, setProfileStatus] = useState(initialData?.profileStatus ?? "draft");

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
                    <label className="block text-sm font-medium mb-1">Name</label>
                    <input
                        type="text"
                        className="input"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        required
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium mb-1">Type</label>
                    <select
                        value={type}
                        onChange={(e) => setType(e.target.value)}
                        className="input"
                    >
                        <option value="Individual">Individual</option>
                        <option value="Team">Team</option>
                    </select>
                </div>

                <div>
                    <label className="block text-sm font-medium mb-1">Sponsor ID</label>
                    <input
                        type="number"
                        value={sponsorId}
                        onChange={(e) => setSponsorId(Number(e.target.value))}
                        className="input"
                        required
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium mb-1">Introduction Paragraph</label>
                    <textarea
                        value={introParagraph}
                        onChange={(e) => setIntroParagraph(e.target.value)}
                        className="input"
                        rows={3}
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium mb-1">Profile Status</label>
                    <select
                        value={profileStatus}
                        onChange={(e) => setProfileStatus(e.target.value as "draft" | "published")}
                        className="input"
                    >
                        <option value="draft">Draft</option>
                        <option value="published">Published</option>
                    </select>
                </div>

                <div className="flex justify-end gap-3 pt-4">
                    <button type="button" onClick={onClose} className="btn-secondary px-4 py-2">
                        Cancel
                    </button>
                    <button type="submit" className="btn-brand px-4 py-2">
                        {isEditing ? "Update Category" : "Create Category"}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default CategoryForm;
