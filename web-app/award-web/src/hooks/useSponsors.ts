import { useEffect, useState } from "react";
import Api from "@/api/Api";
import {UserResponseDto} from "@/types/User.ts";


export function useSponsors() {
    const [sponsors, setSponsors] = useState<UserResponseDto[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchSponsors = async () => {
            try {
                const res = await Api.getUsers();
                setSponsors(res);
            } catch (err) {
                console.error("Failed to fetch sponsors", err);
            } finally {
                setLoading(false);
            }
        };
        fetchSponsors();
    }, []);

    return { sponsors, loading };
}
