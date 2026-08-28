# Phase 1 – Foundation (User-Profile, Skills, Projects)

> Für den vollen Kontext siehe [`README.md`](../../README.md) (Produkt-Roadmap)
> und [`PLAN.md`](../PLAN.md) (Architektur-Entscheidungen). Diese Phase baut
> auf [Phase 0](phase-0-projekt-fundament.md) auf.

## Projekt-Kontext (kurz)

Monolith: .NET 10 API (`GameDevsConnect.Api`, Module unter `Modules/<Name>/`) +
Next.js App Router Frontend (`frontend/`) + PostgreSQL. Auth läuft über GitHub
OAuth (siehe Phase 0) und liefert bereits eine minimale `User`-Entity
(`Id`, `GitHubId`, `Username`, `AvatarUrl`, `CreatedAt`) sowie
Cookie-basierte Sessions sowie `GET /api/auth/me`.

Jeder neue Endpoint in dieser Phase wird als MediatR Command/Query + Handler
in `Modules/<Modul>/{Commands,Queries}` umgesetzt, mit `Result<T>` als
Rückgabetyp (kein Exception-basiertes Fehlerhandling für erwartbare Fälle wie
"nicht gefunden" oder "keine Berechtigung"), Entities in `Modules/<Modul>/Domain`,
EF-Konfiguration in `Modules/<Modul>/Data` als `IEntityTypeConfiguration<T>`.
Details/Beispiel: [`PLAN.md`, Abschnitt 4a](../PLAN.md#4a-backend-pattern-im-detail).

## Voraussetzung

Phase 0 ist abgeschlossen: Login/Logout funktioniert, `User`-Tabelle existiert.

## Ziel dieser Phase

Ein eingeloggter User kann sein Profil ausbauen (Bio, Links, Skills) und ein
Projekt anlegen, verwalten und Mitglieder mit Rollen einladen. Öffentliche
Profil- und Projekt-Seiten sind für nicht eingeloggte Besucher sichtbar
(SSR, SEO).

## Nicht Teil dieser Phase

- Keine Quests (Phase 2), keine Contributions/XP (Phase 3/4).
- Keine Activity-Feeds/Posts (Phase 5) – Tabs dafür dürfen als leere
  Platzhalter existieren.
- Keine Skill-Proficiency-Level (nur "hat Skill X" / "hat nicht Skill X").

## Tasks

### Backend – Datenmodell

- [ ] `User` erweitern: `Bio` (text, nullable), `UpdatedAt`.
- [ ] `UserLink` (Id, UserId FK, Label string, Url string) – generische
      Social-/Website-Links statt fester Spalten pro Plattform.
- [ ] `Skill` (Id, Name unique, Category enum: `Programming`, `Art2D`,
      `Art3D`, `Animation`, `Audio`, `Design`, `Writing`, `Other`). Mit ein
      paar Seed-Werten befüllen (Unity, Unreal, Godot, C#, C++, Blender,
      Maya, Photoshop, 2D Art, 3D Art, Animation, Rigging, Music, Sound
      Design, Game Design, Level Design, Writing).
- [ ] `UserSkill` (UserId FK, SkillId FK – composite PK).
- [ ] `Tag` (Id, Name unique) – freie Projekt-Labels (Genre/Thema), getrennt
      von `Skill`.
- [ ] `Project` (Id, `Slug` unique+url-safe, `Title`, `Description`,
      `LogoUrl` nullable, `BannerUrl` nullable, `Engine` string nullable,
      `Genre` string nullable, `Status` enum: `Concept`, `InDevelopment`,
      `Beta`, `Released`, `Archived`, `Visibility` enum: `Public`,
      `Private`, `CreatedAt`, `UpdatedAt`). **Kein** separates `OwnerId`-Feld
      – der Owner ergibt sich aus `ProjectMember` mit `Role = Owner`
      (verhindert Inkonsistenz zwischen zwei Owner-Quellen).
- [ ] `ProjectTag` (ProjectId FK, TagId FK – composite PK).
- [ ] `ProjectMember` (ProjectId FK, UserId FK – composite PK, `Role` enum:
      `Owner`, `Admin`, `Contributor`, `JoinedAt`). Anwendungslogik muss
      sicherstellen: **genau ein** `Owner` pro Projekt zu jedem Zeitpunkt.
- [ ] Migration erstellen und gegen lokale DB testen.

### Backend – Endpoints

- [ ] `GET /api/users/{username}` – öffentliches Profil (Bio, Links, Skills,
      Projekte als Member/Owner). Kein Auth nötig.
- [ ] `PATCH /api/users/me` – eigenes Profil bearbeiten (Bio, Links).
- [ ] `GET /api/skills` – Liste aller Skills (für Auswahl-UI).
- [ ] `PUT /api/users/me/skills` – Skill-Set des eigenen Profils setzen
      (ersetzt die komplette Liste, einfacher als Einzel-Add/Remove).
- [ ] `POST /api/projects` – erstellt Projekt, Ersteller wird automatisch
      `ProjectMember` mit `Role=Owner`. Slug wird aus `Title` generiert falls
      nicht angegeben, muss eindeutig sein (bei Kollision Suffix anhängen
      oder 409 zurückgeben – Entscheidung dem Agenten überlassen, aber
      konsistent).
- [ ] `GET /api/projects/{slug}` – Projekt-Details. Bei `Visibility=Private`
      nur für Members sichtbar (404 für alle anderen, nicht 403 – kein
      Leaken der Existenz privater Projekte).
- [ ] `PATCH /api/projects/{slug}` – nur `Owner`/`Admin`.
- [ ] `DELETE /api/projects/{slug}` – nur `Owner`.
- [ ] `POST /api/projects/{slug}/members` – Mitglied per Username einladen
      (nur `Owner`/`Admin`; `Admin` darf nur `Contributor` einladen, nicht
      `Admin`).
- [ ] `PATCH /api/projects/{slug}/members/{username}` – Rolle ändern (nur
      `Owner`). Der letzte `Owner` darf nicht herabgestuft werden, ohne dass
      vorher ein anderer Member zum `Owner` gemacht wurde (Ownership-Transfer
      = zwei Schritte: neuen Owner setzen, alten auf `Admin`/`Contributor`).
- [ ] `DELETE /api/projects/{slug}/members/{username}` – entfernen (Owner/
      Admin für andere; jeder Member darf sich selbst entfernen, außer dem
      letzten `Owner`).

### Frontend

- [ ] `/users/[username]` – SSR-Profilseite: Avatar, Bio, Links, Skills als
      Badges, Liste der Projekte (Owner/Member), Platzhalter-Sektion
      "Contributions" (leerer Zustand, Inhalt kommt Phase 3).
- [ ] `/settings/profile` – Formular für Bio, Links, Skill-Auswahl
      (Multi-Select gegen `GET /api/skills`).
- [ ] `/projects/new` – Formular: Titel, Beschreibung, Engine, Genre,
      Visibility.
- [ ] `/projects/[slug]` – SSR-Projektseite mit Tabs: **Overview** (Banner,
      Beschreibung, Tags/Engine/Genre), **Team** (Mitgliederliste mit
      Rollen, Invite-Formular + Rollen-Änderung für Owner/Admin), **Quests**
      (Platzhalter "coming soon"), **Activity** (Platzhalter).
- [ ] `/projects/[slug]/settings` – nur sichtbar/erreichbar für Owner/Admin:
      Felder bearbeiten, Visibility umschalten, Projekt löschen (nur Owner).

## Definition of Done

- [ ] Ein eingeloggter User kann Bio/Links/Skills setzen, und diese
      erscheinen auf seiner öffentlichen Profilseite.
- [ ] Ein User kann ein Projekt erstellen und ist danach automatisch Owner.
- [ ] Der Owner kann einen zweiten registrierten User per Username als
      Contributor einladen; dieser erscheint in der Team-Liste mit korrekter
      Rolle.
- [ ] Ein nicht eingeloggter Besucher sieht öffentliche Profil- und
      Projekt-Seiten, aber keine privaten Projekte (404).
- [ ] Es kann zu keinem Zeitpunkt ein Projekt ohne Owner oder mit zwei Ownern
      geben (durch Tests/manuelle Prüfung der Rollenwechsel-Logik
      abgesichert).
