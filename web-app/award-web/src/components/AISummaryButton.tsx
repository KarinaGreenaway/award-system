import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Bot } from "lucide-react";
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";

interface AISummaryButtonProps {
    summary: string;
    enabled: boolean;
}

export function AISummaryButton({ summary, enabled }: AISummaryButtonProps) {
    const [open, setOpen] = useState(false);

    if (!enabled || !summary) return null;

    return (
        <>
            <Button
                variant="ghost"
                size="icon"
                className="absolute bottom-4 right-4 rounded-full"
                onClick={() => setOpen(true)}
            >
                <Bot className="h-5 w-5" />
            </Button>
            <Dialog open={open} onOpenChange={setOpen}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>AI Summary</DialogTitle>
                    </DialogHeader>
                    <div className="whitespace-pre-line text-gray-700 dark:text-gray-300">
                        {summary}
                    </div>
                </DialogContent>
            </Dialog>
        </>
    );
}