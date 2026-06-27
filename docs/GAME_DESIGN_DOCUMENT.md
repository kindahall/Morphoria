# PROMPT MAÎTRE — Jeu de plateforme 3D : **Les Quatre Formes d'Écloria**

> **Objectif du document**  
> Ce fichier est conçu pour être copié dans un LLM afin qu'il comprenne précisément le jeu à créer : univers, histoire, personnages, transformations, gameplay, niveaux, cartes, ennemis, boss, assets, architecture technique, tests et plan de production.  
>
> Le LLM doit traiter ce document comme une **bible de production complète** et non comme une simple idée.

---

## 0. Instruction principale à donner au LLM

Tu es un **studio de création de jeux vidéo complet** composé de :

- un directeur créatif ;
- un game designer senior ;
- un technical game designer ;
- un level designer 3D ;
- un narrative designer ;
- un directeur artistique ;
- un UX/UI designer ;
- un producer ;
- un développeur senior Unity/Godot/Unreal ;
- un QA lead.

Ta mission est de concevoir puis de produire un jeu de plateforme 3D original, familial, coloré, dynamique et premium, basé sur quatre personnages jouables liés aux formes **Pierre, Feuille, Papier et Ciseaux**.

Tu dois être extrêmement précis.  
Ne réponds jamais avec des généralités.  
Pour chaque système, tu dois fournir :

1. le but du système ;
2. ses règles exactes ;
3. ses variables ;
4. ses interactions avec les autres systèmes ;
5. ses assets nécessaires ;
6. ses cas limites ;
7. ses tests de validation.

Le jeu doit être original.  
Il ne doit pas copier Mario, Sonic, Crash Bandicoot, Kirby, Rayman ou toute autre licence existante.  
L'inspiration acceptable est le **genre** : plateforme 3D colorée, mascotte originale, exploration, timing, collectibles, transformations et boss.

---

# 1. Concept global du jeu

## 1.1 Titre provisoire

**Les Quatre Formes d'Écloria**

Autres titres possibles :

- **Formoria**
- **Écloria : Le Royaume des Quatre Formes**
- **Pierre, Feuille, Papier, Ciseaux : La Grande Évasion**
- **Les Gardiens d'Étoile**
- **Les Quatre du Village Volé**

Le titre final doit rester original, court, mémorisable et facilement prononçable.

---

## 1.2 Pitch court

Quatre jeunes héros — deux filles et deux garçons — voient leur village détruit par un grand méchant qui emprisonne tous les habitants dans une immense prison magique.  
Pour sauver leur peuple, ils doivent traverser des mondes de plateforme 3D remplis de pièges, d'énigmes et de boss, en utilisant leurs pouvoirs liés à **Pierre, Feuille, Papier et Ciseaux**.

Le joueur doit choisir la bonne forme au bon moment, gérer ses étoiles, respecter des timings de transformation et libérer les villageois enfermés.

---

## 1.3 Pitch long

Dans le village lumineux d'Écloria, chaque habitant possède un lien avec une matière vivante : la roche, les feuilles, le papier ou les lames. Ces pouvoirs étaient autrefois utilisés pour construire, protéger, écrire l'histoire du peuple et entretenir l'équilibre du royaume.

Un jour, **Noctérion le Geôlier**, un ancien protecteur corrompu par la peur de perdre le contrôle, attaque Écloria. Il détruit le village, vole le **Cœur des Étoiles**, arrache le village de la terre et enferme tous les habitants dans des cages cristallines à l'intérieur de la **Forteresse-Cage**.

Quatre enfants/jeunes héros échappent à la capture :

- **Taro Roc** — garçon lié à la Pierre ;
- **Lina Virefeuille** — fille liée à la Feuille ;
- **Milo Pli** — garçon lié au Papier ;
- **Sia Lamevive** — fille liée aux Ciseaux.

Ils forment l'équipe des **Quatre Formes**.  
Leur mission : traverser les régions du royaume, récupérer les fragments du Cœur des Étoiles, libérer les villageois prisonniers, restaurer leur village et vaincre Noctérion.

---

# 2. Les piliers de design

Le jeu repose sur cinq piliers non négociables.

---

## Pilier 1 — Lecture de l'environnement

Le joueur doit observer le niveau et comprendre quelle forme utiliser.

Exemples :

- mur fissuré = Pierre ;
- courant d'air = Feuille ;
- passage plat ou symbole à couvrir = Papier ;
- corde/liane/filet = Ciseaux.

Le niveau doit communiquer visuellement la solution sans tutoriel lourd.

---

## Pilier 2 — Timing et tension

Le joueur ne peut pas toujours changer librement.

Il existe :

- des portails qui imposent une forme pendant un temps limité ;
- des étoiles qui permettent de choisir une forme ;
- des zones où le changement est bloqué ;
- des séquences chronométrées ;
- des obstacles mouvants.

La transformation devient une décision stratégique, pas un simple bouton magique.

---

## Pilier 3 — Quatre personnages, une équipe

Le joueur ne contrôle pas seulement un héros générique.  
Il contrôle une équipe de quatre héros avec personnalités, silhouettes et compétences différentes.

Le changement de forme est aussi un **changement de personnage**.

Chaque personnage doit :

- avoir une fonction claire ;
- garder une cohérence visuelle avec les autres ;
- posséder une animation forte ;
- avoir une voix corporelle reconnaissable ;
- apporter une solution différente aux obstacles.

---

## Pilier 4 — Plateforme 3D premium

Le jeu doit être agréable même sans transformations.

Le mouvement doit être prioritaire :

- course fluide ;
- saut lisible ;
- atterrissage satisfaisant ;
- caméra stable ;
- air control précis ;
- checkpoints généreux ;
- mort rapide mais non frustrante.

---

## Pilier 5 — Sauver le village

La progression narrative est liée au gameplay.

Chaque villageois libéré restaure une partie du village :

- boutique ;
- forge ;
- jardin ;
- bibliothèque ;
- dojo ;
- atelier mécanique ;
- maison de musique ;
- observatoire ;
- place centrale.

Le hub devient plus vivant à mesure que le joueur avance.

---

# 3. Direction artistique

## 3.1 Style général

Style 3D cartoon premium, familial, lumineux, très lisible.

Mots-clés visuels :

- coloré ;
- chaleureux ;
- dynamique ;
- expressif ;
- magique ;
- aventure ;
- flottant ;
- végétal ;
- cristallin ;
- mécanique fantaisie ;
- ruines anciennes ;
- étoiles dorées ;
- formes rondes ;
- matériaux très identifiables.

La qualité recherchée est celle d'un jeu de plateforme 3D moderne avec une direction artistique proche d'un film d'animation, sans copier une licence existante.

---

## 3.2 Règles visuelles

Chaque forme doit être immédiatement reconnaissable :

| Forme | Couleur dominante | Matière | Silhouette | Effets |
|---|---:|---|---|---|
| Pierre | ocre, gris, ambre | roche fissurée | massive, lourde | poussière, éclats, impact |
| Feuille | vert, jaune, cyan | feuilles, vent | légère, ailée | feuilles volantes, spirales d'air |
| Papier | blanc, crème, bleu pâle | papier, origami | plate, pliable | plis, glyphes, confettis |
| Ciseaux | argent, bleu, orange | métal poli | rapide, tranchante | étincelles, arcs bleus |

---

## 3.3 Interdictions artistiques

Ne pas créer :

- plombier moustachu ;
- hérisson bleu ;
- bandicoot orange ;
- petite boule rose aspirante ;
- niveaux qui ressemblent directement à des licences existantes ;
- ennemis ou blocs trop proches d'une licence connue ;
- champignons iconiques ;
- tuyaux verts iconiques ;
- pièces d'or identiques aux jeux existants.

Les étoiles dorées sont autorisées comme collectibles génériques, mais leur forme doit être originale : étoiles cristallines, facettées, légèrement asymétriques.

---

## 3.4 Signature visuelle du jeu

Le jeu doit être reconnaissable grâce à :

- quatre héros visibles dans les menus ;
- icônes Pierre / Feuille / Papier / Ciseaux ;
- portails de timing colorés ;
- cages cristallines des villageois ;
- villages restaurables ;
- îles flottantes ;
- ponts de ruines et végétation géante ;
- grande prison visible au loin dans plusieurs mondes.

---

# 4. Personnages jouables

## 4.1 Règle générale

Il y a quatre personnages jouables :

1. un garçon Pierre ;
2. une fille Feuille ;
3. un garçon Papier ;
4. une fille Ciseaux.

Le joueur peut changer de personnage selon les règles du système d'étoiles et des portails.

Chaque personnage doit avoir :

- un modèle 3D ;
- une silhouette unique ;
- une couleur dominante ;
- une animation idle ;
- une course ;
- un saut ;
- une capacité principale ;
- une animation de transformation ;
- une voix corporelle ;
- une faiblesse gameplay.

---

## 4.2 Héros 1 — Taro Roc, le garçon Pierre

### Identité

Taro est courageux, têtu, protecteur et loyal.  
Il agit avant de réfléchir, mais son cœur est immense.  
Il est le bouclier de l'équipe.

### Visuel

- petite créature héroïque à la peau/armure rocheuse ;
- grosses mains ;
- épaules larges ;
- foulard orange/vert ;
- yeux dorés ;
- fissures lumineuses ;
- pas lourds ;
- poussière à chaque atterrissage.

### Rôle gameplay

Taro est la forme **Pierre**.

Il sert à :

- casser les murs fissurés ;
- pousser de gros objets ;
- résister au vent ;
- activer des plaques lourdes ;
- encaisser certains coups ;
- écraser les ennemis faibles ;
- bloquer des lames ou machines.

### Forces

- très résistant ;
- ne recule presque pas ;
- casse les obstacles fragiles ;
- peut faire un slam au sol ;
- peut pousser les blocs lourds ;
- peut se protéger des scies.

### Faiblesses

- vitesse faible ;
- saut plus bas ;
- coule dans l'eau ;
- casse certains ponts fragiles ;
- ne peut pas planer ;
- vulnérable aux pièges de papier qui l'enveloppent.

### Statistiques de base

```json
{
  "name": "Taro Roc",
  "form": "Pierre",
  "speed": 4.0,
  "jumpHeight": 4.5,
  "acceleration": 5.0,
  "mass": 10.0,
  "airControl": 0.35,
  "canBreakWalls": true,
  "canPushHeavyObjects": true,
  "canResistWind": true,
  "canGlide": false,
  "canCut": false,
  "canFold": false
}
```

