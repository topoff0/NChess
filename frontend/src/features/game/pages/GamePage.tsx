import { PieceImages } from "@/features/game/assets/pieces/pieceImages";
import { ChessBoard } from "@/features/game/components/ChessBoard";
import { MoveList } from "@/features/game/components/MoveList";
import { type PromotionPiece, useChessGame } from "@/features/game/hooks/useChessGame";

type GamePageProps = {
  onLogout: () => void;
  onShowHistory: () => void;
};

const PromotionOptions: Array<{ piece: PromotionPiece; label: string }> = [
  { piece: "Q", label: "Queen" },
  { piece: "R", label: "Rook" },
  { piece: "B", label: "Bishop" },
  { piece: "N", label: "Knight" }
];

const getResultTitle = (winner: string | null) => {
  if (winner === "DRAW") {
    return "Draw";
  }

  return `${winner ?? "No one"} wins`;
};

export const GamePage = ({ onLogout, onShowHistory }: GamePageProps) => {
  const { state, selectSquare, choosePromotionPiece, startNewGame } = useChessGame();

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
                isInteractive={!state.isMoving && state.pendingPromotion === null && !state.isGameEnded}
                onSquareClick={selectSquare}
              />
              {state.moveError && <p className="mt-4 text-sm font-bold text-red-700">{state.moveError}</p>}

              {state.pendingPromotion && (
                <div
                  className="fixed inset-0 z-10 flex items-center justify-center bg-forest/70 px-4"
                  role="dialog"
                  aria-modal="true"
                  aria-labelledby="promotion-title">
                  <section className="w-full max-w-sm rounded-lg border-4 border-wood-dark bg-cream p-5 text-center">
                    <h2 id="promotion-title" className="text-xl font-black">
                      Promote pawn
                    </h2>
                    <div className="mt-5 grid grid-cols-4 gap-3">
                      {PromotionOptions.map((option) => (
                        <button
                          className="flex aspect-square items-center justify-center rounded-lg border-4 border-wood-dark bg-fog p-2 disabled:opacity-60"
                          key={option.piece}
                          type="button"
                          disabled={state.isMoving}
                          aria-label={option.label}
                          title={option.label}
                          onClick={() => void choosePromotionPiece(option.piece)}>
                          <img
                            className="h-12 w-12 object-contain sm:h-14 sm:w-14"
                            src={PieceImages[option.piece]}
                            alt=""
                            draggable={false}
                          />
                        </button>
                      ))}
                    </div>
                  </section>
                </div>
              )}

              {state.isGameEnded && (
                <div
                  className="fixed inset-0 z-10 flex items-center justify-center bg-forest/70 px-4"
                  role="dialog"
                  aria-modal="true"
                  aria-labelledby="result-title">
                  <section className="w-full max-w-sm rounded-lg border-4 border-wood-dark bg-cream p-5 text-center">
                    <h2 id="result-title" className="text-2xl font-black">
                      {getResultTitle(state.winner)}
                    </h2>
                    <button
                      className="mt-5 rounded-lg border-4 border-wood-dark bg-lime px-5 py-2 font-bold text-wood-dark disabled:opacity-60"
                      type="button"
                      disabled={state.isMoving}
                      onClick={() => void startNewGame()}>
                      New game
                    </button>
                  </section>
                </div>
              )}
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
