# Phase 4 – XP / Reputation

> Für den vollen Kontext siehe [`README.md`](../../README.md) (Produkt-Roadmap)
> und [`PLAN.md`](../PLAN.md) (Architektur-Entscheidungen). Diese Phase baut
> auf [Phase 3](phase-3-contributions.md) auf.

## Projekt-Kontext (kurz)

Monolith: .NET 10 API (`GameDevsConnect.Api`) + Next.js Frontend + PostgreSQL.
Aus Phase 3 existiert der Erweiterungspunkt bei Submission-Annahme
(`Accept`-Branch im `ReviewSubmissionCommand`-Handler), an dem ein
`Contribution`-Eintrag entsteht.

Backend-Pattern (siehe [`PLAN.md`, Abschnitt 4a](../PLAN.md#4a-backend-pattern-im-detail)):
neue Endpoints als MediatR Command/Query + Handler in `Modules/Xp/
{Commands,Queries}`, Rückgabetyp `Result<T>`, Entities in `Modules/Xp/Domain`,
EF-Konfiguration als `IEntityTypeConfiguration<T>` in `Modules/Xp/Data`.

## Voraussetzung

Phase 3 ist abgeschlossen: Contributions entstehen bei angenommenen
Submissions.

## Ziel dieser Phase

Contributions vergeben nachvollziehbar XP. Level und Reputation werden aus
den XP-Daten abgeleitet und im Profil angezeigt.

## Nicht Teil dieser Phase

- Kein Achievement-System, keine Badges.
- Keine Season/Reset-Mechanik für XP.

## Tasks

### Backend – Datenmodell

- [ ] `XpTransaction` (Id, `UserId` FK, `Amount` int, `Reason` enum:
      `QuestAccepted`, `DifficultyBonus`, `SourceType` string (`"Quest"`),
      `SourceId` Guid, `CreatedAt`). **Wichtig**: Nirgendwo ein direktes
      `User.Xp += amount` – XP ergibt sich ausschließlich aus der Summe
      dieser Tabelle (Nachvollziehbarkeit, siehe README §24).
- [ ] Migration erstellen und testen.

### Backend – Logik

- [ ] Am Erweiterungspunkt aus Phase 3 (`Accept`-Branch): beim Anlegen der
      `Contribution` einen `XpTransaction`-Eintrag erzeugen. Vorschlag für
      die Berechnung (anpassbare Default-Werte, keine harten Vorgaben):
      - Basis = `Quest.XpReward`
      - Difficulty-Bonus: `Easy` +0, `Medium` +25, `Hard` +75
      - Summe als ein `XpTransaction`-Eintrag mit `Reason=QuestAccepted`
        (Bonus kann im selben Eintrag oder als zweiter Eintrag mit
        `Reason=DifficultyBonus` erfasst werden – beides ist ok, Hauptsache
        konsistent).
- [ ] Level als **reine Ableitung** aus der XP-Summe berechnen (kein
      persistiertes `User.Level`-Feld). Vorschlag für eine einfache Formel
      oder Lookup-Tabelle für die ersten ~20 Level, angelehnt an README §12
      (`L1=0, L2=100, L3=250, L4=500, L5=850, ...`) – exakte Formel ist
      Implementierungsdetail, muss nur deterministisch und monoton steigend
      sein.
- [ ] Tages-Limit für XP (Anti-Abuse, README §35): Beim Vergeben prüfen, wie
      viel XP der User in den letzten 24h bereits erhalten hat. Falls das
      konfigurierbare Tageslimit (Default z. B. 1000, als App-Setting/Env-Var)
      überschritten würde: den vergebenen Betrag auf das verbleibende
      Kontingent **kappen**, statt den gesamten Accept-Vorgang fehlschlagen
      zu lassen (Submission-Annahme darf nie an einem XP-Cap scheitern).
- [ ] Bei `Reject` oder `Cancelled` wird **niemals** XP vergeben (kein
      `XpTransaction`-Eintrag) – bereits durch die Statuslogik aus Phase 3
      sichergestellt, hier nur verifizieren.
- [ ] Reputation als abgeleiteter Wert (0–5), berechnet aus dem Verhältnis
      angenommener zu abgelehnter Submissions, on-the-fly berechnet, nicht
      gespeichert. Bei weniger als 3 insgesamt reviewten Submissions: keinen
      Wert anzeigen (z. B. `null`/"noch keine Daten"), damit eine einzelne
      frühe Ablehnung keinen Neuling dauerhaft abstraft.

### Backend – Endpoints

- [ ] `GET /api/users/{username}/xp-summary` – liefert `totalXp`, `level`,
      `xpForNextLevel`, `reputation` (oder `null`), `completedQuests`
      (= Anzahl `Accepted`-Quests, an denen der User beteiligt war),
      `acceptedContributions` (= Anzahl `Contribution`-Einträge).

### Frontend

- [ ] Profilseite: Level-Badge, XP-Fortschrittsbalken zum nächsten Level,
      Reputation-Anzeige (oder "noch keine Daten"), Zähler für "Completed
      Quests" / "Accepted Contributions" (Mockup-Vorbild: README §13).

## Definition of Done

- [ ] Jede angenommene Submission erzeugt genau einen (oder zwei, siehe
      oben) nachvollziehbare `XpTransaction`-Einträge – niemals eine direkte
      Feldänderung an `User`.
- [ ] Die Profilseite zeigt nach einer Annahme sofort aktualisiertes
      Level/XP/Reputation.
- [ ] Ein User kann durch Claimen und Annehmen seiner **eigenen** Quest kein
      XP erhalten (bereits durch Claim-Verbot aus Phase 2 blockiert – hier
      nur end-to-end verifizieren).
- [ ] Abgelehnte Submissions erzeugen nachweislich keine `XpTransaction`.
- [ ] Wird das Tages-XP-Limit überschritten, wird der Accept-Vorgang
      trotzdem erfolgreich abgeschlossen, nur der vergebene XP-Betrag ist
      gekappt.
