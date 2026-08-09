import { useEffect, useState, type FormEvent } from "react";
import { api } from "../api/client";
import type { FriendshipEntry } from "../api/types";

export function Friends() {
  const [friends, setFriends] = useState<FriendshipEntry[]>([]);
  const [pending, setPending] = useState<FriendshipEntry[]>([]);
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");

  useEffect(() => {
    loadAll();
  }, []);

  async function loadAll() {
    const [f, p] = await Promise.all([
      api.get<FriendshipEntry[]>("/api/friendships"),
      api.get<FriendshipEntry[]>("/api/friendships/pending"),
    ]);
    setFriends(f);
    setPending(p);
  }

  async function handleSend(e: FormEvent) {
    e.preventDefault();
    setMessage("");
    try {
      await api.post("/api/friendships", { friendEmail: email });
      setMessage("Request sent!");
      setEmail("");
    } catch (err) {
      setMessage(err instanceof Error ? err.message : "Failed to send request");
    }
  }

async function handleAccept(id: number) {
    // Optimistically remove from pending immediately so the click feels responsive
    setPending((prev) => prev.filter((p) => p.id !== id));
    try {
      await api.post(`/api/friendships/${id}/accept`);
      await loadAll();
    } catch (err) {
      console.error("Failed to accept friend request:", err);
      alert(err instanceof Error ? err.message : "Failed to accept request");
      loadAll(); // resync with server state either way
    }
  }

  return (
    <div style={{ maxWidth: 480, margin: "2rem auto", padding: "0 1rem" }}>
      <h1>Friends</h1>

      <form onSubmit={handleSend} className="card" style={{ display: "flex", gap: "0.5rem", marginBottom: "1.5rem" }}>
        <input
          type="email"
          placeholder="Friend's email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          style={{ flex: 1 }}
          required
        />
        <button className="btn" type="submit">Add Friend</button>
      </form>
      {message && <p>{message}</p>}

      {pending.length > 0 && (
        <>
          <h3>Pending requests</h3>
          <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem", marginBottom: "1.5rem" }}>
            {pending.map((p) => (
              <div key={p.id} className="card" style={{ display: "flex", justifyContent: "space-between" }}>
                <span>{p.friendUsername}</span>
                <button className="btn" onClick={() => handleAccept(p.id)}>Accept</button>
              </div>
            ))}
          </div>
        </>
      )}

      <h3>Friends</h3>
      {friends.length === 0 ? (
        <p>No friends yet — add one above!</p>
      ) : (
        <div style={{ display: "flex", flexDirection: "column", gap: "0.5rem" }}>
          {friends.map((f) => (
            <div key={f.id} className="card">{f.friendUsername}</div>
          ))}
        </div>
      )}
    </div>
  );
}