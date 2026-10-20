import { useUserStore } from "@/stores/userStore";
type QueryParams = { [key: string]: string | number | boolean; };
type BodyParams = { [key: string]: any; };

const baseUrl = 'http://localhost:5093/';

export async function GET(url: string) {
    const { jwt } = useUserStore();
    const requestOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${jwt}`
        },
    };
    const res = await fetch(baseUrl + url, requestOptions);
    if (!res.ok) {
        throw new Error(`Failed to fetch ${url}: ${res.statusText}`);
    }
    return await res.json();
}

export async function POST(url: string, body: BodyParams) {
    const { jwt } = useUserStore();
    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${jwt}`
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
    const { jwt } = useUserStore();
    const requestOptions = {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${jwt}`
        },
    };
    const res = await fetch(baseUrl + url, requestOptions);
    if (!res.ok) {
        throw new Error(`Failed to fetch ${url}: ${res.statusText}`);
    }
    return await res.json();
}