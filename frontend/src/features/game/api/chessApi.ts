import { apiAuthenticatedRequest } from "@/shared/api/apiRequest";
import { API_BASE_URLS } from "@/shared/api/httpClient";

type GameStartRequest = {
  isPlayerPlayWhite: boolean;
};

type MakeMoveRequest = {
  startSquare: number;
  targetSquare: number;
  fenBeforeMove: string;
};

type PromotePawnRequest = MakeMoveRequest & {
  chosenPiece: string;
};

export type GameResponse = {
  isSuccess: boolean;
  message: string;
  fen: string;
  legalMoves: Record<string, number[]> | null;
  moveNotations: string[];
  isGameEnded: boolean;
  winner: string | null;
};

export async function startGame(signal?: AbortSignal): Promise<GameResponse> {
  return apiAuthenticatedRequest<GameResponse>(`${API_BASE_URLS.chess}/api/ChessMovement/OnGameStart`, {
    method: "POST",
    body: { isPlayerPlayWhite: true } satisfies GameStartRequest,
    signal
  });
}

export async function makeMove(request: MakeMoveRequest, signal?: AbortSignal): Promise<GameResponse> {
  return apiAuthenticatedRequest<GameResponse>(`${API_BASE_URLS.chess}/api/ChessMovement/MakeMove`, {
    method: "POST",
    body: request,
    signal
  });
}

export async function promotePawn(request: PromotePawnRequest, signal?: AbortSignal): Promise<GameResponse> {
  return apiAuthenticatedRequest<GameResponse>(`${API_BASE_URLS.chess}/api/ChessMovement/PromotePawn`, {
    method: "POST",
    body: request,
    signal
  });
}
