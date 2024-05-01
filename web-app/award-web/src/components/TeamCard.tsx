import { cn } from "@/lib/utils";
import { Card, CardContent } from "@/components/ui/card";
import {Users, BarChart2, Crown, Star, Pin} from "lucide-react";
import {NomineeSummary} from "@/types/Nominations.ts";

interface TeamCardProps {
    nomination: NomineeSummary;
    isSelected?: boolean;
    onClick?: () => void;
    onRightClick?: (e: React.MouseEvent) => void;
}

export default function TeamCard({ nomination, isSelected, onClick, onRightClick }: TeamCardProps) {
    return (
        <Card
            onClick={onClick}
            onContextMenu={(e) => {
                e.preventDefault();
                onRightClick?.(e);
            }}
            className={cn(
                "card-interactive",
                isSelected && "card-interactive-selected"
            )}
        >
            <CardContent className="card-content-row">
                {/* Left - Team Info */}
                <div className="flex items-center gap-4 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                    <div className="card-icon-wrap">
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
                    {nomination.isWinner && <Crown className="h-5 w-5 text-[color:var(--color-brand)]" />}
                    {nomination.isShortlisted && <Star className="h-5 w-5 text-gray-400" />}
                    {nomination.isPinned && <Pin className="h-5 w-5 text-gray-400" />}
                    <BarChart2 className="h-4 w-4" />
                </div>
            </CardContent>
        </Card>
    );
}
