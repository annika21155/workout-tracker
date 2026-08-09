# Planning & Pivot

## Original concept

The original Phase 2 idea was a restaurant booking app. Some backend scaffolding was started, but the project wasn't reaching a completed, submittable state within the available time.

With the deadline approaching, I decided that continuing to build on an incomplete concept would leave too little time to properly finish and test the application. Instead, I decided to restart with a smaller concept that could realistically be completed within the remaining time.

## The pivot: Workout Tracker

The replacement idea came from a real use case: following a YouTube "2 Week Workout Challenge" playlist and wanting a simple way to keep track of daily progress, earn points, and compete with a friend.

The initial idea was fairly broad:

* Log different types of activity, including gym sessions, yoga, YouTube workouts, and walking
* Track streaks and points
* Set personal goals
* Challenge friends
* Earn trophies or rewards
* Have a general Strava-like approach to activity tracking

During planning, this was narrowed down to a more focused core loop that could realistically be implemented within the deadline:

* A **Challenge** is a YouTube playlist broken into daily entries, with a video link and duration for each day
* Users can join a challenge individually or with a friend
* Completing a day's video awards points equal to the video's duration
* Friends can compete on a live monthly leaderboard
* Activities outside of challenges can still be recorded manually

The narrower scope made it possible to focus on getting the main user experience working properly rather than having a larger number of partially implemented features.

## Scope cuts made deliberately, and why

Because the project had a fixed deadline, some features were deliberately removed from the scope rather than being started and left unfinished.

| Feature cut                               | Reason                                                                                                                                                                                                            |
| ----------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Real push notifications / email reminders | These would require additional infrastructure such as push notification services, API keys, and scheduled jobs. An in-app "Today's Video" card provided a simpler way to remind users about their daily activity. |
| Strava / Garmin / health app sync         | This would require OAuth and integration with external fitness APIs, which was too large for the available development time.                                                                                      |
| Separate friend-pairing system            | This turned out to be unnecessary. Two users joining the same Challenge already provides a simple way to compete together, without needing another database system.                                               |
| Trophy/reward entities                    | A basic streak-based badge could be calculated from existing participant data, so a separate trophy system was left out rather than adding another set of database entities.                                      |

These cuts were mainly about keeping the project achievable and leaving enough time for testing and deployment.

## Advanced feature selection

Three advanced features were chosen early so they could be considered when designing the application architecture rather than added at the very end:

1. **Security measures** — JWT authentication, BCrypt password hashing, and route authorization
2. **WebSockets** — SignalR for the live leaderboard
3. **Theme switching** — light/dark mode using a shared theme context

The advanced features were chosen because they added meaningful functionality to the application while also demonstrating different technical concepts required for the project.
