import { useGameHistory } from "@/features/game/hooks/useGameHistory";

type GameHistoryPageProps = {
  onBack: () => void;
};

const formatFinishedAt = (finishedAt: string): string => {
  return new Date(finishedAt).toLocaleDateString(undefined, {
    year: "numeric",
    month: "short",
    day: "numeric"
  });
};

export const GameHistoryPage = ({ onBack }: GameHistoryPageProps) => {
  const state = useGameHistory();

  return (
    <main className="min-h-screen bg-cream px-6 py-8 text-wood-dark">
      <header className="mx-auto flex max-w-5xl items-center justify-between">
        <h1 className="text-4xl font-black">Game History</h1>

        <button
          className="rounded-2xl border-4 border-wood-dark bg-lime
          px-5 py-2 font-bold text-wood-dark disabled:opacity-60"
          type="button"
          onClick={onBack}>
          Back to game
        </button>
      </header>

      <section className="mx-auto mt-10 max-w-5xl">
        {state.status === "loading" && <p className="text-xl font-black">Loading games...</p>}
        {state.status === "error" && <p className="text-sm font-bold text-red-700">{state.message}</p>}

        {state.status === "ready" && state.games.length === 0 && (
          <p className="text-lg font-bold text-wood-dark/60">You haven't finished a game yet.</p>
        )}

        {state.status === "ready" && state.games.length > 0 && (
          <ul className="flex flex-col gap-3">
            {state.games.map((game) => (
              <li
                className="flex items-center justify-between rounded-2xl border-4 border-wood-dark bg-fog p-4"
                key={game.id}>
                <span className="font-black">{game.result}</span>
                <span className="text-sm font-bold text-wood-dark/70">{formatFinishedAt(game.finishedAt)}</span>
              </li>
            ))}
          </ul>
        )}
      </section>
    </main>
  );
};
