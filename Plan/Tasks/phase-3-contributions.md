# Phase 3 – Contributions

> Für den vollen Kontext siehe [`README.md`](../../README.md) (Produkt-Roadmap)
> und [`PLAN.md`](../PLAN.md) (Architektur-Entscheidungen). Diese Phase baut
> auf [Phase 2](phase-2-quest-system.md) auf.

## Projekt-Kontext (kurz)

Monolith: .NET 10 API (`GameDevsConnect.Api`) + Next.js Frontend + PostgreSQL.
Aus Phase 2 existiert bereits `Quest` (mit vollständigem Status-Enum, aber
bisher nur bis `InProgress`/`Cancelled` genutzt) und `QuestAssignment` (wer
hat aktuell geclaimt). Datei-Uploads landen laut [`PLAN.md`](../PLAN.md) auf
einem VPS-Volume, **nicht** in Azure Blob Storage.

Backend-Pattern (siehe [`PLAN.md`, Abschnitt 4a](../PLAN.md#4a-backend-pattern-im-detail)):
neue Endpoints als MediatR Command/Query + Handler in `Modules/Contributions/
{Commands,Queries}`, Rückgabetyp `Result<T>`, Entities in
`Modules/Contributions/Domain`, EF-Konfiguration als `IEntityTypeConfiguration<T>`
in `Modules/Contributions/Data`.

## Voraussetzung

Phase 2 ist abgeschlossen: Quests können erstellt, gefunden und geclaimt
werden.

## Ziel dieser Phase

Ein Contributor kann für eine geclaimte Quest eine Contribution einreichen
(Beschreibung, Dateien, Links). Der Project Owner/Admin kann sie annehmen,
ablehnen oder Änderungen anfordern. Bei Annahme entsteht ein dauerhafter
`Contribution`-Eintrag, sichtbar im Profil.

## Nicht Teil dieser Phase

- Keine XP-Vergabe (Phase 4) – aber diese Phase muss einen klar erkennbaren
  Erweiterungspunkt dafür schaffen (siehe unten).
- Keine Activity-Feed-Einträge (Phase 5).

## Tasks

### Backend – Datenmodell

- [ ] `QuestSubmission` (Id, `QuestId` FK, `UserId` FK (Autor), `Description`
      text, `Status` enum: `PendingReview`, `ChangesRequested`, `Accepted`,
      `Rejected`, `SubmittedAt`, `ReviewedAt` nullable, `ReviewerId` FK
      nullable, `ReviewComment` text nullable).
- [ ] `SubmissionFile` (Id, `SubmissionId` FK, `FileName`, `ContentType`,
      `SizeBytes`, `StoragePath` – Pfad auf dem VPS-Volume, z. B.
      `/data/uploads/submissions/{submissionId}/{fileId}-{fileName}`,
      `UploadedAt`).
- [ ] `SubmissionLink` (Id, `SubmissionId` FK, `Url`, `Label` nullable) – für
      Git-Repo-Links, externe Portfolio-Links etc. (siehe README §9).
- [ ] `Contribution` (Id, `UserId` FK, `ProjectId` FK, `QuestId` FK,
      `SubmissionId` FK, `CreatedAt`) – entsteht **nur** bei Annahme einer
      Submission, danach unveränderlich, auch wenn Quest/Submission später
      angepasst werden.
- [ ] Migration erstellen und testen.

### Backend – Endpoints & Regeln

- [ ] `POST /api/quests/{questId}/submissions` – nur der User mit aktivem
      Claim (`QuestAssignment` ohne `ReleasedAt`) darf einreichen. Body:
      Description, `links[]`. Setzt `Quest.Status = Submitted`.
- [ ] `POST /api/submissions/{id}/files` – Multipart-Upload, nur der
      Submission-Autor, nur solange `Status` in (`PendingReview`,
      `ChangesRequested`).
- [ ] `GET /api/submissions/{id}/files/{fileId}` – Datei ausliefern, Zugriff
      nur für Submission-Autor und Project-Members (kein öffentlicher
      Zugriff – siehe README §34 zu Asset-Rechten, im MVP bewusst
      restriktiv).
- [ ] `GET /api/quests/{questId}/submissions` – sichtbar für Owner/Admin des
      Projekts sowie den jeweiligen Autor.
- [ ] `POST /api/submissions/{id}/review` (`ReviewSubmissionCommand` +
      `ReviewSubmissionCommandHandler` in `Modules/Contributions/Commands`)
      – nur Owner/Admin. Body: `decision` (`Accept` | `Reject` |
      `RequestChanges`), `comment`.
      - `RequestChanges` → `Submission.Status = ChangesRequested`,
        `Quest.Status = InProgress` (gleicher Contributor behält den Claim,
        kann erneut einreichen).
      - `Reject` → `Submission.Status = Rejected`, `Quest.Status = Open`,
        Claim wird freigegeben (`QuestAssignment.ReleasedAt` setzen) – die
        Quest steht wieder für alle (auch denselben User erneut) offen.
      - `Accept` → `Submission.Status = Accepted`, `Quest.Status = Accepted`
        (Endzustand), lege einen `Contribution`-Eintrag an.
        **Erweiterungspunkt für Phase 4**: An dieser Stelle im Code klar
        markieren (z. B. Kommentar `// TODO Phase 4: XP vergeben` oder ein
        Domain-Event `ContributionAccepted` auslösen), damit Phase 4 die
        XP-Vergabe anschließen kann, ohne diese Stelle erneut suchen zu
        müssen.

### Frontend

- [ ] `/quests/[id]` – Contributor mit aktivem Claim sieht ein
      Submission-Formular (Beschreibung, Datei-Upload, Link-Liste), sobald
      `Status` `InProgress` oder `ChangesRequested` ist. Zeigt zusätzlich
      die Historie bisheriger Submissions inkl. Review-Kommentaren.
- [ ] Owner/Admin-Review-UI auf derselben Seite: Liste der Submissions mit
      Accept-/Reject-/Request-Changes-Buttons + Kommentarfeld.
- [ ] Datei-Upload-UI (einfaches `<input type="file" multiple>` reicht für
      den MVP, kein Drag&Drop nötig).
- [ ] Profilseite: "Contributions"-Sektion (bisher Platzhalter aus Phase 1)
      jetzt mit echten Einträgen (Projekt, Quest, Datum) befüllen.

## Definition of Done

- [ ] Ein Contributor mit aktivem Claim kann eine Submission mit
      Beschreibung, mindestens einer Datei und mindestens einem Link
      einreichen.
- [ ] Der Owner sieht die Submission und kann "Changes Requested" wählen –
      der Contributor kann daraufhin erneut einreichen, ohne den Claim zu
      verlieren.
- [ ] Der Owner kann eine Submission ablehnen – die Quest wird wieder für
      alle offen (Status `Open`).
- [ ] Der Owner kann eine Submission annehmen – danach ist die Quest im
      Endzustand `Accepted`, und ein `Contribution`-Eintrag erscheint auf
      dem öffentlichen Profil des Contributors.
- [ ] Datei-Zugriff ist auf Autor + Project-Members beschränkt (manuell
      geprüft: ein fremder, nicht am Projekt beteiligter User bekommt keinen
      Zugriff auf hochgeladene Dateien).
