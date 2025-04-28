import { NomineeSummary } from "@/types/Nominations";
import { forwardRef } from "react";
import Api from "@/api/Api";

export interface NominationContextMenuProps {
    nomination: NomineeSummary;
    position: {
        x: number;
        y: number;
    };
    onClose: () => void;
    onRefresh: () => void;
}

const NominationContextMenu = forwardRef<HTMLDivElement, NominationContextMenuProps>(
    ({ nomination, position, onClose, onRefresh }, ref) => {
        const handleUpdate = async (field: "isPinned" | "isShortlisted" | "isWinner") => {
            if (!nomination?.id) return;

            const updatedPayload = {
                nomineeId: nomination.nomineeId ?? null,
                teamNominationId: nomination.teamNominationId ?? null,
                location: nomination.location,
                categoryId: nomination.categoryId,
                totalNominations: nomination.totalNominations ?? 0,
                isPinned: field === "isPinned" ? !nomination.isPinned : nomination.isPinned,
                isShortlisted: field === "isShortlisted" ? !nomination.isShortlisted : nomination.isShortlisted,
                isWinner: field === "isWinner" ? !nomination.isWinner : nomination.isWinner,
            };

            try {
                await Api.updateNomineeSummary(nomination.id, updatedPayload);
                onClose();
                onRefresh();
            } catch (err) {
                console.error("Failed to update nominee summary", err);
            }
        };

        return (
            <div
                ref={ref}
                className="fixed z-50 bg-white dark:bg-gray-800 shadow-md rounded-md py-2 w-48 text-sm"
                style={{ top: position.y, left: position.x }}
                onContextMenu={(e) => e.preventDefault()}
            >
                <button
                    onClick={() => handleUpdate("isPinned")}
                    className="block w-full text-left px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-700 focus:outline-none"
                >
                    {nomination.isPinned ? "Unpin" : "Pin"}
                </button>
                <button
                    onClick={() => handleUpdate("isShortlisted")}
                    className="block w-full text-left px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-700 focus:outline-none"
                >
                    {nomination.isShortlisted ? "Unmark as Shortlisted" : "Mark as Shortlisted"}
                </button>
                <button
                    onClick={() => handleUpdate("isWinner")}
                    className="block w-full text-left px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-700 focus:outline-none"
                >
                    {nomination.isWinner ? "Unmark as Winner" : "Mark as Winner"}
                </button>
            </div>
        );
    }
);

export default NominationContextMenu;
