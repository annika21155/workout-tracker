import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { useTheme } from "../context/ThemeContext";

export function Nav() {
  const { user, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  if (!user) return null;

  return (
    <nav
      style={{
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        padding: "0.75rem 1.5rem",
        borderBottom: "1px solid var(--border)",
      }}
    >
      <div style={{ display: "flex", gap: "1.25rem", alignItems: "center" }}>
        <Link to="/" style={{ fontWeight: 700, textDecoration: "none" }}>
          Workout Tracker
        </Link>
        <Link to="/">Dashboard</Link>
        <Link to="/leaderboard">Leaderboard</Link>
        <Link to="/activity">Activity Log</Link>
        <Link to="/friends">Friends</Link>
      </div>
      <div style={{ display: "flex", gap: "0.75rem", alignItems: "center" }}>
        <span style={{ color: "var(--text-secondary)", fontSize: "0.9rem" }}>
          {user.username}
        </span>
        <button className="btn-secondary" onClick={toggleTheme}>
          {theme === "light" ? "🌙 Dark" : "☀️ Light"}
        </button>
        <button className="btn-secondary" onClick={handleLogout}>
          Log out
        </button>
      </div>
    </nav>
  );
}