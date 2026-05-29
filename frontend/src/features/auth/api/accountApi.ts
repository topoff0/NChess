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

export async function startEmailAuth(request: StartEmailAuthRequest): Promise<StartEmailAuthResponse> {
  const response = await fetch(`${API_BASE_URLS.account}/api/Account/start-email-auth`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(request)
  });

  if (!response.ok) {
    throw new Error("Failed to start email authentication");
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
    throw new Error("Failed to verify email");
  }

  return response.json();
}
