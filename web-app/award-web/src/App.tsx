import { Routes, Route } from "react-router-dom";
import DashboardPage from "@/pages/DashboardPage.tsx";
import AppLayoutWithTabs from "@/layouts/AppLayoutWithTabs.tsx";
import LoginPage from "@/pages/Login.tsx";
import ProtectedRoute from "@/routes/ProtectedRoutes.tsx";
import NominationsPage from "@/pages/NominationsPage.tsx";
import CategoryProfilePage from "@/pages/CategoryProfilePage.tsx";
import AnnouncementsPage from "@/pages/Announcements.tsx";
import AwardEventPage from "@/pages/AwardEventPage.tsx";
import AwardsFeedbackPage from "@/pages/Feedback.tsx";
import AwardsManagementPage from "@/pages/AwardsManagementPage.tsx";

export default function App() {
    return (
        <Routes>

            {/* Public Login Route */}
            <Route path="/login" element={<LoginPage />} />

            {/* Protected Dashboard Routes */}
            <Route element={<ProtectedRoute />}>
                <Route path="/" element={<AppLayoutWithTabs />}>
                    <Route index element={<DashboardPage />} />
                    <Route path="nominations" element={<NominationsPage />} />
                    <Route path="category-profile" element={<CategoryProfilePage />} />
                    <Route path="announcements" element={<AnnouncementsPage />} />
                    <Route path="events" element={<AwardEventPage />} />
                    <Route path="feedback" element={<AwardsFeedbackPage />} />
                    <Route path="awards-management" element={<AwardsManagementPage />} />
                </Route>
            </Route>
        </Routes>
    );
}