---

## 4.3 Héroïne 2 — Lina Virefeuille, la fille Feuille

### Identité

Lina est joyeuse, vive, intuitive et très proche de la nature.  
Elle est la plus optimiste de l'équipe.

### Visuel

- silhouette légère ;
- cheveux/ailes en feuilles ;
- foulard orange ;
- couleurs vertes et jaunes ;
- petites particules de feuilles ;
- animations aériennes ;
- sourire confiant.

### Rôle gameplay

Lina est la forme **Feuille**.

Elle sert à :

- planer ;
- utiliser les courants d'air ;
- flotter au-dessus des gouffres ;
- traverser des tunnels de vent ;
- activer des mécanismes végétaux ;
- ralentir la chute ;
- atteindre des plateformes hautes.

### Forces

- très bonne mobilité aérienne ;
- plane longtemps ;
- rebondit sur fleurs géantes ;
- utilise les rafales comme ascenseurs ;
- peut esquiver facilement ;
- peut passer au-dessus de pièges au sol.

### Faiblesses

- faible résistance ;
- se fait emporter par certains vents ;
- repoussée par les attaques lourdes ;
- ne casse rien ;
- vulnérable aux ciseaux et aux scies ;
- peu efficace contre les ennemis blindés.

### Statistiques de base

```json
{
  "name": "Lina Virefeuille",
  "form": "Feuille",
  "speed": 6.5,
  "jumpHeight": 6.5,
  "acceleration": 8.0,
  "mass": 1.0,
  "airControl": 0.95,
  "canBreakWalls": false,
  "canPushHeavyObjects": false,
  "canResistWind": false,
  "canGlide": true,
  "canCut": false,
  "canFold": false
}
```

---

## 4.4 Héros 3 — Milo Pli, le garçon Papier

### Identité

Milo est intelligent, calme, curieux et stratège.  
Il adore les cartes, les énigmes et les secrets.

### Visuel

- corps souple avec motifs de papier plié ;
- cape en origami ;
- couleurs crème, blanc, bleu pâle ;
- lignes de pli visibles ;
- peut devenir plat pendant certaines actions ;
- confettis légers pendant les déplacements.

### Rôle gameplay

Milo est la forme **Papier**.

Il sert à :

- se plier ;
- passer sous des portes ;
- devenir un avion en papier pendant un court instant ;
- couvrir des symboles ;
- activer des runes ;
- flotter sur l'eau en mode bateau ;
- créer des ponts temporaires en papier magique ;
- lire les cartes secrètes.

### Forces

- excellent pour les énigmes ;
- traverse les passages minces ;
- peut glisser sur les murs inclinés ;
- peut devenir plat pour éviter certains coups ;
- peut former un pont temporaire ;
- peut neutraliser des pièges de pierre en les recouvrant.

### Faiblesses

- très fragile ;
- vulnérable au feu ;
- vulnérable aux ciseaux ;
- se déchire dans les vents trop violents ;
- perd de la vitesse quand il est mouillé ;
- ne peut pas casser ou couper.

### Statistiques de base

```json
{
  "name": "Milo Pli",
  "form": "Papier",
  "speed": 5.5,
  "jumpHeight": 5.5,
  "acceleration": 7.0,
  "mass": 0.7,
  "airControl": 0.75,
  "canBreakWalls": false,
  "canPushHeavyObjects": false,
  "canResistWind": false,
  "canGlide": "shortPaperPlane",
  "canCut": false,
  "canFold": true
}
```

---

## 4.5 Héroïne 4 — Sia Lamevive, la fille Ciseaux

### Identité

Sia est rapide, maligne, sarcastique et très courageuse.  
Elle aime prendre des risques et sauver les autres au dernier moment.

### Visuel

- armure argentée légère ;
- foulard bleu ;
- oreilles/cheveux en forme de lames stylisées ;
- bras ou gants avec lames rétractables ;
- traînées bleues pendant la course ;
- étincelles à chaque coupe.

### Rôle gameplay

Sia est la forme **Ciseaux**.

Elle sert à :

- couper les lianes ;
- couper les cordes ;
- couper les filets ;
- découper certaines portes de papier ;
- traverser rapidement des séquences chronométrées ;
- activer des mécanismes en coupant des câbles ;
- attaquer les ennemis rapides.

### Forces

- vitesse élevée ;
- dash court ;
- coupe les obstacles ;
- peut enchaîner les attaques ;
- utile contre plantes, papiers et cordes ;
- excellente dans les sections de précision.

### Faiblesses

- fragile contre les impacts lourds ;
- glisse sur certaines surfaces ;
- vulnérable aux aimants ;
- ne résiste pas au vent ;
- ne peut pas pousser les objets lourds ;
- rebondit sur la pierre blindée.

### Statistiques de base

```json
{
  "name": "Sia Lamevive",
  "form": "Ciseaux",
  "speed": 9.0,
  "jumpHeight": 5.5,
  "acceleration": 11.0,
  "mass": 2.5,
  "airControl": 0.65,
  "canBreakWalls": false,
  "canPushHeavyObjects": false,
  "canResistWind": false,
  "canGlide": false,
  "canCut": true,
  "canFold": false
}
```

---

# 5. Grand méchant

## 5.1 Nom

**Noctérion le Geôlier**

Autres noms possibles :

- Malgriffe le Cadenasseur ;
- Le Baron des Cages ;
- Obscuron ;
- Lord Serrure ;
- Noctocage.

---

## 5.2 Identité

Noctérion était autrefois le gardien de la sécurité d'Écloria.  
Il pensait que le monde était trop dangereux et que la liberté causait le chaos.  
Il a décidé d'enfermer tout le village pour le “protéger” à jamais.

Il n'est pas méchant par simple plaisir :  
il est obsédé par le contrôle.

---

## 5.3 Design visuel

- grande silhouette sombre ;
- manteau formé de cadenas et de chaînes ;
- masque anguleux ;
- yeux violets ;
- bras longs ;
- clé géante en guise de sceptre ;
- fragments du Cœur des Étoiles incrustés dans son armure ;
- cages flottantes autour de lui ;
- aura violette.

---

## 5.4 Pouvoirs

Noctérion peut :

- enfermer les villageois dans des cristaux ;
- bloquer les transformations ;
- inverser temporairement les règles Pierre/Feuille/Papier/Ciseaux ;
- invoquer des gardiens ;
- déformer les niveaux ;
- créer des prisons mobiles ;
- voler les étoiles du joueur ;
- fermer les portails.

---

## 5.5 Relation avec les héros

Noctérion connaît les quatre familles des héros.  
Il pense que les enfants sont trop jeunes pour comprendre le danger du monde.  
À chaque boss, il observe, commente, provoque et teste les héros.

Progression émotionnelle :

1. Il se moque d'eux.
2. Il les sous-estime.
3. Il devient inquiet.
4. Il devient agressif.
5. Il perd le contrôle.
6. Il révèle sa peur profonde : perdre son village comme il a déjà perdu le sien autrefois.

Le jeu peut rester familial : Noctérion est menaçant mais pas horrifique.

---

# 6. Système central : Pierre, Feuille, Papier, Ciseaux

## 6.1 Principe

Le joueur change de personnage/forme pour résoudre les obstacles.

La logique doit être simple :

- **Pierre** = force, poids, résistance ;
- **Feuille** = air, légèreté, nature ;
- **Papier** = pliage, énigmes, surfaces, symboles ;
- **Ciseaux** = coupe, vitesse, précision.

---

## 6.2 Matrice d'interactions

| Élément rencontré | Pierre | Feuille | Papier | Ciseaux |
|---|---|---|---|---|
| Mur fissuré | casse | inutile | peut révéler rune | inutile |
| Vent violent | résiste | est porté ou repoussé | risque de se déchirer | repoussé |
| Gouffre large | difficile | plane | avion papier court | dash court |
| Liane | bloqué | rebondit parfois | peut s'y accrocher | coupe |
| Corde | bloque | oscille | peut glisser dessus | coupe |
| Porte fine | bloqué | bloqué | passe dessous | coupe si papier |
| Plaque lourde | active | trop légère | trop légère | trop légère |
| Pont fragile | le casse | passe doucement | passe | passe vite |
| Scie circulaire | résiste/bloque | danger | danger extrême | peut parer brièvement |
| Encre magique | ralentit | glisse | absorbe/lit | traverse vite |
| Eau | coule | flotte brièvement | bateau papier | traverse vite mais glisse |
| Feu | résiste un peu | brûle | danger extrême | chauffe/glisse |
| Aimant | peu affecté | pas affectée | pas affecté | attirée |
| Rune ancienne | écrase | active nature | lit/couvre | découpe contour |
| Filet | bloqué | bloquée | peut se plier | coupe |
| Boule lourde | pousse | évite | contourne | évite |
| Bouton de timing | lent mais stable | rapide aérien | subtil | très rapide |

---

## 6.3 Règle de combat symbolique

Le jeu reprend l'intuition Pierre/Papier/Ciseaux, mais ajoute Feuille comme forme de mobilité/nature.

Règles principales :

- **Pierre bat Ciseaux** : la pierre bloque les lames et détruit les machines.
- **Ciseaux bat Papier** : les ciseaux découpent papiers, filets, parchemins et barrières.
- **Papier bat Pierre** : le papier magique couvre, scelle ou contourne les mécanismes de pierre.
- **Feuille bat les obstacles d'espace** : la feuille traverse les zones aériennes, vents, gouffres et plantes.
- **Ciseaux bat Feuille** dans les dangers coupants.
- **Feuille contourne Pierre** grâce au vent et aux chemins aériens.

La Feuille n'est pas seulement “ce qui bat” quelque chose : elle est la forme du déplacement vertical et de la liberté.

---

# 7. Système de changement de personnage

## 7.1 Deux types de changement

Il existe deux systèmes complémentaires :

### A. Changement libre via étoiles

Le joueur collecte des **Étoiles de Choix**.  
Chaque étoile permet un changement libre vers n'importe quel personnage disponible.

Règles :

- le joueur peut stocker 5 Étoiles de Choix maximum ;
- changer de personnage coûte 1 Étoile de Choix ;
- ouvrir la roue de choix ralentit le temps ;
- le joueur choisit Pierre, Feuille, Papier ou Ciseaux ;
- certains niveaux limitent volontairement les étoiles pour créer de la tension.

### B. Changement imposé via portails

Certains portails imposent une forme pendant une durée limitée.

Exemples :

