import { useMemo, useState } from "react";
import type { FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { routes } from "../app/router/routes";
import { AUTH_TOKEN_KEY, useAuth } from "../features/auth/AuthProvider";

type StartAuthResponse = {
  isExists: boolean;
  isActive: boolean;
};

type VerifyEmailResponse = {
  jwtToken: string;
};

type LoginResponse = {
  jwtToken: string;
};

type Step = "email" | "verify" | "login";

const API_BASE_URL = (import.meta.env.VITE_ACCOUNT_API_URL as string | undefined)?.trim() ||
  "http://localhost:8080/api/account";

const getBackendErrorMessage = (errorBody: unknown): string => {
  if (typeof errorBody === "string") return errorBody;

  if (errorBody && typeof errorBody === "object") {
    const e = errorBody as Record<string, unknown>;

    if (typeof e.message === "string") return e.message;
    if (typeof e.errorMessage === "string") return e.errorMessage;

    const errors = e.errors;
    if (errors && typeof errors === "object") {
      const firstEntry = Object.values(errors as Record<string, unknown>)[0];
      if (Array.isArray(firstEntry) && typeof firstEntry[0] === "string") {
        return firstEntry[0];
      }
    }
  }

  return "Something went wrong. Please try again.";
};

export const AuthPage = () => {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [step, setStep] = useState<Step>("email");
  const [email, setEmail] = useState("");
  const [verificationCode, setVerificationCode] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const cleanEmail = useMemo(() => email.trim(), [email]);

  const resetMessages = () => {
    setErrorMessage("");
    setSuccessMessage("");
  };

  const submitEmail = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    resetMessages();

    if (!cleanEmail) {
      setErrorMessage("Please enter your email.");
      return;
    }

    try {
      setIsLoading(true);
      const response = await fetch(`${API_BASE_URL}/start-email-auth`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: cleanEmail }),
      });

      const payload = (await response.json()) as StartAuthResponse | Record<string, unknown>;

      if (!response.ok) {
        setErrorMessage(getBackendErrorMessage(payload));
        return;
      }

      const data = payload as StartAuthResponse;
      if (data.isExists && data.isActive) {
        setStep("login");
        setSuccessMessage("Welcome back! Enter your password to continue.");
      } else {
        setStep("verify");
        setSuccessMessage("Verification code sent! Check your email.");
      }
    } catch {
      setErrorMessage("Service is currently unavailable");
    } finally {
      setIsLoading(false);
    }
  };

  const submitVerificationCode = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    resetMessages();

    if (!verificationCode.trim()) {
      setErrorMessage("Please enter verification code.");
      return;
    }

    try {
      setIsLoading(true);
      const response = await fetch(`${API_BASE_URL}/verify-email`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: cleanEmail, verificationCode: verificationCode.trim() }),
      });

      const payload = (await response.json()) as VerifyEmailResponse | Record<string, unknown>;

      if (!response.ok) {
        setErrorMessage(getBackendErrorMessage(payload));
        return;
      }

      const data = payload as VerifyEmailResponse;
      localStorage.setItem(AUTH_TOKEN_KEY, data.jwtToken);
      login();
      navigate(routes.createAccount);
    } catch {
      setErrorMessage("Cannot connect to backend. Check API URL and try again.");
    } finally {
      setIsLoading(false);
    }
  };

  const submitLogin = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    resetMessages();

    if (!password) {
      setErrorMessage("Please enter your password.");
      return;
    }

    try {
      setIsLoading(true);
      const response = await fetch(`${API_BASE_URL}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: cleanEmail, password }),
      });

      const payload = (await response.json()) as LoginResponse | Record<string, unknown>;

      if (!response.ok) {
        setErrorMessage(getBackendErrorMessage(payload));
        return;
      }

      const data = payload as LoginResponse;
      localStorage.setItem(AUTH_TOKEN_KEY, data.jwtToken);
      login();
      navigate(routes.home);
    } catch {
      setErrorMessage("Cannot connect to backend. Check API URL and try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <main className="relative min-h-screen overflow-hidden bg-primary-dark text-text-dark">
      <div
        className="pointer-events-none absolute -left-10 top-0 h-56 w-56 rounded-full bg-accent-100/35 blur-2xl animate-bounce" />
      <div
        className="pointer-events-none absolute right-0 top-16 h-72 w-72 -translate-x-6 rounded-full bg-accent-200/35 blur-3xl animate-pulse" />
      <div
        className="pointer-events-none absolute bottom-0 left-1/2 h-72 w-72 -translate-x-1/2 rounded-full bg-accent-400/30 blur-3xl animate-pulse" />

      <section
        className="relative z-10 mx-auto flex min-h-screen w-full max-w-6xl flex-col items-center justify-center gap-8 px-6 py-12 text-center">
        <Link to={routes.home}
          className="inline-flex -rotate-2 items-center gap-2 rounded-full border-2 border-primary/60 bg-primary px-5 py-2 text-xs font-black uppercase tracking-[0.2em] text-primary-dark shadow-[0_8px_0_0_rgba(0,18,25,0.5)]">
          ♞ Back to Home
        </Link>

        <div className="max-w-2xl space-y-4">
          <h1 className="text-4xl font-black uppercase text-primary sm:text-6xl">
            Join the Chess Adventure
          </h1>
        </div>

        <div
          className="w-full max-w-xl rounded-3xl border-2 border-primary/40 bg-primary/10 p-6 text-left shadow-[0_12px_0_0_rgba(0,18,25,0.5)] backdrop-blur-sm sm:p-8">
          <p className="mb-3 text-center text-xs font-black uppercase tracking-[0.2em] text-accent-100">
            {step === "email" && "Enter Email"}
            {step === "verify" && "Verify Code"}
            {step === "login" && "Sign In"}
          </p>

          {step === "email" && (
            <form className="space-y-4" onSubmit={submitEmail}>
              <input id="email" type="email" required value={email} onChange={(event) =>
                setEmail(event.target.value)}
                className="w-full rounded-xl border-2 border-primary/40 bg-primary/95 px-4 py-3
                          text-primary-dark outline-none transition focus:border-accent-200"
                placeholder="you@example.com"
              />
              <button type="submit" disabled={isLoading}
                className="w-full rounded-2xl border-2 border-primary bg-accent-200 px-6 py-3 text-sm font-black uppercase tracking-[0.16em] text-primary-dark shadow-[0_8px_0_0_rgba(0,18,25,0.5)] transition hover:translate-y-1 hover:shadow-[0_3px_0_0_rgba(0,18,25,0.5)] disabled:cursor-not-allowed disabled:opacity-70">
                {isLoading ? "Sending..." : "Send Verification Code"}
              </button>
            </form>
          )}

          {step === "verify" && (
            <form className="space-y-4" onSubmit={submitVerificationCode}>
              <input id="verificationCode" type="text" required value={verificationCode} onChange={(event) =>
                setVerificationCode(event.target.value)}
                className="w-full rounded-xl border-2 border-primary/40 bg-primary/95 px-4 py-3
                          text-primary-dark outline-none transition focus:border-accent-200"
                placeholder="6-digit code"
              />
              <button type="submit" disabled={isLoading}
                className="w-full rounded-2xl border-2 border-primary bg-accent-200 px-6 py-3 text-sm font-black uppercase tracking-[0.16em] text-primary-dark shadow-[0_8px_0_0_rgba(0,18,25,0.5)] transition hover:translate-y-1 hover:shadow-[0_3px_0_0_rgba(0,18,25,0.5)] disabled:cursor-not-allowed disabled:opacity-70">
                {isLoading ? "Verifying..." : "Verify and Continue"}
              </button>
            </form>
          )}

          {step === "login" && (
            <form className="space-y-4" onSubmit={submitLogin}>
              <input id="password" type="password" required value={password} onChange={(event) =>
                setPassword(event.target.value)}
                className="w-full rounded-xl border-2 border-primary/40 bg-primary/95 px-4 py-3
                          text-primary-dark outline-none transition focus:border-accent-200"
                placeholder="Your secret move"
              />
              <button type="submit" disabled={isLoading}
                className="w-full rounded-2xl border-2 border-primary bg-accent-200 px-6 py-3 text-sm font-black uppercase tracking-[0.16em] text-primary-dark shadow-[0_8px_0_0_rgba(0,18,25,0.5)] transition hover:translate-y-1 hover:shadow-[0_3px_0_0_rgba(0,18,25,0.5)] disabled:cursor-not-allowed disabled:opacity-70">
                {isLoading ? "Signing in..." : "Sign In"}
              </button>
            </form>
          )}

          {errorMessage && (
            <p
              className="mt-4 text-center rounded-xl border border-error/70 bg-error/15 px-4 py-3 text-sm font-semibold text-red-100">
              {errorMessage}
            </p>
          )}

          {successMessage && (
            <p
              className="mt-4 rounded-xl border border-success/70 bg-success/20 px-4 py-3 text-sm font-semibold text-primary-dark">
              {successMessage}
            </p>
          )}
        </div>
      </section>
    </main>
  );
};
