import React, {useEffect, useState} from 'react';
import {AwardProcess, JudgingRound} from '@/types/AwardProcess';

interface JudgingRoundFormProps {
    awardProcess: AwardProcess;
    onSubmit: (data: Partial<JudgingRound> & { awardProcessId: number }) => void;
    initialData?: Partial<JudgingRound>;
    isEditing?: boolean;
    onClose: () => void;
    previousRounds?: JudgingRound[];
}

const JudgingRoundForm: React.FC<JudgingRoundFormProps> = ({
                                                               awardProcess,
                                                               onSubmit,
                                                               initialData = {},
                                                               isEditing = false,
                                                               onClose,
                                                               previousRounds = []
                                                           }) => {
    const [roundName, setRoundName] = useState(initialData.roundName ?? '');
    const [startDate, setStartDate] = useState(initialData.startDate?.slice(0, 10) ?? '');
    const [deadline, setDeadline] = useState(initialData.deadline?.slice(0, 10) ?? '');
    const [candidateCount, setCandidateCount] = useState(initialData.candidateCount ?? 1);
    const [error, setError] = useState<string | null>(null);

    // Automatically set the start date if this is a new round and there are previous rounds
    useEffect(() => {
        if (!isEditing && previousRounds.length > 0) {
            const lastRound = previousRounds[previousRounds.length - 1];
            setStartDate(new Date(new Date(lastRound.deadline).getTime() + 24 * 60 * 60 * 1000).toISOString().slice(0, 10));
        }
    }, [isEditing, previousRounds]);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        const start = new Date(startDate);
        const end = new Date(deadline);
        const processStart = new Date(awardProcess.startDate);
        const processEnd = new Date(awardProcess.endDate);

        if (start >= end) {
            setError('Start date must be before deadline.');
            return;
        }

        if (start < processStart || end > processEnd) {
            setError(`Judging round must fall within the award process period: ${processStart.toLocaleDateString()} → ${processEnd.toLocaleDateString()}.`);
            return;
        }

        if (previousRounds && previousRounds.length > 1) {
            const lastRound = previousRounds[previousRounds.length - 1];
            if (candidateCount > lastRound.candidateCount) {
                setError('Candidate count must be less than or equal to the previous round.');
                return;
            }
        }

        setError(null);
        onSubmit({
            awardProcessId: awardProcess.id,
            roundName,
            startDate,
            deadline,
            candidateCount
        });
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/20">
            <form onSubmit={handleSubmit} className="bg-white dark:bg-gray-900 p-6 rounded-lg shadow-lg w-full max-w-md space-y-4">
                <h2 className="text-xl font-bold text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                    {isEditing ? 'Edit Judging Round' : 'Create Judging Round'}
                </h2>

                <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Round Name</label>
                    <input
                        type="text"
                        className="w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                        value={roundName}
                        onChange={(e) => setRoundName(e.target.value)}
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
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Deadline</label>
                    <input
                        type="date"
                        className="w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                        value={deadline}
                        onChange={(e) => setDeadline(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Candidate Count</label>
                    <input
                        type="number"
                        min={1}
                        className="w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                        value={candidateCount}
                        onChange={(e) => setCandidateCount(parseInt(e.target.value))}
                        required
                    />
                </div>

                {error && <p className="text-sm text-[color:var(--color-brand)]">{error}</p>}

                <div className="flex justify-end gap-3 pt-4">
                    <button
                        type="button"
                        onClick={onClose}
                        className="btn-brand px-4 py-2 rounded text-sm font-medium"
                    >
                        Cancel
                    </button>
                    <button type="submit" className="btn-brand px-4 py-2 rounded text-sm font-medium">
                        {isEditing ? 'Update Round' : 'Create Round'}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default JudgingRoundForm;