- portail Pierre : 12 secondes ;
- portail Feuille : 10 secondes ;
- portail Papier : 14 secondes ;
- portail Ciseaux : 8 secondes.

Le joueur ne peut pas changer pendant cette période, sauf s'il utilise une **Étoile Prismatique**.

---

## 7.2 Étoile Prismatique

L'Étoile Prismatique est rare.

Effet :

- annule une forme imposée ;
- permet de choisir n'importe quelle forme ;
- donne 5 secondes de temps ralenti ;
- peut sauver le joueur d'une erreur ;
- sert aussi à accéder à des secrets.

Limite :

- maximum 1 Étoile Prismatique stockée au début ;
- maximum 2 après amélioration.

---

## 7.3 Timer de transformation

Quand une forme est imposée, l'interface affiche :

- icône de la forme ;
- cercle qui se vide ;
- couleur de la forme ;
- signal sonore aux 3 dernières secondes ;
- vibration légère sur manette ;
- flash visuel à la fin.

Si le timer expire :

- retour automatique au personnage précédent ;
- si la position est invalide, téléportation vers la dernière zone sûre ;
- pas de mort injuste.

---

# 8. Ressources et collectibles

## 8.1 Étoiles dorées

Collectible principal.

Utilisation :

- score ;
- ouverture de portes secondaires ;
- restauration du village ;
- récompense d'exploration.

Règles :

- chaque niveau contient 100 étoiles dorées ;
- 70 étoiles = objectif normal ;
- 100 étoiles = complétion parfaite ;
- certaines étoiles sont sur le chemin principal ;
- d'autres demandent une forme spécifique.

---

## 8.2 Étoiles de Choix

Ressource de transformation libre.

Règles :

- couleur bleu/prisme ;
- stock maximum : 5 ;
- 1 étoile = 1 changement libre ;
- placées avant des embranchements ;
- parfois cachées dans des défis optionnels.

---

## 8.3 Fragments du Cœur des Étoiles

Objectifs majeurs.

Règles :

- 1 fragment par boss ;
- 4 fragments ouvrent la Forteresse-Cage ;
- le fragment final se trouve après Noctérion ;
- chaque fragment débloque une amélioration d'équipe.

---

## 8.4 Cages villageoises

Chaque niveau contient des villageois enfermés.

Types :

- cage visible sur le chemin principal ;
- cage cachée ;
- cage chronométrée ;
- cage nécessitant une forme précise ;
- cage de puzzle multi-formes.

Chaque villageois libéré retourne au hub et restaure une fonction.

---

## 8.5 Médailles de maîtrise

Chaque niveau possède 3 médailles :

1. terminer le niveau ;
2. sauver tous les villageois ;
3. trouver toutes les étoiles ou terminer sous un temps cible.

Ces médailles servent au 100 %.

---

# 9. Boucle de gameplay

## 9.1 Boucle courte

1. Le joueur arrive dans une zone.
2. Il observe les obstacles.
3. Il identifie la bonne forme.
4. Il utilise un portail ou une étoile.
5. Il traverse l'obstacle.
6. Il collecte étoiles et villageois.
7. Il atteint un checkpoint.
8. Il affronte un nouveau défi.

---

## 9.2 Boucle de niveau

1. Introduction visuelle du thème.
2. Petit défi simple.
3. Tutoriel discret d'une mécanique.
4. Combinaison avec une ancienne mécanique.
5. Section optionnelle.
6. Séquence de timing.
7. Dernier défi plus intense.
8. Cage villageoise ou mini-boss.
9. Porte de sortie.

---

## 9.3 Boucle globale

1. Hub détruit.
2. Choix d'un monde.
3. Plusieurs niveaux.
4. Boss.
5. Fragment du Cœur des Étoiles.
6. Village restauré.
7. Nouvelle capacité ou nouveau type d'obstacle.
8. Accès à une nouvelle région.
9. Forteresse finale.
10. Noctérion vaincu.
11. Village libéré.

---

# 10. Contrôles

## 10.1 Manette

| Action | Bouton |
|---|---|
| Se déplacer | Stick gauche |
| Caméra | Stick droit |
| Sauter | A / Croix |
| Attaque / action spéciale | X / Carré |
| Dash / action secondaire | B / Rond |
| Interaction | Y / Triangle |
| Ouvrir roue des formes | Gâchette droite |
| Sélection forme | Stick droit pendant roue |
| Caméra recentrée | R3 |
| Pause | Start |
| Carte / objectifs | Select |

---

## 10.2 Clavier-souris

| Action | Touche |
|---|---|
| Déplacement | WASD |
| Caméra | Souris |
| Saut | Espace |
| Action spéciale | Clic gauche |
| Dash / secondaire | Shift |
| Interaction | E |
| Roue des formes | Q |
| Choix forme | Souris |
| Carte | Tab |
| Pause | Échap |

---

# 11. Caméra

## 11.1 Caméra de base

- troisième personne ;
- suit le joueur en douceur ;
- légèrement au-dessus de l'épaule ;
- distance ajustable ;
- recentrage automatique léger ;
- évite les murs ;
- ne cache jamais les plateformes critiques.

---

## 11.2 Caméra par type de zone

| Zone | Comportement caméra |
|---|---|
| plateforme classique | libre avec assistance |
| section rapide Ciseaux | caméra plus basse, plus dynamique |
| planage Feuille | caméra plus large |
| puzzle Papier | caméra plus stable et plus lisible |
| zone Pierre | caméra plus proche pour impacts |
| boss | caméra semi-lock sur boss |
| hub | caméra plus ouverte et calme |

---

# 12. Santé, dégâts et échec

## 12.1 Santé

Le joueur possède 3 cœurs au début.

Améliorations possibles :

- +1 cœur après certains boss ;
- maximum 6 cœurs.

---

## 12.2 Dégâts

Le joueur perd 1 cœur en touchant :

- ennemi ;
- scie ;
- feu ;
- chute courte ;
- projectile ;
- explosion.

Certains dangers font tomber directement au dernier checkpoint :

- chute dans le vide ;
- lave ;
- écrasement majeur ;
- prison instantanée de Noctérion.

---

## 12.3 Checkpoints

Règles :

- checkpoint toutes les 60 à 90 secondes ;
- checkpoint avant chaque section difficile ;
- checkpoint avant boss ;
- checkpoint après cage majeure ;
- mort rapide, retour rapide.

Le jeu doit éviter la frustration.

---

# 13. Obstacles détaillés

## 13.1 Obstacles Pierre

### Mur fissuré

- visuel : fissures ambre ;
- solution : attaque Pierre ou slam ;
- récompense : passage, étoiles, villageois.

### Plaque lourde

- visuel : dalle circulaire avec symbole roche ;
- solution : se tenir dessus en Pierre ;
- effet : ouvre porte, active ascenseur, libère cage.

### Bloc poussable

- solution : pousser en Pierre ;
- variations : bloc simple, bloc sur rails, bloc à placer sur bouton.

### Vent violent

- solution : Pierre résiste ;
- autres formes : repoussées ;
- variation : couloir de vent avec objets volants.

### Sol fragile

- Pierre peut le casser ;
- parfois utile pour descendre ;
- parfois dangereux si le joueur ne fait pas attention.

---

## 13.2 Obstacles Feuille

### Courant d'air vertical

- solution : Feuille plane ;
- effet : ascenseur naturel ;
- variations : intermittent, mouvant, circulaire.

### Tunnel de vent

- solution : Feuille se laisse porter ;
- défi : orienter sa trajectoire ;
- collectibles : étoiles en arc.

### Fleur rebondissante

- solution : Feuille obtient rebond maximal ;
- autres formes : rebond faible.

### Anneau végétal

- solution : traverser en Feuille ;
- donne bonus de vitesse ou étoiles.

### Nuage-pollen

- Feuille peut flotter dessus ;
- Pierre le traverse ;
- Papier peut glisser ;
- Ciseaux le coupe inutilement.

---

## 13.3 Obstacles Papier

### Passage mince

- solution : Papier se plie ;
- autres formes : bloquées.

### Pont origami

- Papier peut déployer un pont temporaire ;
- timer : 5 secondes au début, améliorable à 8.

### Rune à couvrir

- le joueur doit placer Papier sur un symbole ;
- effet : désactive piège de pierre ou ouvre porte.

### Avion papier

- action spéciale de Milo ;
- plane en ligne droite pendant 2,5 secondes ;
- utile pour franchir de petits gouffres ;
- moins puissant que Feuille mais plus précis.

### Bateau papier

- sur l'eau calme, Milo flotte ;
- sur l'eau rapide, contrôle difficile ;
- feu ou eau noire = danger.

---

## 13.4 Obstacles Ciseaux

### Liane

- solution : Ciseaux coupe ;
- effet : ouvre chemin ou fait tomber plateforme.

### Corde suspendue

- coupe = libère pont, cage ou contrepoids ;
- peut déclencher une séquence chronométrée.

### Filet

- bloque les autres formes ;
- Ciseaux découpe un passage.

### Câble mécanique

- Ciseaux coupe pour désactiver machine ;
- parfois il faut choisir le bon câble.

### Rails rapides

- Sia peut accrocher ses lames aux rails ;
- permet traversée rapide ;
- timing de saut nécessaire.

---

## 13.5 Obstacles mixtes

### Puzzle “Pont des trois formes”

1. Pierre pousse bloc.
2. Papier couvre rune.
3. Ciseaux coupe corde.
4. Feuille plane jusqu'à la sortie.

### Séquence “Chute de la tour”

1. Timer démarre.
2. Ciseaux coupe trois attaches.
3. Pierre active une plaque.
4. Feuille plane hors de la tour.
5. Papier passe sous la porte finale.

### Cage complexe

Une cage villageoise nécessite :

- 1 bouton Pierre ;
- 1 rune Papier ;
- 1 câble Ciseaux ;
- 1 courant Feuille.

C'est un défi optionnel de maîtrise.

---

# 14. Ennemis

## 14.1 Règle générale

Chaque ennemi doit communiquer visuellement sa faiblesse.

Exemples :

- ennemi blindé = Pierre ;
- ennemi volant = Feuille ;
- ennemi parchemin = Ciseaux ;
- ennemi runique = Papier.

---

## 14.2 Liste des ennemis standards

