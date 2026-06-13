import { startEmailAuth, verifyEmail } from "@/features/auth/api/accountApi";
import { CreateProfilePage } from "@/features/auth/pages/CreateProfilePage";
import { saveAccessToken } from "@/shared/auth/tokenStorage";
import { useState } from "react";

type AuthStep = "email" | "code" | "createProfile";
type AuthStatus = "idle" | "loading" | "authenticated";

export const AuthPage = () => {
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
    } catch {
      setError("Failed to send verification code");
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
    } catch {
      setError("Failed to verify email");
      setStatus("idle");
    }
  };

  return (
    <main className="relative min-h-screen overflow-hidden bg-cream px-6 py-8 text-wood-dark">
      <svg
        className="absolute inset-x-0 bottom-0 h-32 w-full text-moss"
        viewBox="0 0 1440 160"
        preserveAspectRatio="none"
        aria-hidden="true">
        <path
          fill="currentColor"
          d="M0 64 C160 112 320 16 480 64 C640 112 800 16 960 64 C1120 112 1280 16 1440 64 L1440 160 L0 160 Z"
        />
      </svg>

      <section className="relative z-10 mx-auto flex min-h-[calc(100vh-4rem)] max-w-md flex-col justify-center items-center">
        <div className="relative inline-flex items-end gap-3">
          <h1 className="text-6xl font-bold text-wood-dark">NChess</h1>
          <span className="-mb-1 text-7xl leading-none text-wood/35" aria-hidden="true">
            ♟
          </span>
        </div>
        <p className="mt-4 max-w-xs text-base leading-7 text-wood-dark/75">Enter email and start your journey</p>

        <div className="w-full mt-10 rounded-3xl border-4 border-wood-dark bg-cream p-5">
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
            <CreateProfilePage onCreated={() => setStatus("authenticated")} />
          )}

          {status === "authenticated" && <p className="mt-4 text-sm font-bold text-moss">You are signed in.</p>}

          {error && <p className="mt-4 text-sm font-bold text-red-700">{error}</p>}
        </div>
      </section>
    </main>
  );
};
