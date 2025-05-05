import React, { useState, useEffect } from 'react';
import { Bot } from 'lucide-react';
import Api from '@/api/Api';
import { Button } from './ui/button';

const FeedbackAISummary: React.FC = () => {
    const [showSummary, setShowSummary] = useState(false);
    const [summary, setSummary] = useState<string | null>(null);
    const [loadingSummary, setLoadingSummary] = useState(false);

    useEffect(() => {
        if (showSummary && !summary) {
            const fetchSummary = async () => {
                try {
                    setLoadingSummary(true);
                    // Replace with real API call
                    const result = {
                        summary: "This event was praised for its organization and atmosphere. Attendees appreciated the clear communication and timely schedule.This event was praised for its organization and atmosphere. Attendees appreciated the clear communication and timely schedule.This event was praised for its organization and atmosphere. Attendees appreciated the clear communication and timely schedule."
                    };
                    setSummary(result.summary);
                } catch (error) {
                    console.error('Failed to fetch feedback summary', error);
                    setSummary('Unable to load AI summary at this time.');
                } finally {
                    setLoadingSummary(false);
                }
            };
            fetchSummary();
        }
    }, [showSummary, summary]);

    return (
        <div className="w-full">
            {/* Bot button */}
            <div className="flex justify-end mb-4">
                <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => setShowSummary(prev => !prev)}
                    className="bg-[color:var(--color-brand)] text-white hover:scale-105 shadow-md dark:shadow-[0_4px_20px_rgba(255,255,255,0.05)] p-4 rounded-full transition-transform"
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
                    {loadingSummary ? (
                        <p className="text-sm text-gray-500 dark:text-gray-400">Loading summary...</p>
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
