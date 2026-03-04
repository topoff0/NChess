import { Link } from "react-router-dom";
import { routes } from "../app/router/routes";

export const HomePage = () => {
  return (
    <main className="relative min-h-screen overflow-hidden bg-primary-dark text-text-dark">
      <div
        className="pointer-events-none absolute -left-16 -top-14 h-52 w-52 rounded-full bg-accent-100/45 blur-2xl animate-pulse" />
      <div
        className="pointer-events-none absolute right-0 top-10 h-64 w-64 -translate-x-10 rounded-full bg-accent-400/40 blur-3xl animate-pulse" />
      <div
        className="pointer-events-none absolute -bottom-20 left-1/2 h-72 w-72 -translate-x-1/2 rounded-full bg-accent-200/35 blur-3xl animate-bounce" />

      <section
        className="relative z-10 mx-auto flex min-h-screen w-full max-w-6xl flex-col items-center justify-center gap-8 px-6 py-16 text-center">
        <div
          className="inline-flex -rotate-2 items-center gap-3 rounded-full border-2 border-primary/60 bg-primary px-6 py-2 text-sm font-black uppercase tracking-[0.2em] text-primary-dark shadow-[0_8px_0_0_rgba(0,18,25,0.5)]">
          <span className="text-xl">♞</span>
          Master your next move
          <span className="text-xl">♞</span>
        </div>

        <div className="space-y-5">
          <h1 className="text-4xl font-black uppercase leading-tight text-primary sm:text-6xl lg:text-7xl">
            Turn Every Move
            <br />
            Into a
            <span className="mx-3 inline-block -rotate-2 rounded-xl bg-accent-100 px-3 py-1 text-primary-dark">
              Hero Moment
            </span>
          </h1>

          <p className="mx-auto max-w-2xl text-lg font-medium text-primary/90 sm:text-2xl">
            Jump into a chess adventure where brave ideas, wild tactics,
            and clever sacrifices unlock your path to greatness.
          </p>
        </div>

        <div className="grid w-full max-w-4xl gap-4 sm:grid-cols-3">
          <div
            className="rounded-3xl border-2 border-accent-100/70 bg-primary/15 p-5 text-left shadow-[0_10px_0_0_rgba(148,210,189,0.2)] backdrop-blur-sm transition hover:-translate-y-1">
            <p className="text-xs font-black uppercase tracking-[0.2em] text-accent-100">Strategy</p>
            <p className="mt-2 text-2xl font-black text-primary">Puzzle Power</p>
            <p className="mt-2 text-sm text-primary/85">See combos before they happen.</p>
          </div>

          <div
            className="rounded-3xl border-2 border-accent-200/70 bg-primary/15 p-5 text-left shadow-[0_10px_0_0_rgba(10,147,150,0.25)] backdrop-blur-sm transition hover:-translate-y-1">
            <p className="text-xs font-black uppercase tracking-[0.2em] text-accent-100">Tempo</p>
            <p className="mt-2 text-2xl font-black text-primary">Fast Tactics</p>
            <p className="mt-2 text-sm text-primary/85">Strike first and keep pressure high.</p>
          </div>

          <div
            className="rounded-3xl border-2 border-accent-400/70 bg-primary/15 p-5 text-left shadow-[0_10px_0_0_rgba(187,62,3,0.3)] backdrop-blur-sm transition hover:-translate-y-1">
            <p className="text-xs font-black uppercase tracking-[0.2em] text-accent-100">Mindset</p>
            <p className="mt-2 text-2xl font-black text-primary">Champion Spirit</p>
            <p className="mt-2 text-sm text-primary/85">Play fearless and enjoy every battle.</p>
          </div>
        </div>

        <Link to={routes.auth}
          className="group relative inline-flex -rotate-1 items-center justify-center gap-3 overflow-hidden rounded-2xl border-2 border-primary bg-accent-200 px-10 py-4 text-lg font-black uppercase tracking-[0.14em] text-primary-dark shadow-[0_10px_0_0_rgba(0,18,25,0.5)] transition duration-300 hover:-translate-y-1 hover:bg-primary hover:text-primary-dark focus-visible:outline-none focus-visible:ring-4 focus-visible:ring-accent-100/70">
          <span
            className="absolute inset-0 translate-y-full bg-primary/20 transition duration-300 group-hover:translate-y-0" />
          <span className="relative">Start Your Auth Journey</span>
          <span className="relative text-2xl transition group-hover:translate-x-1 group-hover:-translate-y-1">♜</span>
        </Link>
      </section>
    </main>
  );
};
