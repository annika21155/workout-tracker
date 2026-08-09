import { useEffect, useState } from "react";
import { api } from "../api/client";
import { getLeaderboardConnection } from "../api/signalr";
import type { LeaderboardEntry } from "../api/types";

export function Leaderboard() {
  const [entries, setEntries] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadLeaderboard();

    const connection = getLeaderboardConnection();
    connection.on("LeaderboardUpdated", loadLeaderboard);

    if (connection.state === "Disconnected") {
      connection.start().catch((err) => console.error("SignalR connection failed:", err));
    }

    return () => {
      connection.off("LeaderboardUpdated", loadLeaderboard);
    };
  }, []);

  async function loadLeaderboard() {
    const data = await api.get<LeaderboardEntry[]>("/api/leaderboard");
    setEntries(data);
    setLoading(false);
  }

  return (
    <div style={{ maxWidth: 480, margin: "2rem auto", padding: "0 1rem" }}>
      <h1>Leaderboard</h1>
      <p style={{ color: "var(--text-secondary)" }}>This month, friends only. Updates live.</p>
      {loading ? (
        <p>Loading...</p>
      ) : entries.length === 0 ? (
        <p>No points logged yet this month.</p>
      ) : (
        <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem" }}>
          {entries.map((e) => (
            <div key={e.userId} className="card" style={{ display: "flex", justifyContent: "space-between" }}>
              <span>#{e.rank} {e.username}</span>
              <span style={{ fontWeight: 700 }}>{e.totalPoints} pts</span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}