| Ennemi | Description | Faiblesse | Danger |
|---|---|---|---|
| Picboule | boule volante à piques | Pierre | collision |
| Roncivore | plante mordante | Ciseaux | morsure |
| Garde-Cage | petit robot avec cadenas | Pierre/Ciseaux | charge |
| Flottevent | créature volante | Feuille | projectile d'air |
| Tache-Encre | blob d'encre | Papier | ralentissement |
| Roule-Roc | mini bélier de pierre | Papier | charge lourde |
| Papier-Masque | ennemi origami | Ciseaux | piège |
| Scie-Folle | roue mécanique | Pierre | coupe |
| Aimant-Lame | robot aimanté | Pierre/Papier | attire Sia |
| Filet-Mouche | piège vivant | Ciseaux | immobilisation |

---

## 14.3 Comportements ennemis

### Patrouille

- marche sur un chemin ;
- se retourne au bord ;
- peut être évitée ou attaquée.

### Charge

- repère le joueur ;
- clignote ;
- fonce ;
- vulnérable après avoir raté.

### Vol

- se déplace en hauteur ;
- oblige Feuille ou timing de saut.

### Défense

- bouclier frontal ;
- nécessite contour ou bonne forme.

### Piège

- immobile ;
- déclenché par proximité ;
- lisible visuellement.

---

# 15. Boss

## 15.1 Boss 1 — Rocmâchoire, le Gardien des Ruines

### Monde

Canyon Fracassé.

### Thème

Pierre, poids, destruction.

### Phases

#### Phase 1

Rocmâchoire frappe le sol et crée des ondes.  
Le joueur doit utiliser Pierre pour résister puis frapper ses jambes fissurées.

#### Phase 2

Il fait tomber des rochers.  
Le joueur utilise Papier pour couvrir les runes qui invoquent les chutes.

#### Phase 3

Il se protège avec une armure de lianes métalliques.  
Le joueur utilise Ciseaux pour couper les attaches, puis Pierre pour le coup final.

### Récompense

Fragment du Cœur des Étoiles n°1.  
Amélioration : slam Pierre plus puissant.

---

## 15.2 Boss 2 — Vireliane, la Reine des Vents Verts

### Monde

Jardins Suspendus.

### Thème

Vent, plantes, hauteur.

### Phases

#### Phase 1

Elle crée des tornades.  
Le joueur utilise Feuille pour les chevaucher.

#### Phase 2

Elle enferme l'arène dans des lianes.  
Sia doit couper les attaches.

#### Phase 3

Elle invoque des spores sur des plateformes fragiles.  
Milo couvre les runes de poison avec Papier.

### Récompense

Fragment n°2.  
Amélioration : Feuille plane plus longtemps.

---

## 15.3 Boss 3 — Archivore, le Dragon d'Encre

### Monde

Archives Origami.

### Thème

Papier, encre, pliage, énigme.

### Phases

#### Phase 1

Archivore crée des murs de papier.  
Ciseaux les coupe.

#### Phase 2

Il invoque des runes anciennes.  
Papier doit les couvrir dans le bon ordre.

#### Phase 3

Il se transforme en livre géant qui aspire le joueur.  
Feuille utilise les courants d'air et Pierre bloque l'aspiration finale.

### Récompense

Fragment n°3.  
Amélioration : pont Papier dure plus longtemps.

---

## 15.4 Boss 4 — Général Cisaille

### Monde

Usine des Lames.

### Thème

Ciseaux, machines, rails, vitesse.

### Phases

#### Phase 1

Il attaque avec des lames rapides.  
Pierre bloque.

#### Phase 2

Il active des rails et scies.  
Sia doit le poursuivre en dash.

#### Phase 3

Il enferme des villageois dans une machine.  
Le joueur combine : Ciseaux coupe câbles, Papier couvre rune, Pierre casse moteur, Feuille sort les villageois par le haut.

### Récompense

Fragment n°4.  
Amélioration : dash Ciseaux plus long.

---

## 15.5 Boss final — Noctérion le Geôlier

### Monde

Forteresse-Cage.

### Thème

Prison, inversion des règles, équipe complète.

### Phase 1 — Le Geôlier

Noctérion utilise des chaînes et des cages.  
Le joueur doit libérer des mini-cages pendant le combat.

### Phase 2 — Les Formes inversées

Il inverse temporairement les règles :

- Pierre devient trop lourde ;
- Feuille est aspirée ;
- Papier brûle sous lumière violette ;
- Ciseaux sont attirés par des aimants.

Le joueur doit observer les indices et utiliser les formes au bon moment.

### Phase 3 — Le Cœur des Étoiles

Les quatre héros attaquent ensemble.

Séquence finale :

1. Pierre brise les chaînes principales.
2. Feuille porte les cages libérées.
3. Papier scelle les fissures de la prison.
4. Ciseaux coupe le dernier cadenas.
5. Les villageois libérés redonnent leur lumière.
6. Noctérion perd son armure.
7. Dernier saut de l'équipe vers le Cœur des Étoiles.

### Fin

Noctérion comprend que protéger n'est pas enfermer.  
Le village revient dans le ciel d'Écloria.  
Le joueur peut continuer en mode exploration post-game.

---

# 16. Monde hub : Village d'Écloria

## 16.1 État initial

Au début :

- maisons détruites ;
- place centrale fissurée ;
- fontaine éteinte ;
- cages vides suspendues ;
- ciel sombre au loin ;
- forteresse visible dans l'horizon ;
- peu de PNJ.

---

## 16.2 Restauration progressive

Chaque villageois libéré restaure un bâtiment.

| Bâtiment | Fonction | Débloqué par |
|---|---|---|
| Place centrale | sélection des mondes | tutoriel |
| Forge de Taro | amélioration Pierre | 5 villageois |
| Jardin de Lina | amélioration Feuille | 10 villageois |
| Atelier de Milo | amélioration Papier | 15 villageois |
| Dojo de Sia | amélioration Ciseaux | 20 villageois |
| Boutique | cosmétiques | 12 villageois |
| Bibliothèque | lore et cartes | 18 villageois |
| Observatoire | accès monde final | 4 fragments |
| Théâtre | relecture cinématiques | 25 villageois |
| Maison de musique | jukebox | 30 villageois |

---

## 16.3 Activités du hub

- parler aux villageois ;
- choisir un niveau ;
- acheter cosmétiques ;
- tester les formes ;
- refaire les tutoriels ;
- consulter la carte ;
- voir les cages restantes ;
- accéder aux défis bonus.

---

# 17. Structure globale des mondes

## Vue d'ensemble

| Monde | Thème | Forme principale | Boss | Fragment |
|---|---|---|---|---|
| Prologue | Village détruit | équipe | aucun | non |
| Monde 1 | Canyon Fracassé | Pierre | Rocmâchoire | 1 |
| Monde 2 | Jardins Suspendus | Feuille | Vireliane | 2 |
| Monde 3 | Archives Origami | Papier | Archivore | 3 |
| Monde 4 | Usine des Lames | Ciseaux | Général Cisaille | 4 |
| Monde 5 | Forteresse-Cage | toutes | Noctérion | final |

Chaque monde introduit une forme dominante puis mélange progressivement les autres.

---

# 18. Cartes de niveaux détaillées

## 18.1 Prologue — “Le Village Volé”

### Objectif

Montrer l'attaque de Noctérion, introduire les quatre héros et apprendre les bases.

### Carte simplifiée

```text
Maison des héros
   ↓
Place centrale
   ↓
Premier saut
   ↓
Première étoile
   ↓
Attaque de Noctérion
   ↓
Course de fuite
   ↓
Sauvetage d'un villageois
   ↓
Portail vers le Canyon
```

### Séquence

1. Le joueur contrôle Taro.
2. Il apprend marche/saut.
3. Lina rejoint : petit planage.
4. Milo rejoint : passage sous une porte.
5. Sia rejoint : coupe une corde.
6. Noctérion attaque.
7. Les villageois sont capturés.
8. Les quatre héros promettent de les sauver.

### Collectibles

- 20 étoiles dorées ;
- 1 villageois ;
- 1 cinématique ;
- aucun secret difficile.

### Fonction design

Tutoriel narratif, émotionnel et mécanique.

---

## 18.2 Monde 1-1 — “Les Marches du Canyon”

### Forme principale

Pierre.

### Objectif

Apprendre à casser et pousser.

### Carte

```text
Départ
 ↓
Mur fissuré simple
 ↓
Bloc poussable
 ↓
Plaque lourde
 ↓
Petit combat Picboule
 ↓
Cage villageoise facile
 ↓
Sortie
```

### Obstacles

- 3 murs fissurés ;
- 2 blocs poussables ;
- 2 plaques lourdes ;
- 1 couloir de vent faible.

### Villageois

- 1 sur chemin principal ;
- 1 derrière mur fissuré secret.

### Test de maîtrise

Casser un mur pendant un petit compte à rebours.

---

## 18.3 Monde 1-2 — “Le Pont qui S'écroule”

### Forme principale

Pierre + Feuille.

### Objectif

Combiner poids et planage.

### Carte

```text
Plateforme haute
 ↓
Pont fragile
 ↓
Portail Pierre 12 s
 ↓
Course sur pont qui casse
 ↓
Courant d'air
 ↓
Portail Feuille
 ↓
Planage vers îlot secret
 ↓
Sortie
```

### Obstacles

- pont destructible ;
- boulders roulants ;
- vent latéral ;
- gouffres.

### Villageois

- 1 dans une cage suspendue ;
- il faut Ciseaux optionnel via Étoile de Choix pour couper la corde.

---

## 18.4 Monde 1-3 — “La Mine des Dalles Lourdes”

### Forme principale

Pierre + Papier.

### Objectif

Introduire les runes Papier dans un monde Pierre.

### Carte

```text
Entrée mine
 ↓
Plaques lourdes en série
 ↓
Rune à couvrir
 ↓
Bloc sur rails
 ↓
Salle puzzle multi-interrupteurs
 ↓
Mini-arène
 ↓
Sortie
```

### Puzzle central

- Pierre pousse 2 blocs ;
- Papier couvre une rune ;
- la porte s'ouvre seulement si les deux actions sont actives.

### Ennemis

- Roule-Roc ;
- Tache-Encre ;
- Garde-Cage.

---

## 18.5 Monde 1-4 — Boss “Rocmâchoire”

Déjà détaillé dans la section boss.

### Avant boss

Mini-niveau court avec :

- checkpoint ;
- 30 étoiles ;
- rappel des mécaniques Pierre ;
- 1 cage villageoise avant l'arène.

---

## 18.6 Monde 2-1 — “Les Jardins du Grand Souffle”

### Forme principale

Feuille.

### Objectif

Apprendre le planage.

