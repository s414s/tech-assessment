// Bookings are not allowed on Saturdays and Sundays
export function isWeekend(date: Date): boolean {
    const day = date.getDay();
    return day === 0 || day === 6;
}

// Today, or the next Monday when today falls on a weekend (YYYY-MM-DD)
export function firstBookableDate(): string {
    const date = new Date();
    while (isWeekend(date)) date.setDate(date.getDate() + 1);
    return date.toLocaleDateString('en-CA');
}

export function formatDate(date: string): string {
    return new Intl.DateTimeFormat('en-GB', {
        weekday: 'long',
        day: 'numeric',
        month: 'long',
        year: 'numeric',
    }).format(new Date(`${date}T12:00:00`));
}
