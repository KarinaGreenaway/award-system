import React, { useState } from 'react';
import ReviewCard from './ReviewCard';
import { useReviews } from "@/hooks/useReviews.ts";

const ReviewsSection: React.FC = () => {
    const [expanded, setExpanded] = useState<boolean>(false);
    const [selectedReviewId, setSelectedReviewId] = useState<number | null>(null);
    const { reviews, loading, error } = useReviews();

    const displayedReviews = expanded ? reviews : reviews.slice(0, 2);

    return (
        <div className="w-full">
            <h2 className="text-2xl pl-1 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] mb-6">The Reviews</h2>

            {loading && (
                <p className="text-gray-500 dark:text-gray-400 mb-4">Loading reviews...</p>
            )}

            {error && (
                <p className="text-red-500 dark:text-red-400 mb-4">{error}</p>
            )}

            {!loading && !error && (
                <>
                    <div className="space-y-4 max-h-[400px] overflow-y-auto no-scrollbar p-1">
                        {displayedReviews.map((review) => (
                            <ReviewCard
                                key={review.id}
                                review={review}
                                isSelected={selectedReviewId === review.id}
                                onClick={() =>
                                    setSelectedReviewId(prev => (prev === review.id ? null : review.id))
                                }
                            />
                        ))}
                    </div>

                    {reviews.length > 3 && (
                        <button
                            onClick={() => setExpanded(prev => !prev)}
                            className="mt-4 font-semibold text-[color:var(--color-brand-hover)] hover:font-bold"
                        >
                            {expanded ? 'Show Less Reviews' : 'See All Reviews'}
                        </button>
                    )}
                </>
            )}
        </div>
    );
};

export default ReviewsSection;
