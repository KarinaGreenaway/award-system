import {useEffect, useRef, useState} from "react";
import {useSelectedCategory} from "@/context/CategoryProvider";
import {Button} from "@/components/ui/button";
import {Bell, Plus} from "lucide-react";
import Api from "@/api/Api";
import {TargetAudience} from "@/types/enums/TargetAudience.ts";
import {Announcement, CreateAnnouncementPayload} from "@/types/Announcements.ts";

export default function AnnouncementsPage() {
    const { selectedCategoryId } = useSelectedCategory();
    const [announcements, setAnnouncements] = useState<any[]>([]);
    const [selectedAnnouncement, setSelectedAnnouncement] = useState<any | null>(null);
    const [isCreating, setIsCreating] = useState(false);
    const [loading, setLoading] = useState(true);
    const [searchQuery, setSearchQuery] = useState("");
    const [contextMenu, setContextMenu] = useState<{
        x: number;
        y: number;
        item: any | null;
    } | null>(null);
    const menuRef = useRef<HTMLDivElement | null>(null);

    // Form state
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [image, setImage] = useState<File | null>(null);
    const [isPushNotification, setIsPushNotification] = useState(false);
    const [isScheduled, setIsScheduled] = useState(false);
    const [scheduledTime, setScheduledTime] = useState("");
    const [status, setStatus] = useState<"draft" | "published">("draft");
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        const fetchAnnouncements = async () => {
            if (!selectedCategoryId) return;

            setLoading(true);
            try {
                const data = await Api.getAnnouncementsBySponsor(selectedCategoryId);
                setAnnouncements(data);
            } catch (err) {
                console.error("Failed to fetch announcements", err);
            } finally {
                setLoading(false);
            }
        };

        fetchAnnouncements();
    }, [selectedCategoryId]);

    useEffect(() => {
        if (selectedAnnouncement && !isCreating) {
            setTitle(selectedAnnouncement.title);
            setDescription(selectedAnnouncement.description);
            setIsPushNotification(selectedAnnouncement.isPushNotification);
            setIsScheduled(!!selectedAnnouncement.scheduledTime);
            setScheduledTime(selectedAnnouncement.scheduledTime || "");
            setStatus(selectedAnnouncement.status || "draft");
        }
    }, [selectedAnnouncement, isCreating]);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (contextMenu && menuRef.current && !menuRef.current.contains(event.target as Node)) {
                closeContextMenu();
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [contextMenu]);

    const handleRightClick = (e: React.MouseEvent, item: any) => {
        e.preventDefault();
        setContextMenu({ x: e.pageX, y: e.pageY, item });
    };

    const closeContextMenu = () => setContextMenu(null);

    const handleAnnouncementClick = (announcement: any) => {
        setSelectedAnnouncement(announcement);
        setIsCreating(false);
    };

    const handleNewAnnouncement = () => {
        setIsCreating(true);
        setSelectedAnnouncement(null);
        // Reset form
        setTitle("");
        setDescription("");
        setImage(null);
        setIsPushNotification(false);
        setIsScheduled(false);
        setScheduledTime("");
        setStatus("draft");
    };

    const handleSave = async () => {
        if (!title || !description || !selectedCategoryId) {
            alert("Please fill in all required fields");
            return;
        }

        if (status === "published" && description.trim() === "") {
            alert("To publish, please write your announcement description.");
            return;
        }

        setIsSaving(true);

        try {
            let imageUrl = selectedAnnouncement?.imageUrl || '';

            // Upload new image if one was selected
            if (image) {
                try {
                    imageUrl = await Api.uploadImage(image);
                } catch (err) {
                    console.error("Image upload failed", err);
                    alert("Image upload failed. Please try again.");
                    return;
                }
            }

            const payload = {
                title,
                description,
                imageUrl,
                isPushNotification,
                scheduledTime: isScheduled ? scheduledTime : null,
                status,
                audience: TargetAudience.MobileUsers,
            } as CreateAnnouncementPayload;

            let response: Announcement | null = null;
            if (isCreating) {
                response = await Api.createAnnouncement(payload);
                setAnnouncements(prev => [response!, ...prev]);
            } else if (selectedAnnouncement) {
                response = await Api.updateAnnouncement(selectedAnnouncement.id, payload);
                setAnnouncements(prev => prev.map(a => a.id === response!.id ? response! : a));
            }

            setIsCreating(false);
            setSelectedAnnouncement(response);
        } catch (err) {
            console.error("Failed to save announcement", err);
            alert("Failed to save announcement. Please try again.");
        } finally {
            setIsSaving(false);
        }
    };
    const handleDeleteAnnouncement = async () => {
        if (!selectedAnnouncement) return;

        try {
            await Api.deleteAnnouncement(selectedAnnouncement.id);
            setAnnouncements(prev => prev.filter(a => a.id !== selectedAnnouncement.id));
            setSelectedAnnouncement(null);
        } catch (err) {
            console.error("Failed to delete announcement", err);
        }
    };

    const handleStatusChange = (newStatus: boolean) => {
        setStatus(newStatus ? "published" : "draft");
    };

    if (loading) {
        return (
            <p className="p-8 text-gray-500 dark:text-gray-400">
                Loading announcements...
            </p>
        );
    }

    return (
        <div className="flex flex-col lg:flex-row h-full relative">
            {/* Left panel - Announcement cards */}
            <div className="lg:w-1/2 xl:w-2/5 p-4 overflow-y-auto border-gray-200 dark:border-gray-700">
                <div className="mb-4 space-y-2">
                    <h2 className="text-2xl text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] mb-6">
                        The Announcements
                    </h2>

                    <Button
                        onClick={handleNewAnnouncement}
                        className="mb-4 bg-[color:var(--color-brand)] text-white hover:bg-[color:var(--color-brand-hover)]"
                    >
                        <Plus className="h-4 w-4 mr-2" />
                        New Announcement
                    </Button>

                    <input
                        type="text"
                        placeholder="Search announcements..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-md text-sm w-full bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                    />
                </div>

                <div className="flex flex-col gap-4">
                    {announcements
                        .filter(announcement =>
                            announcement.title.toLowerCase().includes(searchQuery.toLowerCase())
                        )
                        .map(announcement => (
                            <div
                                key={announcement.id}
                                className={`relative p-4 rounded-lg border ${announcement.status === 'draft'
                                    ? 'border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-800 shadow-inner'
                                    : 'border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 shadow-sm'} 
                                    cursor-pointer hover:shadow-md transition-shadow`}
                                onClick={() => handleAnnouncementClick(announcement)}
                                onContextMenu={(e) => handleRightClick(e, announcement)}
                            >
                                <div className="flex items-start gap-3">
                                    <div className="p-2 rounded-full bg-[color:var(--color-brand-light)] dark:bg-[color:var(--color-brand-dark)] text-[color:var(--color-brand)]">
                                        <Bell className="h-5 w-5" />
                                    </div>
                                    <div className="flex-1">
                                        <h3 className="font-medium text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                                            {announcement.title}
                                        </h3>
                                        <p className="text-sm text-gray-600 dark:text-gray-400 mt-1">
                                            {announcement.description.length > 100
                                                ? `${announcement.description.substring(0, 100)}...`
                                                : announcement.description}
                                        </p>
                                    </div>
                                </div>
                                {announcement.status === 'draft' && (
                                    <span className="absolute bottom-2 right-2 px-2 py-1 text-xs font-medium rounded bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300">
                                        DRAFT
                                    </span>
                                )}
                            </div>
                        ))}
                </div>
            </div>

            {/* Right panel - Details/Edit Form */}
            <div className="lg:w-1/2 xl:w-3/5 flex-1 p-6 overflow-y-auto bg-gray-50 dark:bg-gray-900 rounded-md relative">
                {isCreating || selectedAnnouncement ? (
                    <div className="space-y-6">
                        <h2 className="text-xl font-semibold text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]">
                            {isCreating ? "Create New Announcement" : "Edit Announcement"}
                        </h2>

                        {/* Title */}
                        <div>
                            <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                                Title *
                            </label>
                            <input
                                type="text"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                className="w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                placeholder="Enter announcement title"
                            />
                        </div>

                        {/* Description */}
                        <div>
                            <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                                Description *
                            </label>
                            <textarea
                                rows={4}
                                value={description}
                                onChange={(e) => setDescription(e.target.value)}
                                className="w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                placeholder="Enter announcement description"
                            />
                        </div>

                        {/* Image Upload */}
                        {/* Image Upload */}
                        <div>
                            <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                                Image
                            </label>
                            <div className="flex items-center gap-4">
                                <label className="cursor-pointer">
                                    <span className="sr-only">Choose image</span>
                                    <input
                                        type="file"
                                        accept="image/*"
                                        onChange={(e) => setImage(e.target.files?.[0] || null)}
                                        className="hidden"
                                        id="announcement-image-upload"
                                    />
                                    <Button
                                        type="button"
                                        variant="outline"
                                        className="border-[color:var(--color-brand)] text-[color:var(--color-brand)] hover:bg-[color:var(--color-brand-light)]"
                                        onClick={() => document.getElementById('announcement-image-upload')?.click()}
                                    >
                                        Choose Image
                                    </Button>
                                </label>
                                {image && (
                                    <span className="text-sm text-gray-600 dark:text-gray-400">
                {image.name}
            </span>
                                )}
                            </div>

                            {/* Image Previews */}
                            <div className="mt-4 space-y-4">
                                {/* Current Image Preview */}
                                {selectedAnnouncement?.imageUrl && !image && (
                                    <div>
                                        <p className="text-sm text-gray-500 mb-2">Current Image:</p>
                                        <div className="relative">
                                            <img
                                                src={selectedAnnouncement.imageUrl}
                                                alt="Current announcement"
                                                className="max-w-full h-auto max-h-48 rounded-md border dark:border-gray-700"
                                            />
                                            <button
                                                onClick={() => {
                                                    // This would need API support to clear the image
                                                    setImage(null);
                                                    // You might want to add a API call to clear the image
                                                }}
                                                className="absolute top-2 right-2 p-1 bg-white/80 dark:bg-gray-800/80 rounded-full hover:bg-white dark:hover:bg-gray-700"
                                                title="Remove image"
                                            >
                                            </button>
                                        </div>
                                    </div>
                                )}

                                {/* New Image Preview */}
                                {image && (
                                    <div>
                                        <div className="flex items-center justify-between mb-2">
                                            <p className="text-sm text-gray-500">Preview:</p>
                                            <button
                                                onClick={() => setImage(null)}
                                                className="text-xs px-2 py-1 rounded btn-brand"
                                            >
                                                Remove
                                            </button>
                                        </div>
                                        <img
                                            src={URL.createObjectURL(image)}
                                            alt="Preview"
                                            className="max-w-full h-auto max-h-48 rounded-md border dark:border-gray-700"
                                        />
                                    </div>
                                )}
                            </div>
                        </div>

                        {/* Toggles */}
                        <div className="space-y-4">
                            <div className="flex items-center gap-4">
                                <label className="inline-flex items-center cursor-pointer">
                                    <input
                                        type="checkbox"
                                        checked={isPushNotification}
                                        onChange={(e) => setIsPushNotification(e.target.checked)}
                                        className="sr-only peer"
                                    />
                                    <div className="relative w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer dark:bg-gray-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600 peer-checked:bg-[color:var(--color-brand)]"></div>
                                    <span className="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">
                                        Push Notification
                                    </span>
                                </label>

                                <label className="inline-flex items-center cursor-pointer">
                                    <input
                                        type="checkbox"
                                        checked={isScheduled}
                                        onChange={(e) => setIsScheduled(e.target.checked)}
                                        className="sr-only peer"
                                    />
                                    <div className="relative w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer dark:bg-gray-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600 peer-checked:bg-[color:var(--color-brand)]"></div>
                                    <span className="ml-3 text-sm font-medium text-gray-700 dark:text-gray-300">
                                        Schedule
                                    </span>
                                </label>
                            </div>

                            {/* Scheduled Date/Time */}
                            {isScheduled && (
                                <div>
                                    <label className="block mb-2 text-sm font-medium text-gray-700 dark:text-gray-300">
                                        Scheduled Date & Time
                                    </label>
                                    <input
                                        type="datetime-local"
                                        value={scheduledTime}
                                        onChange={(e) => setScheduledTime(e.target.value)}
                                        className="w-full rounded-md p-2 text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)]"
                                    />
                                </div>
                            )}

                            {/* Status Toggle */}
                            <div className="space-y-2">
                                <p className="text-sm text-gray-700 dark:text-gray-300">
                                    Current Status:{" "}
                                    <span className="font-medium">
                                        {status}
                                    </span>
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
                                        />
                                        <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer dark:bg-gray-700 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600 peer-checked:bg-[color:var(--color-brand)]"></div>
                                    </label>
                                    <span className={`text-sm font-medium ${status === "published" ? "text-[color:var(--color-brand)]" : "text-gray-500 dark:text-gray-400"}`}>
                                        Publish
                                    </span>
                                </div>
                            </div>
                        </div>

                        {/* Actions */}
                        <div className="flex gap-4 pt-4">
                            <Button
                                onClick={handleSave}
                                className="btn-brand"
                                disabled={isSaving}
                            >
                                {isSaving ? "Saving..." : "Save"}
                            </Button>
                            {!isCreating && (
                                <Button
                                    variant="destructive"
                                    onClick={handleDeleteAnnouncement}
                                    className="btn-brand"
                                >
                                    Delete
                                </Button>
                            )}
                        </div>
                    </div>
                ) : (
                    <div className="flex items-center justify-center h-full">
                        <p className="text-gray-500 dark:text-gray-400">
                            {announcements.length === 0
                                ? "No announcements yet. Create your first one!"
                                : "Select an announcement to view and edit"}
                        </p>
                    </div>
                )}
            </div>

            {/* Context Menu */}
            {contextMenu && contextMenu.item && (
                <div
                    ref={menuRef}
                    className="fixed z-50 w-52 py-1 bg-white dark:bg-gray-800 rounded-md shadow-lg border border-gray-200 dark:border-gray-700"
                    style={{ top: contextMenu.y, left: contextMenu.x }}
                >
                    <button
                        className="w-full text-left px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
                        onClick={() => {
                            handleAnnouncementClick(contextMenu.item);
                            closeContextMenu();
                        }}
                    >
                        View Details
                    </button>
                    <button
                        className="w-full text-left px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
                        onClick={closeContextMenu}
                    >
                        Delete
                    </button>
                </div>
            )}
        </div>
    );
}