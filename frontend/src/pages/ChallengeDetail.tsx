import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { api } from "../api/client";
import type { ChallengeDetail as ChallengeDetailType, ChallengeDay } from "../api/types";

export function ChallengeDetail() {
  const { id } = useParams<{ id: string }>();
  const [challenge, setChallenge] = useState<ChallengeDetailType | null>(null);
  const [todayVideo, setTodayVideo] = useState<ChallengeDay | null>(null);
  const [notJoined, setNotJoined] = useState(false);
  const [complete, setComplete] = useState(false);
  const [completing, setCompleting] = useState(false);
  const [streak, setStreak] = useState<number | null>(null);

  useEffect(() => {
    if (!id) return;
    loadChallenge();
    loadToday();
  }, [id]);

  async function loadChallenge() {
    const data = await api.get<ChallengeDetailType>(`/api/challenges/${id}`);
    setChallenge(data);
  }

  async function loadToday() {
    setNotJoined(false);
    setComplete(false);
    try {
      const data = await api.get<ChallengeDay | { message: string }>(`/api/challenges/${id}/today`);
      if ("message" in data) {
        setComplete(true);
        setTodayVideo(null);
      } else {
        setTodayVideo(data);
      }
    } catch {
      setNotJoined(true);
    }
  }

  async function handleJoin() {
    if (!id) return;
    await api.post(`/api/challenges/${id}/join`);
    loadToday();
  }

  async function handleComplete() {
    if (!id || !todayVideo) return;
    setCompleting(true);
    try {
      const res = await api.post<{ pointsEarned: number; currentStreak: number }>(
        `/api/challenges/${id}/days/${todayVideo.id}/complete`
      );
      setStreak(res.currentStreak);
      loadToday();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to mark complete");
    } finally {
      setCompleting(false);
    }
  }

  if (!challenge) return <p style={{ padding: "2rem" }}>Loading...</p>;

  return (
    <div style={{ maxWidth: 640, margin: "2rem auto", padding: "0 1rem" }}>
      <h1>{challenge.title}</h1>
      <p style={{ color: "var(--text-secondary)" }}>{challenge.description}</p>

      {notJoined && (
        <div className="card">
          <p>You haven't joined this challenge yet.</p>
          <button className="btn" onClick={handleJoin}>
            Join Challenge
          </button>
        </div>
      )}

      {!notJoined && complete && (
        <div className="card" style={{ borderColor: "var(--success)" }}>
          <p>🎉 You've completed every day in this challenge!</p>
        </div>
      )}

      {!notJoined && todayVideo && (
        <div className="card">
          <h3 style={{ marginTop: 0 }}>Today's Video — Day {todayVideo.dayNumber}</h3>
          <p style={{ fontWeight: 600 }}>{todayVideo.videoTitle}</p>
          <p style={{ color: "var(--text-secondary)" }}>{todayVideo.durationMinutes} minutes</p>
          <a href={todayVideo.videoUrl} target="_blank" rel="noreferrer">
            <button className="btn-secondary" style={{ marginRight: "0.5rem" }}>
              Watch on YouTube
            </button>
          </a>
          <button className="btn" onClick={handleComplete} disabled={completing}>
            {completing ? "Saving..." : "Mark Complete"}
          </button>
          {streak !== null && (
            <p style={{ marginTop: "0.5rem", color: "var(--success)" }}>Current streak: {streak} 🔥</p>
          )}
        </div>
      )}

      <h3 style={{ marginTop: "2rem" }}>All Days</h3>
      <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem" }}>
        {challenge.days
          .sort((a, b) => a.dayNumber - b.dayNumber)
          .map((d) => (
            <div key={d.id} className="card">
              Day {d.dayNumber}: {d.videoTitle} — {d.durationMinutes} min
            </div>
          ))}
      </div>
    </div>
  );
}