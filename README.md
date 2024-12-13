Ski-Service-Webanwendung
Projektbeschreibung
Dieses Projekt ist eine Webanwendung zur Verwaltung und Buchung von Ski-Service-Dienstleistungen. Es besteht aus zwei Hauptkomponenten:

Frontend: Eine benutzerfreundliche Oberfläche, die es Nutzern ermöglicht, sich zu registrieren, einzuloggen und Ski-Services zu buchen.
Backend: Eine robuste REST-API, die mit .NET Core und Entity Framework entwickelt wurde und die Logik für Benutzerauthentifizierung, Service-Buchung und Datenbankverwaltung bereitstellt.
Ziel des Projekts
Das Ziel dieses Projekts war es:

Eine vollständige Full-Stack-Webanwendung zu entwickeln.
Authentifizierung und Autorisierung mit JWT (JSON Web Tokens) zu implementieren.
Eine RESTful-API für Benutzer- und Serviceverwaltung bereitzustellen.
Die Benutzererfahrung durch ein modernes und responsives Frontend zu verbessern.
Funktionen
Frontend
Registrierung und Login: Nutzer können ein Konto erstellen und sich einloggen.
Service-Buchung: Nutzer können Ski-Service-Dienstleistungen auswählen und buchen.
Responsive Design: Die Anwendung ist für verschiedene Geräte (Desktop, Tablet, Mobilgeräte) optimiert.
Backend
Benutzerverwaltung: CRUD-Operationen für Benutzer (Erstellen, Lesen, Aktualisieren, Löschen).
Service-Buchung: Verwaltung von Ski-Service-Aufträgen, einschließlich Zuweisung von Aufträgen an Benutzer.
JWT-Authentifizierung: Sichere Authentifizierung und Autorisierung.
CORS-Unterstützung: Ermöglicht den Zugriff auf die API vom Frontend.
Technologie-Stack
Frontend:
HTML, CSS (Bootstrap für Styling)
JavaScript
Backend:
.NET Core
Entity Framework Core
Microsoft SQL Server
Tools:
Postman (zum Testen der API)
Git & GitHub (Versionskontrolle)
Visual Studio
Setup und Installation
Voraussetzungen
.NET 6.0 SDK
Node.js (für Frontend-Paketverwaltung)
Microsoft SQL Server
Schritte zur Installation
Backend starten:

Navigieren Sie in das Backend-Verzeichnis.
Erstellen Sie die Datenbank, indem Sie dotnet ef database update ausführen.
Starten Sie den Server mit dotnet run.
Frontend starten:

Öffnen Sie die index.html-Datei im Browser, um die Anwendung zu testen.
API testen:

Verwenden Sie Postman oder einen ähnlichen Client, um die API-Endpunkte zu testen.
Wichtige Endpunkte
Benutzer-Endpunkte
POST /api/Users/Register: Benutzerregistrierung
POST /api/Users/Login: Benutzerlogin
GET /api/Users/Me: Authentifizierten Benutzer abrufen
Service-Endpunkte
GET /api/ServiceOrders: Alle Service-Buchungen abrufen
POST /api/ServiceOrders: Neue Service-Buchung erstellen
PUT /api/ServiceOrders/{id}: Service-Buchung aktualisieren
DELETE /api/ServiceOrders/{id}: Service-Buchung löschen
Autor(en)
Lenny Brun
Ege Ulu
Dieses Projekt wurde im Rahmen eines praktischen Moduls für die Schule entwickelt.

Lizenz
Dieses Projekt steht unter der MIT-Lizenz.

