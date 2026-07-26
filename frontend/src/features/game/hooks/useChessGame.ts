import { makeMove, promotePawn, startGame, type GameResponse } from "@/features/game/api/chessApi";
import { parseFenBoard, type FenPiece } from "@/features/game/lib/fen";
import { useEffect, useReducer } from "react";

export type PromotionPiece = "Q" | "R" | "B" | "N";

type PendingPromotion = {
  fenBeforeMove: string;
  startSquare: number;
  targetSquare: number;
};

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
      pendingPromotion: PendingPromotion | null;
      isGameEnded: boolean;
      winner: string | null;
    };

type GameAction =
  | { type: "load-failed"; message: string }
  | { type: "game-loaded"; game: GameResponse }
  | { type: "square-selected"; square: number | null }
  | { type: "promotion-requested"; promotion: PendingPromotion }
  | { type: "move-started" }
  | { type: "move-succeeded"; game: GameResponse }
  | { type: "move-failed"; message: string }
  | { type: "new-game-started" };

const toReadyState = (game: GameResponse): GameState => ({
  status: "ready",
  fen: game.fen,
  legalMoves: game.legalMoves ?? {},
  moveNotations: game.moveNotations,
  selectedSquare: null,
  isMoving: false,
  moveError: null,
  pendingPromotion: null,
  isGameEnded: game.isGameEnded,
  winner: game.winner
});

const gameReducer = (state: GameState, action: GameAction): GameState => {
  switch (action.type) {
    case "load-failed":
      return { status: "error", message: action.message };
    case "game-loaded":
      return toReadyState(action.game);
    case "square-selected":
      return state.status === "ready" ? { ...state, selectedSquare: action.square } : state;
    case "promotion-requested":
      return state.status === "ready"
        ? {
            ...state,
            selectedSquare: null,
            pendingPromotion: action.promotion,
            moveError: null
          }
        : state;
    case "move-started":
      return state.status === "ready" ? { ...state, isMoving: true, moveError: null } : state;
    case "move-succeeded":
      return toReadyState(action.game);
    case "move-failed":
      return state.status === "ready"
        ? { ...state, isMoving: false, moveError: action.message, selectedSquare: null }
        : state;
    case "new-game-started":
      return { status: "loading" };
  }
};

const getPieceAtSquare = (fen: string, square: number): FenPiece | null => {
  return parseFenBoard(fen.split(" ")[0]).find((boardSquare) => boardSquare.square === square)?.piece ?? null;
};

const isPlayerPromotionMove = (fen: string, startSquare: number, targetSquare: number): boolean => {
  const piece = getPieceAtSquare(fen, startSquare);

  return piece === "P" && targetSquare >= 56;
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

  const startNewGame = async () => {
    dispatch({ type: "new-game-started" });

    try {
      const game = await startGame();
      dispatch({ type: "game-loaded", game });
    } catch (error) {
      dispatch({
        type: "load-failed",
        message: error instanceof Error ? error.message : "Failed to start a new game"
      });
    }
  };

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

  const choosePromotionPiece = async (piece: PromotionPiece) => {
    if (state.status !== "ready" || state.pendingPromotion === null || state.isMoving) {
      return;
    }

    dispatch({ type: "move-started" });

    try {
      const game = await promotePawn({ ...state.pendingPromotion, chosenPiece: piece });
      dispatch({ type: "move-succeeded", game });
    } catch (error) {
      dispatch({
        type: "move-failed",
        message: error instanceof Error ? error.message : "Failed to promote the pawn"
      });
    }
  };

  const selectSquare = (square: number) => {
    if (state.status !== "ready" || state.isMoving || state.pendingPromotion !== null || state.isGameEnded) {
      return;
    }

    const legalTargets = state.selectedSquare !== null ? (state.legalMoves[state.selectedSquare] ?? []) : [];

    if (state.selectedSquare !== null && legalTargets.includes(square)) {
      if (isPlayerPromotionMove(state.fen, state.selectedSquare, square)) {
        dispatch({
          type: "promotion-requested",
          promotion: {
            fenBeforeMove: state.fen,
            startSquare: state.selectedSquare,
            targetSquare: square
          }
        });
        return;
      }

      void move(state.fen, state.selectedSquare, square);
      return;
    }

    const hasLegalMoves = (state.legalMoves[square]?.length ?? 0) > 0;
    dispatch({ type: "square-selected", square: hasLegalMoves ? square : null });
  };

  return { state, selectSquare, choosePromotionPiece, startNewGame };
};
