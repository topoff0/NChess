import { apiAuthenticatedRequest, apiJsonRequest } from "@/shared/api/apiRequest";
import { API_BASE_URLS } from "@/shared/api/httpClient";

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
  return apiJsonRequest<StartEmailAuthResponse>(`${API_BASE_URLS.account}/api/Account/start-email-auth`, {
    method: "POST",
    body: request
  });
}

export async function verifyEmail(request: VerifyEmailRequest): Promise<VerifyEmailResponse> {
  return apiJsonRequest<VerifyEmailResponse>(`${API_BASE_URLS.account}/api/Account/verify-email`, {
    method: "POST",
    body: request
  });
}

export async function getCurrentUser(): Promise<CurrentUser> {
  return apiAuthenticatedRequest<CurrentUser>(`${API_BASE_URLS.account}/api/Account/me`);
}
