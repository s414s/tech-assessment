import { apiBaseUrl } from "@/helpers/config";
import { showError } from "@/helpers/toast";
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

// Non-500 errors carry a ProblemDetails body with a user-facing title (e.g. a DomainException message).
// Every failed request also raises an error toast.
async function toApiError(res: Response, url: string): Promise<ApiError> {
    if (res.status !== 500) {
        try {
            const problem: ProblemDetails = await res.json();

            showError(problem.detail ? `${problem.title}: ${problem.detail}` : problem.title);

            return new ApiError(res.status, problem.title, problem);
        } catch {
            // Body was empty or not JSON; fall through to the generic error
        }
    }
    showError(`Request failed (${res.status} ${res.statusText})`);
    return new ApiError(res.status, `Failed to fetch ${url}: ${res.statusText}`);
}

// Shared by every method so all requests and responses are logged to the console
async function request(method: string, url: string, body?: BodyParams) {
    const requestOptions: RequestInit = {
        method,
        headers: {
            'Content-Type': 'application/json',
        },
        body: body === undefined ? undefined : JSON.stringify(body),
    };

    console.log(`[API] → ${method} ${url}`, body ?? '');

    const res = await fetch(baseUrl + url, requestOptions);
    if (!res.ok) {
        const error = await toApiError(res, url);
        console.error(`[API] ← ${res.status} ${method} ${url}`, error.problem ?? error.message);
        throw error;
    }

    const data = await res.json();
    console.log(`[API] ← ${res.status} ${method} ${url}`, data);
    return data;
}

export async function GET(url: string) {
    return request('GET', url);
}

export async function POST(url: string, body: BodyParams) {
    return request('POST', url, body);
}

export async function DELETE(url: string, body?: string) {
    return request('DELETE', url);
}
