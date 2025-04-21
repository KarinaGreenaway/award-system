import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useEffect, useState } from "react";

interface Metric {
    title: string;
    value: string | number;
    description: string;
}

export default function DashboardPage() {
    const [metrics, setMetrics] = useState<Metric[]>([]);

    useEffect(() => {
        // fake data for now
        setMetrics([
            {
                title: "Total Revenue",
                value: "$45,231.89",
                description: "+20.1% from last month",
            },
            {
                title: "Subscriptions",
                value: "+2,350",
                description: "+180.1% from last month",
            },
            {
                title: "Sales",
                value: "+12,234",
                description: "+19% from last month",
            },
            {
                title: "Active Now",
                value: "+573",
                description: "+201 since last hour",
            },
        ]);
    }, []);

    return (
        <section className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {metrics.map((metric, index) => (
                <Card key={index} className="bg-muted text-muted-foreground">
                    <CardHeader>
                        <CardTitle className="text-base font-medium">
                            {metric.title}
                        </CardTitle>
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold text-foreground">{metric.value}</div>
                        <p className="text-sm text-muted-foreground">{metric.description}</p>
                    </CardContent>
                </Card>
            ))}
        </section>
    );
}
