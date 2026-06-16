import { throwApiError } from "@/shared/api/apiError";
import { getAccessToken } from "@/shared/auth/tokenStorage";

type JsonRequestOptions = {
  method?: "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
  body?: unknown;
  headers?: HeadersInit;
};

type AuthenticatedRequestOptions = Omit<JsonRequestOptions, "headers"> & {
  headers?: HeadersInit;
};

export async function apiJsonRequest<TResponse>(url: string, options: JsonRequestOptions = {}): Promise<TResponse> {
  const response = await fetch(url, {
    method: options.method ?? "GET",
    headers: {
      "Content-Type": "application/json",
      ...options.headers
    },
    body: options.body !== undefined ? JSON.stringify(options.body) : undefined
  });

  if (!response.ok) {
    await throwApiError(response);
  }

  return response.json();
}

export async function apiAuthenticatedRequest<TResponse>(
  url: string,
  options: AuthenticatedRequestOptions = {}
): Promise<TResponse> {
  const token = getAccessToken();

  if (!token) {
    throw new Error("Access token is missing");
  }

  return apiJsonRequest<TResponse>(url, {
    ...options,
    headers: {
      Authorization: `Bearer ${token}`,
      ...options.headers
    }
  });
}
