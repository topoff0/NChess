import { createContext, useContext, useEffect, useMemo, useState } from "react";

type AuthContextType = {
  isAuthenticated: boolean;
  login: (token: string) => void;
  logout: () => void;
};

export const AUTH_TOKEN_KEY = "nchess_auth_token";

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem(AUTH_TOKEN_KEY);
    setIsAuthenticated(Boolean(token));
  }, []);

  const login = (token: string) => {
    localStorage.setItem(AUTH_TOKEN_KEY, token);
    setIsAuthenticated(true);
  }
  const logout = () => {
    localStorage.removeItem(AUTH_TOKEN_KEY);
    setIsAuthenticated(false);
  };

  const value = useMemo(() => ({
    isAuthenticated,
    login,
    logout,
  }),
    [isAuthenticated],);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
};
