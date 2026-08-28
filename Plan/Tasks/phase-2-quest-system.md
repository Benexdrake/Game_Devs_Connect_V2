# Phase 2 – Quest System

> Für den vollen Kontext siehe [`README.md`](../../README.md) (Produkt-Roadmap)
> und [`PLAN.md`](../PLAN.md) (Architektur-Entscheidungen). Diese Phase baut
> auf [Phase 1](phase-1-foundation.md) auf.

## Projekt-Kontext (kurz)

Monolith: .NET 10 API (`GameDevsConnect.Api`, Module unter `Modules/<Name>/`) +
Next.js App Router Frontend (`frontend/`) + PostgreSQL. Aus Phase 1 existieren
bereits `User`, `Skill`, `Project`, `ProjectMember` (Rollen Owner/Admin/
Contributor).

Backend-Pattern (siehe [`PLAN.md`, Abschnitt 4a](../PLAN.md#4a-backend-pattern-im-detail)):
neue Endpoints als MediatR Command/Query + Handler in `Modules/Quests/
{Commands,Queries}`, Rückgabetyp `Result<T>`, Entities in `Modules/Quests/Domain`,
EF-Konfiguration als `IEntityTypeConfiguration<T>` in `Modules/Quests/Data`.

## Voraussetzung

Phase 1 ist abgeschlossen: Projekte mit Mitgliedern/Rollen funktionieren.

## Ziel dieser Phase

Ein Project Owner/Admin kann eine Quest erstellen, andere User können sie
finden (Liste + Filter) und beanspruchen ("claimen"). Die eigentliche
Abgabe/Review (Submission) ist **nicht** Teil dieser Phase, siehe Phase 3.

## Nicht Teil dieser Phase

- Keine Submissions/Contributions (Phase 3) – der Quest-Lifecycle endet in
  dieser Phase bei `InProgress`/`Cancelled`, die Status `Submitted`,
  `InReview`, `ChangesRequested`, `Accepted`, `Rejected` werden erst in
  Phase 3 verdrahtet (Enum-Werte dürfen aber schon jetzt vollständig
  angelegt werden, siehe unten).
- Keine XP-Vergabe (Phase 4).
- Kein Multi-Contributor-Handling über `MaxContributors=1` hinaus (Feld
  existiert, Logik für mehrere gleichzeitige Claims ist optional/kann
  simpel bleiben).

## Tasks

### Backend – Datenmodell

- [ ] `Quest` (Id, `ProjectId` FK, `CreatorId` FK User, `Title`,
      `Description`, `Category` enum – gleiche Werte wie `Skill.Category`
      aus Phase 1, `Difficulty` enum: `Easy`, `Medium`, `Hard`, `XpReward`
      int, `Status` enum mit **allen** Werten aus dem Produkt-Roadmap
      (`Open`, `Claimed`, `InProgress`, `Submitted`, `InReview`,
      `ChangesRequested`, `Accepted`, `Rejected`, `Cancelled`) – auch wenn
      diese Phase nur bis `InProgress`/`Cancelled` aktiv nutzt,
      `Deadline` nullable, `MaxContributors` int default 1, `CreatedAt`,
      `UpdatedAt`).
- [ ] `QuestSkill` (QuestId FK, SkillId FK – composite PK) – benötigte
      Skills für die Quest (für Filterung/Matching).
- [ ] `QuestAssignment` (Id, QuestId FK, UserId FK, `ClaimedAt`,
      `ReleasedAt` nullable) – Historie aller Claims, auch freigegebener.
- [ ] Migration erstellen und testen.

### Backend – Endpoints & Regeln

- [ ] `POST /api/projects/{slug}/quests` – nur Owner/Admin. Body: Titel,
      Beschreibung, Category, Difficulty, XpReward, Deadline?,
      MaxContributors?, `requiredSkillIds[]`.
- [ ] `PATCH /api/projects/{slug}/quests/{questId}` – nur Owner/Admin, nur
      solange `Status = Open`.
- [ ] `DELETE /api/projects/{slug}/quests/{questId}` – hart löschen nur wenn
      nie geclaimt wurde; sonst stattdessen `Status = Cancelled` setzen
      (Soft-Delete für Nachvollziehbarkeit).
- [ ] `GET /api/quests` – öffentliche Liste, Query-Parameter: `search`,
      `category`, `skillId`, `projectSlug`, `difficulty`, `minXp`,
      `engine` (join über `Project.Engine`). Nur `Visibility=Public`-Projekte
      bzw. Quests aus Projekten, in denen der aktuelle User Member ist.
- [ ] `GET /api/quests/{questId}` – Detailansicht (gleiche Sichtbarkeitsregel
      wie oben).
- [ ] `POST /api/quests/{questId}/claim` – **Anti-Abuse-Regel**: Der
      Quest-`CreatorId` darf seine eigene Quest **nicht** claimen (kommt aus
      Produkt-Roadmap §35, harte Regel, kein Soft-Warning). Setzt
      `Status = InProgress` (Vereinfachung: `Claimed` als eigener
      Zwischenzustand wird für den MVP übersprungen, `QuestAssignment` hält
      trotzdem `ClaimedAt` fest) und legt `QuestAssignment` an. Schlägt fehl
      (409), wenn `Status != Open` oder `MaxContributors` bereits erreicht.
- [ ] `POST /api/quests/{questId}/release` – der aktuelle Claimer gibt die
      Quest zurück: `ReleasedAt` setzen, `Status` zurück auf `Open` (wenn
      dadurch keine aktiven Claims mehr existieren).

### Frontend

- [ ] `/quests` – SSR Discover-Quests-Seite mit Filtern (Category, Skill,
      Difficulty, Projekt, Min-XP), siehe Mockup in README §15.
- [ ] `/quests/[id]` – Detailseite: Beschreibung, benötigte Skills,
      XP-Reward, Status, "Claim Quest"-Button (deaktiviert/ausgeblendet für
      den Ersteller, wenn bereits geclaimt, oder wenn `Status != Open`).
- [ ] Projekt-Seite (`/projects/[slug]`): "Quests"-Tab jetzt mit echten
      Daten befüllen; Owner/Admin sehen zusätzlich "Neue Quest"-Button sowie
      Edit-/Cancel-Aktionen pro Quest.
- [ ] `/projects/[slug]/quests/new` und `/quests/[id]/edit` – Formulare.

## Definition of Done

- [ ] Owner kann unter seinem Projekt eine Quest mit Skill-Tags, Difficulty
      und XP-Reward anlegen.
- [ ] Die Quest erscheint in `/quests` und die Filter (Kategorie, Skill,
      Projekt, Difficulty, Min-XP) funktionieren korrekt.
- [ ] Ein anderer (nicht-Owner) User kann die Quest claimen; der Ersteller
      selbst kann es nicht (Server lehnt mit klarer Fehlermeldung ab, nicht
      nur UI-seitig versteckt).
- [ ] Nach dem Claim verschwindet die Quest aus der Standard-"Open"-Filteransicht.
- [ ] Ein Claim kann wieder freigegeben werden, danach ist die Quest erneut
      claimbar.
