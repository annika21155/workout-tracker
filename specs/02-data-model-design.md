# Data Model Design

## Entities and reasoning

The database was designed around the main features of the app while trying to keep the number of entities fairly small. The final design uses seven main entities:

**User** — stores the basic account information needed for authentication, including username, email, and the BCrypt password hash.

**Challenge** — represents a reusable workout challenge, such as a "2 Week Workout Challenge". A challenge can be created once and joined by multiple users. It stores information such as the YouTube playlist URL, number of days, and whether the challenge is public or private.

**ChallengeDay** — represents an individual day/video within a challenge. This was kept separate from `Challenge` so that each challenge can have a different number of days, with each day having its own video URL, title, and duration. The duration is also used to determine how many points the user earns.

**ChallengeParticipant** — records which users have joined which challenges. This also ended up being useful for the friend functionality. Rather than creating a separate pairing system, two users participating in the same challenge can effectively compete against each other through their `ChallengeParticipant` records.

**DayCompletion** — records when a participant completes a particular challenge day. `PointsEarned` is stored when the completion is created rather than being calculated every time it is displayed. This means that if the duration of a challenge video is changed later, points that were already earned don't unexpectedly change.

**ActivityLog** — stores activities that aren't part of a challenge, such as going for a walk or run. The points are also stored when the activity is logged so that the historical value doesn't change later.

**Friendship** — stores friend requests and accepted friendships. It uses `UserId` for the person sending the request, `FriendUserId` for the recipient, and a status such as `Pending` or `Accepted`.

## Deliberately not modeled

A couple of possible entities were considered but weren't needed:

* **Reward/Trophy table** — a basic badge or reward can be calculated from existing streak information such as `ChallengeParticipant.CurrentStreak`, so another database table wasn't necessary.
* **Separate Pairing entity** — this wasn't needed because `ChallengeParticipant` already provides a way for two users to participate in and compete within the same challenge.

Keeping these out of the database helped avoid adding tables for features that could be handled using the existing data.

## Relationships & cascade behaviour

The main relationships were configured as follows:

* `Challenge → ChallengeDay`: cascade delete, because challenge days belong entirely to their challenge
* `Challenge → ChallengeParticipant`: cascade delete
* `User → ChallengeParticipant`: cascade delete
* `ChallengeParticipant → DayCompletion`: cascade delete
* `ChallengeDay → DayCompletion`: cascade delete
* `User → ActivityLog`: cascade delete
* `Challenge → CreatedByUser`: **restrict**, so deleting a user doesn't automatically delete a challenge that other users may still be using
* `Friendship`'s two `User` foreign keys: both **restrict**, avoiding multiple cascade paths back to the `User` table

The cascade rules were chosen based on whether the related data should still exist if its parent record is deleted.

## Leaderboard query design

A separate leaderboard table wasn't needed. Instead, the monthly friends leaderboard is calculated from the existing completion and activity data when it is requested.

The basic process is:

1. Find the current user's accepted friendships and include the current user as well.
2. Find their `DayCompletion` points through `ChallengeParticipant` and their `ActivityLog` points.
3. Filter both sets of points to the current calendar month.
4. Add the points for each user and sort them from highest to lowest.
5. When a new completion or activity is recorded, SignalR sends a refresh notification to connected clients so they can request the updated leaderboard.

This approach meant the leaderboard didn't need its own stored data, reducing the amount of duplicated information in the database while still allowing it to update in real time.
