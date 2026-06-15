import { throwApiError } from "@/shared/api/apiError";
import { API_BASE_URLS } from "@/shared/api/httpClient";
import { getAccessToken } from "@/shared/auth/tokenStorage";

type StartEmailAuthRequest = {
  email: string;
};

type StartEmailAuthResponse = {
  isCodeSent: boolean;
};

type VerifyEmailRequest = {
  email: string;
  verificationCode: string;
};

type VerifyEmailResponse = {
  jwtToken: string;
  profileRequired: boolean;
};

export type CurrentUser = {
  id: string;
  email: string;
  username: string | null;
  imagePath: string;
  isProfileCreated: boolean;
};

export async function startEmailAuth(request: StartEmailAuthRequest): Promise<StartEmailAuthResponse> {
  const response = await fetch(`${API_BASE_URLS.account}/api/Account/start-email-auth`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(request)
  });

  if (!response.ok) {
    await throwApiError(response);
  }

  return response.json();
}

export async function verifyEmail(request: VerifyEmailRequest): Promise<VerifyEmailResponse> {
  const response = await fetch(`${API_BASE_URLS.account}/api/Account/verify-email`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(request)
  });

  if (!response.ok) {
    await throwApiError(response);
  }

  return response.json();
}

export async function getCurrentUser(): Promise<CurrentUser> {
  const token = getAccessToken();

  if (!token) {
    throw new Error("Access token is missing");
  }

  const response = await fetch(`${API_BASE_URLS.account}/api/Account/me`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  if (!response.ok) {
    await throwApiError(response);
  }

  return response.json();
}
