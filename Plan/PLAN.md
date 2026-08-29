# Gamedevs Connect V2 – Technischer Umsetzungsplan (Monolith)

Dieser Plan ergänzt die Produkt-Roadmap in [README.md](README.md) um die technische
Umsetzung als Monolith (statt Microservices) und dokumentiert, was aus der alten
Version (`Game_Devs_Connect`) übernommen bzw. verworfen wird.

---

## 1. Kernentscheidungen (Delta zur alten Version)

| Bereich | Alt | Neu (V2) |
|---|---|---|
| Architektur | 9 Microservices (Aspire, Gateway, REST+gRPC) | Modularer Monolith, 1 Deployable |
| Backend | .NET 8/9 gemischt | .NET 10 |
| Frontend Web | Next.js 15 (Pages Router) | Next.js (App Router) + TypeScript, mit SSR für öffentliche Seiten |
| Mobile | keins | React Native + Expo, TypeScript (Phase 7) |
| Datenbank | SQL Server | PostgreSQL |
| Login | Discord OAuth (NextAuth) | GitHub OAuth (weitere Git-Provider später möglich) |
| Auth-Vertrauen | Backend vertraute teils client-seitig mitgegebenen `ownerId`/`userId` | User-Identität kommt ausschließlich aus validiertem Auth-Context |
| Hosting | Azure (Terraform, App Service/Azure SQL) | Linux-VPS + Docker Compose, lokal per Docker zum Entwickeln/Testen |
| Datei-Storage | Azure Blob Storage | Lokales Dateisystem auf dem VPS (Volume), kein Cloud-Storage |
| Backend-Projekte | 9 Services × 3 Assemblies (`.Contract`/`.Application`/Api) | 1 Projekt `GameDevsConnect.Api`, Module als Ordner |
| Backend-Pattern | Dünne Endpoints direkt gegen `GDCDbContext`, kein einheitliches Pattern | MediatR CQRS-light (Command/Query + Handler) + `Result<T>`, Minimal APIs (siehe Abschnitt 4a) |

---

## 2. Was aus der alten Version übernommen werden kann

- **Entity-Grundgerüst**: `GDCDbContext`-Modelle für User, Profile, Project, Quest,
  Post, Tag, File, Notification sind ein brauchbarer Ausgangspunkt – müssen aber um
  das eigentliche Produkt-Kernstück erweitert werden (Skill, ProjectMember-Rollen,
  QuestSubmission, Contribution, XPTransaction – das existierte im alten Projekt
  noch gar nicht).
- **OAuth-Callback-Muster**: "User existiert? sonst anlegen, dann Session/JWT
  ausstellen" – gleiches Muster, nur mit GitHub statt Discord als Provider.
- **File-Upload-Muster**: Metadaten-Zeile (`FileDTO`) + separater Byte-Storage –
  Muster bleibt, nur der Storage-Provider wechselt von Azure Blob auf ein lokales
  Dateisystem-Volume auf dem VPS.
- **Frontend-UX-Muster**: Infinite Scroll für Feeds/Quest-Listen, modal-basiertes
  Erstellen von Posts/Quests – als Referenz übernehmbar, da wieder Next.js zum
  Einsatz kommt (wenn auch App Router statt Pages Router, Code wird neu
  geschrieben, kein 1:1-Copy-Paste wegen NextAuth/Discord-Kopplung).
- **CI-Grundidee**: GitHub Actions + Docker Build – aber radikal vereinfacht auf
  1 Backend-Image + 1 Frontend-Image statt einer 9-Service-Matrix.

## 3. Was bewusst verworfen wird

- Aspire `AppHost`/`ServiceDefaults`, Service Discovery
- API-Gateway-Aggregationslayer + gRPC-`Protos` pro Service
- `.Contract`/`.Application`-3-Projekt-Split pro Service
- Statischer `X-Access-Key`-Header für Service-zu-Service-Calls
- Terraform-Multi-Service-Infrastruktur (Azure), `docker-compose` mit 9+ Containern
- Azure Blob Storage, Azure SQL, jegliche Azure-Abhängigkeit (kein Zugang mehr
  vorhanden – die alte Version entstand mit Azure-Zugang aus einer Weiterbildung)
- Discord als Login-Provider
- Next.js-Frontend-Code selbst (Pages Router, NextAuth-Wiring) – Framework bleibt
  Next.js, aber Codebase wird neu aufgesetzt (App Router, eigener Auth-Flow)

---

## 4. Zielarchitektur (kurz)

- **1 ASP.NET Core 10 Web-API-Projekt** (`GameDevsConnect.Api`), intern nach
  Feature-Modulen strukturiert: `Modules/Auth`, `Modules/Users`,
  `Modules/Projects`, `Modules/Quests`, `Modules/Contributions`,
  `Modules/Xp`, `Modules/Social` (Follow/Feed/Posts/Comments/Likes),
  `Modules/Notifications`. Jedes Modul organisiert sich intern als
  MediatR-CQRS-light (Command/Query + Handler pro Use-Case) – Details und
  ein konkretes Beispiel in Abschnitt 4a.
