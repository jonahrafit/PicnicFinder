INSERT INTO CategoryActivities (Name) 
VALUES 
    ('Activités sportives'),
    ('Activités de détente'),
    ('Activités sociales'),
    ('Activités récréatives'),
    ('Activités aventureuses'),
    ('Activités culturelles'),
    ('Autres activités');

-- Insertion des activités dans la table Activities
INSERT INTO SpaceActivities (Name, CategoryId) VALUES
('Piscine', 1),  -- Activité sportive
('Terrain de football', 1),
('Basketball', 1),
('Volleyball', 1),
('Tennis', 1),
('Badminton', 1),
('Pétanque', 1),
('Ping-pong', 1),
('Course à pied ou jogging', 1),
('Mini-golf', 1),
('Skatepark ou zone de roller', 1),
('Tir à l''arc', 1),
('Promenades dans la nature', 2),  -- Activité de détente
('Randonnée', 2),
('Observation des oiseaux', 2),
('Lecture dans un espace calme', 2),
('Aire de jeux pour enfants', 2),
('Hamacs ou zones de relaxation', 2),
('Yoga ou méditation en plein air', 2),
('Pêche', 2),
('Barbecue', 3),  -- Activité sociale
('Feu de camp', 3),
('Espace pour danses ou soirées dansantes', 3),
('Jeux de société en plein air', 3),
('Karaoké', 3),
('Repas collectifs sous des pergolas', 3),
('Photobooth pour photos souvenirs', 3),
('Tournoi ou concours sportif', 3),
('Billard', 4),  -- Activité récréative
('Baby-foot', 4),
('Escape game en plein air', 4),
('Chasse au trésor', 4),
('Jeux gonflables', 4),
('Toboggans aquatiques', 4),
('Slackline ou parcours d''équilibre', 4),
('Ateliers de bricolage ou peinture pour les enfants', 4),
('Tyrolienne', 5),  -- Activité aventureuse
('Escalade', 5),
('Parcours aventure', 5),
('Vélo tout terrain (VTT)', 5),
('Canoë ou kayak', 5),
('Spéléologie', 5),
('Descente en rappel', 5),
('Randonnée équestre', 5),
('Visite guidée des environs', 6),  -- Activité culturelle
('Ateliers éducatifs', 6),
('Expositions ou galeries temporaires', 6),
('Spectacles ou concerts', 6),
('Projection de films en plein air', 6),
('Ateliers culinaires', 6),
('Espace pour drones ou avions télécommandés', 7),  -- Autres activités
('Terrain pour cerfs-volants', 7),
('Zone pour camping temporaire', 7),
('Atelier de danse ou de fitness', 7),
('Observation des étoiles', 7);
