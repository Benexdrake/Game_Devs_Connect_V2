# Gamedevs Connect – MVP / Product Roadmap

## 1. Vision

Gamedevs Connect soll eine Plattform sein, auf der Game Developer gemeinsam Spiele
entwickeln können.

Die ursprüngliche Idee war ein "Twitter für Game Developer".

Die neue Richtung orientiert sich stärker an GitHub:

> Projekte stehen im Mittelpunkt.
> Nutzer können zu Projekten beitragen.
> Contributions werden sichtbar und erzeugen Reputation.

Gamedevs Connect soll dadurch eine Mischung aus:

- GitHub
- Community
- Portfolio
- Collaboration Platform
- später eventuell Marketplace

für Indie Game Development werden.

---

# 2. Kernproblem

Ein Indie Developer hat häufig ein Problem:

> "Ich kann mein Spiel programmieren, aber mir fehlen Artists, Musiker,
> 3D Artists, Writer, Sound Designer etc."

Gleichzeitig gibt es viele Entwickler und Kreative, die:

- an interessanten Projekten mitarbeiten möchten
- Erfahrung sammeln möchten
- ein Portfolio aufbauen möchten
- andere Game Developer kennenlernen möchten
- ihre Fähigkeiten zeigen möchten

Gamedevs Connect verbindet diese beiden Seiten.

---

# 3. Kernprinzip

Die Plattform basiert auf folgendem Kreislauf:

    Project
       ↓
    Quest erstellen
       ↓
    Community findet Quest
       ↓
    User übernimmt Quest
       ↓
    Contribution wird eingereicht
       ↓
    Project Owner akzeptiert Contribution
       ↓
    User erhält XP / Reputation
       ↓
    Contribution erscheint im Profil
       ↓
    Andere Projekte entdecken den User

Das ist der zentrale Product Loop.

---

# 4. Produktpositionierung

Nicht:

> "Twitter für Game Developer"

Sondern:

> "Build games together."

Oder:

> "The collaboration network for indie game developers."

Langfristig:

> "GitHub for Game Development."

---

# 5. Zielgruppe

Primäre Zielgruppe:

- Solo Game Developer
- Indie Developer
- kleine Indie Studios
- Programmierer
- 2D Artists
- 3D Artists
- Animator
- Composer
- Sound Designer
- Writer
- Game Designer

Nicht primär:

- große AAA Studios
- Unternehmen mit komplexen Enterprise-Anforderungen

---

# 6. MVP-Ziel

Das MVP soll nur eine Frage beantworten:

> Kann ein Game Developer ein Projekt veröffentlichen,
> eine Aufgabe/Quest erstellen und einen passenden Contributor finden,
> der tatsächlich etwas zum Projekt beiträgt?

Wenn diese Frage mit "Ja" beantwortet werden kann,
ist das Produkt grundsätzlich interessant.

---

# 7. MVP Features

## 7.1 Authentication

- Registrierung
- Login
- Logout
- Passwort vergessen
- E-Mail Verification
- OAuth optional

Technologie:

- ASP.NET Core Identity / bestehendes Auth-System

---

# 7.2 User Profile

Jeder User bekommt ein öffentliches Profil.

Beispiel:

    Alex
    Game Developer

    Level 12
    3.450 XP

    Skills
    ├── Unity
    ├── C#
    ├── Blender
    └── Game Design

    Contributions
    ├── Dark Forest
    ├── Cyberpunk Delivery
    └── Space Colony

    Completed Quests: 27
    Accepted Contributions: 23

Profil sollte zunächst enthalten:

- Username
- Avatar
- Bio
- Skills
- Website
- Social Links
- XP
- Level
- Completed Quests
- Projects
- Contributions

---

# 7.3 Projects

Das Project ist das wichtigste Objekt der Plattform.

Beispiel:

    Dark Forest

    Indie RPG
    Unity

    Description:
    Dark Forest is a small atmospheric RPG...

    Team
    ├── Alex
    ├── Sarah
    └── Michael

    Open Quests
    ├── Medieval Props
    ├── Main Menu Music
    └── Character Concept

    Activity
    ├── Sarah completed Medieval Props
    ├── Alex created a new Quest
    └── Michael joined the project

Project Properties:

- Name
- Slug
- Description
- Logo
- Banner
- Tags
- Engine
- Genre
- Status
- Owner
- Members
- Visibility
- CreatedAt

Project Status:

