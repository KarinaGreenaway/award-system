import { useEffect, useState } from "react";
import { useAwardEvent } from "@/hooks/useAwardEvent";
import { Button } from "@/components/ui/button";
import Api from "@/api/Api.ts";

export default function AwardEventPage() {
    const { event, loading, error, refetch } = useAwardEvent();

    const [name, setName] = useState("");
    const [location, setLocation] = useState("");
    const [eventDateTime, setEventDateTime] = useState("");
    const [description, setDescription] = useState("");
    const [status, setStatus] = useState<"draft" | "published">("draft");
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        if (event) {
            setName(event.name);
            setLocation(event.location);
            setEventDateTime(formatDatetimeLocal(event.eventDateTime));
            setDescription(event.description ?? "");
            setStatus(event.status ?? "draft");
        }
    }, [event]);

    const handleSave = async () => {
        if (!name || !location || !eventDateTime) {
            alert("Name, location, and event date/time are required.");
            return;
        }

        setIsSaving(true);
        if (!event) {
            alert("No event to update.");
            return;
        }

        try {
            await Api.updateAwardEvent(event.id, {
                name,
                location,
                eventDateTime,
                description,
                status
            });
            alert("Event updated successfully");
            refetch();
        } catch (err) {
            console.error("Failed to update event", err);
            alert("Error saving event.");
        } finally {
            setIsSaving(false);
        }
    };

    const handleStatusChange = (newStatus: boolean) => {
        setStatus(newStatus ? "published" : "draft");
    };

    function formatDatetimeLocal(datetime: string | Date | null): string {
        if (!datetime) return "";
        const dt = typeof datetime === "string" ? new Date(datetime) : datetime;
        const offset = dt.getTimezoneOffset();
        const local = new Date(dt.getTime() - offset * 60000);
        return local.toISOString().slice(0, 16);
    }

    if (loading) return <p className="p-6 text-gray-400">Loading event...</p>;
    if (error) return <p className="p-6 text-[color:var(--color-brand)]">{error}</p>;

    return (
        <div className="flex flex-col lg:flex-row h-full relative">
            {/* Left panel - Event details */}
            <div className="lg:w-1/2 xl:w-1/2 p-6 overflow-y-auto">
                <h1 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] mb-6">
                    The Awards Event
                </h1>

                {/* Name */}
                <div className="mb-6">
                    <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                        Event Name *
                    </label>
                    <input
                        type="text"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        className="w-full rounded-md p-2 text-sm text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
                    />
                </div>

                {/* Location */}
                <div className="mb-6">
                    <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                        Location *
                    </label>
                    <input
                        type="text"
                        value={location}
                        onChange={(e) => setLocation(e.target.value)}
                        className="w-full rounded-md p-2 text-sm text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
                    />
                </div>

                {/* Date & Time */}
                <div className="mb-6">
                    <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                        Date & Time *
                    </label>
                    <input
                        type="datetime-local"
                        value={eventDateTime}
                        onChange={(e) => setEventDateTime(e.target.value)}
                        className="w-full rounded-md p-2 text-sm text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
                    />
                </div>

                {/* Description */}
                <div className="mb-6">
                    <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                        Event Description
                    </label>
                    <textarea
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        rows={4}
                        className="w-full rounded-md p-2 text-sm text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800"
                    />
                </div>

                {/* Status Toggle */}
                <div className="space-y-2 mb-6">
                    <p className="text-sm text-gray-700 dark:text-gray-300">
                        Current Status: <span className="font-medium">{status}</span>
                    </p>
                    <div className="flex items-center gap-3">
                        <span className={`text-sm font-medium ${status === "draft" ? "text-[color:var(--color-brand)]" : "text-gray-500 dark:text-gray-400"}`}>
                            Draft
                        </span>
                        <label className="relative inline-flex items-center cursor-pointer">
                            <input
                                type="checkbox"
                                className="sr-only peer"
                                checked={status === "published"}
                                onChange={(e) => handleStatusChange(e.target.checked)}
                                disabled={isSaving}
                            />
                            <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer dark:bg-gray-700 peer-checked:bg-[color:var(--color-brand)] peer-checked:after:translate-x-full after:content-[''] after:absolute after:left-[2px] after:top-[2px] after:bg-white after:border after:border-gray-300 after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600"></div>
                        </label>
                        <span className={`text-sm font-medium ${status === "published" ? "text-[color:var(--color-brand)]" : "text-gray-500 dark:text-gray-400"}`}>
                            Publish
                        </span>
                    </div>
                </div>

                {/* Save */}
                <div className="pt-4">
                    <Button
                        onClick={handleSave}
                        className="btn-brand"
                        disabled={isSaving}
                    >
                        {isSaving ? "Saving..." : "Save"}
                    </Button>
                </div>
            </div>

            {/* Right panel - Empty for now */}
            <div className="right-panel" />
        </div>
    );
}
