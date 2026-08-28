# Phase 0 – Projekt-Fundament

> Diese Datei ist so geschrieben, dass sie einem Coding-Agenten eigenständig
> übergeben werden kann. Für den vollen Produkt-/Architektur-Kontext siehe
> [`README.md`](../../README.md) (Produkt-Roadmap) und [`PLAN.md`](../PLAN.md)
> (Architektur-Entscheidungen).

## Projekt-Kontext (kurz)

Gamedevs Connect ist eine Plattform, auf der Game Devs gemeinsam an Projekten
arbeiten (Projekte → Quests → Contributions → XP/Reputation). V2 wird als
**Monolith** gebaut:

- **Backend**: .NET 10, ein Projekt `GameDevsConnect.Api`, PostgreSQL via EF
  Core/Npgsql. Intern nach Feature-Modulen strukturiert, jedes Modul als
  MediatR-CQRS-light (Command/Query + Handler pro Use-Case) mit eigenem
  `Result<T>`-Rückgabetyp statt Exceptions für erwartbare Fehler, Minimal-API-
  Endpoints statt MVC-Controllers, kein Repository/UnitOfWork (Handler nutzen
  `AppDbContext` direkt). **Vollständiges Beispiel und Begründung in
  [`PLAN.md`, Abschnitt 4a](../PLAN.md#4a-backend-pattern-im-detail) – bitte
  vor dem Start dieser Phase lesen.**
- **Frontend Web**: Next.js (App Router), TypeScript. Rendert öffentliche
  Seiten serverseitig (SEO/Social-Previews), ruft dafür die Backend-API auf.
  Reine Rendering-Schicht, keine eigene Business-Logik.
- **Auth**: GitHub OAuth2 (kein Discord, kein Passwort-Login im MVP).
- **Deployment**: 1 Linux-VPS, Docker Compose (`api`, `web`, `db`, Reverse
  Proxy), kein Cloud-Provider (kein Azure – kein Zugang mehr vorhanden).

## Repo-Struktur

```
GameDevsConnect.sln
src/
  GameDevsConnect.Api/
    Program.cs                        # Modul-Registrierungen + Middleware, bewusst schlank
    Modules/
      Auth/
        Endpoints/AuthEndpoints.cs     # Minimal-API-Mapping für dieses Modul
        Commands/LoginWithGitHubCommand.cs (+ Handler)
        Commands/GitHubCallbackCommand.cs (+ Handler)
        Queries/GetCurrentUserQuery.cs (+ Handler)
      Users/
        Domain/User.cs
        Data/UserConfiguration.cs      # IEntityTypeConfiguration<User>
        (Endpoints/Commands/Queries kommen ab Phase 1)
    Infrastructure/
      Persistence/AppDbContext.cs      # sammelt IEntityTypeConfiguration<T> per Assembly-Scan
      Persistence/Migrations/
    Shared/
      Result.cs                       # eigener Result<T>-Typ
      Endpoints/ResultExtensions.cs    # Result<T> → HTTP-Status, zentral
frontend/                              Next.js App (App Router)
mobile/                                (erst Phase 7)
docker-compose.yml
Caddyfile
```

Diese Struktur ist verbindlich für den Rest des Umsetzungsplans – spätere
Phasen-Dateien gehen von `Modules/<Name>/{Commands,Queries,Domain,Data,Endpoints}`
sowie `Result<T>` als Rückgabetyp aus.

## Ziel dieser Phase

Ein leeres, aber **lauffähiges** Monolith-Skeleton, bei dem sich ein User per
GitHub einloggen kann und der Login-Zustand über Reloads hinweg erhalten
bleibt – lokal per Docker Compose und einmal testweise auf einem echten VPS.

## Nicht Teil dieser Phase

- Keine Produkt-Features (User-Profile-Felder, Projects, Quests etc.) – das
  kommt ab Phase 1.
- Keine automatisierte CD auf den VPS (Deployment darf manuell/per SSH-Skript
  passieren).
- Keine weiteren OAuth-Provider außer GitHub.

## Tasks

### Backend

- [ ] .NET 10 Solution `GameDevsConnect.sln` + Web-API-Projekt
      `src/GameDevsConnect.Api` anlegen.
- [ ] NuGet-Pakete: `Npgsql.EntityFrameworkCore.PostgreSQL`,
      `Microsoft.AspNetCore.Authentication.Cookies`, `MediatR`.
- [ ] `Shared/Result.cs` – eigener `Result<T>`-Typ mit Varianten mindestens
      für `Success`, `NotFound`, `Conflict`, `Forbidden`, `ValidationError`
      (siehe PLAN.md §4a).
- [ ] `Shared/Endpoints/ResultExtensions.cs` – `ToHttpResult()`-Extension,
      die `Result<T>` zentral auf HTTP-Status (200/201, 404, 409, 403, 400)
      mapped. Wird von jedem Minimal-API-Endpoint verwendet.
- [ ] `Infrastructure/Persistence/AppDbContext.cs`, der beim Start per
      Assembly-Scan alle `IEntityTypeConfiguration<T>`-Klassen anwendet
      (`modelBuilder.ApplyConfigurationsFromAssembly(...)`).
- [ ] `Modules/Users/Domain/User.cs` (erste Entity):
      - `Id` (Guid, PK)
      - `GitHubId` (string oder long, unique – GitHub-User-ID, nicht der
        veränderliche Username)
      - `Username` (string, aus GitHub-Login übernommen, bei Kollision mit
        Suffix eindeutig machen)
      - `AvatarUrl` (string, von GitHub übernommen)
      - `CreatedAt` (timestamptz)
- [ ] `Modules/Users/Data/UserConfiguration.cs` (`IEntityTypeConfiguration<User>`).
- [ ] Erste EF-Core-Migration (`InitialCreate`) + Verifizieren, dass sie gegen
      eine lokale Postgres-Instanz läuft.
- [ ] MediatR registrieren (`AddMediatR(cfg =>
      cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))`) –
      ein einziger Scan reicht, da alles in einem Projekt liegt.
- [ ] GitHub OAuth App registrieren (Dev-Instanz): Client-ID/-Secret als
      Environment-Variablen/User-Secrets, **nicht** einchecken.
- [ ] OAuth2-Flow implementieren, als Commands/Query in `Modules/Auth`
      (siehe Repo-Struktur oben), gemappt in `Modules/Auth/Endpoints/AuthEndpoints.cs`:
      - `GET /api/auth/login/github` (`LoginWithGitHubCommand`) – generiert
        `state` (gegen CSRF), redirect zu GitHub Authorize-URL.
      - `GET /api/auth/callback/github` (`GitHubCallbackCommand`) – validiert
        `state`, tauscht `code` gegen Access Token, ruft GitHub-API `/user`
        ab, legt User an oder matched per `GitHubId`, setzt Session-Cookie,
        redirect zum Frontend.
      - `GET /api/auth/me` (`GetCurrentUserQuery`) – liefert aktuellen
        eingeloggten User, `Result<T>.NotFound`/eigener 401-Fall wenn keine
        gültige Session.
      - `POST /api/auth/logout` – löscht Session-Cookie.
- [ ] Session-Mechanismus: ASP.NET Core Cookie-Authentication (HttpOnly,
      Secure, `SameSite=Lax`). Kein JWT nötig, da Backend und Frontend hinter
      demselben Reverse Proxy/derselben Domain laufen (siehe Routing unten).
- [ ] CORS für lokale Entwicklung konfigurieren (Next.js Dev-Server läuft auf
      anderem Port als die API), inkl. `AllowCredentials`.
- [ ] `Program.cs` bewusst schlank halten: pro Modul eine
      `MapXyzEndpoints(app)`-Extension aufrufen statt Routen inline zu
      definieren (vermeidet einen wachsenden Monolith-`Program.cs`).

### Frontend

- [ ] Next.js-Projekt (App Router, TypeScript) in `frontend/` aufsetzen.
- [ ] Login-Seite mit "Login with GitHub"-Button → Link/Redirect zu
      `/api/auth/login/github`.
- [ ] Nach erfolgreichem Callback: Startseite zeigt eingeloggten Zustand
      (Username/Avatar aus `/api/auth/me`), inkl. Logout-Button.
- [ ] `next.config` `rewrites()` (lokal) bzw. Reverse-Proxy-Routing (prod):
      `/api/*` zeigt auf das Backend, alles andere auf die Next.js-App – so
      wirkt alles nach außen als **eine** Domain/Origin (wichtig für
      Cookie-basierte Auth ohne Cross-Site-Komplikationen).

### Infra / DevOps

- [ ] Dockerfile Backend (multi-stage, `mcr.microsoft.com/dotnet/aspnet:10.0`
      als Runtime-Base).
- [ ] Dockerfile Frontend (multi-stage, Node-Base, `next build` +
      `output: 'standalone'`).
- [ ] `docker-compose.yml` mit Services `api`, `web`, `db` (Postgres-Image) +
      Named Volume für Postgres-Daten.
- [ ] Reverse Proxy (empfohlen: Caddy) vor `api`+`web`, Pfad-Routing wie oben
      beschrieben (`/api/*` → `api`, Rest → `web`); `Caddyfile` an den Repo-Root
      oder `deploy/`.
- [ ] GitHub Actions Workflow: Build Backend (`dotnet build`), Build Frontend
      (`npm ci && npm run build`), Docker-Images bauen und nach GHCR (oder
      Docker Hub) pushen. Kein automatisches Deployment.
- [ ] Test-Deployment auf einem echten (ggf. günstigen) Linux-VPS: Docker
      installieren, `docker compose up -d`, Domain auf den Server zeigen
      lassen, Caddy holt automatisch ein Let's-Encrypt-Zertifikat. GitHub
      OAuth App Callback-URL auf die echte Domain anpassen (Dev- und
      Prod-OAuth-App können getrennt bleiben).

## Definition of Done

- [ ] `docker compose up` startet lokal `api`+`web`+`db` fehlerfrei.
- [ ] Ein Nutzer kann auf der Startseite "Login with GitHub" klicken, wird zu
      GitHub weitergeleitet, nach Bestätigung zurück zur App geleitet und ist
      dort als eingeloggt erkennbar (Username/Avatar sichtbar).
- [ ] Ein Reload der Seite verliert den Login-Zustand **nicht**.
- [ ] Logout entfernt den Login-Zustand zuverlässig.
- [ ] Derselbe Stack läuft einmal erfolgreich auf einem echten VPS unter einer
      echten Domain mit gültigem TLS-Zertifikat.