- Concept
- In Development
- Beta
- Released
- Archived

---

# 7.4 Project Visibility

MVP:

- Public
- Private

Public Projects:

- Jeder kann sie sehen
- Quests können öffentlich sein

Private Projects:

- Nur Mitglieder sehen das Projekt
- Quests können nur für Mitglieder sichtbar sein

---

# 7.5 Quests

Quests sind das wichtigste Collaboration Feature.

Beispiel:

    Create Medieval Barrel Props

    Project:
    Dark Forest

    Category:
    3D Art

    Difficulty:
    Medium

    Reward:
    250 XP

    Description:
    We need 3 low-poly medieval barrel assets.

    Requirements:
    - Blender
    - Low Poly
    - PBR

    Status:
    Open

Quest Properties:

- Title
- Description
- ProjectId
- CreatorId
- Category
- Difficulty
- XPReward
- Status
- CreatedAt
- Deadline optional
- MaxContributors optional

---

# 8. Quest Lifecycle

Eine Quest sollte einen klaren State Machine Lifecycle besitzen.

    OPEN
      ↓
    CLAIMED
      ↓
    IN_PROGRESS
      ↓
    SUBMITTED
      ↓
    REVIEW
      ↓
    ACCEPTED

Alternative:

    SUBMITTED
       ↓
    REJECTED
       ↓
    IN_PROGRESS

Oder:

    SUBMITTED
       ↓
    CHANGES_REQUESTED
       ↓
    IN_PROGRESS

Final:

    ACCEPTED
    CANCELLED

---

# 9. Quest Submission

Ein User kann eine Quest übernehmen.

Danach kann er eine Contribution einreichen.

Eine Submission kann enthalten:

- Beschreibung
- Dateien
- Bilder
- Links
- Git Repository Link
- Kommentare

Beispiel:

    Quest:
    Create Medieval Barrel Props

    Submission:

    "Created 3 low-poly barrel variants."

    Files:
    - barrel_01.fbx
    - barrel_02.fbx
    - barrel_03.fbx

    Preview:
    image.png

    Status:
    Waiting for Review

---

# 10. Contribution

Wenn der Project Owner eine Submission akzeptiert:

    Submission
         ↓
      ACCEPTED
         ↓
      Contribution
         ↓
      XP awarded
         ↓
      Profile updated

Contribution sollte dauerhaft im Profil sichtbar sein.

Beispiel:

    Alex contributed to Dark Forest

    Quest:
    Medieval Props

    Category:
    3D Art

    +250 XP

---

# 11. XP System

XP soll nicht nur ein Spielzeug sein.

XP soll Contributions repräsentieren.

Beispiel:

    Quest completed              +100 XP
    Contribution accepted        +100 XP
    Difficult Quest              Bonus
    Project milestone            Bonus optional

Nicht:

    Login                         +1 XP
    Post erstellen                +5 XP
    Like bekommen                 +1 XP

Sonst entsteht XP Farming.

---

# 12. Level System

Beispiel:

    Level 1     0 XP
    Level 2     100 XP
    Level 3     250 XP
    Level 4     500 XP
    Level 5     850 XP
    ...

Die genaue Formel kann später angepasst werden.

Wichtig:

Level soll nicht der wichtigste Wert sein.

Wichtiger sind:

- Accepted Contributions
- Reputation
- Skills
- Projects
- Portfolio

---

# 13. Reputation

Langfristig sollte Reputation wichtiger sein als XP.

Beispiel:

    Alex

    Level 18
    4.820 XP

    Reputation: 4.8 / 5

    43 Quests completed
    38 Contributions accepted
    12 Projects contributed to

    Skills:

    Unity       █████████░
    C#          ████████░░
    Blender     ██████░░░░

Reputation darf nicht leicht manipulierbar sein.

---

# 14. Skills

Users können Skills besitzen.

Beispiele:

- Unity
- Unreal Engine
- Godot
- C#
- C++
- Blender
- Maya
- 3ds Max
- Photoshop
- Substance Painter
- 2D Art
- 3D Art
- Animation
- Rigging
- Music
- Sound Design
- Game Design
- Level Design
- Writing

Projects und Quests können ebenfalls Skills/Tags besitzen.

Dadurch wird später Matching möglich.

---

# 15. Quest Discovery

User sollte Quests finden können.