- **1 Next.js-App** für Web (App Router), rendert öffentliche Seiten
  (Profile, Projekt-Seiten, Discovery) serverseitig für SEO/Social-Previews
  (Open-Graph-Tags) und ruft dafür die .NET-API auf – bleibt reine
  Rendering-/BFF-Schicht, keine eigene Business-Logik.
- **1 React Native (Expo) App** für Mobile, ab Phase 7.
- **Geteilte API-Typen**: TypeScript-Typen für Web *und* Mobile aus einer Quelle
  generieren (z. B. via OpenAPI/NSwag-Client oder `openapi-typescript`), statt wie
  im alten Projekt `interfaces/*.ts` von Hand parallel zu den DTOs zu pflegen.
- **PostgreSQL** via EF Core + Npgsql.
- **Auth**: GitHub OAuth2 Code Flow – Backend tauscht Code gegen Access Token,
  matched/legt User an, stellt eigenes Session-Cookie oder JWT aus. Jeder Endpoint
  liest die User-Identität ausschließlich aus diesem validierten Context (Fix
  gegenüber der alten Version).
- **Deployment**: 1 Linux-VPS, Docker Compose mit den Containern `api` (.NET),
  `web` (Next.js), `db` (Postgres) sowie einem Reverse Proxy (z. B. Caddy – macht
  TLS/Let's-Encrypt praktisch automatisch) davor. Datei-Uploads landen auf einem
  gemounteten Volume. Lokale Entwicklung nutzt dieselbe `docker-compose`-Datei
  (ohne Reverse Proxy/TLS).

## 4a. Backend-Pattern im Detail

Angelehnt an ein bewährtes MediatR/Result-Pattern aus einer bestehenden
.NET-API, aber deutlich abgespeckt: dort existiert eine zusätzliche
Integration/Concrete-Command-Zweiteilung, weil viele externe Drittsysteme
orchestriert werden müssen – das entfällt hier komplett, da Gamedevs Connect
kaum externe Integrationen hat (nur GitHub OAuth).

**Solution-/Projektstruktur:**

```
GameDevsConnect.slnx
GameDevsConnect.Api/
    Program.cs                         # Modul-Registrierungen + Middleware, bewusst schlank
    Modules/
      Auth/
        Endpoints/AuthEndpoints.cs      # Minimal-API-Mapping für dieses Modul
        Commands/LoginWithGitHubCommand.cs
        Commands/GitHubCallbackCommand.cs
        Queries/GetCurrentUserQuery.cs
      Users/
        Endpoints/UserEndpoints.cs
        Commands/UpdateUserProfileCommand.cs
        Commands/SetUserSkillsCommand.cs
        Queries/GetUserProfileQuery.cs
        Domain/User.cs
        Domain/UserLink.cs
        Domain/Skill.cs
        Domain/UserSkill.cs
        Data/UserConfiguration.cs       # IEntityTypeConfiguration<User>
        Data/SkillConfiguration.cs
      Projects/ ...
      Quests/ ...
      Contributions/ ...
      Xp/ ...
      Social/ ...
      Notifications/ ...
    Infrastructure/
      Persistence/AppDbContext.cs       # sammelt alle IEntityTypeConfiguration<T> per Assembly-Scan
      Persistence/Migrations/
      Files/IFileStorage.cs             # + lokale Disk-Implementierung
    Shared/
      Result.cs                        # Result<T>-Typ (siehe unten)
      Endpoints/ResultExtensions.cs     # zentrales Mapping Result<T> → HTTP-Status
```

**Ein Use-Case, End-to-End (Beispiel `CreateProject`):**

1. `Modules/Projects/Endpoints/ProjectEndpoints.cs` – Minimal-API-Route
   `app.MapPost("/api/projects", ...)`, liest Request-Body, ruft
   `mediator.Send(new CreateProjectCommand(...))`, gibt das Ergebnis über
   `ResultExtensions.ToHttpResult()` zurück.
2. `Modules/Projects/Commands/CreateProjectCommand.cs` – `record` mit den
   Eingabedaten, implementiert `IRequest<Result<ProjectDto>>`.
3. `Modules/Projects/Commands/CreateProjectCommandHandler.cs` –
   `IRequestHandler<CreateProjectCommand, Result<ProjectDto>>`, injiziert
   `AppDbContext` **direkt** (kein Repository/UnitOfWork), macht die
   Slug-Eindeutigkeitsprüfung, legt `Project` + `ProjectMember` (Role=Owner)
   an, `SaveChangesAsync`, gibt `Result<ProjectDto>.Success(...)` oder
   `Result<ProjectDto>.Conflict(...)` zurück.

**`Result<T>` (eigener, kleiner Typ – keine externe Library nötig):**

Ein einfacher Result-Typ mit Varianten mindestens für `Success`,
`NotFound`, `Conflict`, `Forbidden`, `ValidationError` reicht aus. Die
zentrale `ResultExtensions.ToHttpResult()`-Methode mappt diese Varianten
einmalig auf HTTP-Statuscodes (200/201, 404, 409, 403, 400) – Handler selbst
werfen keine Exceptions für erwartbare Fehlerfälle.

**Weitere Konventionen:**

- **Keine Repository-/UnitOfWork-Abstraktion** – `AppDbContext` ist die
  Abstraktion, wird direkt in Handler injiziert.
- **Minimal APIs statt MVC-Controller** – pro Modul eine
  `Endpoints/<Modul>Endpoints.cs`-Datei mit einer statischen
  `MapXyz(this WebApplication app)`-Extension, die in `Program.cs`
  aufgerufen wird. Hält `Program.cs` klein, vermeidet den 800-Zeilen-
  `Program.cs`-Anti-Pattern.
- **Ein öffentlicher Typ pro Datei**, Feature-Unterordner innerhalb eines
  Moduls (`Commands/`, `Queries/`, `Domain/`, `Data/`, `Endpoints/`).
- **MediatR-Registrierung** über Assembly-Scan (`AddMediatR(cfg =>
  cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))`) – bei einem
  einzigen Projekt reicht ein einziger Scan-Aufruf, keine
  Assembly-Marker-Klassen pro Modul nötig.
- Validierung für den MVP inline im Handler (FluentValidation ist ein
  Kandidat für später, aber kein Muss – nicht vorab einbauen, bevor es
  gebraucht wird).

## 5. Bewusst offen gelassen (nicht blockierend für Phase 0)

- Konkreter VPS-Anbieter (z. B. Hetzner, Netcup, IONOS – austauschbar, solange es
  ein Linux-Server mit Docker ist).
- Styling-Ansatz im Web-Frontend (z. B. Tailwind + shadcn/ui als naheliegender
  Default, aber nicht festgelegt).
- Ob/wann später doch auf S3-kompatiblen Storage (z. B. self-hosted MinIO oder
  Cloudflare R2) gewechselt wird, falls lokales Dateisystem-Storage an Grenzen
  stößt (Backup, Skalierung) – für den MVP unnötig.
- Deploy-Automatisierung: CI baut vorerst nur Docker-Images und pusht sie in eine
  Registry (Docker Hub oder GHCR); das eigentliche Ausrollen auf den VPS
  (`docker compose pull && up -d`) läuft anfangs manuell/per SSH-Skript, kein
  automatisches CD in Phase 0 nötig.

---

## 6. Phasenplan

Jede Phase ist als eigene, für sich lauffähige Aufgabenstellung in
[`Tasks/`](Tasks/) dokumentiert (Kontext, Voraussetzungen, Out-of-Scope,
detaillierte Tasks, Definition of Done) – gedacht, um einem Coding-Agenten
jeweils eine einzelne Datei zu übergeben.

| Phase | Ziel | Datei |
|---|---|---|
| 0 | Monolith-Skeleton, Login end-to-end (lokal + 1x VPS) | [phase-0-projekt-fundament.md](Tasks/phase-0-projekt-fundament.md) |
| 1 | User-Profile (Bio/Skills), Projects + Mitglieder-Rollen | [phase-1-foundation.md](Tasks/phase-1-foundation.md) |
| 2 | Quest-Erstellung, -Suche, -Claim | [phase-2-quest-system.md](Tasks/phase-2-quest-system.md) |
| 3 | Submissions, Review-Flow, Contributions | [phase-3-contributions.md](Tasks/phase-3-contributions.md) |
| 4 | XP-Transaktionen, Level, Reputation | [phase-4-xp-reputation.md](Tasks/phase-4-xp-reputation.md) |
| 5 | Follow, Activity-Feed, Posts/Comments/Likes, Notifications | [phase-5-social-layer.md](Tasks/phase-5-social-layer.md) |
| 6 | Discover-Seiten, Volltextsuche | [phase-6-discovery.md](Tasks/phase-6-discovery.md) |
| 7 | React-Native/Expo-App auf derselben API | [phase-7-mobile.md](Tasks/phase-7-mobile.md) |

Phasen 1–6 entsprechen inhaltlich den Phasen 1–6 aus der Produkt-Roadmap in
[README.md](../README.md) §38, Phase 0 (Fundament) und Phase 7 (Mobile) sind
rein technische Ergänzungen dieses Umsetzungsplans.

---

## 7. Definition of Done

Unverändert gegenüber Produkt-Roadmap §39: Das MVP gilt als fertig, wenn der
komplette Flow von Projekt-Erstellung → Quest → Claim → Submission → Acceptance
→ XP → sichtbare Contribution im Profil funktioniert – jetzt eben auf dem
Monolithen mit GitHub-Login statt Discord.
