# Phase 5 – Social Layer

> Für den vollen Kontext siehe [`README.md`](../../README.md) (Produkt-Roadmap)
> und [`PLAN.md`](../PLAN.md) (Architektur-Entscheidungen). Diese Phase baut
> auf [Phase 4](phase-4-xp-reputation.md) auf.

## Projekt-Kontext (kurz)

Monolith: .NET 10 API (`GameDevsConnect.Api`) + Next.js Frontend + PostgreSQL.
Bisherige Phasen haben Users, Projects, Quests, Contributions und XP
eingeführt. Diese Phase fügt Follow/Feed/Posts/Comments/Likes/Notifications
hinzu – bewusst als **Nebenprodukt** echter Aktivität, nicht als
eigenständiges Twitter-artiges Kernfeature (siehe README §17/§47).

Backend-Pattern (siehe [`PLAN.md`, Abschnitt 4a](../PLAN.md#4a-backend-pattern-im-detail)):
neue Endpoints als MediatR Command/Query + Handler in `Modules/Social/
{Commands,Queries}`, Rückgabetyp `Result<T>`, Entities in
`Modules/Social/Domain`, EF-Konfiguration als `IEntityTypeConfiguration<T>`
in `Modules/Social/Data`. `Notification` liegt analog in `Modules/Notifications`.

## Voraussetzung

Phase 4 ist abgeschlossen: Contributions und XP funktionieren end-to-end.

## Ziel dieser Phase

Nutzer können Usern/Projekten folgen und sehen daraus einen Home-Feed aus
echten Ereignissen. Projekte können Updates posten, die kommentiert und
geliked werden können. Relevante Ereignisse erzeugen In-App-Notifications.

## Nicht Teil dieser Phase

- Keine E-Mail-Benachrichtigungen (README §20: "E-Mail später").
- Kein Realtime/WebSocket-Push – Polling oder einfacher Reload reicht für
  den MVP.
- Kein globaler, von allen Posts gespeister Feed – der Feed besteht aus
  Activity-Events gefolgter User/Projekte (README §17).

## Tasks

### Backend – Datenmodell

- [ ] `Follow` (Id, `FollowerUserId` FK User, `TargetType` enum: `User`,
      `Project`, `TargetId` Guid, `CreatedAt`, unique constraint auf
      (`FollowerUserId`, `TargetType`, `TargetId`)) – **eine** Tabelle für
      beide Follow-Arten (Fix gegenüber der alten Version, die zwei
      getrennte Tabellen ohne Unique-Constraint hatte).
- [ ] `Post` (Id, `ProjectId` FK, `AuthorId` FK User, `Body` text,
      `CreatedAt`, `UpdatedAt`, `IsDeleted` bool) – Projekt-Updates, siehe
      README §18. Autor muss Project-Member sein.
- [ ] `PostAttachment` (Id, `PostId` FK, `StoragePath`, `ContentType`) –
      optionales Bild/Screenshot pro Post (VPS-Volume, gleiches Muster wie
      `SubmissionFile` aus Phase 3).
- [ ] `Comment` (Id, `PostId` FK, `AuthorId` FK User, `Body` text,
      `CreatedAt`, `IsDeleted`) – **eigene** Entity, nicht wie im alten
      Projekt ein `Post` mit `ParentId`.
- [ ] `Like` (`UserId` FK, `PostId` FK – composite PK, `CreatedAt`) –
      korrekter Composite-Key (Fix gegenüber altem `PostLikeDTO` ohne PK,
      das Mehrfach-Likes durch denselben User erlaubte).
- [ ] `ActivityEvent` (Id, `ProjectId` FK nullable, `ActorUserId` FK, `Type`
      enum: `QuestCreated`, `ContributionAccepted`, `MemberJoined`,
      `ProjectPosted`, `LevelUp`, `Payload` jsonb, `CreatedAt`) – zentrale
      Quelle sowohl für den Project-Activity-Tab als auch den Home-Feed.
- [ ] `Notification` (Id, `UserId` FK (Empfänger), `Type` enum,
      `ActivityEventId` FK nullable, `IsRead` bool, `CreatedAt`).
- [ ] Migration erstellen und testen.

### Backend – Logik

- [ ] An den relevanten Stellen aus vorherigen Phasen (Quest erstellt in
      Phase 2, Submission accepted in Phase 3, Member joined in Phase 1,
      Post erstellt in dieser Phase) jeweils einen `ActivityEvent`-Eintrag
      schreiben. Ein einfacher In-Process-Dispatcher reicht (kein Message
      Bus nötig – Monolith!): z. B. eine kleine Liste von Domain-Events, die
      nach `SaveChanges()` verarbeitet wird, oder direkte Service-Aufrufe.
- [ ] Aus relevanten `ActivityEvent`s zusätzlich `Notification`-Einträge für
      betroffene User erzeugen (z. B. Submission-Autor bei Review-Ergebnis,
      Follower bei neuer Quest im gefolgten Projekt, User bei neuem
      Follower).

### Backend – Endpoints

- [ ] `POST /api/users/{username}/follow`, `DELETE /api/users/{username}/follow`
- [ ] `POST /api/projects/{slug}/follow`, `DELETE /api/projects/{slug}/follow`
- [ ] `GET /api/feed` – Home-Feed: `ActivityEvent`s von gefolgten
      Usern/Projekten, paginiert, neueste zuerst.
- [ ] `GET /api/projects/{slug}/activity` – projekt-scoped Activity-Liste.
- [ ] `POST /api/projects/{slug}/posts`, `GET /api/projects/{slug}/posts`,
      `DELETE /api/posts/{id}` (Autor oder Owner/Admin).
- [ ] `POST /api/posts/{id}/comments`, `DELETE /api/comments/{id}`.
- [ ] `POST /api/posts/{id}/like`, `DELETE /api/posts/{id}/like`.
- [ ] `GET /api/notifications` (paginiert), `PATCH /api/notifications/{id}/read`,
      `PATCH /api/notifications/read-all`.

### Frontend

- [ ] `/` – Home-Feed für eingeloggte User (ersetzt den bisherigen
      Platzhalter/Redirect). **Wichtig**: nicht eingeloggte Besucher sehen
      stattdessen eine Landing-/Marketing-Seite, nicht den Feed (Unterschied
      zur alten Version, wo der Feed direkt die Startseite war).
- [ ] Projekt-Seite: "Activity"-Tab (bisher Platzhalter) jetzt mit echten
      `ActivityEvent`s befüllen.
- [ ] Post-Erstellungsformular + Kommentar-Thread + Like-Button auf der
      Projekt-Seite.
- [ ] Notification-Glocke/Dropdown in der Navigation + `/notifications`
      Gesamtliste.
- [ ] Follow-/Unfollow-Buttons auf Profil- und Projekt-Seiten.

## Definition of Done

- [ ] Folgt ein User einem Projekt, tauchen dessen neue Quests und
      angenommene Contributions in seinem Home-Feed auf.
- [ ] Ein Projekt-Update inkl. Kommentar und Like funktioniert
      Ende-zu-Ende.
- [ ] Bei einem Submission-Review entsteht eine Notification für den
      Submission-Autor und lässt sich korrekt als gelesen markieren.
- [ ] Nicht eingeloggte Besucher sehen eine Landing-Page statt des Feeds.