Beispiel:

    Browse Quests

    [ Search ]

    Category:
    [3D Art]

    Engine:
    [Unity]

    Difficulty:
    [Medium]

    Reward:
    [100+ XP]

    ─────────────────────

    Create Medieval Props
    Dark Forest
    250 XP
    Blender / 3D Art

    ─────────────────────

    Main Menu Music
    Cyberpunk Delivery
    300 XP
    Music / BGM

MVP:

- Search
- Category Filter
- Skill Filter
- Project Filter

Später:

- Recommendation System
- Skill Matching
- Personalized Feed

---

# 16. Project Discovery

User sollte auch Projekte entdecken können.

Beispiel:

    Discover Projects

    Trending
    Recently Updated
    Looking for Contributors
    New Projects

Project Cards:

    [IMAGE]

    Dark Forest
    Indie RPG

    Unity
    RPG
    3D

    4 Members
    7 Open Quests

    [View Project]

---

# 17. Activity Feed

Der bisherige Twitter-Ansatz kann weiterhin verwendet werden.

Aber:

> Der Feed ist nicht mehr das Hauptprodukt.

Der Feed entsteht aus Project Activity.

Beispiele:

    Sarah completed a Quest in Dark Forest

    Alex created a new Project

    Michael joined Cyberpunk Delivery

    Lisa reached Level 10

    Dark Forest released a new Demo

Der Feed wird damit ein Nebenprodukt
der tatsächlichen Contributions.

---

# 18. Posts

Posts können weiterhin existieren.

Aber sie sind nicht mehr das zentrale Objekt.

MVP:

- User kann Project Update posten
- User kann Project Activity kommentieren
- Likes optional

Beispiel:

    Dark Forest

    "New character system implemented!"

    [Screenshot]

    24 Likes
    7 Comments

Posts gehören idealerweise zu:

- User
- Project

Nicht nur zum globalen Feed.

---

# 19. Follows

Follows können aus dem bisherigen System übernommen werden.

User kann folgen:

- anderen Usern
- Projects

Feed zeigt anschließend relevante Activity.

---

# 20. Notifications

Notifications:

    Sarah accepted your Quest submission.

    You received 250 XP.

    Alex followed your Project.

    New Quest available in Dark Forest.

    You were invited to Project X.

MVP:

- In-App Notifications

E-Mail später.

---

# 21. Project Team

Ein Project besitzt Members.

Roles:

    Owner
    Admin
    Contributor

Owner:

- Project löschen
- Quests erstellen
- Submissions akzeptieren
- Members verwalten

Admin:

- Quests verwalten
- Submissions reviewen

Contributor:

- Quests bearbeiten
- Contributions erstellen

---

# 22. Datenmodell

Vereinfachtes Modell:

    User
      │
      ├── Skills
      ├── Projects
      ├── Contributions
      ├── Quests
      └── Notifications

    Project
      │
      ├── Members
      ├── Quests
      ├── Posts
      └── Activity

    Quest
      │
      ├── Creator
      ├── Project
      ├── Assignee
      └── Submissions

    Submission
      │
      ├── Quest
      ├── User
      └── Files

    Contribution
      │
      ├── User
      ├── Project
      └── Quest

---

# 23. Mögliche Entities

Backend:

    User
    UserSkill
    Skill

    Project
    ProjectMember
    ProjectTag
    Tag

    Quest
    QuestAssignment
    QuestSubmission

    Contribution

    Post
    Comment
    Like

    Follow

    Notification

    XPTransaction
    Achievement (später)

Nicht alles muss sofort implementiert werden.

---

# 24. XP sollte nachvollziehbar sein

Nicht einfach:

    User.XP += 250

Besser:

    XPTransaction

    Id
    UserId
    Amount
    Reason
    SourceType
    SourceId
    CreatedAt

Beispiel:

    +250
    QuestCompleted
    QuestId: 123

Damit kann später nachvollzogen werden,
warum ein User XP besitzt.

---

# 25. Backend Architektur

Aktuell existieren bereits mehrere Microservices.

Diese müssen für das MVP nicht zwingend verändert werden.

Wichtig:

Nicht weiter Microservices bauen, nur weil die Architektur
bereits darauf basiert.

Produktentwicklung hat Priorität.

Wenn bestehende Services funktionieren:

    Keep them.

Wenn ein Feature mit einem bestehenden Service umgesetzt werden kann:

    Reuse it.

Keine neuen Services nur für:

- XP Service
- Feed Service
- Achievement Service
- Reputation Service

