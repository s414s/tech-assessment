export function toMinutes(time: string): number {
    const [hours = 0, minutes = 0] = time.split(':').map(Number);
    return hours * 60 + minutes;
}

export function formatTime(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;

    return `${String(hours).padStart(2, '0')}:${String(mins).padStart(2, '0')}`;
}

export function toDate(minutes: number, dateAsString: string): Date {
    const date = new Date(`${dateAsString}T00:00:00`);
    date.setMinutes(minutes);
    return date;
}

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
