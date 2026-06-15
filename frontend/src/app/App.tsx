import { AuthPage } from "@/features/auth/pages/AuthPage";
import { GamePage } from "@/features/game/pages/GamePage";
import { getAccessToken } from "@/shared/auth/tokenStorage";
import { useState } from "react";

export const App = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(() => Boolean(getAccessToken()));

  if (isAuthenticated) {
    return <GamePage />;
  }

  return <AuthPage onAuthenticated={() => setIsAuthenticated(true)} />;
};