wenn dadurch die Entwicklung langsamer wird.

---

# 26. Empfohlene Backend Struktur

Langfristig könnten Services beispielsweise sein:

    Identity Service
    User Service
    Project Service
    Quest Service
    Social Service
    Notification Service
    Media Service

Aber:

Diese Aufteilung ist keine MVP-Anforderung.

Ein modularer Monolith wäre ebenfalls vollkommen ausreichend.

---

# 27. Frontend

React + TypeScript.

Empfohlene Hauptbereiche:

    /login
    /register

    /home

    /discover
    /projects
    /projects/:slug

    /quests
    /quests/:id

    /users/:username

    /settings

Später:

    /notifications
    /messages
    /marketplace

---

# 28. Project Page

Die Project Page ist wahrscheinlich die wichtigste Seite.

Layout:

    ┌───────────────────────────────────────────┐
    │ Banner                                    │
    │                                           │
    │ Dark Forest                               │
    │ Indie RPG · Unity                         │
    │                                           │
    │ [Follow] [Join Project]                   │
    └───────────────────────────────────────────┘

    [Overview] [Quests] [Team] [Activity]

    ─────────────────────────────────────────────

    DESCRIPTION

    Dark Forest is ...

    ─────────────────────────────────────────────

    OPEN QUESTS

    ┌──────────────────────────────────────────┐
    │ Medieval Props                250 XP    │
    │ 3D Art · Blender                       │
    └──────────────────────────────────────────┘

    ┌──────────────────────────────────────────┐
    │ Main Menu Music              300 XP     │
    │ Music · BGM                             │
    └──────────────────────────────────────────┘

---

# 29. User Profile

Profile sollte stärker wie GitHub aussehen.

    Alex

    Game Developer

    Level 18
    4,820 XP

    [Follow]

    ABOUT

    Indie developer focusing on Unity and gameplay systems.

    SKILLS

    Unity
    C#
    Game Design

    CONTRIBUTIONS

    Dark Forest
    ├── Inventory System
    ├── Save System
    └── Quest System

    Cyberpunk Delivery
    └── UI Framework

    ACTIVITY

    ...

Das Profil wird dadurch gleichzeitig Portfolio.

---

# 30. Mobile App

NICHT im ersten MVP.

React Native erst später.

Grund:

Mobile App verdoppelt nicht nur UI-Arbeit,
sondern auch Testing, Navigation, Notifications etc.

MVP:

    Responsive React Web App

Danach:

    React Native

Mobile könnte später besonders für Notifications,
Chat und schnelle Quest-Aktionen interessant sein.

---

# 31. Monetarisierung

Nicht sofort priorisieren.

Zunächst:

    User Growth
    Engagement
    Contributions
    Retention

Später:

## Pro Developer

Beispiel:

    Free
    - Public Projects
    - Basic Profile
    - Basic Quests

    Pro
    €7.99 / month

    - Unlimited Projects
    - Advanced Portfolio
    - Analytics
    - Featured Profile
    - Advanced Discovery

## Studio

Beispiel:

    €29 / month

    - Private Projects
    - Team Management
    - Private Quests
    - Advanced Permissions

---

# 32. Marketplace – später

Eine mögliche spätere Entwicklung:

Quest:

    Create character model

    Reward:
    €150

Dann:

    Quest = Community Contribution

oder:

    Quest = Paid Task

Damit entsteht langfristig ein Marketplace.

Aber:

NICHT im MVP.

---

# 33. Marketplace Problem

Sobald Geld involviert ist, entstehen zusätzliche Probleme:

- Payments
- Refunds
- Disputes
- Taxes
- Fraud
- Contracts
- Licensing
- Asset Ownership
- Copyright
- Chargebacks

Deshalb zuerst:

    XP / Reputation / Contributions

Später:

    Paid Contributions

---

# 34. Asset Licensing

Für Game Development sehr wichtig.

Wenn User Assets beitragen:

    Wer besitzt das Asset?

    Darf das Projekt es kommerziell verwenden?

    Darf der Contributor es weiterverwenden?

    Ist es exklusiv?

Dieses Problem sollte im MVP möglichst einfach gelöst werden.

MVP:

> Contributor und Project Owner müssen sich außerhalb der Plattform
> über die Rechte einigen.

Später:

- Standard License
- Commercial License
- Exclusive License
- Attribution

---

# 35. Anti-Abuse

