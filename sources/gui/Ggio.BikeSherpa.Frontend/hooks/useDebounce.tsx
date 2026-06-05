import { useEffect, useRef } from 'react';

export function useDebounce(callback: () => void, delay: number, deps: unknown[]): void {
    const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

    useEffect(() => {
        if (timerRef.current) clearTimeout(timerRef.current);

        timerRef.current = setTimeout(callback, delay);

        return () => {
            if (timerRef.current) clearTimeout(timerRef.current);
        };
    }, [...deps, callback, delay]);
}