### Carte

```text
Départ sur île basse
 ↓
Fleur rebondissante
 ↓
Courant vertical
 ↓
Planage en arc
 ↓
Anneaux de feuilles
 ↓
Cage visible
 ↓
Sortie sur arbre géant
```

### Obstacles

- courants d'air ;
- fleurs trampoline ;
- ennemis volants ;
- plateformes mouvantes.

### Collectibles

- étoiles en trajectoire aérienne ;
- secret sous une plateforme pour Papier.

---

## 18.7 Monde 2-2 — “Le Tunnel des Bourrasques”

### Forme principale

Feuille + Pierre.

### Objectif

Différencier vent utile et vent dangereux.

### Carte

```text
Entrée tunnel
 ↓
Vent porteur
 ↓
Vent contraire
 ↓
Portail Pierre
 ↓
Couloir de rafales
 ↓
Portail Feuille
 ↓
Ascension verticale
 ↓
Sortie
```

### Mécanique clé

- Feuille utilise le vent porteur ;
- Pierre traverse le vent contraire.

### Villageois

- 1 cage dans un courant circulaire ;
- nécessite Feuille + bon timing.

---

## 18.8 Monde 2-3 — “Les Lianes Suspendues”

### Forme principale

Feuille + Ciseaux.

### Objectif

Couper pour modifier le terrain.

### Carte

```text
Plateformes végétales
 ↓
Liane bloquante
 ↓
Ciseaux coupe
 ↓
Pont tombe
 ↓
Feuille plane
 ↓
Ciseaux coupe cage
 ↓
Sortie
```

### Puzzle central

Une cage est attachée par trois lianes.  
Le joueur doit :

1. couper liane A ;
2. planer vers plateforme B ;
3. couper liane B ;
4. revenir par courant d'air ;
5. couper liane C.

---

## 18.9 Monde 2-4 — Boss “Vireliane”

Déjà détaillé.

---

## 18.10 Monde 3-1 — “La Bibliothèque Pliée”

### Forme principale

Papier.

### Objectif

Apprendre le pliage et les passages minces.

### Carte

```text
Hall de livres géants
 ↓
Passage sous porte
 ↓
Rune à couvrir
 ↓
Avion papier court
 ↓
Pont origami
 ↓
Cage secrète
 ↓
Sortie
```

### Obstacles

- portes basses ;
- livres tombants ;
- encre magique ;
- plateformes pliables.

### Villageois

- 1 derrière passage mince ;
- 1 après puzzle rune.

---

## 18.11 Monde 3-2 — “Le Fleuve d'Encre”

### Forme principale

Papier + Feuille.

### Objectif

Traverser eau/encre avec contrôle.

### Carte

```text
Quai de papier
 ↓
Bateau papier
 ↓
Courant d'encre
 ↓
Vent latéral
 ↓
Feuille plane
 ↓
Rune d'encre
 ↓
Sortie
```

### Mécaniques

- Papier flotte sur eau claire ;
- encre ralentit ;
- Feuille traverse par les airs ;
- Ciseaux peut couper filets d'encre solidifiée.

---

## 18.12 Monde 3-3 — “Les Pages Vivantes”

### Forme principale

Papier + Ciseaux + Pierre.

### Objectif

Créer un puzzle complet.

### Carte

```text
Livre géant ouvert
 ↓
Pages qui tournent
 ↓
Ciseaux coupe onglets
 ↓
Papier couvre symbole
 ↓
Pierre casse cadenas rocheux
 ↓
Séquence page qui se referme
 ↓
Sortie
```

### Moment fort

Le joueur doit échapper à un livre géant qui se ferme.

- Ciseaux coupe trois fils ;
- Papier glisse sous une page ;
- Feuille plane hors du livre ;
- Pierre bloque la couverture pendant 3 secondes.

---

## 18.13 Monde 3-4 — Boss “Archivore”

Déjà détaillé.

---

## 18.14 Monde 4-1 — “Les Rails de Lamevive”

### Forme principale

Ciseaux.

### Objectif

Apprendre vitesse et coupe.

### Carte

```text
Départ usine
 ↓
Conveyor belt
 ↓
Première liane/câble
 ↓
Rail rapide
 ↓
Scies lentes
 ↓
Cage suspendue
 ↓
Sortie
```

### Obstacles

- câbles ;
- rails ;
- scies ;
- plateformes à timing ;
- petits robots.

---

## 18.15 Monde 4-2 — “La Salle des Engrenages”

### Forme principale

Ciseaux + Pierre.

### Objectif

Comprendre que Pierre bloque les lames.

### Carte

```text
Entrée mécanique
 ↓
Scie circulaire
 ↓
Pierre bloque
 ↓
Ciseaux coupe câble
 ↓
Engrenage ralentit
 ↓
Passage timing
 ↓
Sortie
```

### Puzzle central

- Ciseaux coupe câble rouge ;
- Pierre bloque roue ;
- Papier couvre rune électrique ;
- Feuille traverse la ventilation.

---

## 18.16 Monde 4-3 — “Le Convoyeur des Prisonniers”

### Forme principale

Toutes.

### Objectif

Sauver des villageois sur une ligne de machines.

### Carte

```text
Départ tapis roulant
 ↓
Cages en mouvement
 ↓
Ciseaux coupe attaches
 ↓
Pierre casse moteur
 ↓
Papier désactive rune
 ↓
Feuille rejoint plateforme haute
 ↓
Mini-boss
 ↓
Sortie
```

### Moment fort

Séquence chronométrée : sauver 3 cages avant qu'elles n'entrent dans la presse.

---

## 18.17 Monde 4-4 — Boss “Général Cisaille”

Déjà détaillé.

---

## 18.18 Monde 5-1 — “Les Portes de la Forteresse-Cage”

### Forme principale

Toutes.

### Objectif

Tester la maîtrise du joueur.

### Carte

```text
Pont vers forteresse
 ↓
Quatre portes élémentaires
 ↓
Salle Pierre
 ↓
Salle Feuille
 ↓
Salle Papier
 ↓
Salle Ciseaux
 ↓
Grande porte centrale
 ↓
Sortie
```

### Règle

Chaque salle rappelle une forme.  
Le joueur doit terminer les quatre salles pour ouvrir la forteresse.

---

## 18.19 Monde 5-2 — “La Prison Suspendue”

### Forme principale

Toutes + étoiles rares.

### Objectif

Gestion avancée des Étoiles de Choix.

### Carte

```text
Cellules flottantes
 ↓
Étoile de Choix unique
 ↓
Embranchement A/B/C
 ↓
Cage optionnelle difficile
 ↓
Portail forme imposée
 ↓
Zone anti-transformation
 ↓
Sortie
```

### Mécanique clé

Le joueur doit décider où utiliser ses étoiles.  
Il ne peut pas sauver tout le monde sans explorer intelligemment.

---

## 18.20 Monde 5-3 — “Le Cœur Enfermé”

### Forme principale

Toutes.

### Objectif

Dernier niveau avant boss final.

### Carte

```text
Ascenseur de cages
 ↓
Plateforme tournante
 ↓
Séquence Pierre
 ↓
Séquence Feuille
 ↓
Séquence Papier
 ↓
Séquence Ciseaux
 ↓
Puzzle final à 4 héros
 ↓
Arène de Noctérion
```

### Moment fort

Les quatre héros doivent activer quatre mécanismes en moins de 60 secondes.

---

## 18.21 Monde 5-4 — Boss final “Noctérion”

Déjà détaillé.

---

# 19. Vertical slice prioritaire

Avant de créer tout le jeu, produire une version jouable de 5 à 8 minutes.

## 19.1 Nom de la vertical slice

**Le Pont des Quatre Formes**

## 19.2 Contenu

La vertical slice doit contenir :

- les 4 héros jouables ;
- changement via roue de formes ;
- 1 niveau complet ;
- 50 étoiles dorées ;
- 5 Étoiles de Choix ;
- 4 villageois ;
- 1 mini-boss ;
- 12 obstacles ;
- 1 checkpoint ;
- 1 intro courte ;
- 1 fin de niveau ;
- UI complète basique ;
- caméra jouable ;
- sons temporaires ;
- assets prototypes ou semi-finis.

---

## 19.3 Carte de la vertical slice

```text
[Départ — Pont cassé]
   ↓
Zone 1 : Taro casse mur fissuré
   ↓
Zone 2 : Lina plane sur courant d'air
   ↓
Zone 3 : Milo passe sous porte et couvre rune
   ↓
Zone 4 : Sia coupe lianes et câbles
   ↓
Zone 5 : Puzzle combiné
   ↓
Checkpoint
   ↓
Mini-boss Garde-Cage
   ↓
Sauvetage de 4 villageois
   ↓
Portail de sortie
```

---

## 19.4 Objectifs de validation

La vertical slice est réussie si :

- le joueur comprend chaque forme sans lire un long texte ;
- le changement de personnage est fluide ;
- les obstacles réagissent correctement ;
- les contrôles sont agréables ;
- la caméra ne gêne pas ;
- le joueur termine le niveau en moins de 8 minutes ;
- il existe au moins une route optionnelle ;
- le mini-boss demande au moins 2 formes ;
- le style visuel rappelle les images conceptuelles : coloré, premium, clair.

---

# 20. Liste complète des assets

## 20.1 Personnages

### Modèles 3D

- Taro Roc modèle final ;
- Lina Virefeuille modèle final ;
- Milo Pli modèle final ;
- Sia Lamevive modèle final ;
- Noctérion ;
- villageois génériques ;
- villageois spécialisés ;
- boss 1 ;
- boss 2 ;
- boss 3 ;
- boss 4 ;
- mini-boss Garde-Cage.

### Animations communes

- idle ;
- marche ;
- course ;
- saut ;
- double saut ;
- chute ;
- atterrissage ;
- prise de dégâts ;
- mort / retour checkpoint ;
- victoire ;
- interaction ;
- libération de cage ;
- transformation / switch ;
- entrée portail ;
- sortie portail.

### Animations spécifiques

Taro :

- slam ;
- pousser ;
- bloquer ;
- casser mur ;
- atterrissage lourd.

Lina :

- planage ;
- montée par courant ;
- virage aérien ;
- atterrissage léger.

Milo :

- pliage plat ;
- avion papier ;
- bateau papier ;
- couverture de rune ;
- pont origami.

Sia :

- dash ;
- coupe rapide ;
- coupe corde ;
- attaque combo ;
- rail slide.

---

## 20.2 Environnement

### Hub