Das XP-System darf nicht leicht ausgenutzt werden.

Nicht erlauben:

    User erstellt Quest
    eigener Account erledigt Quest
    +5000 XP

MVP-Schutz:

- Quest Creator darf eigene Quest nicht selbst abschließen
- Project Owner muss Submission akzeptieren
- XP erst nach Acceptance
- Limits für XP pro Tag
- Cancellation / Rejection berücksichtigen

Später:

- Reputation
- Trust Score
- Abuse Detection
- Moderation

---

# 36. Moderation

MVP:

- Report User
- Report Project
- Report Quest
- Admin Panel basic

Reports:

    Spam
    Harassment
    Copyright
    Scam
    NSFW
    Other

---

# 37. Was NICHT ins MVP kommt

Sehr wichtig.

Nicht bauen:

- Chat
- Voice Chat
- Video Calls
- Marketplace
- Payments
- AI Matching
- AI Quest Generation
- Discord Integration
- Steam Integration
- Git Integration
- Game Launcher
- Mobile App
- komplexes Achievement System
- komplexes Recommendation System
- eigene Cloud IDE
- File Hosting für große Game Builds

Diese Features können später kommen.

---

# 38. MVP Roadmap

## Phase 1 – Foundation

Ziel:

Basis funktioniert.

Tasks:

- [ ] Authentication überprüfen
- [ ] User Profile
- [ ] Skills
- [ ] Project Entity
- [ ] Project CRUD
- [ ] Project Members
- [ ] Project Page

---

## Phase 2 – Quest System

Ziel:

Ein User kann Hilfe für sein Projekt anfordern.

Tasks:

- [ ] Quest Entity
- [ ] Quest erstellen
- [ ] Quest bearbeiten
- [ ] Quest löschen
- [ ] Quest Listing
- [ ] Quest Detail Page
- [ ] Quest Search
- [ ] Quest Categories
- [ ] Quest Assignment
- [ ] Quest Status

---

## Phase 3 – Contributions

Ziel:

Ein User kann tatsächlich etwas beitragen.

Tasks:

- [ ] Quest übernehmen
- [ ] Submission erstellen
- [ ] Dateien/Links hinzufügen
- [ ] Submission reviewen
- [ ] Accept
- [ ] Reject
- [ ] Changes Requested
- [ ] Contribution erstellen

---

## Phase 4 – XP / Reputation

Ziel:

Contributions werden sichtbar und belohnt.

Tasks:

- [ ] XP Transaction
- [ ] XP Calculation
- [ ] Level Calculation
- [ ] Profile XP
- [ ] Completed Quests
- [ ] Contributions
- [ ] Reputation basic

---

## Phase 5 – Social Layer

Ziel:

Community entsteht.

Tasks:

- [ ] Follow User
- [ ] Follow Project
- [ ] Project Activity
- [ ] User Activity
- [ ] Feed
- [ ] Comments
- [ ] Likes optional
- [ ] Notifications

---

## Phase 6 – Discovery

Ziel:

User finden Projekte und Quests.

Tasks:

- [ ] Discover Projects
- [ ] Discover Quests
- [ ] Search
- [ ] Skill Filters
- [ ] Genre Filters
- [ ] Engine Filters
- [ ] "Looking for Contributors"

---

# 39. MVP Definition of Done

Das MVP ist fertig, wenn folgender Flow funktioniert:

    1. User A registriert sich.

    2. User A erstellt ein Game Project.

    3. User A erstellt eine Quest:

       "Create 3 Medieval Props"

    4. User B entdeckt die Quest.

    5. User B übernimmt die Quest.

    6. User B reicht eine Contribution ein.

    7. User A akzeptiert die Contribution.

    8. User B erhält XP.

    9. User B erhält eine Contribution auf seinem Profil.

    10. Die Contribution erscheint in der Project Activity.

    11. Andere User können Project und Contributor entdecken.

Wenn dieser Flow funktioniert:

    MVP COMPLETE.

---

# 40. Wichtigster Product Metric

Nicht:

    Anzahl Posts

Nicht:

    Anzahl Likes

Nicht:

    Anzahl registrierter User

Sondern:

> Accepted Contributions per Week

Beispiel:

    Week 1: 4
    Week 2: 8
    Week 3: 17
    Week 4: 31

Das zeigt, ob die Plattform tatsächlich Zusammenarbeit erzeugt.

