import * as signalR from "@microsoft/signalr";
import { API_URL, getToken } from "./client";

let connection: signalR.HubConnection | null = null;

export function getLeaderboardConnection(): signalR.HubConnection {
  if (connection) return connection;
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${API_URL}/hubs/leaderboard`, {
      accessTokenFactory: () => getToken() || "",
    })
    .withAutomaticReconnect()
    .build();
  return connection;
}