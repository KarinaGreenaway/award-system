import { Card, CardContent } from "@/components/ui/card";
import {Star, BarChart2, User, Crown, Pin} from "lucide-react";
import { cn } from "@/lib/utils.ts";
import {NomineeSummary} from "@/types/Nominations.ts";

interface NomineeCardProps {
    nominee: NomineeSummary;
    isSelected: boolean;
    onClick: () => void;
    onRightClick?: (e: React.MouseEvent) => void;
}

export default function NomineeCard({ nominee, isSelected, onClick, onRightClick }: NomineeCardProps) {
    return (
        <Card
            onClick={onClick}
            onContextMenu={(e) => {
                e.preventDefault();
                onRightClick?.(e);
            }}
            className={cn(
                "card-interactive",
                isSelected && "card-interactive-selected",
            )}
        >
            <CardContent className="card-content-row">
                {/* Left - icon and info */}
                <div className="flex items-center gap-4 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                    <div className="card-icon-wrap">
                        <User className="h-6 w-6 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]" />
                    </div>
                    <div>
                        <div className="text-sm font-semibold">
                            {nominee.nomineeName}
                        </div>
                        <div className="text-xs text-gray-500 dark:text-gray-400">
                            {nominee.location || "Location unknown"}
                        </div>
                    </div>
                </div>

                {/* Right - stats */}
                <div className="flex items-center gap-2">
                    {nominee.isWinner && <Crown className="h-5 w-5 text-[color:var(--color-brand)]" />}
                    {nominee.isShortlisted && <Star className="h-5 w-5 text-gray-400" />}
                    {nominee.isPinned && <Pin className="h-5 w-5 text-gray-400" />}
                    <span className="text-sm font-medium text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                        {nominee.totalNominations}
                    </span>
                    <BarChart2 className="h-4 w-4 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]" />
                </div>

            </CardContent>
        </Card>
    );
}