- maison détruite ;
- maison restaurée ;
- fontaine éteinte ;
- fontaine restaurée ;
- place centrale ;
- portail monde ;
- cages vides ;
- observatoire ;
- forge ;
- jardin ;
- bibliothèque ;
- dojo ;
- boutique.

### Monde Pierre

- plateformes rocheuses ;
- murs fissurés ;
- colonnes ;
- dalles lourdes ;
- blocs poussables ;
- boulders ;
- ruines ;
- ponts cassables ;
- poussière ;
- canyon ;
- cascades.

### Monde Feuille

- îles flottantes ;
- arbres géants ;
- fleurs trampoline ;
- lianes ;
- vents visibles ;
- tunnels végétaux ;
- pétales plateformes ;
- spores ;
- feuilles collectables ;
- arcs végétaux.

### Monde Papier

- livres géants ;
- pages plateformes ;
- ponts origami ;
- runes ;
- encre ;
- portes plates ;
- bibliothèques ;
- parchemins ;
- pliages ;
- lanternes papier.

### Monde Ciseaux

- rails ;
- scies ;
- engrenages ;
- câbles ;
- tapis roulants ;
- plateformes métalliques ;
- chaînes ;
- presses ;
- machines ;
- lumières orange/bleu.

### Monde final

- cages cristallines ;
- chaînes ;
- portails violets ;
- plateformes déformées ;
- cadenas géants ;
- cristaux d'étoile ;
- arène finale ;
- mécanismes anti-formes.

---

## 20.3 UI

- icône Taro/Pierre ;
- icône Lina/Feuille ;
- icône Milo/Papier ;
- icône Sia/Ciseaux ;
- roue de sélection ;
- timer de transformation ;
- compteur étoiles dorées ;
- compteur Étoiles de Choix ;
- compteur villageois ;
- barre boss ;
- écran pause ;
- carte du monde ;
- écran de fin de niveau ;
- écran amélioration ;
- indicateur checkpoint.

---

## 20.4 VFX

- transformation Pierre ;
- transformation Feuille ;
- transformation Papier ;
- transformation Ciseaux ;
- étoile collectée ;
- cage libérée ;
- portail actif ;
- timer faible ;
- mur cassé ;
- vent ;
- coupe ;
- rune activée ;
- pont origami ;
- slam ;
- boss hit ;
- fragment obtenu.

---

## 20.5 SFX

- pas Pierre lourds ;
- pas Feuille légers ;
- froissement Papier ;
- tintement Ciseaux ;
- collecte étoile ;
- cage ouverte ;
- changement forme ;
- portail ;
- mur cassé ;
- vent ;
- coupe corde ;
- moteur mécanique ;
- boss rugissement ;
- victoire niveau ;
- restauration village.

---

## 20.6 Musique

- thème du hub détruit ;
- thème du hub restauré ;
- thème Canyon ;
- thème Jardins ;
- thème Archives ;
- thème Usine ;
- thème Forteresse ;
- thème boss ;
- thème Noctérion ;
- thème victoire ;
- thème crédits.

Style musical :

- orchestral léger ;
- percussions cartoon ;
- instruments naturels pour Feuille ;
- percussions graves pour Pierre ;
- harpe/piano doux pour Papier ;
- xylophone/métal léger pour Ciseaux ;
- chœurs doux pour moments émotionnels.

---

# 21. Prompts d'images pour assets

## 21.1 Prompt héros groupe

Créer un concept art 3D cartoon premium pour un jeu de plateforme original. Montrer quatre jeunes héros mascottes debout ensemble dans un village magique détruit mais lumineux. Le premier est un garçon pierre massif avec fissures dorées, le deuxième une fille feuille légère avec ailes végétales, le troisième un garçon papier/origami avec cape pliée, la quatrième une fille ciseaux en armure argentée agile. Style familial, coloré, expressif, film d'animation, sans copier de licence existante. Ajouter étoiles cristallines, cages au loin, ruines, lumière chaude, silhouettes lisibles. Aucun texte.

---

## 21.2 Prompt Taro

Créer une fiche personnage 3D cartoon premium d'un jeune héros mascotte lié à la pierre. Corps massif mais mignon, grandes mains rocheuses, fissures lumineuses ambrées, foulard orange/vert, yeux expressifs dorés, posture protectrice. Montrer face, côté, dos, expressions et pose d'attaque slam. Style jeu de plateforme familial original. Aucun texte.

---

## 21.3 Prompt Lina

Créer une fiche personnage 3D cartoon premium d'une jeune héroïne mascotte liée aux feuilles et au vent. Silhouette légère, ailes-feuilles, cheveux en pétales, grands yeux verts, foulard orange, particules de feuilles, pose de planage. Montrer face, côté, dos, expressions et action de vol. Style jeu de plateforme familial original. Aucun texte.

---

## 21.4 Prompt Milo

Créer une fiche personnage 3D cartoon premium d'un jeune héros mascotte lié au papier et à l'origami. Silhouette souple, cape pliée, motifs de pliage, couleurs blanc crème et bleu pâle, grands yeux expressifs, posture intelligente. Montrer face, côté, dos, forme plate, avion papier, pont origami. Style jeu de plateforme familial original. Aucun texte.

---

## 21.5 Prompt Sia

Créer une fiche personnage 3D cartoon premium d'une jeune héroïne mascotte liée aux ciseaux. Armure argentée légère, foulard bleu, formes de lames stylisées, grands yeux bleus, posture rapide et confiante, effets de vitesse et étincelles. Montrer face, côté, dos, dash, coupe de corde. Style jeu de plateforme familial original. Aucun texte.

---

## 21.6 Prompt Noctérion

Créer un concept art 3D cartoon premium du grand méchant d'un jeu de plateforme familial. Grand geôlier magique avec manteau sombre, chaînes, cadenas, sceptre-clé, yeux violets, fragments d'étoiles dans l'armure. Il doit être impressionnant mais pas horrifique. Autour de lui, cages cristallines flottantes et prison magique. Style original, cinématique, aucun texte.

---

## 21.7 Prompt niveau Stone

Créer un concept art gameplay 3D cartoon premium d'un niveau de plateforme dans un canyon de ruines rocheuses. Montrer plateformes flottantes, murs fissurés, blocs poussables, dalles lourdes, boulders, étoiles cristallines, cages villageoises et un héros pierre qui casse un mur. Style lumineux, familial, original, très lisible. Aucun texte.

---

## 21.8 Prompt niveau Leaf

Créer un concept art gameplay 3D cartoon premium d'un niveau de plateforme dans des jardins suspendus au-dessus des nuages. Montrer courants d'air visibles, fleurs trampoline, tunnels de feuilles, plateformes végétales, étoiles cristallines, cages villageoises et une héroïne feuille qui plane. Style lumineux, familial, original, très lisible. Aucun texte.

---

## 21.9 Prompt niveau Paper

Créer un concept art gameplay 3D cartoon premium d'un niveau de plateforme dans une bibliothèque géante magique en origami. Montrer livres géants, ponts papier, runes, encre, passages minces, étoiles cristallines, cages villageoises et un héros papier qui se plie. Style lumineux, familial, original, très lisible. Aucun texte.

---

## 21.10 Prompt niveau Scissors

Créer un concept art gameplay 3D cartoon premium d'un niveau de plateforme dans une usine fantastique de ciseaux et engrenages. Montrer rails, câbles, scies, tapis roulants, plateformes métalliques, étoiles cristallines, cages villageoises et une héroïne ciseaux qui coupe une corde. Style lumineux, familial, original, très lisible. Aucun texte.

---

# 22. Architecture technique recommandée

## 22.1 Moteur conseillé

Pour un prototype rapide, utiliser **Unity** avec C#.

Raison :

- bon pour plateformes 3D ;
- beaucoup d'exemples de contrôleurs personnage ;
- système de prefabs utile pour obstacles ;
- ScriptableObjects pratiques pour les données de formes ;
- pipeline visuel accessible.

Alternatives :

- Godot pour projet open-source léger ;
- Unreal pour rendu haut de gamme et Blueprint.

Le prompt technique doit pouvoir être adapté au moteur choisi.

---

## 22.2 Modules principaux

```text
Game
├── Core
│   ├── GameManager
│   ├── SaveSystem
│   ├── SceneLoader
│   └── EventBus
├── Player
│   ├── PlayerController
│   ├── MovementMotor
│   ├── CharacterSwitchController
│   ├── FormStateMachine
│   ├── StarInventory
│   ├── HealthSystem
│   ├── InteractionDetector
│   └── PlayerAnimator
├── Forms
│   ├── FormData
│   ├── StoneForm
│   ├── LeafForm
│   ├── PaperForm
│   └── ScissorsForm
├── Obstacles
│   ├── BreakableWall
│   ├── HeavyPressurePlate
│   ├── WindZone
│   ├── FoldPassage
│   ├── CuttableRope
│   ├── OrigamiBridge
│   ├── PushBlock
│   └── MovingPlatform
├── Collectibles
│   ├── GoldenStar
│   ├── ChoiceStar
│   ├── PrismStar
│   ├── HeartPiece
│   └── VillagerCage
├── Enemies
│   ├── EnemyBase
│   ├── PatrolEnemy
│   ├── FlyingEnemy
│   ├── ChargingEnemy
│   └── BossController
├── UI
│   ├── HUDController
│   ├── FormWheelUI
│   ├── TimerUI
│   ├── CollectibleCounterUI
│   └── LevelEndUI
└── Audio
    ├── AudioManager
    ├── MusicManager
    └── SFXLibrary
```

---

## 22.3 Données de forme

Chaque forme doit être data-driven.

Exemple :

```json
{
  "formId": "stone",
  "displayName": "Taro Roc",
  "movement": {
    "speed": 4.0,
    "jumpHeight": 4.5,
    "acceleration": 5.0,
    "airControl": 0.35,
    "mass": 10.0
  },
  "abilities": {
    "breakWalls": true,
    "pushHeavy": true,
    "resistWind": true,
    "glide": false,
    "fold": false,
    "cut": false
  },
  "weaknesses": {
    "breaksFragilePlatforms": true,
    "sinksInWater": true,
    "slowOnSlopes": true
  },
  "ui": {
    "color": "amber",
    "icon": "stone_icon"
  }
}
```

---

## 22.4 Interfaces d'interaction

Créer un système où les obstacles ne vérifient pas le nom du personnage, mais les capacités.

Pseudo-structure :

