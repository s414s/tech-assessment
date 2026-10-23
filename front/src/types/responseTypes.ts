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
    locationId: number;
    locationName: string;
    userId: number;
    from: Date;  // Assuming 'from' and 'until' are represented as strings in ISO 8601 format (e.g., "YYYY-MM-DD")
    to: Date;
    itemType: string;
}

export interface UserBooking {
    id: number;
    locationId: number;
    locationName: string;
    itemType: string;
    from: Date;
    to: Date;
}

export interface City {
    id: number;
    name: string;
}

export interface Location {
    id: number;
    name: string;
}

export interface User {
    id: number;
    name: string;
}

export interface ItemType {
    id: number;
    name: string;
}

// Mirrors BookingSystem.Application.Common.PaginatedList<T>
export interface PaginatedList<T> {
    items: T[];
    pageNumber: number;
    totalPages: number;
    totalCount: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}

// GET api/items
export interface GetFilteredItemsResponse {
    items: PaginatedList<ItemDto>;
}

export interface ItemDto {
    id: number;
    name: string;
    type: number; // ItemType enum value, matches ItemType.id from api/items/types
    locationId: number;
    locationName: string;
}

// GET api/bookings/item/{itemId}
export interface GetItemBookingsResponse {
    bookings: BookingDto[];
}

export interface BookingDto {
    from: string; // ISO 8601 date-time
    to: string;
    userId: number;
}

// GET api/bookings/user/{userId}?page=&limit=
export interface GetUsersBookingsResponse {
    bookings: PaginatedList<BookingInfoDto>;
}

export interface BookingInfoDto {
    id: number;
    from: string; // ISO 8601 date-time
    to: string;
    locationId: number;
    locationName: string;
    itemName: string;
}

export interface CreateBookingResponse {
    id: number;
}

// Error body written by BookingSystem.Presentation.Middlewares.GlobalExceptionHandler (RFC 7807)
export interface ProblemDetails {
    type: string;
    title: string;
    status: number;
    detail: string;
    instance: string;
    traceId?: string; // ProblemDetails.Extensions are serialized as top-level properties
}