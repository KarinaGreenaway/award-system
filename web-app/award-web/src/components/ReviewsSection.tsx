import React, { useEffect, useState } from 'react';
import ReviewCard from './ReviewCard';

interface Review {
    id: number;
    submittedAt: string;
    questions: { question: string; answer: string }[];
}

const ReviewsSection: React.FC = () => {
    const [reviews, setReviews] = useState<Review[]>([]);
    const [expanded, setExpanded] = useState<boolean>(false);
    const [selectedReviewId, setSelectedReviewId] = useState<number | null>(null);

    useEffect(() => {
        const fetchReviews = async () => {
            const data: Review[] = Array.from({ length: 10 }, (_, i) => ({
                id: i + 1,
                submittedAt: '2025-04-01T10:00:00Z',
                questions: [
                    { question: 'How was the event?', answer: 'Fantastic!' },
                    { question: 'Would you attend again?', answer: 'Absolutely.' },
                ],
            }));
            setReviews(data);
        };

        fetchReviews();
    }, []);

    const displayedReviews = expanded ? reviews : reviews.slice(0, 2);

    return (
        <div className="w-full">
            <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">Reviews</h2>

            {/* Scrollable Review List */}
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

            {/* Expand / Collapse Controls */}
            {reviews.length > 3 && (
                <button
                    onClick={() => setExpanded(prev => !prev)}
                    className="mt-4 font-semibold text-[color:var(--color-brand-hover)] hover:font-bold"
                >
                    {expanded ? 'Show Less Reviews' : 'See All Reviews'}
                </button>
            )}
        </div>
    );
};

export default ReviewsSection;
