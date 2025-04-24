import { Card, CardContent } from "@/components/ui/card";
import {Star, BarChart2, User, Crown} from "lucide-react";
import { cn } from "@/lib/utils.ts";
import {NomineeSummary} from "@/types/Nominations.ts";

interface NomineeCardProps {
    nominee: NomineeSummary;
    isSelected: boolean;
    onClick: () => void;
}

export default function NomineeCard({ nominee, isSelected, onClick }: NomineeCardProps) {
    return (
        <Card
            onClick={onClick}
            className={cn(
                "cursor-pointer transition-all duration-200 ease-out hover:scale-[1.01] active:scale-[0.98] border border-transparent",
                "hover:border-[color:var(--color-content-light)] dark:hover:border-[color:var(--color-content-dark)]",
                isSelected && "ring-2 ring-[color:var(--color-brand)]"
            )}
        >
            <CardContent className="p-4 flex flex-row items-center justify-between">
                {/* Left - icon and info */}
                <div className="flex items-center gap-4 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                    <div className="p-2 bg-[color:var(--color-tabs-light)] dark:bg-[color:var(--color-tabs-dark)] rounded-full">
                        <User className="h-6 w-6 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]" />
                    </div>
                    <div>
                        <div className="text-sm font-semibold">
                            {nominee.nomineeName}
                        </div>
                        <div className="text-xs text-gray-500 dark:text-gray-400">
                            Department or Role
                        </div>
                    </div>
                </div>

                {/* Right - stats */}
                <div className="flex items-center gap-2">
                    {nominee.isWinner && <Crown className="h-5 w-5 text-[color:var(--color-brand)]" />}
                    {nominee.isShortlisted && <Star className="h-5 w-5 text-gray-400" />}
                    <span className="text-sm font-medium text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                        {nominee.totalNominations}
                    </span>
                    <BarChart2 className="h-4 w-4 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]" />
                </div>

            </CardContent>
        </Card>
    );
}
