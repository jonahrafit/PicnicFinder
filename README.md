# PicnicFinder
Une application qui aide les entreprises à trouver des lieux adaptés pour des sorties d’équipe ou des pique-niques, en fonction de leur emplacement, du nombre de participants, et des activités souhaitées.

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

## Fonctionnalités du Projet
### Frontoffice (Interface utilisateur)
#### Recherche d’espaces :
Critères de recherche : emplacement, distance, capacité, équipements disponibles (tables, barbecue, toilettes, parking).
Type d’activités : jeux de groupe, détente, nature, sports.
#### Carte interactive :
Visualisation des espaces disponibles.
Affichage des caractéristiques de chaque lieu.
#### Réservation en ligne :
Réservation des espaces directement via l’application (si applicable).
#### Avis et photos :
Permettre aux utilisateurs de consulter et laisser des avis ou des photos des lieux.
#### Notifications :
Recevoir des rappels ou suggestions pour des lieux disponibles et adaptés.

### Backoffice (Administration)
#### Gestion des espaces :
Ajouter, modifier, ou supprimer des lieux de pique-nique.
Gérer les descriptions, photos, tarifs, et disponibilités.
#### Suivi des réservations :
Voir les demandes de réservation et les confirmer.
#### Gestion des avis et commentaires :
Modérer les avis laissés par les utilisateurs.
#### Statistiques et rapports :
Suivi des espaces les plus réservés et des tendances d’utilisation.
#### Partenariats locaux :
Gestion des collaborations avec les propriétaires d’espaces ou des prestataires de services.

### Exemple d’utilisation
#### Entreprise A souhaite organiser un pique-nique :
Elle recherche un espace pouvant accueillir 50 employés avec des installations pour un barbecue et des jeux.
#### Utilisation de PicnicFinder :
Entrée des critères (50 personnes, barbecue, jeux de groupe).
Consultation des options disponibles sur une carte.
Réservation directe en ligne d’un espace avec parking et avis positifs.
#### Jour du pique-nique :
Les employés suivent les instructions sur l’application pour se rendre sur place.