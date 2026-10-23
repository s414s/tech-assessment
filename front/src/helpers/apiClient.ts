import { apiBaseUrl } from "@/helpers/config";
import type { ProblemDetails } from "@/types/responseTypes";
type QueryParams = { [key: string]: string | number | boolean; };
type BodyParams = { [key: string]: any; };

const baseUrl = apiBaseUrl;

export class ApiError extends Error {
    readonly status: number;
    readonly problem?: ProblemDetails;

    constructor(status: number, message: string, problem?: ProblemDetails) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.problem = problem;
    }
}

// Non-500 errors carry a ProblemDetails body with a user-facing title (e.g. a DomainException message)
async function toApiError(res: Response, url: string): Promise<ApiError> {
    if (res.status !== 500) {
        try {
            const problem: ProblemDetails = await res.json();
            return new ApiError(res.status, problem.title, problem);
        } catch {
            // Body was empty or not JSON; fall through to the generic error
        }
    }
    return new ApiError(res.status, `Failed to fetch ${url}: ${res.statusText}`);
}

export async function GET(url: string) {
    const requestOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
    };
    const res = await fetch(baseUrl + url, requestOptions);
    if (!res.ok) {
        throw await toApiError(res, url);
    }
    return await res.json();
}

export async function POST(url: string, body: BodyParams) {
    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(body)
    };
    const res = await fetch(baseUrl + url, requestOptions);
    if (!res.ok) {
        throw await toApiError(res, url);
    }
    return await res.json();
}

export async function DELETE(url: string, body?: string) {
    const requestOptions = {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
        },
    };
    const res = await fetch(baseUrl + url, requestOptions);
    if (!res.ok) {
        throw await toApiError(res, url);
    }
    return await res.json();
}
