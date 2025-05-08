import React from 'react';
import {FeedbackResponseDto} from "@/types/Feedback.ts";

interface ReviewCardProps {
    review: FeedbackResponseDto;
    isSelected: boolean;
    onClick: (id: number) => void;
}

const timeAgo = (dateString: string): string => {
    const date = new Date(dateString);
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const minutes = Math.floor(diff / (1000 * 60));
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    const weeks = Math.floor(diff / (1000 * 60 * 60 * 24 * 7));
    const months = Math.floor(diff / (1000 * 60 * 60 * 24 * 30));

    if (months > 0) return `${months} month${months > 1 ? 's' : ''} ago`;
    if (weeks > 0) return `${weeks} week${weeks > 1 ? 's' : ''} ago`;
    if (days > 0) return `${days} day${days > 1 ? 's' : ''} ago`;
    if (hours > 0) return `${hours} hour${hours > 1 ? 's' : ''} ago`;
    return `${minutes} minute${minutes !== 1 ? 's' : ''} ago`;
};

const ReviewCard: React.FC<ReviewCardProps> = ({ review, isSelected, onClick }) => {
    const previewQs = review.answers.slice(0, 2);

    return (
        <div
            className={`w-full p-4 bg-white dark:bg-gray-800 rounded shadow cursor-pointer transition-transform ${
                isSelected ? 'ring-2 ring-[color:var(--color-brand)]' : 'hover:ring-1 ring-gray-300'
            }`}
            onClick={() => onClick(review.id)}
        >
            <p className="text-sm text-gray-500 dark:text-gray-400">
                {timeAgo(review.submittedAt)}
            </p>

            <div className="mt-2 space-y-4">
                {(isSelected ? review.answers : previewQs).map((qa, index) => (
                    <div key={index} className="mb-4">
                        <p className="text-sm font-semibold text-gray-700 dark:text-gray-200 line-clamp-2">
                            {qa.question}
                        </p>
                        <p className="text-sm text-gray-900 dark:text-white line-clamp-2">
                            {qa.answer}
                        </p>
                    </div>
                ))}
                {!isSelected && review.answers.length > 2 && (
                    <p className="text-xs italic text-gray-400 dark:text-gray-500">...more</p>
                )}
            </div>
        </div>
    );
};

export default ReviewCard;
