import { cn } from "@/lib/utils";
import { Card, CardContent } from "@/components/ui/card";
import { Users, BarChart2 } from "lucide-react";
import {NomineeSummary} from "@/types/Nominations.ts";

interface TeamCardProps {
    nomination: NomineeSummary;
    isSelected?: boolean;
    onClick?: () => void;
}

export default function TeamCard({ nomination, isSelected, onClick }: TeamCardProps) {
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
                {/* Left - Team Info */}
                <div className="flex items-center gap-4 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                    <div className="p-2 bg-[color:var(--color-tabs-light)] dark:bg-[color:var(--color-tabs-dark)] rounded-full">
                        <Users className="h-6 w-6 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]" />
                    </div>
                    <div>
                        <div className="text-sm font-semibold">{nomination.teamName || "Team Name"}</div>
                        <div className="text-xs text-gray-500 dark:text-gray-400">
                            {nomination.location || "Location unknown"}
                        </div>
                    </div>
                </div>

                {/* Right - Stats */}
                <div className="flex items-center gap-2 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                    <span className="text-sm font-medium">{nomination.totalNominations}</span>
                    <BarChart2 className="h-4 w-4" />
                </div>
            </CardContent>
        </Card>
    );
}
