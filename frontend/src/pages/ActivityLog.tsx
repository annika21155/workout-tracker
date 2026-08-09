import { useEffect, useState, type FormEvent } from "react";
import { api } from "../api/client";
import type { ActivityLogEntry } from "../api/types";

const ACTIVITY_TYPES = ["Walk", "Run", "Gym", "Yoga", "Other"];

export function ActivityLog() {
  const [logs, setLogs] = useState<ActivityLogEntry[]>([]);
  const [activityType, setActivityType] = useState(ACTIVITY_TYPES[0]);
  const [duration, setDuration] = useState(20);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadLogs();
  }, []);

  async function loadLogs() {
    setLoading(true);
    const data = await api.get<ActivityLogEntry[]>("/api/activitylogs");
    setLogs(data);
    setLoading(false);
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    await api.post("/api/activitylogs", { activityType, durationMinutes: duration });
    setDuration(20);
    loadLogs();
  }

  async function handleDelete(id: number) {
    await api.del(`/api/activitylogs/${id}`);
    loadLogs();
  }

  return (
    <div style={{ maxWidth: 480, margin: "2rem auto", padding: "0 1rem" }}>
      <h1>Activity Log</h1>
      <form onSubmit={handleSubmit} className="card" style={{ display: "flex", gap: "0.5rem", marginBottom: "1.5rem" }}>
        <select value={activityType} onChange={(e) => setActivityType(e.target.value)}>
          {ACTIVITY_TYPES.map((t) => (
            <option key={t} value={t}>{t}</option>
          ))}
        </select>
        <input
          type="number"
          value={duration}
          onChange={(e) => setDuration(Number(e.target.value))}
          style={{ width: 80 }}
          required
        />
        <button className="btn" type="submit">Log it</button>
      </form>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem" }}>
          {logs.map((log) => (
            <div key={log.id} className="card" style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <span>
                {log.activityType} — {log.durationMinutes} min <span className="pill">{log.pointsEarned} pts</span>
              </span>
              <button className="btn-secondary" onClick={() => handleDelete(log.id)}>Delete</button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}