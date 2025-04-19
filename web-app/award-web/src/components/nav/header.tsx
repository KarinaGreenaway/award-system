import {Link} from "react-router-dom";

export function Header() {
    return (
        <header
            className="sticky top-0 z-50 w-full border-b border-border/40 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
            <div className="container flex h-14 max-w-screen-2xl items-center">
                <div className="mr-4 flex">
                    <Link to="/" className="mr-6 flex items-center space-x-2">
                        <span className="font-bold">My App</span>
                    </Link>
                    <nav className="flex items-center space-x-6 text-sm font-medium">
                        <Link
                            to="/about"
                            className="transition-colors hover:text-foreground/80 text-foreground"
                        >
                            About
                        </Link>
                        <Link
                            to="/products"
                            className="transition-colors hover:text-foreground/80 text-foreground"
                        >
                            Products
                        </Link>
                        <Link
                            to="/contact"
                            className="transition-colors hover:text-foreground/80 text-foreground"
                        >
                            Contact
                        </Link>
                    </nav>
                </div>
            </div>
        </header>
    );
}
