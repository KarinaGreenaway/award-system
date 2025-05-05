import React, { useState } from 'react';
import { Bot } from 'lucide-react';
import { Button } from './ui/button';
import { useFeedbackAISummary } from "@/hooks/useFeedbackAISummary.ts";

const FeedbackAISummary: React.FC = () => {
    const [showSummary, setShowSummary] = useState(false);
    const { summary, loading, error } = useFeedbackAISummary();

    return (
        <div className="w-full">
            {/* Bot button */}
            <div className="flex justify-end mb-4">
                <Button
                    variant="ghost"
                    size="lg"
                    onClick={() => setShowSummary(prev => !prev)}
                    className="bg-[color:var(--color-brand)] shadow-xl text-white hover:scale-105 dark:shadow-[0_4px_20px_rgba(255,255,255,0.05)] p-4 rounded-full transition-transform"
                >
                    <Bot className="h-6 w-6" />
                </Button>
            </div>

            {/* AI Summary panel */}
            {showSummary && (
                <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 p-4 rounded-lg shadow-md">
                    <div className="flex items-center justify-between mb-2">
                        <div className="flex items-center gap-2">
                            <Bot className="h-5 w-5 text-[color:var(--color-brand)]" />
                            <span className="text-sm font-semibold text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                                AI Summary
                            </span>
                        </div>
                    </div>
                    {loading ? (
                        <p className="text-sm text-gray-500 dark:text-gray-400">Loading summary...</p>
                    ) : error ? (
                        <p className="text-sm text-red-500 dark:text-red-400">{error}</p>
                    ) : (
                        <p className="text-sm text-gray-700 dark:text-gray-300 whitespace-pre-line">
                            {summary}
                        </p>
                    )}
                </div>
            )}
        </div>
    );
};

export default FeedbackAISummary;
