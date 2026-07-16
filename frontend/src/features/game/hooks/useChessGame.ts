import { makeMove, startGame, type GameResponse } from "@/features/game/api/chessApi";
import { useEffect, useReducer } from "react";

type GameState =
  | { status: "loading" }
  | { status: "error"; message: string }
  | {
      status: "ready";
      fen: string;
      legalMoves: Record<string, number[]>;
      moveNotations: string[];
      selectedSquare: number | null;
      isMoving: boolean;
      moveError: string | null;
    };

type GameAction =
  | { type: "load-failed"; message: string }
  | { type: "game-loaded"; game: GameResponse }
  | { type: "square-selected"; square: number | null }
  | { type: "move-started" }
  | { type: "move-succeeded"; game: GameResponse }
  | { type: "move-failed"; message: string };

const toReadyState = (game: GameResponse): GameState => ({
  status: "ready",
  fen: game.fen,
  legalMoves: game.legalMoves ?? {},
  moveNotations: game.moveNotations,
  selectedSquare: null,
  isMoving: false,
  moveError: null
});

const gameReducer = (state: GameState, action: GameAction): GameState => {
  switch (action.type) {
    case "load-failed":
      return { status: "error", message: action.message };
    case "game-loaded":
      return toReadyState(action.game);
    case "square-selected":
      return state.status === "ready" ? { ...state, selectedSquare: action.square } : state;
    case "move-started":
      return state.status === "ready" ? { ...state, isMoving: true, moveError: null } : state;
    case "move-succeeded":
      return toReadyState(action.game);
    case "move-failed":
      return state.status === "ready"
        ? { ...state, isMoving: false, moveError: action.message, selectedSquare: null }
        : state;
  }
};

export const useChessGame = () => {
  const [state, dispatch] = useReducer(gameReducer, { status: "loading" });

  useEffect(() => {
    const controller = new AbortController();

    const loadGame = async () => {
      try {
        const game = await startGame(controller.signal);
        dispatch({ type: "game-loaded", game });
      } catch (error) {
        if (controller.signal.aborted) {
          return;
        }

        dispatch({
          type: "load-failed",
          message: error instanceof Error ? error.message : "Failed to start the game"
        });
      }
    };

    void loadGame();

    return () => controller.abort();
  }, []);

  const move = async (fenBeforeMove: string, startSquare: number, targetSquare: number) => {
    dispatch({ type: "move-started" });

    try {
      const game = await makeMove({ startSquare, targetSquare, fenBeforeMove });
      dispatch({ type: "move-succeeded", game });
    } catch (error) {
      dispatch({
        type: "move-failed",
        message: error instanceof Error ? error.message : "Failed to make the move"
      });
    }
  };

  const selectSquare = (square: number) => {
    if (state.status !== "ready" || state.isMoving) {
      return;
    }

    const legalTargets = state.selectedSquare !== null ? (state.legalMoves[state.selectedSquare] ?? []) : [];

    if (state.selectedSquare !== null && legalTargets.includes(square)) {
      void move(state.fen, state.selectedSquare, square);
      return;
    }

    const hasLegalMoves = (state.legalMoves[square]?.length ?? 0) > 0;
    dispatch({ type: "square-selected", square: hasLegalMoves ? square : null });
  };

  return { state, selectSquare };
};
