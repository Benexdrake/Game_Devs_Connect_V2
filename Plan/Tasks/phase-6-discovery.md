# Phase 6 – Discovery

> Für den vollen Kontext siehe [`README.md`](../../README.md) (Produkt-Roadmap)
> und [`PLAN.md`](../PLAN.md) (Architektur-Entscheidungen). Diese Phase baut
> auf [Phase 5](phase-5-social-layer.md) auf.

## Projekt-Kontext (kurz)

Monolith: .NET 10 API (`GameDevsConnect.Api`) + Next.js Frontend + PostgreSQL.
Alle Kern-Entitäten (User, Project, Quest, Contribution, ActivityEvent)
existieren bereits aus den vorherigen Phasen.

Backend-Pattern (siehe [`PLAN.md`, Abschnitt 4a](../PLAN.md#4a-backend-pattern-im-detail)):
Discovery-/Such-Endpoints als MediatR Queries (kein `Command`, da rein
lesend) in `Modules/Projects/Queries` bzw. `Modules/Quests/Queries`,
Rückgabetyp `Result<T>`.

## Voraussetzung

Phase 5 ist abgeschlossen.

## Ziel dieser Phase

Nutzer können Projekte und Quests gezielt entdecken (sortierte Listen,
Volltextsuche über Projekte/Quests/User), statt nur über den Feed oder
direkte Links zu stolpern.

## Nicht Teil dieser Phase

- Keine KI-gestützten Empfehlungen/Matching (README §37 – explizit nicht im
  MVP).
- Keine personalisierten Rankings – reine, nachvollziehbare Sortierkriterien
  reichen.

## Tasks

### Backend

- [ ] `GET /api/projects/discover?sort=trending|recent|new|looking-for-contributors`:
      - `trending` = z. B. Projekte mit den meisten `ActivityEvent`-Einträgen
        der letzten 7 Tage.
      - `recent` = `UpdatedAt` absteigend.
      - `new` = `CreatedAt` absteigend.
      - `looking-for-contributors` = Projekte mit mindestens einer Quest im
        Status `Open` (**berechnetes** Kriterium, kein gespeichertes Flag –
        bleibt dadurch immer korrekt).
- [ ] Volltextsuche: `tsvector`-Spalte (generiert aus Titel+Beschreibung(+Tags))
      auf `Project` und `Quest` ergänzen, GIN-Index setzen.
      `GET /api/search?q=...&type=projects|quests|users` (bei `users` reicht
      ein einfacher `ILIKE` auf `Username`).

### Frontend

- [ ] `/discover` – Sektionen/Tabs: Trending, Recently Updated, Looking for
      Contributors, New (Mockup-Vorbild: README §16).
- [ ] Globale Suchleiste in der Navigation → `/search?q=...`
      Ergebnis-Seite, gruppiert nach Projects/Quests/Users.
- [ ] `/quests`-Seite (aus Phase 2) erhält dieselbe Suchleiste/denselben
      Such-Endpoint für Konsistenz.

## Definition of Done

- [ ] `/discover` zeigt für jede Sektion eine plausibel sortierte,
      tatsächlich unterschiedliche Projektliste.
- [ ] Eine Suche nach einem Stichwort, das in einem Quest-Titel, einem
      Projekt-Titel oder einem Username vorkommt, liefert das jeweilige
      Ergebnis in der richtigen Kategorie.
- [ ] "Looking for Contributors" zeigt ausschließlich Projekte mit
      mindestens einer aktuell offenen Quest.
