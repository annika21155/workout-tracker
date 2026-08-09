# AI Collaboration Log

AI tools were used throughout the development of this project as a coding and problem-solving aid. Claude (Anthropic) was used for things such as discussing design decisions, generating some boilerplate code, explaining errors, and helping debug issues. I still made the project decisions, ran and tested the code locally, and iterated on the implementation based on what worked.

This log gives an overview of where AI was used during development.

## 1. Concept refinement

The project started as a rough idea: a gamified fitness app involving YouTube playlists, reminders, points, and a leaderboard that could be used with friends.

Claude was used to help turn the initial idea into a more practical scope and to think through some of the design questions before development began. This included discussing how the daily video reminder could work within the available development time and what type of leaderboard made the most sense.

Some of the decisions made during this stage were:

* Daily reminder = an in-app "Today's Video" card rather than real push notifications, keeping the feature achievable within the project scope
* Leaderboard = monthly, friends-only, and shared across all challenges

These were ultimately project and scope decisions rather than things automatically decided by the AI.

## 2. Backend development

Claude was used alongside the backend development process, particularly for scaffolding, explaining unfamiliar concepts, and troubleshooting errors.

Some of the areas where it helped included:

* Setting up the 7-entity data model and `AppDbContext`, including EF Core relationships and cascade-delete behaviour
* Structuring the JWT authentication system, including DTOs, `TokenService`, `AuthService`, and `AuthController`
* Explaining the required ASP.NET Core middleware order (`UseAuthentication` → `UseAuthorization` → `MapControllers`)
* Getting the initial CRUD controllers working for Challenges, ActivityLogs, Friendships, and Leaderboard
* Setting up a helper extension method for retrieving the current user's ID from JWT claims
* Setting up the SignalR hub and the CORS configuration needed for cross-origin communication

Code suggested by Claude was run and tested locally rather than being assumed to work. Build errors, warnings, and runtime exceptions were investigated during development, with terminal output and screenshots sometimes used to help identify the cause.

The general workflow was:

**implement → run → encounter an issue → investigate → make changes → test again**

## 3. Frontend development

The frontend was developed in stages using Vite, React, and TypeScript.

Claude was used as a reference when setting up parts of the project structure and when troubleshooting TypeScript or React issues. The main areas included:

* API client setup
* Authentication and auth state
* Theme context
* Dashboard and challenge creation
* Challenge Detail and the "Today's Video" feature
* Live Leaderboard using SignalR
* Activity Log
* Friends
* App routing

The frontend was built incrementally rather than generating the entire application in one step. Features were tested as they were added and adjusted when problems appeared.

## 4. Debugging examples

Several issues came up during development. Claude was useful for interpreting error messages and suggesting possible fixes, which were then tested in the project.

Some representative examples include:

* A missing EF Core migration causing `SQLite Error 1: no such table: Users` on the first run
* A `verbatimModuleSyntax` TypeScript configuration issue requiring the `type` keyword for type-only imports such as `ReactNode` and `FormEvent`
* A duplicate constructor left behind after adding `IHubContext` as a dependency to a controller
* A JWT token being copied incompletely into a `curl` request, resulting in an authentication failure

These were resolved through a combination of checking the project code, reading the error output, making changes, and testing the result.

## 5. Deployment debugging

Some of the most useful debugging happened after the application was deployed, as several issues only became apparent in the production environment.

### CORS failure

The deployed frontend's Vercel URL needed to be added to the backend's allowed origins. This was identified from the browser's CORS errors and corrected in the backend configuration.

### JWT signing failure

The deployed backend initially returned a generic 500 error. Checking the Render deployment logs revealed:

`IDX10720: key size must be greater than 256 bits, key has 184 bits`

The issue was traced to the JWT secret being incorrectly copied into Render's environment variables. Replacing it with the correct value resolved the problem.

### Native container crash

A more difficult issue involved the application container exiting with code 139 due to a crash inside `PhysicalFilesWatcher`.

An environment-variable configuration change was initially attempted, but it did not reliably resolve the issue. The configuration pipeline was eventually changed directly in `Program.cs`, explicitly setting `reloadOnChange: false` for the relevant configuration sources. This removed the timing dependency and resolved the crash.

## 6. Human decisions and contributions

AI suggestions were treated as a development aid rather than as automatic decisions. The project direction, scope, implementation choices, and final changes were reviewed and tested during development.

Some notable decisions included:

* Choosing SQLite over PostgreSQL for local simplicity, while accepting its persistence limitations for the deployed application
* Prioritising core functionality before spending time on visual design
* Creating a visual direction based on a warm, earthy, Headspace-inspired moodboard for a future styling pass
* Deciding which suggested fixes to implement after testing them
* Configuring deployment settings and environment variables
* Testing the application throughout development and resolving issues as they appeared

Overall, Claude was used as a coding assistant and debugging resource throughout the project. It helped with ideas, explanations, boilerplate, and troubleshooting, but the development process remained iterative and involved testing and making decisions based on the actual behaviour of the application.
