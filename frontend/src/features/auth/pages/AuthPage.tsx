import { startEmailAuth, verifyEmail } from "@/features/auth/api/accountApi";
import { saveAccessToken } from "@/shared/auth/tokenStorage";
import { useState } from "react";

export const AuthPage = () => {
  type AuthStep = "email" | "code";
  type AuthStatus = "idle" | "loading" | "authenticated" | "profileRequired";

  const [step, setStep] = useState<AuthStep>("email");
  const [status, setStatus] = useState<AuthStatus>("idle");
  const [email, setEmail] = useState("");
  const [verificationCode, setVerificationCode] = useState("");
  const [error, setError] = useState<string | null>(null);

  const isLoading = status == "loading";

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
      setError("Fail to send verification code");
      setStatus("idle");
    }
  };

  const handleVerifyEmail = async () => {
    setError(null);
    setStatus("loading");

    try {
      const result = await verifyEmail({ email, verificationCode });

      saveAccessToken(result.jwtToken);
      setStatus(result.profileRequired ? "profileRequired" : "authenticated");
    } catch {
      setError("Failed to verify email");
      setStatus("idle");
    }
  };

  return (
    <main>
      <h1>Chess</h1>

      {step === "email" && (
        <form
          onSubmit={(event) => {
            event.preventDefault();
            void handleStartEmailAuth();
          }}>
          <label htmlFor="email">Email</label>

          <input id="email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} />

          <button type="submit" disabled={isLoading || email.trim().length === 0}>
            {isLoading ? "Sending..." : "Send code"}
          </button>
        </form>
      )}

      {step === "code" && (
        <form
          onSubmit={(event) => {
            event.preventDefault();
            void handleVerifyEmail();
          }}>
          <label htmlFor="verificationCode">Code</label>

          <input
            id="verificationCode"
            inputMode="numeric"
            value={verificationCode}
            onChange={(event) => setVerificationCode(event.target.value)}
          />

          <button type="submit" disabled={isLoading || verificationCode.trim().length === 0}>
            {isLoading ? "Verifying..." : "Sign in"}
          </button>
        </form>
      )}

      {error && <p>{error}</p>}

      {status === "authenticated" && <p>Authenticated</p>}

      {status === "profileRequired" && <p>Profile is required</p>}
    </main>
  );
};
