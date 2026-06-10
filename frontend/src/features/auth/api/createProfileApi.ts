import { API_BASE_URLS } from "@/shared/api/httpClient";
import { getAccessToken } from "@/shared/auth/tokenStorage";

type CreateProfileRequest = {
  username: string;
  profileImage: File | null;
};

type CreateProfileResponse = {
  isCreated: boolean;
};

export async function createProfile(request: CreateProfileRequest): Promise<CreateProfileResponse> {
  const token = getAccessToken();

  if (!token) {
    throw new Error("Access token is missing");
  }

  const formData = new FormData();
  formData.append("Username", request.username);

  if (request.profileImage) {
    formData.append("ProfileImage", request.profileImage);
  }

  const response = await fetch(`${API_BASE_URLS.account}/api/Account/create-profile`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`
    },
    body: formData
  });

  if (!response.ok) {
    throw new Error("Failed to create profile");
  }

  return response.json();
}
