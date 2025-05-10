import React, { useState } from 'react';
import { AwardProcess, CreateAwardProcessPayload } from '@/types/AwardProcess';

interface AwardProcessFormProps {
    onSubmit: (data: CreateAwardProcessPayload) => void;
    initialData?: AwardProcess;
    isEditing?: boolean;
    onClose: () => void;
}

const AwardProcessForm: React.FC<AwardProcessFormProps> = ({
                                                               onSubmit,
                                                               initialData = {} as Partial<AwardProcess>,
                                                               isEditing = false,
                                                               onClose
                                                           }) => {
    const [awardsName, setAwardsName] = useState(initialData.awardsName ?? '');
    const [startDate, setStartDate] = useState(initialData.startDate?.slice(0, 10) ?? '');
    const [endDate, setEndDate] = useState(initialData.endDate?.slice(0, 10) ?? '');
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        const start = new Date(startDate);
        const end = new Date(endDate);
        const now = new Date();

        if (start >= end) {
            setError('Start date must be before end date.');
            return;
        }

        if (end <= now) {
            setError('End date must be in the future.');
            return;
        }

        setError(null);

        onSubmit({
            awardsName,
            startDate,
            endDate
        });
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-4">
            <h2 className="text-xl font-bold text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] mb-2">
                {isEditing ? 'Edit Award Process' : 'Create Award Process'}
            </h2>

            <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Awards Name</label>
                <input
                    type="text"
                    className="w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                    value={awardsName}
                    onChange={(e) => setAwardsName(e.target.value)}
                    required
                />
            </div>

            <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Start Date</label>
                <input
                    type="date"
                    className="w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                    value={startDate}
                    onChange={(e) => setStartDate(e.target.value)}
                    required
                />
            </div>

            <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">End Date</label>
                <input
                    type="date"
                    className="w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                    value={endDate}
                    onChange={(e) => setEndDate(e.target.value)}
                    required
                />
            </div>

            {error && <p className="text-sm text-red-600">{error}</p>}

            <div className="pt-4 flex justify-end gap-3">
                <button type="button" onClick={onClose} className="btn-secondary px-4 py-2 rounded text-sm font-medium">
                    Cancel
                </button>
                <button
                    type="submit"
                    className="btn-brand px-4 py-2 rounded text-sm font-medium"
                >
                    {isEditing ? 'Update Award Process' : 'Create Award Process'}
                </button>
            </div>
        </form>
    );
};

export default AwardProcessForm;
