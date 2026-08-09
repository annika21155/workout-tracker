using Microsoft.AspNetCore.SignalR;

namespace WorkoutTracker.Api.Hubs;

public class LeaderboardHub : Hub
{
    // Clients don't need to call anything on this hub directly —
    // the server pushes updates to them. This class can stay mostly empty;
    // it exists so SignalR has a named connection point to broadcast through.
}