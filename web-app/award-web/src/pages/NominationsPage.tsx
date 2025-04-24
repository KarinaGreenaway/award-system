import { useState } from "react";
import { useSelectedCategory } from "@/context/CategoryProvider";
import { useNominations } from "@/hooks/useNominations";
import NomineeCard from "@/components/NomineeCard";
import TeamCard from "@/components/TeamCard";
import {CategoryType} from "@/types/enums/CategoryType.ts";

export default function NominationsPage() {
    const { selectedCategoryId } = useSelectedCategory();
    const { category, data, loading } = useNominations(selectedCategoryId ?? 0);
    const [selectedId, setSelectedId] = useState<number | null>(null);


    if (loading) return <p className="p-8 text-gray-500 dark:text-gray-400">Loading nominations...</p>;

    return (
        <div className="flex flex-col lg:flex-row h-full">
            <div className="lg:w-1/2 xl:w-2/5 p-4 overflow-y-auto border-r border-gray-200 dark:border-gray-700">
                <h2 className="text-xl font-bold mb-4 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                    {category?.name} Nominations
                </h2>

                <div className="flex flex-col gap-4">
                {category?.type === CategoryType.Individual
                        ? data.map((n: any) => (
                            <NomineeCard
                                key={n.nomineeId}
                                nominee={n}
                                isSelected={selectedId === n.nomineeId}
                                onClick={() => setSelectedId(n.nomineeId)}
                            />
                        ))
                        : data.map((n: any) => (
                            <TeamCard
                                key={n.id}
                                nomination={n}
                                isSelected={selectedId === n.id}
                                onClick={() => setSelectedId(n.id)}
                            />
                        ))}
                </div>
            </div>

            <div className="flex-1 p-6 overflow-y-auto bg-gray-50 dark:bg-gray-900 border-t lg:border-t-0 lg:border-l border-gray-200 dark:border-gray-700">
                {selectedId ? (
                    <p className="text-sm text-gray-400">Nomination details for ID {selectedId} (coming soon)</p>
                ) : (
                    <p className="text-gray-500 dark:text-gray-400">Select a nominee or team to view details.</p>
                )}
            </div>
        </div>
    );
}
