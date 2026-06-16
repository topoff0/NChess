import { startEmailAuth, verifyEmail } from "@/features/auth/api/accountApi";
import { AuthLayout } from "@/features/auth/components/AuthLayout";
import { CreateProfilePage } from "@/features/auth/pages/CreateProfilePage";
import { saveAccessToken } from "@/shared/auth/tokenStorage";
import { useState } from "react";

type AuthStep = "email" | "code" | "createProfile";
type AuthStatus = "idle" | "loading" | "authenticated";

type AuthPageProps = {
  onAuthenticated: () => void;
};

const isValidEmail = (value: string) => {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
};

const isValidVerificationCode = (value: string) => {
  return /^\d{6}$/.test(value);
};

export const AuthPage = ({ onAuthenticated }: AuthPageProps) => {
  const [step, setStep] = useState<AuthStep>("email");
  const [status, setStatus] = useState<AuthStatus>("idle");
  const [email, setEmail] = useState("");
  const [verificationCode, setVerificationCode] = useState("");
  const [error, setError] = useState<string | null>(null);

  const isLoading = status === "loading";

  const trimmedEmail = email.trim();
  const trimmedVerificationCode = verificationCode.trim();

  const emailValidationError =
    trimmedEmail.length > 0 && !isValidEmail(trimmedEmail) ? "Enter a valid email address" : null;

  const verificationCodeValidationError =
    trimmedVerificationCode.length > 0 && !isValidVerificationCode(trimmedVerificationCode)
      ? "Verification code must be exactly 6 digits"
      : null;

  const canSubmitEmail = trimmedEmail.length > 0 && !emailValidationError;
  const canSubmitVerificationCode = trimmedVerificationCode.length > 0 && !verificationCodeValidationError;

  const handleStartEmailAuth = async () => {
    if (!canSubmitEmail) {
      setError("Enter a valid email address");
      return;
    }

    setError(null);
    setStatus("loading");

    try {
      const result = await startEmailAuth({ email: trimmedEmail });

      if (result.isCodeSent) {
        setStep("code");
        setStatus("idle");
      }
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to send verification code");
      }
      setStatus("idle");
    }
  };

  const handleVerifyEmail = async () => {
    if (!canSubmitVerificationCode) {
      setError("VerificationCode must be exactly 6 digits");
      return;
    }

    setError(null);
    setStatus("loading");

    try {
      const result = await verifyEmail({ email: trimmedEmail, verificationCode: trimmedVerificationCode });

      saveAccessToken(result.jwtToken);
      if (result.profileRequired) {
        setStep("createProfile");
        setStatus("idle");
        return;
      }

      setStatus("authenticated");
      onAuthenticated();
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError("Failed to verify email");
      }
      setStatus("idle");
    }
  };

  return (
    <AuthLayout>
      {step === "email" && (
        <form
          className="space-y-4"
          onSubmit={(event) => {
            event.preventDefault();
            void handleStartEmailAuth();
          }}>
          <label className="block text-sm font-black" htmlFor="email">
            Email
          </label>

          <input
            className="h-12 w-full rounded-2xl border-4 border-wood-dark bg-fog px-4 font-bold text-wood-dark outline-none focus:border-moss"
            id="email"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
          />
          {emailValidationError && <p className="text-sm font-bold text-red-700">{emailValidationError}</p>}

          <button
            className="h-12 w-full rounded-2xl border-4 border-wood-dark bg-lime font-black text-wood-dark disabled:opacity-60"
            type="submit"
            disabled={isLoading || !canSubmitEmail}>
            {isLoading ? "Sending..." : "Send code"}
          </button>
        </form>
      )}

      {step === "code" && (
        <form
          className="space-y-4"
          onSubmit={(event) => {
            event.preventDefault();
            void handleVerifyEmail();
          }}>
          <label className="block text-sm font-black" htmlFor="verificationCode">
            Code
          </label>

          <input
            className="h-12 w-full rounded-2xl border-4 border-wood-dark bg-fog px-4 font-bold text-wood-dark outline-none focus:border-moss"
            id="verificationCode"
            inputMode="numeric"
            value={verificationCode}
            onChange={(event) => setVerificationCode(event.target.value)}
          />
          {verificationCodeValidationError && (
            <p className="text-sm font-bold text-red-700">{verificationCodeValidationError}</p>
          )}

          <button
            className="h-12 w-full rounded-2xl border-4 border-wood-dark bg-lime font-black text-wood-dark disabled:opacity-60"
            type="submit"
            disabled={isLoading || !canSubmitVerificationCode}>
            {isLoading ? "Checking..." : "Play"}
          </button>
        </form>
      )}

      {step === "createProfile" && status !== "authenticated" && (
        <CreateProfilePage
          onCreated={() => {
            setStatus("authenticated");
            onAuthenticated();
          }}
        />
      )}

      {status === "authenticated" && <p className="mt-4 text-sm font-bold text-moss">You are signed in.</p>}

      {error && <p className="mt-4 text-sm font-bold text-red-700">{error}</p>}
    </AuthLayout>
  );
};
