import type { VDataTable } from "vuetify/components/VDataTable";

export interface UserInfo {
    id: number,
    name: string,
    surname: string,
    role: string,
}

export interface LoginResponse {
    token: string;
}

export interface LoginInfo {
    username: string;
    password: string;
}

export interface Booking {
    id: number;
    location: number;
    from: Date;  // Assuming 'from' and 'until' are represented as strings in ISO 8601 format (e.g., "YYYY-MM-DD")
    to: Date;
    itemType: string;
}

export type ReadonlyHeaders = VDataTable['$props']['headers'];