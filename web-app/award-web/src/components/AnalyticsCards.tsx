import React, { useEffect, useState } from 'react';

const AnalyticsCards: React.FC = () => {
    const [totalReviews, setTotalReviews] = useState<number>(0);
    const [responseRate, setResponseRate] = useState<number>(0);

    useEffect(() => {
        // Fetch analytics data from FeedbackAnalytics endpoint
        // Replace with actual API call
        const fetchAnalytics = async () => {
            // Example data
            setTotalReviews(120);
            setResponseRate(75);
        };

        fetchAnalytics();
    }, []);

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 border-gray-300">
            <div className="bg-white dark:bg-gray-800 p-4 shadow border border-gray-300 dark:border-gray-600 rounded-tl-xl">
                <h2 className="text-lg font-semibold text-gray-700 dark:text-gray-200">Total Reviews</h2>
                <p className="text-2xl font-bold text-gray-900 dark:text-white">{totalReviews}</p>
            </div>
            <div className="bg-white dark:bg-gray-800 p-4 shadow border border-gray-300 dark:border-gray-600 rounded-tr-xl">
                <h2 className="text-lg font-semibold text-gray-700 dark:text-gray-200">Response Rate</h2>
                <p className="text-2xl font-bold text-gray-900 dark:text-white">{responseRate}%</p>
            </div>
        </div>
    );
};

export default AnalyticsCards;
