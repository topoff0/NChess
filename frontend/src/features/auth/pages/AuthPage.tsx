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

export const AuthPage = ({ onAuthenticated }: AuthPageProps) => {
  const [step, setStep] = useState<AuthStep>("email");
  const [status, setStatus] = useState<AuthStatus>("idle");
  const [email, setEmail] = useState("");
  const [verificationCode, setVerificationCode] = useState("");
  const [error, setError] = useState<string | null>(null);

  const isLoading = status === "loading";

  const handleStartEmailAuth = async () => {
    setError(null);
    setStatus("loading");

    try {
      const result = await startEmailAuth({ email });

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
    setError(null);
    setStatus("loading");

    try {
      const result = await verifyEmail({ email, verificationCode });

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

          <button
            className="h-12 w-full rounded-2xl border-4 border-wood-dark bg-lime font-black text-wood-dark disabled:opacity-60"
            type="submit"
            disabled={isLoading || email.trim().length === 0}>
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

          <button
            className="h-12 w-full rounded-2xl border-4 border-wood-dark bg-lime font-black text-wood-dark disabled:opacity-60"
            type="submit"
            disabled={isLoading || verificationCode.trim().length === 0}>
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
