# Frontend Navigation Redesign

Ausgangspunkt: Feedback nach dem ersten Tailwind/Design-System-Pass ("sieht schon
besser aus, aber..."). Referenzen vom Nutzer: Screenshots von X (Twitter) und
GitHub, plus der Wunsch, sich nochmal am Navbar-Stil des alten Projekts
(`GameDevsConnect.Frontend`, `styles/navbar/navbar.module.css`) zu orientieren.

## Ist-Zustand

- Eine einzelne Top-Bar (`SiteHeader.tsx`), volle Breite, Inhalt mit
  `gap-4 px-4 py-3` verteilt (kein zentrierter max-width-Container).
- Enthält: Brand-Link, "Discover"-Link, Suchleiste, `LoginLink`, `NotificationBell`.
- Kein Sidebar/floating Nav.
- "Mein Profil", "Profil bearbeiten", "Neues Projekt", "Quests entdecken",
  "Logout" liegen als Text-Links in `HomeFeed.tsx` (nur auf der Feed-Seite
  sichtbar, nicht global erreichbar).
- Globaler Auth-Gate ist bereits über `proxy.ts` gelöst (jede Seite außer
  `/login` verlangt die `gdc_session`-Cookie).

## Referenzen

- **Altes Projekt** (`navbar.module.css`): eine schwebende, vertikal
  zentrierte Pille (`position: fixed; top: 30%`) mit quadratischen
  Icon-Buttons (60×60px, `border-radius: 8px`, Farbverlauf color1/color4).
- **X/Twitter**: linke Sidebar mit Icon+Label-Einträgen (Home, Explore,
  Notifications, Profile, ...), zentrierte Feed-Spalte mit fester max-width,
  rechte Spalte mit Suche/Trends. Profilseite hat oben rechts einen
  "Edit profile"-Pill-Button direkt im Profil-Header.
- **GitHub**: schlanke, eher kompakte Top-Bar (Logo, Suche, Icons, Avatar),
  Seiteninhalt darunter in einer eigenen Breite, nicht die Top-Bar selbst
  die primäre Navigation.

## Geplante Änderungen

### 1. Linke schwebende Icon-Nav (`LeftNav.tsx` – neu)

- Fixed, vertikal zentriert am linken Rand (`top: 50%`, `-translate-y-1/2`),
  Panel-Optik (2px `border-border-strong`, `bg-surface`, `rounded-lg`),
  angelehnt an die alte `.nav`/`.nav_item`-Optik.
- Einträge (Icon als Emoji, da keine Icon-Bibliothek installiert ist):
  - 🏠 Home → `/`
  - 🧭 Discover → `/discover`
  - ⚔ Quests → `/quests`
  - 🔔 Notifications → `/notifications`
  - 👤 Profil → `/users/{me.username}` (Username wird client-seitig per
    `/api/auth/me` geholt, analog zum bestehenden `NotificationBell`-Muster)
- Aktiver Eintrag wird per `usePathname()` hervorgehoben.
- Nur ab `lg`-Breakpoint sichtbar (`hidden lg:flex`), um Überlappung mit
  Content auf schmalen Fenstern zu vermeiden – kein zusätzliches
  Mobile-Bottom-Bar-Pattern (Umfang bewusst begrenzt).

### 2. Top-Bar zentrieren (`SiteHeader.tsx` – Umbau)

- Äußeres `<header>` bleibt volle Breite (Border/Background), der Inhalt
  wird in einen `mx-auto max-w-[1200px]` Container gepackt (GitHub-Stil:
  schlank, nicht auseinandergezogen).
- Enthält nur noch: Brand-Link (Home), Suchleiste, `NotificationBell`
  (rechtsbündig). "Discover" fliegt raus (jetzt in der linken Nav).
- `LoginLink` wird entfernt – seit dem globalen Auth-Gate über `proxy.ts`
  ist jede erreichbare Seite außer `/login` ohnehin eingeloggt, der Link
  würde nie mehr sichtbar werden.
- Rendert weiterhin `null` auf `/login` (inkl. der neuen `LeftNav`).

### 3. Profilseite bekommt Account-Aktionen (`users/[username]/page.tsx`)

- Bei `isOwnProfile`: statt nur "Profil bearbeiten" zusätzlich einen
  "+ Neues Projekt"-Button, beide als Pill-Buttons oben im Profil-Header
  (Twitter-Stil: Buttons rechtsbündig neben Avatar/Name).
- "Meine Projekte" ist bereits als "Projects"-Sektion auf der Profilseite
  vorhanden (Phase 1) – hier keine strukturelle Änderung nötig.

### 4. Feed-Seite aufräumen (`HomeFeed.tsx`)

- Nav-Zeile mit den fünf Text-Links entfällt (jetzt redundant zur linken
  Icon-Nav bzw. zur Profilseite).
- Übrig bleibt nur der Logout-Button, weiterhin oben bei der
  "Angemeldet als..."-Zeile.

## Entscheidungen (Antworten des Nutzers)

1. **Icons**: `lucide-react` statt Emoji.
2. **Kleine Screens**: linke Nav wird auf Mobile zu einer Bottom-Bar
   (fixed unten, horizontal), statt komplett zu verschwinden.
3. **Positionierung**: vertikal zentriert wie im alten Projekt
   (`position: fixed; top: 50%` o. ä., floating Pille).
4. **Max-width**: Top-Bar-Inhalt und Seiteninhalt (`PageContainer`) nutzen
   dieselbe max-width, sodass alles wie bei X in einem gemeinsamen
   zentrierten Block sitzt. `PageContainer` wird daher von `max-w-3xl` auf
   dieselbe Breite wie die Top-Bar (`max-w-[1200px]`, siehe Punkt 4)
   angehoben – Formulare/Detailseiten bekommen zusätzlich ein inneres
   `max-w-xl`/`max-w-md`, wo bisher schon eine engere Breite genutzt wurde,
   damit lange Formulare nicht auf 1200px auseinandergezogen werden.

## Umsetzungs-Tickets (GitHub, Frontend-Board #3)

- Epic: **Navigation Redesign: Linke Icon-Nav + zentrierte Top-Bar**
  - Linke schwebende Icon-Nav mit `lucide-react` (Home/Discover/Quests/
    Notifications/Profil), vertikal zentriert, Panel-Optik
  - Responsive: Bottom-Bar auf kleinen Screens statt linker Nav
  - Top-Bar + Seiteninhalt auf gemeinsame max-width (1200px) zentrieren
  - Profil bearbeiten / Neues Projekt als Pill-Buttons in den
    Profil-Header verschieben (Twitter-Stil)
  - HomeFeed-Nav aufräumen (Text-Links entfernen, nur Logout bleibt)

## Status

`LeftNav.tsx` existiert als erster Entwurf (noch mit Emoji statt
`lucide-react`, noch nicht in `SiteHeader.tsx` eingebunden). Wird im Zuge
der Umsetzung auf `lucide-react` + Responsive-Verhalten überarbeitet.
