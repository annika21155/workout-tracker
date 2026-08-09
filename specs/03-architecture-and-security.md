# Architecture & Security Decisions

## Authentication: JWT over session cookies

The frontend (Vercel) and backend (Render) are deployed separately and use different domains. Because of this, I chose JWT authentication rather than session cookies.

Using cookies across separate domains would require additional configuration around things like `SameSite`, `Secure`, and cross-origin cookies. JWTs were a simpler option for this setup because the frontend can send the token directly in the `Authorization` header with each request.

### Implementation

* Passwords are hashed using BCrypt (`BCrypt.Net-Next`) before being stored. Plaintext passwords are never stored or logged.
* `TokenService` creates JWTs using `HS256` and a symmetric key stored in configuration as `Jwt:Key`.
* The JWT bearer middleware checks the token's issuer, audience, lifetime, and signing key when requests are authenticated.
* Endpoints that modify data require `[Authorize]`. Public challenge browsing endpoints can be accessed with `[AllowAnonymous]`.
* The current user's identity is taken from the JWT claims rather than from an ID supplied in the request. This helps prevent a user from simply changing an ID in a request to perform an action as another user.

The authentication middleware also needs to be in the correct order in ASP.NET Core:

```text
app.UseCors(...)          // runs before authentication
app.UseAuthentication(); // identifies the user
app.UseAuthorization();  // checks whether they have permission
app.MapControllers();
```

## Real-time updates: SignalR

SignalR was used for the live leaderboard.

When a new `DayCompletion` or `ActivityLog` is created, the `LeaderboardHub` sends a `LeaderboardUpdated` event to connected clients. The clients don't receive the updated leaderboard data directly through SignalR. Instead, they use the event as a signal to request the latest leaderboard from the API.

I chose this approach because it keeps the SignalR hub relatively simple and means the existing leaderboard query can still be used to calculate the current results.

The current implementation broadcasts the update to all connected clients. A more advanced version could use SignalR groups to only notify users who share the relevant friend group, but this wasn't necessary for the scope of the project. The SignalR message itself only indicates that something has changed and doesn't contain private leaderboard information.

## CORS

Because the frontend and backend are hosted separately, CORS needed to be configured on the backend.

A specific list of allowed origins is used rather than allowing every origin. This includes the local Vite development server and the deployed Vercel frontend.

`AllowCredentials()` is also enabled for the SignalR connection. The SignalR setup requires explicit origins rather than using a wildcard `*`.

## Deployment architecture

The application is split into a frontend and backend service:

* **Backend:** A Dockerized .NET 10 API running on Render. The Dockerfile uses a multi-stage build, with the .NET SDK image used to build the application and the smaller ASP.NET runtime image used to run it.
* **Frontend:** A Vite/React application deployed to Vercel as a static build. The API URL is supplied through the `VITE_API_URL` environment variable, allowing the same frontend code to work with either the local or production backend.
* **Database:** SQLite is used for both local development and production.

Database migrations are applied automatically when the backend starts using `db.Database.Migrate()`. This was used because there wasn't a convenient pre-deployment migration step available with the Render setup being used.

Using SQLite in production was a deliberate trade-off. It kept the setup simple and allowed the application to be deployed within the available time, although it has an important limitation described below.

## Known limitations

Some limitations were accepted as part of the project scope rather than being hidden:

* **SQLite on Render's free tier is not persistent.** The free service uses an ephemeral filesystem, meaning the database can be reset when the service restarts or is redeployed. This was acceptable for the timeframe of the submission. A production version would use a persistent database such as managed PostgreSQL instead.
* **Leaderboard updates are broadcast globally.** The current SignalR implementation doesn't restrict notifications to individual friend groups. This was considered acceptable because the notification only tells clients to refresh their data.
* **No automated test suite was created.** Due to the time available, testing was mainly done manually. Backend endpoints were tested using `curl`, while the frontend was tested by walking through the different application flows in the browser.

These limitations were identified during development and deployment and are documented as areas that could be improved in a future version.
