import { getFinishedGames, type FinishedGame } from "@/features/game/api/gameHistoryApi";
import { useEffect, useState } from "react";

type GameHistoryState =
  { status: "loading" } | { status: "error"; message: string } | { status: "ready"; games: FinishedGame[] };

export const useGameHistory = () => {
  const [state, setState] = useState<GameHistoryState>({ status: "loading" });

  useEffect(() => {
    const controller = new AbortController();

    const loadHistory = async () => {
      try {
        const games = await getFinishedGames(controller.signal);
        setState({ status: "ready", games });
      } catch (error) {
        if (controller.signal.aborted) {
          return;
        }

        setState({
          status: "error",
          message: error instanceof Error ? error.message : "Failed to load your games"
        });
      }
    };

    void loadHistory();

    return () => controller.abort();
  }, []);

  return state;
};
