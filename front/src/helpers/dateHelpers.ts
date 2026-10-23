export function toMinutes(time: string): number {
    const [hours = 0, minutes = 0] = time.split(':').map(Number);
    return hours * 60 + minutes;
}

export function customFormatTime(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;

    return `${String(hours).padStart(2, '0')}:${String(mins).padStart(2, '0')}`;
}

export function formatTime(date: string): string {
    return date.slice(11, 16);
}

export function toDate(minutes: number, dateAsString: string): Date {
    const date = new Date(`${dateAsString}T00:00:00`);
    date.setMinutes(minutes);
    return date;
}

export function formatDate(date: string): string {
    return new Intl.DateTimeFormat('en-GB', {
        weekday: 'long',
        day: 'numeric',
        month: 'long',
        year: 'numeric',
    }).format(new Date(`${date}T12:00:00`));
}
