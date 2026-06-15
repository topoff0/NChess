import type { ReactNode } from "react";

type AuthLayoutProps = {
  children: ReactNode;
};

export const AuthLayout = ({ children }: AuthLayoutProps) => {
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

        <div className="w-full mt-10 rounded-3xl border-4 border-wood-dark bg-cream p-5">{children}</div>
      </section>
    </main>
  );
};
