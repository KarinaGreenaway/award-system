import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

export default function LoginPage() {
    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-100">
            <div className="w-full max-w-md bg-white p-8 rounded-xl shadow-md space-y-6">
                <h1 className="text-3xl font-bold text-center">Login</h1>
                <form className="space-y-4">
                    <div>
                        <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                            Email
                        </label>
                        <Input id="email" type="email" placeholder="you@example.com" />
                    </div>
                    <div>
                        <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                            Password
                        </label>
                        <Input id="password" type="password" placeholder="••••••••" />
                    </div>
                    <Button className="w-full" type="submit">
                        Sign In
                    </Button>
                </form>
            </div>
        </div>
    );
}
