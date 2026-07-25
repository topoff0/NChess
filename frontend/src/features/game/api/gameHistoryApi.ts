import { apiAuthenticatedRequest } from "@/shared/api/apiRequest";
import { API_BASE_URLS } from "@/shared/api/httpClient";

export type FinishedGame = {
  id: number;
  finalFen: string;
  moves: string[];
  result: string;
  finishedAt: string;
};

type GetFinishedGameResponse = {
  isSuccess: boolean;
  message: string;
  games: FinishedGame[];
};

export async function getFinishedGames(signal?: AbortSignal): Promise<FinishedGame[]> {
  const response = await apiAuthenticatedRequest<GetFinishedGameResponse>(
    `${API_BASE_URLS.chess}/api/ChessMovement/FinishedGames`,
    {
      signal
    }
  );

  return response.games;
}
