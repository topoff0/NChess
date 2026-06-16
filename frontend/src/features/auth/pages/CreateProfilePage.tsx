import { createProfile } from "@/features/auth/api/createProfileApi";
import { useState } from "react";

type CreateProfilePageProps = {
  onCreated: () => void;
};

export const CreateProfilePage = ({ onCreated }: CreateProfilePageProps) => {
  const [username, setUsername] = useState("");
  const [profileImage, setProfileImage] = useState<null | File>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const profileImagePreviewUrl = profileImage ? URL.createObjectURL(profileImage) : null;

  const trimmedUsername = username.trim();
  const usernameValidationError =
    trimmedUsername.length > 0 && (trimmedUsername.length < 3 || trimmedUsername.length > 50)
      ? "Username must be between 3 and 50 characters"
      : null;

  const MaxProfileImageSizeInBytes = 5 * 1024 * 1024;
  const profileImageValidationError =
    profileImage && profileImage.size > MaxProfileImageSizeInBytes ? "Profile image size must no exceed 5MB" : null;

  const canSubmitProfile = trimmedUsername.length > 0 && !usernameValidationError && !profileImageValidationError;

  const handleCreateProfile = async () => {
    if (!canSubmitProfile) {
      setError("Please fix validation errors");
      return;
    }

    setError(null);
    setIsLoading(true);

    try {
      const result = await createProfile({ username: trimmedUsername, profileImage: profileImage });

      if (result.isCreated) {
        onCreated();
      }
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to create profile");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <form
        className="space-y-4 "
        onSubmit={(event) => {
          event.preventDefault();
          void handleCreateProfile();
        }}>
        <label className="block text-sm font-black" htmlFor="username">
          Username
        </label>
        <input
          className="w-full h-12 rounded-2xl border-4 border-wood-dark bg-fog px-4
          font-bold text-wood-dark outline-none focus:border-moss"
          id="username"
          value={username}
          onChange={(event) => setUsername(event.target.value)}
        />
        {usernameValidationError && <p className="text-sm font-bold text-red-700">{usernameValidationError}</p>}
        <label
          className="mx-auto flex aspect-square w-32 cursor-pointer items-center justify-center
          overflow-hidden
          rounded-2xl border-4 border-wood-dark bg-fog text-center text-sm font-black text-wood-dark"
          htmlFor="profileImage">
          {profileImagePreviewUrl ? (
            <img className="h-full w-full object-cover" src={profileImagePreviewUrl} alt="Selected profile" />
          ) : (
            "Profile image"
          )}
        </label>
        {profileImageValidationError && <p className="text-sm font-bold text-red-700">{profileImageValidationError}</p>}
        <input
          className="sr-only"
          id="profileImage"
          type="file"
          accept="image/*"
          onChange={(event) => setProfileImage(event.currentTarget.files?.[0] ?? null)}
        />
        <button
          className="w-full h-12 rounded-2xl border-4 border-wood-dark bg-lime
          font-bold text-wood-dark disabled:opacity-60"
          type="submit"
          disabled={isLoading || !canSubmitProfile}>
          {isLoading ? "Creating..." : "Create profile"}
        </button>
      </form>
      {error && <p className="mt-4 text-sm font-bold text-red-700">{error}</p>}
    </>
  );
};
