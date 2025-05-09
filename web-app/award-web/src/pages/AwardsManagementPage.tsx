import { useState } from "react";
import { Button } from "@/components/ui/button";
import { AwardProcess, JudgingRound } from "@/types/AwardProcess";
import { useAwardProcesses } from "@/hooks/useAwardProcesses";
import { useJudgingRounds } from "@/hooks/useJudgingRounds";
import { useCategories } from "@/hooks/useCategories";
import AwardProcessForm from "@/components/AwardProcessForm";
import JudgingRoundForm from "@/components/JudgingRoundForm";
import CategoryForm from "@/components/CategoryForm";
import { AwardCategoryResponseDto } from "@/types/AwardCategory";
import {Card, CardContent} from "@/components/ui/card.tsx";
import { CategoryType } from "@/types/enums/CategoryType";
import {cn} from "@/lib/utils.ts";
import {Plus, Trophy} from "lucide-react";

export default function AwardsManagementPage() {
    const {
        awardProcesses,
        loading: loadingProcesses,
        createAwardProcess,
        updateAwardProcess,
        deleteAwardProcess,
        refetch: refetchProcesses,
    } = useAwardProcesses();

    const [selectedProcessId, setSelectedProcessId] = useState<number | null>(null);
    const {
        judgingRounds,
        loading: loadingRounds,
        createJudgingRound,
        updateJudgingRound,
        deleteJudgingRound,
        refetch: refetchRounds,
    } = useJudgingRounds(selectedProcessId);

    const {
        categories,
        loading: loadingCategories,
        createCategory,
        updateCategory,
        deleteCategory,
    } = useCategories(selectedProcessId);

    const categoryTypeText = {
        [CategoryType.Individual]: "Individual",
        [CategoryType.Team]: "Team"
    };

    const [showProcessForm, setShowProcessForm] = useState(false);
    const [editingProcess, setEditingProcess] = useState<AwardProcess | null>(null);
    const [showRoundForm, setShowRoundForm] = useState(false);
    const [editingRound, setEditingRound] = useState<JudgingRound | null>(null);
    const [showCategories, setShowCategories] = useState(false);
    const [showCategoryForm, setShowCategoryForm] = useState(false);
    const [editingCategory, setEditingCategory] = useState<AwardCategoryResponseDto | null>(null);

    const handleSelectProcess = (id: number) => {
        setSelectedProcessId(id);
    };

    return (
        <div className="flex flex-col lg:flex-row h-full relative">
            {/* Left panel - Award Processes */}
            <div className="lg:w-1/2 p-6 overflow-y-auto dark:border-gray-700">
                <div className="flex justify-between items-center mb-6">
                    <h2 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] mb-6">
                        The Award Processes
                    </h2>
                </div>

                <div className="flex flex-col gap-4">
                    {loadingProcesses ? (
                        <p className="text-sm text-gray-500 dark:text-gray-400">Loading...</p>
                    ) : (
                        awardProcesses
                            .sort((a, b) => new Date(a.endDate).getTime() - new Date(b.endDate).getTime())
                            .map((process) => (
                                <Card
                                    key={process.id}
                                    onClick={() => handleSelectProcess(process.id)}
                                    className={cn(
                                        "card-interactive",
                                        selectedProcessId === process.id && "card-interactive-selected"
                                    )}
                                >
                                    <CardContent className="card-content-row">
                                        {/* Left - Icon and Info */}
                                        <div className="flex items-center gap-4 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                                            <div className="card-icon-wrap text-[color:var(--color-brand)]">
                                                <Trophy className="h-6 w-6 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]" />
                                            </div>
                                            <div>
                                                <div className="text-sm font-semibold">{process.awardsName}</div>
                                                <div className="text-xs text-gray-500 dark:text-gray-400">
                                                    {new Date(process.startDate).toLocaleDateString()} → {new Date(process.endDate).toLocaleDateString()}
                                                </div>

                                                {/* ACTIVE state signifier */}
                                                {new Date(process.endDate) > new Date() &&
                                                new Date() > new Date(process.startDate) ? (
                                                    <div className="flex items-center gap-2 pt-2">
                                                        <span className="px-2 py-1 text-xs font-medium rounded border-2 border-[color:var(--color-brand)] text-[color:var(--color-brand-hover)]">
                                                            ACTIVE
                                                        </span>
                                                    </div>
                                                ) : null}
                                            </div>
                                        </div>
                                        <div className="flex flex-col gap-1">
                                            <Button
                                                size="sm"
                                                onClick={(e) => {
                                                    e.stopPropagation();
                                                    setEditingProcess(process);
                                                    setShowProcessForm(true);
                                                }}
                                                className="btn-brand mb-2 mt-1"
                                            >
                                                Edit
                                            </Button>
                                            <Button
                                                variant="destructive"
                                                size="sm"
                                                onClick={(e) => {
                                                    e.stopPropagation();
                                                    if (confirm("Are you sure you want to delete this award process?")) {
                                                        deleteAwardProcess(process.id);
                                                    }
                                                }}
                                                className="btn-secondary"
                                            >
                                                Delete
                                            </Button>
                                        </div>
                                    </CardContent>
                                </Card>
                            ))
                    )}
                </div>

                {/* New Award Process button below the Award Process cards */}
                <div className="flex justify-end mt-6">
                    <Button
                        className="btn-brand"
                        onClick={() => {
                            setEditingProcess(null);
                            setShowProcessForm(true);
                        }}
                    >
                        <Plus className="h-4 w-4" />New Award Process
                    </Button>
                </div>
            </div>



            {/* Right panel - Judging Rounds/ Categories */}
            <div className="right-panel lg:w-1/2 p-6 overflow-y-auto">
                <div className="flex justify-between items-center mb-6 h-10">
                    <div className="flex items-center gap-2">
                        <h2 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                            {showCategories ? "Categories" : "Judging Rounds"}
                        </h2>
                    </div>

                    {/* Rounds and Categories Switch */}
                    <div className="flex rounded-md overflow-hidden ml-4 justify-end dark:border-gray-700 shadow-md">
                        <button
                            onClick={() => setShowCategories(false)}
                            className={`px-4 py-1 text-sm font-medium border-gray-300 dark:border-gray-700 ${
                                !showCategories
                                    ? "bg-[color:var(--color-brand)] text-white"
                                    : "bg-white dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600"
                            }`}
                        >
                            Rounds
                        </button>
                        <button
                            onClick={() => setShowCategories(true)}
                            className={`px-4 py-1 text-sm font-medium ${
                                showCategories
                                    ? "bg-[color:var(--color-brand)] text-white"
                                    : "bg-white dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600"
                            }`}
                        >
                            Categories
                        </button>
                    </div>
                </div>

                {/* Render categories or rounds based on the state */}
                {!selectedProcessId ? (
                    <p className="text-gray-500 dark:text-gray-400">Select an award process to view its details.</p>
                ) : showCategories ? (
                    loadingCategories ? (
                        <p className="text-sm text-gray-500 dark:text-gray-400">Loading categories...</p>
                    ) : categories.length === 0 ? (
                        <p className="text-gray-400">No categories for this process.</p>
                    ) : (
                        categories
                            .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime())
                            .map((category) => (
                                <div key={category.id} className="mb-4 p-4 space-y-4 shadow-md p-4 rounded-md bg-white dark:bg-gray-800">
                                    <div className="flex justify-between">
                                        <div>
                                            <h4 className="block text-sm font-semibold mb-1 pl-1 pb-1 text-gray-700 dark:text-gray-200">{category.name}</h4>
                                            <p className="text-sm text-gray-500 dark:text-gray-400 pl-1 pb-2 capitalize">
                                                Type: {categoryTypeText[category.type as CategoryType] ?? "Unknown"}
                                            </p>
                                            <p className="text-sm text-gray-500 dark:text-gray-400 pl-1 pb-2 capitalize">
                                                Sponsor: {category.sponsorName}
                                            </p>
                                            {category.introductionParagraph && (
                                                <p className="text-sm pl-1 pb-1 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                                                    {category.introductionParagraph}
                                                </p>
                                            )}
                                        </div>
                                        <div className="flex flex-col gap-1">
                                            <Button
                                                size="sm"
                                                onClick={() => {
                                                    setEditingCategory(category);
                                                    setShowCategoryForm(true);
                                                }}
                                                className="btn-brand mb-2 mt-1"
                                            >
                                                Edit
                                            </Button>
                                            <Button
                                                variant="destructive"
                                                size="sm"
                                                onClick={() => {
                                                    if (confirm("Delete this category?")) {
                                                        deleteCategory(category.id);
                                                    }
                                                }}
                                                className="btn-secondary"
                                            >
                                                Delete
                                            </Button>
                                        </div>
                                    </div>
                                </div>
                            ))
                    )
                ) : loadingRounds ? (
                    <p className="text-sm text-gray-500 dark:text-gray-400">Loading...</p>
                ) : judgingRounds.length === 0 ? (
                    <p className="text-gray-400">No judging rounds for this process.</p>
                ) : (
                    judgingRounds
                        .sort((a, b) => new Date(a.deadline).getTime() - new Date(b.deadline).getTime())
                        .map((round) => (
                        <div key={round.id} className="mb-4 p-4 space-y-4 shadow-md p-4 rounded-md bg-white dark:bg-gray-800">
                            <div className="flex justify-between">
                                <div>
                                    <h4 className="block text-sm font-semibold mb-1 pl-1 pb-1 text-gray-700 dark:text-gray-200">{round.roundName}</h4>
                                    <p className="w-full rounded-md p-2 mb-3 border text-sm border-gray-300 dark:border-gray-600 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                                        {new Date(round.startDate).toLocaleString()} → {new Date(round.deadline).toLocaleString()}
                                    </p>
                                    <p className="block text-sm font-medium mb-1 pl-1 pb-1 text-gray-700 dark:text-gray-200">Candidate Count: {round.candidateCount}</p>
                                </div>
                                <div className="flex flex-col gap-1">
                                    <Button
                                        size="sm"
                                        onClick={() => {
                                            setEditingRound(round);
                                            setShowRoundForm(true);
                                        }}
                                        className="btn-brand mb-2 mt-1"
                                    >
                                        Edit
                                    </Button>
                                    <Button
                                        variant="destructive"
                                        size="sm"
                                        onClick={() => {
                                            if (confirm("Delete this judging round?")) {
                                                deleteJudgingRound(round.id);
                                            }
                                        }}
                                        className="btn-secondary"
                                    >
                                        Delete
                                    </Button>
                                </div>
                            </div>
                        </div>
                    ))
                )}

                <div className="h-10 flex justify-end">
                    {selectedProcessId ? (
                        <Button
                            className="btn-brand"
                            onClick={() => {
                                if (showCategories) {
                                    setEditingCategory(null);
                                    setShowCategoryForm(true);
                                } else {
                                    setEditingRound(null);
                                    setShowRoundForm(true);
                                }
                            }}
                        >
                            {showCategories ? "+ Add Category" : "+ Add Round"}
                        </Button>
                    ) : (
                        <div className="w-[140px] h-10" /> // matches button height preventing jumping
                    )}
                </div>
            </div>



            {/* Modals */}
            {showProcessForm && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/30">
                    <div className="bg-white dark:bg-gray-900 p-6 rounded-lg shadow-md w-full max-w-md">
                        <AwardProcessForm
                            initialData={editingProcess ?? undefined}
                            isEditing={!!editingProcess}
                            onClose={() => {
                                setShowProcessForm(false);
                                setEditingProcess(null);
                            }}
                            onSubmit={async (data) => {
                                if (editingProcess) {
                                    await updateAwardProcess(editingProcess.id, data);
                                } else {
                                    await createAwardProcess(data);
                                }
                                setShowProcessForm(false);
                                refetchProcesses();
                            }}
                        />
                    </div>
                </div>
            )}

            {showRoundForm && selectedProcessId && (
                <JudgingRoundForm
                    previousRounds={judgingRounds}
                    initialData={editingRound ?? undefined}
                    isEditing={!!editingRound}
                    awardProcess={awardProcesses.find((p) => p.id === selectedProcessId)!}
                    onClose={() => {
                        setShowRoundForm(false);
                        setEditingRound(null);
                    }}
                    onSubmit={async (data) => {
                        if (editingRound) {
                            await updateJudgingRound(editingRound.id, data);
                        } else {
                            await createJudgingRound(data);
                        }
                        setShowRoundForm(false);
                        setEditingRound(null);
                        refetchRounds();
                    }}
                />
            )}
            {showCategoryForm && selectedProcessId && (
                <CategoryForm
                    awardProcessId={selectedProcessId}
                    initialData={editingCategory ?? undefined}
                    isEditing={!!editingCategory}
                    onClose={() => {
                        setShowCategoryForm(false);
                        setEditingCategory(null);
                    }}
                    onSubmit={async (data) => {
                        if (editingCategory) {
                            await updateCategory(editingCategory.id, data);
                        } else {
                            await createCategory(data);
                        }
                        setShowCategoryForm(false);
                        setEditingCategory(null);
                    }}
                />
            )}
        </div>
    );
}
