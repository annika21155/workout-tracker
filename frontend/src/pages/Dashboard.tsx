import { useEffect, useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { api } from "../api/client";
import type { ChallengeSummary } from "../api/types";

interface DayInput {
  dayNumber: number;
  videoUrl: string;
  videoTitle: string;
  durationMinutes: number;
}

export function Dashboard() {
  const [challenges, setChallenges] = useState<ChallengeSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [joinedIds, setJoinedIds] = useState<Set<number>>(new Set());

  // Form state
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [playlistUrl, setPlaylistUrl] = useState("");
  const [days, setDays] = useState<DayInput[]>([
    { dayNumber: 1, videoUrl: "", videoTitle: "", durationMinutes: 20 },
  ]);
  const [formError, setFormError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    loadChallenges();
  }, []);

  async function loadChallenges() {
    setLoading(true);
    try {
      const data = await api.get<ChallengeSummary[]>("/api/challenges");
      setChallenges(data);
    } finally {
      setLoading(false);
    }
  }

  async function handleJoin(id: number) {
    try {
      await api.post(`/api/challenges/${id}/join`);
      setJoinedIds((prev) => new Set(prev).add(id));
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to join");
    }
  }

  function addDay() {
    setDays((prev) => [
      ...prev,
      { dayNumber: prev.length + 1, videoUrl: "", videoTitle: "", durationMinutes: 20 },
    ]);
  }

  function updateDay(index: number, field: keyof DayInput, value: string | number) {
    setDays((prev) => prev.map((d, i) => (i === index ? { ...d, [field]: value } : d)));
  }

  function removeDay(index: number) {
    setDays((prev) =>
      prev.filter((_, i) => i !== index).map((d, i) => ({ ...d, dayNumber: i + 1 }))
    );
  }

  async function handleCreate(e: FormEvent) {
    e.preventDefault();
    setFormError("");
    setSubmitting(true);
    try {
      await api.post("/api/challenges", {
        title,
        description,
        youtubePlaylistUrl: playlistUrl,
        durationDays: days.length,
        isPublic: true,
        days,
      });
      setTitle("");
      setDescription("");
      setPlaylistUrl("");
      setDays([{ dayNumber: 1, videoUrl: "", videoTitle: "", durationMinutes: 20 }]);
      setShowForm(false);
      loadChallenges();
    } catch (err) {
      setFormError(err instanceof Error ? err.message : "Failed to create challenge");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div style={{ maxWidth: 720, margin: "2rem auto", padding: "0 1rem" }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <h1>Challenges</h1>
        <button className="btn" onClick={() => setShowForm((s) => !s)}>
          {showForm ? "Cancel" : "+ New Challenge"}
        </button>
      </div>

      {showForm && (
        <form onSubmit={handleCreate} className="card" style={{ marginBottom: "1.5rem" }}>
          <h3 style={{ marginTop: 0 }}>Create a challenge from a YouTube playlist</h3>
          <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem" }}>
            <input placeholder="Title (e.g. 2 Week Workout Challenge)" value={title} onChange={(e) => setTitle(e.target.value)} required />
            <input placeholder="Description" value={description} onChange={(e) => setDescription(e.target.value)} />
            <input placeholder="YouTube playlist URL" value={playlistUrl} onChange={(e) => setPlaylistUrl(e.target.value)} required />

            <h4>Days</h4>
            {days.map((day, i) => (
              <div key={i} style={{ display: "flex", gap: "0.5rem", alignItems: "center" }}>
                <span style={{ width: 24 }}>#{day.dayNumber}</span>
                <input
                  placeholder="Video title"
                  value={day.videoTitle}
                  onChange={(e) => updateDay(i, "videoTitle", e.target.value)}
                  style={{ flex: 2 }}
                  required
                />
                <input
                  placeholder="Video URL"
                  value={day.videoUrl}
                  onChange={(e) => updateDay(i, "videoUrl", e.target.value)}
                  style={{ flex: 3 }}
                  required
                />
                <input
                  type="number"
                  placeholder="Minutes"
                  value={day.durationMinutes}
                  onChange={(e) => updateDay(i, "durationMinutes", Number(e.target.value))}
                  style={{ width: 80 }}
                  required
                />
                <button type="button" className="btn-secondary" onClick={() => removeDay(i)}>
                  ✕
                </button>
              </div>
            ))}
            <button type="button" className="btn-secondary" onClick={addDay}>
              + Add day
            </button>

            {formError && <p style={{ color: "var(--danger)" }}>{formError}</p>}
            <button className="btn" type="submit" disabled={submitting}>
              {submitting ? "Creating..." : "Create Challenge"}
            </button>
          </div>
        </form>
      )}

      {loading ? (
        <p>Loading challenges...</p>
      ) : challenges.length === 0 ? (
        <p>No challenges yet — create the first one!</p>
      ) : (
        <div style={{ display: "flex", flexDirection: "column", gap: "0.75rem" }}>
          {challenges.map((c) => (
            <div key={c.id} className="card" style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
              <div>
                <h3 style={{ margin: 0 }}>{c.title}</h3>
                <p style={{ margin: "0.25rem 0", color: "var(--text-secondary)" }}>{c.description}</p>
                <span style={{ fontSize: "0.85rem", color: "var(--text-secondary)" }}>
                  {c.durationDays} days · {c.participantCount} participant{c.participantCount === 1 ? "" : "s"}
                </span>
              </div>
              <div style={{ display: "flex", gap: "0.5rem" }}>
                {!joinedIds.has(c.id) && (
                  <button className="btn-secondary" onClick={() => handleJoin(c.id)}>
                    Join
                  </button>
                )}
                <Link to={`/challenges/${c.id}`}>
                  <button className="btn">View</button>
                </Link>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}