Weitere interessante Metrics:

    Active Projects
    Open Quests
    Quest Acceptance Rate
    Submission Acceptance Rate
    Contributors per Project
    Weekly Active Contributors
    Returning Contributors

---

# 41. Early User Strategy

Nicht direkt versuchen, tausende User zu bekommen.

Ziel:

    10-20 aktive Game Developer

Davon:

    5 Project Owners
    10 Contributors

Erste Projects könnten sogar von mir selbst
oder Freunden erstellt werden.

Beispielsweise:

    Project A
    Project B
    Project C

Jedes Projekt sollte mehrere echte Quests besitzen.

Ziel:

> Die Plattform soll beim ersten Besuch bereits lebendig wirken.

---

# 42. Launch Strategy

Phase 1:

    Build privately

Phase 2:

    5-10 Game Developer testen lassen

Phase 3:

    Feedback sammeln

Phase 4:

    Quest System verbessern

Phase 5:

    Public Beta

Mögliche Communities:

- Reddit
- Discord
- Indie Game Developer Communities
- Game Jams
- itch.io Communities
- lokale Game Dev Communities

---

# 43. Wichtig: Nicht zu früh monetarisieren

Die ersten Fragen sollten sein:

    Finden Leute Projekte?

    Erstellen Leute Quests?

    Werden Quests übernommen?

    Werden Contributions akzeptiert?

    Kommen Contributors zurück?

Wenn ja:

    Product-Market-Fit weiter testen.

Erst danach:

    Premium
    Studio
    Marketplace

---

# 44. Technische Priorität

Bei jedem Feature fragen:

> Hilft dieses Feature dabei, dass ein Project eine Contribution bekommt?

Wenn:

    JA
        → hohe Priorität

    NEIN
        → wahrscheinlich später

Beispiele:

    Quest System
        → JA

    Submission
        → JA

    Contribution Profile
        → JA

    Chat
        → NEIN

    Fancy Profile Animation
        → NEIN

    AI Recommendations
        → NEIN

---

# 45. Architektur-Prinzip

Nicht die Architektur zum Produkt machen.

Das Produkt ist:

    Projects
       ↓
    Quests
       ↓
    Contributions
       ↓
    Reputation

Die Architektur soll diesen Flow möglichst schnell unterstützen.

---

# 46. Langfristige Vision

Wenn das MVP funktioniert, kann sich Gamedevs Connect entwickeln zu:

    Gamedevs Connect

          │
          ├── Projects
          │
          ├── Quests
          │
          ├── Contributions
          │
          ├── Profiles
          │
          ├── Reputation
          │
          ├── Community
          │
          ├── Portfolio
          │
          ├── Teams
          │
          ├── Marketplace
          │
          └── Jobs

Langfristig könnte ein User sagen:

> "Das ist mein Game Developer Profil."

und dort stehen:

    Skills
    Projects
    Contributions
    Reputation
    Portfolio
    Experience

---

# 47. Die zentrale Idee

Gamedevs Connect sollte nicht versuchen,
Twitter zu kopieren.

Twitter beantwortet:

> "Was passiert gerade?"

GitHub beantwortet:

> "Woran arbeiten Menschen und was haben sie beigetragen?"

Gamedevs Connect sollte beantworten:

> "Welche Spiele werden gerade entwickelt,
> wo wird Hilfe benötigt und was kann ich beitragen?"

Das ist die eigentliche Identität des Produkts.

---

# 48. Final Product Loop

Der wichtigste Loop der gesamten Plattform:

                    ┌─────────────┐
                    │   PROJECT   │
                    └──────┬──────┘
                           │
                           ▼
                    ┌─────────────┐
                    │    QUEST    │
                    └──────┬──────┘
                           │
                           ▼
                    ┌─────────────┐
                    │ CONTRIBUTOR │
                    └──────┬──────┘
                           │
                           ▼
                    ┌─────────────┐
                    │ SUBMISSION  │
                    └──────┬──────┘
                           │
                           ▼
                    ┌─────────────┐
                    │  ACCEPTED   │
                    └──────┬──────┘
                           │
                 ┌─────────┴─────────┐
                 ▼                   ▼
             PROJECT             USER
            improved           gains XP
                                 │
                                 ▼
                            CONTRIBUTION
                                 │
                                 ▼
                              REPUTATION
                                 │
                                 ▼
                            NEW PROJECTS
                                 │
                                 └───────→ QUEST
