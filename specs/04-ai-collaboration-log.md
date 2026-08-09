# AI Collaboration Log

AI tools were used throughout the development of this project as a coding and problem-solving aid. Claude (Anthropic) was used to help discuss ideas, explain unfamiliar concepts, generate some initial code and boilerplate, and troubleshoot errors during development.

The project was developed iteratively rather than being generated all at once. Suggested code and solutions were tested locally, errors were investigated, and changes were made based on the actual behaviour of the application. This log summarises the main areas where AI assistance was used.

## 1. Concept refinement

The project started from a fairly loose idea: a gamified fitness app involving YouTube playlists, reminders, points, a leaderboard, and the ability to compete with a friend.

Claude was used during the planning stage to help break this idea into smaller features and consider what was realistic to implement within the available time. This included discussing questions such as how the daily reminder should work and whether the leaderboard should be global, challenge-specific, or limited to friends.

Some of the decisions made during this stage were:

* Daily reminder = an in-app "Today's Video" card rather than real push notifications, as push notifications would have added significant implementation work
* Leaderboard = monthly, friends-only, and shared across all challenges

These were scope decisions made during planning based on the available development time and the requirements of the project.

## 2. Backend development

Claude was used alongside the backend development rather than being responsible for the entire backend. It was particularly useful for getting initial structures in place, explaining framework behaviour, and helping investigate errors.

Areas where it was used included:

* The initial 7-entity data model and `AppDbContext`, including EF Core relationships and cascade-delete behaviour
* JWT authentication, including the DTOs, token service, authentication service, and controller structure
* Explaining the ASP.NET Core middleware ordering required for authentication and authorization (`UseAuthentication` → `UseAuthorization` → `MapControllers`)
* Initial CRUD controller structure for Challenges, ActivityLogs, Friendships, and Leaderboard
* A helper extension method for retrieving the current user's ID from JWT claims
* SignalR hub setup and the CORS configuration needed for cross-origin communication

The code was run and checked locally throughout development. When something failed, terminal output, browser errors, and screenshots were sometimes used to help identify the problem. Changes were then tested again rather than assuming the suggested solution was correct.

The general development loop was:

**implement → run → find an issue → investigate → change the code → test again**

## 3. Frontend development

The frontend was also built incrementally using Vite, React, and TypeScript.

Claude was used for some of the initial project setup and to help with implementation and debugging. The main areas included:

* Vite/React/TypeScript project setup
* API client
* Authentication context
* Theme context
* Dashboard and challenge creation
* Challenge Detail and the "Today's Video" flow
* Live Leaderboard using SignalR
* Activity Log
* Friends
* Application routing in `App.tsx`

The features were developed and tested in stages rather than creating the entire frontend in one pass. This made it possible to find and fix problems as individual features were added.

## 4. Debugging examples

A significant part of the AI assistance involved interpreting errors and suggesting possible causes or fixes. Some representative examples were:

* A missing EF Core migration causing `SQLite Error 1: no such table: Users` when the application was first run
* A `verbatimModuleSyntax` TypeScript configuration issue requiring the `type` keyword for type-only imports such as `ReactNode` and `FormEvent`
* A duplicate constructor left behind after `IHubContext` was added as another controller dependency
* A JWT token being copied incompletely into a `curl` request, which caused authentication to fail

In each case, the error was reproduced or checked in the project before the suggested fix was accepted.

## 5. Deployment debugging

Deployment introduced several problems that hadn't appeared during local development. AI assistance was useful for working through the error messages and narrowing down possible causes.

### CORS failure

The deployed frontend was hosted at a different URL from the backend, so the production Vercel URL needed to be added to the backend's allowed-origin list.

### JWT signing failure

The deployed backend initially returned a generic 500 error. Looking through the Render deployment logs revealed:

`IDX10720: key size must be greater than 256 bits, key has 184 bits`

The problem was traced back to the JWT secret being incorrectly copied into the Render environment variable. Replacing it with the correct value fixed the authentication issue.

### Native container crash

The most difficult deployment problem was a container crash with exit code 139, which pointed to a segmentation fault involving `PhysicalFilesWatcher`.

An environment-variable based fix was tried first:

`hostBuilder__reloadConfigOnChange=false`

However, this didn't reliably solve the problem. After investigating how ASP.NET Core was loading its configuration, the fix was moved into `Program.cs`, where `reloadOnChange: false` was explicitly set on the configuration sources. This resolved the crash.

This was a useful example of how the deployment environment could behave differently from the local development environment and required investigation rather than simply applying the first suggested fix.

## 6. Human decisions throughout

Although AI was used extensively during development, the final project decisions were made based on the requirements, available time, and testing of the application.

Some of the main decisions included:

* Choosing SQLite instead of PostgreSQL for simplicity during development, while accepting the persistence limitations of the deployment environment
* Prioritising the core functionality before spending time on visual styling
* Providing a visual moodboard with a warm, earthy, Headspace-inspired direction for the planned styling
* Deciding which suggested implementations and fixes were appropriate for the project
* Configuring deployment settings and environment variables
* Testing the application locally and in production and making changes when issues appeared

Overall, Claude was used as a development assistant throughout the project. It was particularly useful for explaining technical concepts, getting initial code structures in place, and helping troubleshoot problems, while the implementation still involved ongoing testing, debugging, and decision-making.
