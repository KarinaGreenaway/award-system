import { useState, useRef, useEffect } from "react";
import { useSelectedCategory } from "@/context/CategoryProvider";
import { useNominations } from "@/hooks/useNominations";
import NomineeCard from "@/components/NomineeCard";
import TeamCard from "@/components/TeamCard";
import { CategoryType } from "@/types/enums/CategoryType";
import NominationContextMenu from "@/context/NominationContextMenu";
import { NomineeSummary, Nomination } from "@/types/Nominations";
import Api from "@/api/Api";
import { Button } from "@/components/ui/button";
import { ChevronLeft, ChevronRight, Bot } from "lucide-react";
import { useFeatureFlags } from "@/hooks/useFeatureFlags";

export default function NominationsPage() {
    const { selectedCategoryId } = useSelectedCategory();
    const { category, data, loading, refetch } = useNominations(selectedCategoryId ?? null);
    const [selectedId, setSelectedId] = useState<number | null>(null);
    const [nominations, setNominations] = useState<Nomination[]>([]);
    const [currentNominationIndex, setCurrentNominationIndex] = useState(0);
    const [detailsLoading, setDetailsLoading] = useState(false);
    const [showAISummary, setShowAISummary] = useState(false);
    const { features } = useFeatureFlags();
    const [searchQuery, setSearchQuery] = useState("");
    const [locationFilter, setLocationFilter] = useState<"All" | "UK" | "SA">("All");



    const userRole = localStorage.getItem("mock_role");
    const isAdmin = userRole === "Admin";

    const userId = Number(localStorage.getItem("mock_user_id"));
    const isSponsor = category?.sponsorId === userId;

    const isEditable = isAdmin || isSponsor;


    const [contextMenu, setContextMenu] = useState<{
        x: number;
        y: number;
        item: NomineeSummary | null;
    } | null>(null);

    const menuRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (
                contextMenu &&
                menuRef.current &&
                !menuRef.current.contains(event.target as Node)
            ) {
                closeContextMenu();
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [contextMenu]);

    useEffect(() => {
        // Reset nomination details when category changes
        setSelectedId(null);
        setNominations([]);
        setCurrentNominationIndex(0);
        setDetailsLoading(false);
    }, [selectedCategoryId]);


    const handleRightClick = (e: React.MouseEvent, item: NomineeSummary) => {
        e.preventDefault();
        setContextMenu({ x: e.pageX, y: e.pageY, item });
    };

    const closeContextMenu = () => setContextMenu(null);

    const fetchNominationDetails = async (id: number, teamNominationId: number) => {
        setDetailsLoading(true);
        try {
            if (category?.type === CategoryType.Individual) {
                const nominations = await Api.getNominationsByNomineeId(id);
                setNominations(nominations);
                setCurrentNominationIndex(0);
            } else {
                const nomination = await Api.getTeamNomination(teamNominationId);
                setNominations(nomination ? [nomination] : []);
                setCurrentNominationIndex(0);
            }
        } catch (err) {
            console.error("Failed to fetch nomination details", err);
        } finally {
            setDetailsLoading(false);
        }
    };

    const handleCardClick = async (id: number, teamNominationId: number) => {
        setSelectedId(id);
        await fetchNominationDetails(id, teamNominationId);
    };

    const handlePrevNomination = () => {
        if (currentNominationIndex > 0) {
            setCurrentNominationIndex(currentNominationIndex - 1);
        }
    };

    const handleNextNomination = () => {
        if (currentNominationIndex < nominations.length - 1) {
            setCurrentNominationIndex(currentNominationIndex + 1);
        }
    };

    const currentNomination = nominations[currentNominationIndex];

    if (loading) {
        return (
            <p className="p-8 text-gray-500 dark:text-gray-400">
                Loading nominations...
            </p>
        );
    }

    return (
        <div className="flex flex-col lg:flex-row h-full relative">
            {/* Left panel - Nomination cards */}
            <div className="lg:w-1/2 xl:w-1/2 p-4 overflow-y-auto border-gray-200 dark:border-gray-700">
                <div className="mb-4 space-y-2">
                    <h2 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                        The {category?.name} Nominations
                    </h2>

                    {!isEditable && (
                        <div className="mb-4 p-3 rounded bg-[color:var(--color-brand-hover)] text-white rounded-xl">
                            You are in read-only mode. Only the category sponsor or an admin can manage these nominations.
                        </div>
                    )}

                    <div className="mb-2 flex flex-col sm:flex-row gap-4 mt-6">
                        <input
                            type="text"
                            placeholder="Search by name..."
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-md text-sm w-full sm:w-1/2 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                        />

                        <select
                            value={locationFilter}
                            onChange={(e) => setLocationFilter(e.target.value as "All" | "UK" | "SA")}
                            className="px-2 py-2 border border-gray-300 dark:border-gray-700 rounded-md text-sm bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                        >
                            <option value="All">All Locations</option>
                            <option value="UK">UK</option>
                            <option value="SA">SA</option>
                        </select>
                    </div>
                </div>


                {data.length === 0 && (
                    <div className="p-8 text-gray-500 dark:text-gray-400">
                        No nominations found for this category.
                    </div>
                )}

                <div className="flex flex-col gap-4">
                    {data
                        .filter((n: any) => {
                            const name = category?.type === CategoryType.Individual ? n.nomineeName : n.teamName;
                            const matchesSearch = name?.toLowerCase().includes(searchQuery.toLowerCase());
                            const matchesLocation = locationFilter === "All" || n.location === locationFilter;
                            return matchesSearch && matchesLocation;
                        })
                        .sort((a, b) => {
                            const getScore = (item: any) =>
                                (item.isWinner ? 4 : 0) +
                                (item.isShortlisted ? 2 : 0) +
                                (item.isPinned ? 1 : 0);

                            const scoreComparison = getScore(b) - getScore(a);

                            if (scoreComparison !== 0) {
                                return scoreComparison;
                            }

                            const getTotalNominations = (item: any) => item.totalNominations || 0;

                            return getTotalNominations(b) - getTotalNominations(a);
                        })
                        .map((n: any) => {
                            const id =
                                category?.type === CategoryType.Individual ? n.nomineeId : n.id;

                            return category?.type === CategoryType.Individual ? (
                                <NomineeCard
                                    key={id}
                                    nominee={n}
                                    isSelected={selectedId === id}
                                    onClick={() => handleCardClick(id, n.teamNominationId)}
                                    onRightClick={(e) => handleRightClick(e, n)}
                                />
                            ) : (
                                <TeamCard
                                    key={id}
                                    nomination={n}
                                    isSelected={selectedId === id}
                                    onClick={() => handleCardClick(id, n.teamNominationId)}
                                    onRightClick={(e) => handleRightClick(e, n)}
                                />
                            );
                        })}
                </div>
            </div>

            {/* Right panel - Details */}
            <div className="right-panel">
                {detailsLoading ? (
                    <p className="text-sm text-gray-400">Loading details...</p>
                ) : currentNomination ? (
                    <div className="space-y-6">
                        {/* Header with navigation */}
                        <div className="flex justify-between items-center mb-6">
                            <div>
                                <h2 className="text-xl font-semibold mb-2 dark:text-gray-300">
                                    {category?.type === CategoryType.Individual
                                        ? currentNomination.nomineeId
                                            ? `${currentNomination.nomineeName}`
                                            : "Nominee"
                                        : currentNomination.teamName || "Team"}
                                </h2>
                                <p className="text-sm text-gray-500 dark:text-gray-400">
                                    {currentNomination.location || "Location unknown"}
                                </p>
                            </div>

                            {/* Previous and Next Nomination */}
                            {category?.type === CategoryType.Individual && nominations.length > 1 && (
                                <div className="flex gap-2 ml-auto">
                                    <Button
                                        variant="outline"
                                        className=" text-gray-400 hover:scale-[1.01]"
                                        size="sm"
                                        onClick={handlePrevNomination}
                                        disabled={currentNominationIndex === 0}
                                    >
                                        <ChevronLeft className="h-4 w-4" />
                                    </Button>
                                    <Button
                                        variant="outline"
                                        className="text-gray-400 hover:scale-[1.01]"
                                        size="sm"
                                        onClick={handleNextNomination}
                                        disabled={currentNominationIndex === nominations.length - 1}
                                    >
                                        <ChevronRight className="h-4 w-4" />
                                    </Button>
                                    <span className="text-sm flex items-center px-2 text-gray-400">
                                        {currentNominationIndex + 1} of {nominations.length}
                                    </span>
                                </div>
                            )}

                            {/* AI Summary Button (floating) */}
                            {features.enableAI && currentNomination.aiSummary && (
                                <Button
                                    variant="ghost"
                                    size="lg"
                                    className="bottom-6 shadow-xl right-6 z-20 bg-[color:var(--color-brand)] text-white hover:scale-105 shadow-lg dark:shadow-[0_4px_20px_rgba(255,255,255,0.1)] p-4 rounded-full transition-transform duration-200"
                                    onClick={() => setShowAISummary(prev => !prev)} // toggle instead of just open
                                >
                                    <Bot className="h-8 w-8" />
                                </Button>
                            )}

                        </div>

                        {/* Nomination Details */}
                        <div className="space-y-4">
                            <h3 className="text-lg dark:text-gray-300">Nomination Details</h3>

                            {/* AI Summary Section */}
                            {features.enableAI && showAISummary && currentNomination.aiSummary && (
                                <div className="relative card-interactive-selected bg-white dark:bg-gray-800 p-4 rounded-lg shadow border border-gray-200 dark:border-gray-700">
                                    <div className="flex items-center justify-between mb-2">
                                        <div className="flex items-center gap-2">
                                            <Bot className="h-5 w-5 text-[color:var(--color-brand)]" />
                                            <span className="text-sm font-semibold text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                          AI Summary
                        </span>
                                        </div>
                                    </div>
                                    <p className="text-sm text-gray-700 dark:text-gray-300 whitespace-pre-line">
                                        {currentNomination.aiSummary}
                                    </p>
                                </div>
                            )}

                            {/* Team Members */}
                            {category?.type === CategoryType.Team && currentNomination.teamMembers && currentNomination.teamMembers.length > 0 && (
                                <div className="mt-6">
                                    <h3 className="text-lg mb-2 dark:text-gray-300">Team Members</h3>
                                    <div className="p-2 rounded-lg">
                                        <div className="flex flex-wrap gap-2">
                                            {currentNomination.teamMembers?.map(member => (
                                                <span
                                                    key={member.id}
                                                    className="inline-flex items-center px-3 py-1 rounded-full shadow-md text-sm font-medium bg-[color:var(--color-tabs-light)] dark:bg-[color:var(--color-tabs-dark)] text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] shadow-sm"
                                                >
                                              {member.teamMemberName}
                                            </span>
                                            ))}
                                        </div>

                                    </div>

                                </div>
                            )}

                            {/* Answers */}
                            {currentNomination.answers?.length > 0 ? (
                                currentNomination.answers.map((answer) => (
                                    <div className="bg-white dark:bg-gray-800 p-4 rounded-lg shadow border border-gray-200 dark:border-gray-700">
                                        <h4 className="text-sm font-semibold text-gray-800 dark:text-gray-100 mb-2">
                                            {answer.question}
                                        </h4>
                                        <p className="text-sm text-gray-700 dark:text-gray-300">{answer.answer}</p>
                                    </div>

                                ))
                            ) : (
                                <p className="text-gray-500 dark:text-gray-400">No answers provided</p>
                            )}
                        </div>


                    </div>
                ) : selectedId === null ? (
                    <p className="text-gray-500 dark:text-gray-400">
                        Select a nominee or team to view details
                    </p>
                ) : (
                    <p className="text-sm text-gray-400">
                        Nomination details (coming soon)
                    </p>
                )}
            </div>


            {/* Context Menu */}
            {contextMenu && contextMenu.item && isEditable && (
                <NominationContextMenu
                    ref={menuRef}
                    nomination={contextMenu.item}
                    position={{ x: contextMenu.x, y: contextMenu.y }}
                    onClose={closeContextMenu}
                    onRefresh={refetch}
                />
            )}
        </div>
    );
}