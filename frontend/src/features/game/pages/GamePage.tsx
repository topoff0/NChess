import { ChessBoard } from "@/features/game/components/ChessBoard";
import { MoveList } from "@/features/game/components/MoveList";
import { useChessGame } from "@/features/game/hooks/useChessGame";

type GamePageProps = {
  onLogout: () => void;
  onShowHistory: () => void;
};

export const GamePage = ({ onLogout, onShowHistory }: GamePageProps) => {
  const { state, selectSquare } = useChessGame();

  return (
    <main className="min-h-screen bg-cream px-6 py-8 text-wood-dark">
      <header className="mx-auto flex max-w-5xl items-center justify-between">
        <h1 className="text-4xl font-black">NChess</h1>

        <div className="flex gap-3">
          <button
            className="rounded-2xl border-4 border-wood-dark bg-fog
          px-5 py-2 font-bold text-wood-dark disabled:opacity-60"
            type="button"
            onClick={onShowHistory}>
            History
          </button>

          <button
            className="rounded-2xl border-4 border-wood-dark bg-lime
          px-5 py-2 font-bold text-wood-dark disabled:opacity-60"
            type="button"
            onClick={onLogout}>
            Logout
          </button>
        </div>
      </header>

      <section className="mx-auto mt-10 flex max-w-5xl flex-col gap-8 sm:flex-row">
        <div className="flex-1">
          {state.status === "loading" && <p className="text-xl font-black">Loading game...</p>}
          {state.status === "error" && <p className="text-sm font-bold text-red-700">{state.message}</p>}

          {state.status === "ready" && (
            <>
              <ChessBoard
                fen={state.fen}
                legalMoves={state.legalMoves}
                selectedSquare={state.selectedSquare}
                onSquareClick={selectSquare}
              />
              {state.moveError && <p className="mt-4 text-sm font-bold text-red-700">{state.moveError}</p>}
            </>
          )}
        </div>

        {state.status === "ready" && (
          <aside
            className="flex max-h-64 w-full flex-col overflow-hidden
             rounded-2xl border-4 border-wood-dark bg-fog
             p-4 sm:max-h-144 sm:w-56">
            <h2 className="mb-2 shrink-0 text-sm font-black">Moves</h2>
            <MoveList moveNotations={state.moveNotations} />
          </aside>
        )}
      </section>
    </main>
  );
};
