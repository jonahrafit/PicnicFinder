# PicnicFinder

## Technologies définies :

### Backoffice :
. SGBD : SQL Server
Base de données relationnelle pour gérer et stocker les données du projet.
Utilisée pour structurer les informations (utilisateurs, lieux, réservations, etc.).

. Accès aux données : Entity Framework (EF)
Framework d'accès aux données ORM (Object-Relational Mapping) pour .NET.
Permet une interaction simplifiée avec SQL Server via des objets et des modèles.
Idéal pour créer des backends robustes en minimisant les requêtes SQL complexes.

. Présentation : Razor Pages
Modèle de programmation pour .NET qui combine HTML, CSS, et logique côté serveur dans des fichiers Razor (.cshtml).
Convient pour créer des interfaces utilisateur simples pour l'administration (backoffice).
Exemple : un tableau pour gérer les lieux, utilisateurs, ou réservations.

### Frontoffice :
. SGBD : SQL Server
Base de données commune avec le backoffice pour maintenir la cohérence des données.

. Accès aux données : ADO.NET
Technologie d'accès aux données bas niveau.
Permet des interactions directes avec SQL Server via des commandes SQL et des DataSets.
Idéal pour une gestion fine et des performances élevées.

. Présentation : MVC (Model-View-Controller)
Architecture utilisée pour séparer les responsabilités :
Modèle : Gestion des données (via ADO.NET).
Vue : Affichage des pages web (HTML/CSS avec Razor pour le rendu dynamique).
Contrôleur : Logique métier et interaction avec l'utilisateur.
Adaptée pour des applications front-end dynamiques et évolutives.

### REST API
Point d'accès pour communiquer avec le backend (via Entity Framework).
Fournit une interface pour les actions comme :
Afficher les lieux disponibles.
Réserver un espace.
Consulter les avis.
Permet l’interaction entre le frontoffice et d’autres plateformes (par exemple, une application mobile).