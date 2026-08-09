import { createContext, useContext, useState, type ReactNode } from "react";
import { api } from "../api/client";

interface User {
  userId: number;
  username: string;
  email: string;
}

interface AuthResponse extends User {
  token: string;
}

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(() => {
    const stored = localStorage.getItem("user");
    return stored ? JSON.parse(stored) : null;
  });

  function saveSession(res: AuthResponse) {
    localStorage.setItem("token", res.token);
    const userData = { userId: res.userId, username: res.username, email: res.email };
    localStorage.setItem("user", JSON.stringify(userData));
    setUser(userData);
  }

  async function login(email: string, password: string) {
    const res = await api.post<AuthResponse>("/api/auth/login", { email, password });
    saveSession(res);
  }

  async function register(username: string, email: string, password: string) {
    const res = await api.post<AuthResponse>("/api/auth/register", { username, email, password });
    saveSession(res);
  }

  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ user, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
}