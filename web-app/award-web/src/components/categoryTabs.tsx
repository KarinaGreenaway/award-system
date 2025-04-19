"use client";

import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useState, useEffect } from "react";

type Category = {
    id: number;
    name: string;
};

const mockCategories: Category[] = [
    { id: 1, name: "Category A" },
    { id: 2, name: "Category B" },
    { id: 3, name: "Category C" },
    { id: 4, name: "Category D" },
    { id: 5, name: "Category E" },
    { id: 6, name: "Category F" },
];

export default function CategoryTabs() {
    const [categories, setCategories] = useState<Category[]>([]);

    useEffect(() => {
        setCategories(mockCategories);
    }, []);

    if (categories.length === 0) return null;

    return (
        <Tabs defaultValue={categories[0]?.id.toString()} className="w-full">
            <div className="overflow-x-auto no-scrollbar">
                <TabsList className="flex flex-nowrap gap-2 bg-[color:var(--color-sidebar-light)] dark:bg-[color:var(--color-sidebar-dark)] rounded-lg px-2 py-5 whitespace-nowrap">
                {categories.map((category) => (
                    <TabsTrigger
                        key={category.id}
                        value={category.id.toString()}
                        className="text-[color:var(--color-text-light)] dark:text-[color:var(--color-text-dark)] data-[state=active]:text-white data-[state=active]:bg-[color:var(--color-brand)] hover:bg-gray-200 dark:hover:bg-gray-700 rounded-lg transition-all duration-200 ease-out transform hover:scale-[1.01] active:scale-[0.98] px-5 py-3 whitespace-nowrap"
                    >
                        {category.name}
                    </TabsTrigger>
                ))}
                </TabsList>
            </div>
        </Tabs>
    );
}
