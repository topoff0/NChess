import { routes } from "../app/router/routes";
import { DecorativeCircles } from "../shared/ui/DecorativeCircles";
import { FeatureCard } from "../shared/ui/FeatureCard";
import { Signboard } from "../shared/ui/Signboard";
import { StartButton } from "../shared/ui/StartButton";

export const HomePage = () => {
  return (
    <main className="relative min-h-screen overflow-hidden bg-primary-dark text-text-dark">

      <DecorativeCircles />

      <section
        className="relative z-10 mx-auto flex min-h-screen w-full max-w-6xl flex-col items-center justify-center gap-8 px-6 py-16 text-center">

        <Signboard />

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
            "On the Chess-board lie and hypocrisy do not survive long." – Emanuel Lasker
          </p>
        </div>

        <div className="grid w-full max-w-4xl gap-4 sm:grid-cols-3">
          <FeatureCard title="Strategy" subtitle="Puzzle Power" description="See combos before they happen."
            colorClass="border-accent-100/70 bg-primary/15" />
          <FeatureCard title="Tempo" subtitle="Fast Tactics" description="Strike first and keep pressure high."
            colorClass="border-accent-200/70 bg-primary/15" />
          <FeatureCard title="Mindset" subtitle="Champion Spirit" description="Play fearless and enjoy every battle."
            colorClass="border-accent-400/70 bg-primary/15" />
        </div>

        <StartButton to={routes.auth} text="Start Your Journey" />

      </section>
    </main>
  );
};
