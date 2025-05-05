import {useFeedbackAnalytics} from "@/hooks/useFeedbackAnalytics.ts";

const AnalyticsCards: React.FC = () => {
    const {analytics} = useFeedbackAnalytics();

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 border-gray-300">
            <div className="bg-white dark:bg-gray-800 p-4 shadow border border-gray-300 dark:border-gray-600 rounded-tl-xl pl-5">
                <h2 className="text-lg font-semibold text-gray-700 dark:text-gray-200">Total Reviews</h2>
                <p className="text-2xl font-bold text-gray-900 dark:text-white">{analytics?.totalFeedbacks}</p>
            </div>
            <div className="bg-white dark:bg-gray-800 p-4 shadow border border-gray-300 dark:border-gray-600 rounded-tr-xl pl-5">
                <h2 className="text-lg font-semibold text-gray-700 dark:text-gray-200">Response Rate</h2>
                <p className="text-2xl font-bold text-gray-900 dark:text-white">{analytics?.responseRate}%</p>
            </div>
        </div>
    );
};

export default AnalyticsCards;
