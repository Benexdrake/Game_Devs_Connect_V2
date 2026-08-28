# Phase 7 – Mobile (React Native / Expo)

> Für den vollen Kontext siehe [`README.md`](../../README.md) (Produkt-Roadmap)
> und [`PLAN.md`](../PLAN.md) (Architektur-Entscheidungen). Diese Phase baut
> auf [Phase 6](phase-6-discovery.md) auf und ist **bewusst** die letzte
> MVP-Phase (siehe README §30: Mobile absichtlich nicht vor dem Web-MVP).

## Projekt-Kontext (kurz)

Monolith: .NET 10 API (`GameDevsConnect.Api`) + Next.js Frontend (Web) +
PostgreSQL. Die komplette API (Auth, Users, Projects, Quests, Contributions,
XP, Social, Discovery) existiert bereits und wird von der Mobile-App
**wiederverwendet** – es wird keine mobile-spezifische Backend-Logik
gebraucht, nur ggf. ein zusätzlicher OAuth-Redirect für den mobilen Flow.

## Voraussetzung

Phase 6 ist abgeschlossen – die Web-App deckt den vollständigen Produkt-Loop
ab.

## Ziel dieser Phase

Eine React-Native/Expo-App bietet die Kernflows (Login, Feed, Projekt/Quest
ansehen, Quest claimen, Profil, Notifications) mobil an, auf derselben API.

## Nicht Teil dieser Phase

- Kein Push-Notification-Service (Expo Push o. Ä.) – In-App-Liste reicht für
  den MVP, wie auf Web.
- Kein Offline-Modus.
- Kein App-Store-Release-Prozess (Signing, Store-Listings) – Fokus liegt auf
  einer funktionierenden Dev/TestFlight-/internen Build.

## Tasks

### Setup

- [ ] Expo-Projekt unter `mobile/` (TypeScript, Expo Router).
- [ ] Falls noch nicht vorhanden: `packages/api-client` als gemeinsames
      Package für generierte API-Typen (aus der Backend-OpenAPI-Spec, z. B.
      via `openapi-typescript`), das sowohl `frontend/` als auch `mobile/`
      einbinden (npm/pnpm Workspaces).

### Auth

- [ ] GitHub OAuth via `expo-auth-session` (Browser-basierter Flow).
      Zusätzliche Redirect-URI für die App (z. B. Custom Scheme oder Expo's
      Proxy-Redirect) in der GitHub OAuth App hinterlegen, **zusätzlich**
      zur bestehenden Web-Redirect-URI (nicht ersetzen).
- [ ] Session sicher speichern via `expo-secure-store`.

### Screens

- [ ] Login-Screen.
- [ ] Home-Feed (analog `/` auf Web).
- [ ] Projekt-Detail (Overview/Team/Quests/Activity).
- [ ] Quest-Liste + Quest-Detail inkl. Claim-Aktion.
- [ ] Profil (eigenes + fremde), inkl. XP/Level/Reputation-Anzeige.
- [ ] Notifications-Liste.

## Definition of Done

- [ ] Ein User kann sich auf einem echten Gerät oder Simulator per GitHub
      einloggen.
- [ ] Der Login-Zustand bleibt nach Neustart der App erhalten.
- [ ] Der User kann eine offene Quest browsen und claimen – der Zustand
      spiegelt sich danach korrekt auch auf der Web-Version desselben
      Accounts wider (gleiche API, gleiche Datenbank).
- [ ] Die Notification-Liste zeigt dieselben Einträge wie auf Web.
