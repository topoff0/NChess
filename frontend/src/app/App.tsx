import { getCurrentUser } from "@/features/auth/api/accountApi";
import { AuthLayout } from "@/features/auth/components/AuthLayout";
import { AuthPage } from "@/features/auth/pages/AuthPage";
import { CreateProfilePage } from "@/features/auth/pages/CreateProfilePage";
import { GameHistoryPage } from "@/features/game/pages/GameHistoryPage";
import { GamePage } from "@/features/game/pages/GamePage";
import { clearAccessToken, getAccessToken } from "@/shared/auth/tokenStorage";
import { useEffect, useState } from "react";

type AuthState = "checking" | "guest" | "profileRequired" | "authenticated";
type GameView = "game" | "history";

export const App = () => {
  const [authState, setAuthState] = useState<AuthState>(() => (getAccessToken() ? "checking" : "guest"));
  const [gameView, setGameView] = useState<GameView>("game");

  useEffect(() => {
    if (authState !== "checking") {
      return;
    }

    const checkCurrentUser = async () => {
      try {
        const currentUser = await getCurrentUser();

        if (currentUser.isProfileCreated) {
          setAuthState("authenticated");
          return;
        }

        setAuthState("profileRequired");
      } catch {
        clearAccessToken();
        setAuthState("guest");
      }
    };

    void checkCurrentUser();
  }, [authState]);

  const handleLogout = () => {
    clearAccessToken();
    setAuthState("guest");
  };

  if (authState === "checking") {
    return (
      <main className="flex min-h-screen items-center justify-center bg-cream text-wood-dark">
        <p className="text-xl font-black">Loading...</p>
      </main>
    );
  }

  if (authState === "authenticated") {
    return gameView === "history" ? (
      <GameHistoryPage onBack={() => setGameView("game")} />
    ) : (
      <GamePage onLogout={handleLogout} onShowHistory={() => setGameView("history")} />
    );
  }

  if (authState === "profileRequired") {
    return (
      <AuthLayout>
        <CreateProfilePage onCreated={() => setAuthState("authenticated")} />
      </AuthLayout>
    );
  }

  if (authState === "guest") {
    return <AuthPage onAuthenticated={() => setAuthState("authenticated")} />;
  }
};
