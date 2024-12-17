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


--------------------------------------------------------
# PicnicFinder
PicnicFinder est une application qui aide les entreprises à trouver et réserver des lieux adaptés pour des sorties d’équipe ou des pique-niques. L'application propose une interface de gestion pour les administrateurs, une plateforme de recherche pour les employés, et un espace dédié pour les propriétaires de lieux.

---

## Fonctionnalités

### 1. Écran de login (Backoffice et Frontoffice)
- **Authentification :**
  - **Administrateurs :** Accès au tableau de bord du Backoffice.
  - **Employés :** Accès à l’espace de recherche avec email/mot de passe.
  - **Propriétaires :** Accès à leur espace de gestion via login.

---

### 2. Backoffice (Administrateur)
- **Gestion des utilisateurs :**
  - Valider ou rejeter les inscriptions des propriétaires.
  - Activer/désactiver les comptes des employés et propriétaires.

- **Gestion des espaces :**
  - Modérer les informations des espaces proposés (approbation avant publication).
  - Supprimer ou désactiver les espaces inappropriés.

- **Suivi des réservations :**
  - Consulter les réservations effectuées par les employés.
  - Gérer les conflits liés aux réservations.

- **Rapports :**
  - Générer des statistiques sur :
    - Nombre de réservations par mois.
    - Espaces les plus populaires.
    - Performances des propriétaires.

---

### 3. Frontoffice (Employé)
- **Recherche d’espaces :**
  - Critères de recherche : localisation, prix, capacité, équipements, disponibilité.
  - Affichage des résultats avec filtres (tri par distance, tarif, note des utilisateurs).

- **Réservation d’un espace :**
  - Consulter les détails d’un espace (photos, description, équipements).
  - Sélectionner une date et confirmer la réservation.

- **Suivi des réservations :**
  - Historique des réservations effectuées.
  - Possibilité d’annuler une réservation.

---

### 4. Frontoffice (Propriétaire)
- **Inscription :**
  - Création d’un compte avec nom, email, téléphone, mot de passe.
  - Validation du compte par l’administrateur.

- **Gestion des espaces :**
  - Ajouter un espace avec :
    - Nom, localisation (via carte interactive), capacité, prix, équipements disponibles.
    - Photos et description détaillée.
  - Modifier ou supprimer les espaces.

- **Suivi des réservations :**
  - Consultation des réservations sur leurs espaces.
  - Validation ou rejet des demandes de réservation.

---
## Base de Données (SQL Server)

### Tables principales :
- **Users :**
  - `UserID`, `Name`, `Email`, `Phone`, `Password`, `Role` (Admin/Employé/Propriétaire).
- **Spaces :**
  - `SpaceID`, `Name`, `Location`, `Capacity`, `Price`, `Description`, `OwnerID`.
- **Reviews (optionnel) :**
  - `ReviewID`, `SpaceID`, `EmployeeID`, `Rating`, `Comment`.

### Relations principales :
- Un propriétaire peut avoir plusieurs espaces.
- Un employé peut effectuer plusieurs réservations.
- Une réservation est associée à un espace et à un employé.

---

## Exemple d’utilisation

1. **Recherche d’un espace :**
   - Un employé entre les critères (localisation, capacité, équipements, etc.).
   - Résultats affichés sur une carte interactive avec filtres.

2. **Réservation :**
   - L’employé sélectionne un espace, consulte les détails, et confirme une réservation.

3. **Gestion des espaces (propriétaire) :**
   - Un propriétaire ajoute un nouvel espace avec les informations nécessaires.
   - Les informations sont validées par un administrateur avant publication.

4. **Rapports (administrateur) :**
   - L’administrateur génère des rapports pour analyser les performances des propriétaires et des espaces.