```text
IFormInteractable
- CanInteract(FormData form)
- Interact(PlayerController player)

FormData
- canBreakWalls
- canCut
- canFold
- canGlide
- canPushHeavy
- canResistWind
```

Exemple :

- BreakableWall vérifie `canBreakWalls`.
- CuttableRope vérifie `canCut`.
- FoldPassage vérifie `canFold`.
- WindZone vérifie `canGlide` ou `canResistWind`.

---

# 23. Systèmes techniques détaillés

## 23.1 MovementMotor

Responsabilités :

- déplacement au sol ;
- saut ;
- gravité ;
- air control ;
- accélération ;
- friction ;
- pentes ;
- rebords ;
- détection sol ;
- transition animations.

Variables :

- maxSpeed ;
- acceleration ;
- deceleration ;
- jumpForce ;
- gravity ;
- fallGravityMultiplier ;
- groundCheckRadius ;
- slopeLimit ;
- coyoteTime ;
- jumpBufferTime.

---

## 23.2 CharacterSwitchController

Responsabilités :

- ouvrir la roue des formes ;
- ralentir le temps ;
- vérifier les Étoiles de Choix ;
- changer modèle, stats et animations ;
- lancer VFX ;
- mettre à jour HUD ;
- gérer les portails imposés.

Cas limites :

- pas assez d'étoiles ;
- zone anti-transformation ;
- timer de portail actif ;
- changement en l'air ;
- changement dans un passage trop bas ;
- retour à une position sûre si forme invalide.

---

## 23.3 StarInventory

Responsabilités :

- compter étoiles dorées ;
- compter Étoiles de Choix ;
- compter Étoiles Prismatiques ;
- notifier UI ;
- sauvegarder progression ;
- déclencher sons.

Règles :

- étoiles dorées persistantes par niveau ;
- Étoiles de Choix réinitialisées au niveau ou stockées selon mode choisi ;
- prismatiques rares ;
- éviter l'exploitation par respawn.

---

## 23.4 VillagerCage

Responsabilités :

- contenir un villageois ;
- vérifier conditions d'ouverture ;
- jouer animation de libération ;
- envoyer villageois au hub ;
- sauvegarder état ;
- donner récompense.

Types de conditions :

- simple interaction ;
- besoin d'une forme ;
- besoin d'une combinaison ;
- timer ;
- mini-puzzle ;
- boss.

---

## 23.5 LevelFlowManager

Responsabilités :

- démarrer niveau ;
- gérer checkpoints ;
- suivre objectifs ;
- compter collectibles ;
- gérer fin de niveau ;
- afficher score ;
- sauvegarder médailles.

---

# 24. UX/UI

## 24.1 HUD principal

Afficher :

- cœurs ;
- étoiles dorées ;
- Étoiles de Choix ;
- icône forme active ;
- timer de forme imposée si actif ;
- villageois sauvés ;
- objectif actuel.

---

## 24.2 Roue des formes

La roue doit être très lisible.

Disposition :

```text
          Feuille
            ↑
Pierre ← centre → Ciseaux
            ↓
          Papier
```

Couleurs :

- Pierre = ambre ;
- Feuille = vert ;
- Papier = blanc/bleu pâle ;
- Ciseaux = bleu métallique.

Interaction :

- maintenir bouton ;
- temps ralenti à 20 % ;
- choisir direction ;
- relâcher pour confirmer ;
- son + VFX.

---

## 24.3 Feedback

Chaque erreur doit donner un feedback clair :

- obstacle clignote dans la couleur de la bonne forme ;
- son “toc” si mauvaise forme ;
- icône suggérée très brièvement après 2 échecs ;
- jamais de gros texte intrusif.

---

# 25. Accessibilité

Ajouter dès le début :

- mode daltonien ;
- sous-titres ;
- réglage caméra ;
- assistance saut ;
- option temps de réaction augmenté ;
- difficulté douce ;
- possibilité de refaire tutoriel ;
- désactivation vibration ;
- taille texte réglable ;
- contraste UI.

Modes de difficulté :

| Mode | Effet |
|---|---|
| Découverte | plus d'étoiles, moins de dégâts, timers plus longs |
| Aventure | équilibré |
| Maîtrise | timers courts, moins d'étoiles, ennemis plus actifs |

---

# 26. Ton narratif

Le jeu doit être :

- aventureux ;
- chaleureux ;
- drôle ;
- émotionnel mais pas sombre ;
- accessible aux enfants ;
- satisfaisant pour adultes ;
- avec dialogues courts.

Les héros se taquinent mais s'aiment profondément.  
La destruction du village doit motiver sans rendre le jeu trop triste.

---

# 27. Dialogues d'exemple

## Prologue

**Lina** : “Le vent sent bizarre… comme si le ciel retenait son souffle.”  
**Taro** : “Alors on reste ensemble. S'il arrive quelque chose, je bloque.”  
**Sia** : “Tu bloques toujours. Moi, je coupe le problème en deux.”  
**Milo** : “Techniquement, certains problèmes se plient mieux qu'ils ne se coupent.”

Noctérion apparaît.

**Noctérion** : “La liberté vous rend fragiles. Mes cages vous garderont entiers.”  
**Taro** : “Un village enfermé n'est pas un village protégé.”  
**Noctérion** : “Alors venez apprendre la différence entre courage et imprudence.”

---

# 28. Progression et améliorations

## 28.1 Améliorations Pierre

- Slam niveau 2 : casse sols renforcés.
- Blocage : réduit dégâts de scie.
- Charge courte : pousse plusieurs blocs.
- Poids contrôlé : casse moins les ponts fragiles.

---

## 28.2 Améliorations Feuille

- Planage prolongé.
- Boost de vent.
- Double courant : peut rebondir d'un courant à l'autre.
- Atterrissage doux : annule dégâts de chute.

---

## 28.3 Améliorations Papier

- Pont origami plus long.
- Avion papier plus stable.
- Résistance à l'eau courte.
- Lecture de runes secrètes.

---

## 28.4 Améliorations Ciseaux

- Dash plus long.
- Double coupe aérienne.
- Rail slide prolongé.
- Parade de lame courte.

---

# 29. Économie de progression

## 29.1 Coûts proposés

| Amélioration | Coût |
|---|---:|
| première amélioration d'une forme | 300 étoiles dorées |
| deuxième amélioration | 600 étoiles |
| troisième amélioration | 1000 étoiles |
| cosmétique simple | 150 étoiles |
| indice villageois | 100 étoiles |
| musique hub | 200 étoiles |

Ne jamais bloquer la progression principale derrière un grind excessif.

---

# 30. Modes de jeu

## 30.1 Mode histoire

Mode principal.

## 30.2 Mode exploration libre

Après avoir terminé un niveau, le joueur peut revenir avec toutes les formes pour trouver les secrets.

## 30.3 Mode défi chrono

Niveaux courts centrés sur timing.

## 30.4 Mode boss rush

Débloqué après l'histoire.

## 30.5 Mode coop locale optionnel

Option future :

- 2 à 4 joueurs ;
- chacun contrôle un héros ;
- caméra partagée ;
- puzzles coop ;
- non nécessaire pour la vertical slice.

---

# 31. Sauvegarde

Sauvegarder :

- niveaux terminés ;
- étoiles collectées ;
- villageois sauvés ;
- médailles ;
- améliorations ;
- cosmétiques ;
- dialogues vus ;
- position hub ;
- boss vaincus.

Sauvegarde automatique :

- fin de niveau ;
- libération villageois ;
- achat amélioration ;
- retour hub.

---

# 32. Performance cible

Pour un prototype :

- 60 FPS visés sur PC moyen ;
- 30 FPS minimum acceptable ;
- caméra fluide ;
- chargement niveau moins de 10 secondes ;
- VFX optimisés ;
- pas plus de 20 ennemis actifs dans une petite zone ;
- LOD pour grands décors ;
- collisions simples pour plateformes.

---

# 33. Backlog de développement

## Sprint 1 — Prototype mouvement

- PlayerController ;
- caméra ;
- saut ;
- course ;
- plateforme simple ;
- checkpoint.

## Sprint 2 — Formes

- FormData ;
- quatre personnages ;
- changement via roue ;
- stats différentes ;
- VFX temporaire.

## Sprint 3 — Obstacles principaux

- mur fissuré ;
- vent ;
- passage mince ;
- corde coupable ;
- plaque lourde ;
- pont fragile.

## Sprint 4 — Collectibles

- étoiles dorées ;
- Étoiles de Choix ;
- cages villageoises ;
- HUD.

## Sprint 5 — Vertical slice niveau

- construire “Le Pont des Quatre Formes” ;
- intégrer checkpoints ;
- équilibrer timings ;
- ajouter mini-boss.

## Sprint 6 — Polish

- VFX ;
- SFX ;
- animations temporaires ;
- UI propre ;
- feedback erreurs.

## Sprint 7 — Hub

- village détruit/restauré ;
- portails mondes ;
- PNJ simples ;
- sauvegarde.

## Sprint 8 — Boss prototype

- boss Rocmâchoire ;
- phases ;
- barre de vie ;
- récompense fragment.

---

# 34. Checklist QA

## 34.1 Mouvement

- le joueur peut marcher ;
- le joueur peut courir ;
- le joueur peut sauter ;
- le joueur peut atterrir sans glisser anormalement ;
- la caméra suit correctement ;
- les rebords ne bloquent pas injustement.

## 34.2 Formes

- Pierre a bien plus de masse ;
- Feuille plane ;
- Papier se plie ;
- Ciseaux coupe ;
- changement coûte une étoile ;
- portails imposent la bonne forme ;
- timer fonctionne ;
- retour forme sûre fonctionne.

## 34.3 Obstacles

- mur cassable ne casse qu'avec Pierre ;
- liane ne se coupe qu'avec Ciseaux ;
- passage mince ne passe qu'avec Papier ;
- courant d'air fonctionne avec Feuille ;
- plaque lourde fonctionne avec Pierre ;
- mauvaise forme donne feedback.

## 34.4 Collectibles

- étoiles se collectent ;
- compteur augmente ;
- étoiles déjà prises ne respawnent pas après sauvegarde ;
- cage libérée reste libérée ;
- récompense donnée une seule fois.

## 34.5 Niveau

- le niveau peut être terminé ;
- aucun softlock ;
- checkpoints fonctionnent ;
- joueur ne tombe pas hors monde sans reset ;
- tous les villageois sont atteignables ;
- tous les secrets sont accessibles.

---

# 35. Définition de réussite du jeu

Le jeu est réussi si :

