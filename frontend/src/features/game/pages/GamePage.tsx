import { InitialBoardSquares, PieceSymbols } from "@/features/game/lib/fen";

type GamePageProps = {
  onLogout: () => void;
};

export const GamePage = ({ onLogout }: GamePageProps) => {
  return (
    <main className="min-h-screen bg-cream px-6 py-8 text-wood-dark">
      <header className="mx-auto flex max-w-5xl items-center justify-between">
        <h1 className="text-4xl font-black">NChess</h1>

        <button
          className="rounded-2xl border-4 border-wood-dark bg-lime
        px-5 py-2 font-bold text-wood-dark disabled:opacity-60"
          type="button"
          onClick={onLogout}>
          Logout
        </button>
      </header>

      <section className="mx-auto mt-10 max-w-5xl">
        <div className="mx-auto grid aspect-square w-full max-w-xl grid-cols-8 overflow-hidden rounded-2xl border-4 border-wood-dark">
          {InitialBoardSquares.map((square) => (
            <div
              className={`flex aspect-square items-center justify-center text-3xl font-bold sm:text-5xl
              ${square.isLight ? "bg-cream" : "bg-wood"}`}
              key={square.key}>
              {square.piece && (
                <span className={square.piece === square.piece.toUpperCase() ? "text-fog" : "text-forest"}>
                  {PieceSymbols[square.piece]}
                </span>
              )}
            </div>
          ))}
        </div>
      </section>
    </main>
  );
};
