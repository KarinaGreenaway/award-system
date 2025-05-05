import React from 'react';
import AnalyticsCards from '@/components/AnalyticsCards';
import AestheticImage from '@/components/Image';
import ReviewsSection from '@/components/ReviewsSection';
import FeedbackAISummary from '@/components/FeedbackAISummary';

const AwardsFeedbackPage: React.FC = () => {
    return (
        <div className="p-6 bg-gray-100 dark:bg-gray-900 min-h-screen rounded-xl">
            <h1 className="text-3xl font-bold text-gray-900 dark:text-white mb-6">Awards Feedback</h1>

            <div className="w-full">
                <AnalyticsCards />
                <AestheticImage />
            </div>

            {/* Flex row Reviews and AI summary */}
            <div className="mt-10 flex flex-col lg:flex-row gap-6">
                <div className="w-full lg:w-2/3">
                    <ReviewsSection />
                </div>

                <div className="w-full lg:w-1/3">
                    <FeedbackAISummary />
                </div>
            </div>
        </div>
    );
};

export default AwardsFeedbackPage;