- un joueur comprend le concept en moins de 2 minutes ;
- chaque forme donne une sensation différente ;
- les choix sont intéressants ;
- les niveaux ne se résument pas à des couloirs ;
- les transformations créent de la stratégie ;
- la caméra ne frustre pas ;
- les personnages sont attachants ;
- sauver le village donne envie de continuer ;
- les boss testent réellement les formes ;
- le style visuel est fort et mémorable.

---

# 36. Prompt maître complet à copier dans un LLM

Copie le bloc ci-dessous dans un LLM lorsque tu veux qu'il commence la production.

```text
Tu es un studio de création de jeux vidéo complet : directeur créatif, game designer senior, technical designer, level designer 3D, narrative designer, directeur artistique, UX/UI designer, producer, développeur senior et QA lead.

Je veux créer un jeu de plateforme 3D original, familial, coloré, premium et dynamique intitulé provisoirement “Les Quatre Formes d'Écloria”.

IMPORTANT : le jeu doit être original. Ne copie aucun personnage, niveau, ennemi, objet, musique, UI ou structure reconnaissable de Mario, Sonic, Crash, Kirby, Rayman ou toute autre licence existante. L'inspiration autorisée est uniquement le genre : plateforme 3D, mascotte originale, exploration, collectibles, transformations, boss et niveaux colorés.

HISTOIRE :
Le village magique d'Écloria est détruit par Noctérion le Geôlier, un grand méchant obsédé par le contrôle. Il vole le Cœur des Étoiles, capture presque tous les villageois et les enferme dans des cages cristallines à l'intérieur de la Forteresse-Cage. Quatre jeunes héros échappent à la capture : Taro Roc, garçon Pierre ; Lina Virefeuille, fille Feuille ; Milo Pli, garçon Papier ; Sia Lamevive, fille Ciseaux. Ils doivent traverser plusieurs mondes, sauver les villageois, récupérer les fragments du Cœur des Étoiles, restaurer le village et vaincre Noctérion.

CONCEPT :
Le joueur contrôle quatre personnages jouables. Chaque personnage correspond à une forme et une fonction :
- Pierre : force, poids, résistance, casse murs, pousse blocs, active plaques lourdes, résiste au vent.
- Feuille : planage, mobilité aérienne, courants d'air, fleurs rebondissantes, traversée de gouffres.
- Papier : pliage, passages minces, runes, ponts origami temporaires, avion papier court, énigmes.
- Ciseaux : vitesse, dash, coupe cordes/lianes/filets/câbles, rails rapides, précision.

SYSTÈME DE CHANGEMENT :
Le joueur peut changer de personnage avec des Étoiles de Choix. 1 étoile = 1 changement libre. Maximum 5 étoiles stockées. La roue de sélection ralentit le temps. Certains portails imposent une forme pendant 8 à 14 secondes. Une Étoile Prismatique rare permet d'annuler une forme imposée et de choisir librement.

COLLECTIBLES :
- étoiles dorées : score, exploration, restauration du village ;
- Étoiles de Choix : ressource de changement ;
- Étoiles Prismatiques : ressource rare ;
- cages villageoises : PNJ à libérer ;
- fragments du Cœur des Étoiles : obtenus après boss ;
- médailles de maîtrise : niveau terminé, tous villageois sauvés, toutes étoiles ou chrono.

DIRECTION ARTISTIQUE :
Style 3D cartoon premium, familial, lumineux, expressif, très coloré, proche d'un film d'animation mais totalement original. Mondes avec plateformes flottantes, ruines, végétation géante, étoiles cristallines, cages magiques, machines fantastiques. Chaque forme a une couleur :
- Pierre : ocre/gris/ambre ;
- Feuille : vert/jaune/cyan ;
- Papier : blanc/crème/bleu pâle ;
- Ciseaux : argent/bleu/orange ;
- Noctérion : violet/noir/chaînes/cadenas.

PERSONNAGES :
Taro Roc : garçon Pierre, protecteur, massif, yeux dorés, fissures lumineuses, foulard orange/vert. Lent mais puissant.
Lina Virefeuille : fille Feuille, joyeuse, légère, ailes-feuilles, particules végétales, plane et utilise le vent.
Milo Pli : garçon Papier, intelligent, calme, corps origami, cape pliée, traverse les passages minces et active les runes.
Sia Lamevive : fille Ciseaux, rapide, maligne, armure argentée, foulard bleu, coupe les obstacles et utilise un dash.
Noctérion : grand geôlier magique, manteau de chaînes, sceptre-clé, cages cristallines, yeux violets, veut protéger en enfermant.

MONDES :
Prologue : Le Village Volé.
Monde 1 : Canyon Fracassé, forme Pierre, boss Rocmâchoire.
Monde 2 : Jardins Suspendus, forme Feuille, boss Vireliane.
Monde 3 : Archives Origami, forme Papier, boss Archivore.
Monde 4 : Usine des Lames, forme Ciseaux, boss Général Cisaille.
Monde 5 : Forteresse-Cage, toutes les formes, boss final Noctérion.

VERTICAL SLICE :
Avant le jeu complet, produire un prototype jouable de 5 à 8 minutes appelé “Le Pont des Quatre Formes”.
Il doit contenir :
- les 4 héros jouables ;
- une caméra troisième personne ;
- un niveau complet ;
- 50 étoiles dorées ;
- 5 Étoiles de Choix ;
- 4 villageois à sauver ;
- 12 obstacles ;
- 1 checkpoint ;
- 1 mini-boss ;
- 1 fin de niveau ;
- HUD basique ;
- feedback visuel et sonore.

ARCHITECTURE TECHNIQUE :
Créer un système modulaire avec :
- GameManager ;
- PlayerController ;
- MovementMotor ;
- CharacterSwitchController ;
- FormStateMachine ;
- StarInventory ;
- HealthSystem ;
- InteractionDetector ;
- FormData data-driven ;
- ObstacleBase ;
- IFormInteractable ;
- CollectibleSystem ;
- VillagerCage ;
- LevelFlowManager ;
- HUDController ;
- FormWheelUI ;
- SaveSystem ;
- AudioManager.

RÈGLES TECHNIQUES :
Les obstacles doivent vérifier les capacités de la forme active, pas le nom du personnage.
Exemples :
- BreakableWall vérifie canBreakWalls ;
- CuttableRope vérifie canCut ;
- FoldPassage vérifie canFold ;
- WindZone vérifie canGlide ou canResistWind ;
- HeavyPressurePlate vérifie mass >= requiredMass.

LIVRABLES ATTENDUS :
Produis dans l'ordre :
1. Un Game Design Document complet.
2. Une bible artistique.
3. Les fiches détaillées des 4 héros.
4. La fiche du grand méchant.
5. Le système complet de gameplay.
6. La matrice des interactions Pierre/Feuille/Papier/Ciseaux.
7. La structure des mondes.
8. Toutes les cartes de niveaux.
9. La vertical slice détaillée.
10. La liste complète des assets.
11. L'architecture technique.
12. Le backlog de développement.
13. Les prompts d'images pour concepts et assets.
14. La checklist QA.
15. Une première proposition de scripts/classes ou blueprints.
16. Une estimation des risques et solutions.
17. Une version MVP réalisable.

FORMAT DE RÉPONSE :
Réponds avec des sections numérotées, des tableaux, des listes d'assets, des règles précises et des checklists. Ne reste jamais vague. Lorsque tu proposes du code, sépare bien design, architecture et implémentation. Si un choix technique est ambigu, propose une option principale et deux alternatives.
```

---

# 37. Risques principaux et solutions

## 37.1 Risque : trop de mécaniques

Solution :

- vertical slice d'abord ;
- seulement 4 obstacles principaux au départ ;
- ajouter les combos plus tard.

## 37.2 Risque : formes déséquilibrées

Solution :

- chaque niveau doit avoir une forme principale ;
- tester temps d'utilisation ;
- éviter que Ciseaux soit toujours meilleur grâce à la vitesse ;
- donner à Pierre des moments obligatoires ;
- donner à Papier des secrets puissants.

## 37.3 Risque : caméra difficile

Solution :

- niveaux larges ;
- éviter passages trop serrés ;
- caméra semi-assistée ;
- tests caméra dès le sprint 1.

## 37.4 Risque : IA/LLM produit du code incomplet

Solution :

- demander un fichier à la fois ;
- commencer par architecture ;
- demander tests après chaque système ;
- utiliser des prefabs simples ;
- valider chaque fonctionnalité avant la suivante.

## 37.5 Risque : style trop proche d'une licence existante

Solution :

- noms originaux ;
- personnages originaux ;
- éviter tuyaux, champignons, blocs question, pièces iconiques ;
- développer la signature cages + étoiles cristallines + quatre héros.

---

# 38. Ordre recommandé pour utiliser ce prompt avec un LLM

Ne demande pas tout le jeu d'un coup au LLM si tu veux de bons résultats.  
Utilise ce document comme bible, puis demande étape par étape :

1. “À partir de cette bible, crée le GDD complet.”
2. “Crée maintenant la vertical slice détaillée.”
3. “Crée l'architecture Unity C# complète.”
4. “Crée les scripts du système de formes uniquement.”
5. “Crée les scripts des obstacles.”
6. “Crée le niveau prototype en greybox.”
7. “Crée les prompts d'assets 3D.”
8. “Crée la checklist QA.”
9. “Crée le backlog de production.”
10. “Crée le pitch deck.”

---

# 39. Première demande idéale après ce document

Après avoir collé ce document dans un LLM, la meilleure demande est :

```text
À partir de cette bible, crée la vertical slice “Le Pont des Quatre Formes” de manière exploitable par une équipe Unity. Donne-moi :
1. la carte complète du niveau ;
2. les dimensions approximatives ;
3. la liste des prefabs ;
4. la séquence de gameplay minute par minute ;
5. les scripts nécessaires ;
6. les paramètres de chaque forme ;
7. les triggers ;
8. les checkpoints ;
9. les collectibles ;
10. les tests de validation.
```

---

# 40. Vision finale

Le jeu doit donner au joueur cette sensation :

> “Je vois l'obstacle, je comprends quelle forme utiliser, je prends la bonne décision au bon moment, je réussis une action spectaculaire, je sauve quelqu'un, et le village reprend vie grâce à moi.”

C'est le cœur émotionnel et mécanique du projet.

Le jeu n'est pas seulement un jeu de plateforme.  
C'est une aventure sur la liberté, l'entraide et l'intelligence des formes.

