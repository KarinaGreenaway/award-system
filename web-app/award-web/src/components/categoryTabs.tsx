import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useCategories } from "@/hooks/useCategories";
import {useSelectedCategory} from "@/context/CategoryProvider.tsx";

export default function CategoryTabs() {
    const { categories, loading } = useCategories();
    const { selectedCategoryId, setSelectedCategoryId } = useSelectedCategory();

    const userId = Number(localStorage.getItem("mock_user_id"));
    const defaultCategoryId =
        categories.find((category) => category.sponsorId === userId)?.id?.toString() ?? categories[0]?.id?.toString();

    if (loading || !Array.isArray(categories) ||categories.length === 0) return null;

    return (
        <Tabs
            value={
                selectedCategoryId !== null
                    ? selectedCategoryId.toString()
                    : defaultCategoryId
            }
            onValueChange={(val) => setSelectedCategoryId(parseInt(val))}
            className="w-full"
        >
            <div className="overflow-x-auto w-full no-scrollbar">
                <TabsList className="flex gap-2 px-2 py-5 whitespace-nowrap flex-nowrap">
                    {categories.map((category) => (
                        <TabsTrigger
                            key={category.id}
                            value={category.id.toString()}
                            className="px-5 py-3 rounded-lg whitespace-nowrap transition-all duration-200 ease-out transform hover:scale-[1.01] active:scale-[0.98] text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] data-[state=active]:text-white data-[state=active]:bg-[color:var(--color-brand)] hover:bg-gray-200 dark:hover:bg-gray-700 data-[state=active]:hover:bg-[color:var(--color-brand)]"
                        >
                            {category.name}
                        </TabsTrigger>
                    ))}
                </TabsList>
            </div>
        </Tabs>
    );
}
