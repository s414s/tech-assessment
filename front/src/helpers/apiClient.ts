import { apiBaseUrl } from "@/helpers/config";
type QueryParams = { [key: string]: string | number | boolean; };
type BodyParams = { [key: string]: any; };

const baseUrl = apiBaseUrl;

export async function GET(url: string) {
    const requestOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
    };
    const res = await fetch(baseUrl + url, requestOptions);
    if (!res.ok) {
        throw new Error(`Failed to fetch ${url}: ${res.statusText}`);
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
        throw new Error(`Failed to fetch ${url}: ${res.statusText}`);
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
        throw new Error(`Failed to fetch ${url}: ${res.statusText}`);
    }
    return await res.json();
}