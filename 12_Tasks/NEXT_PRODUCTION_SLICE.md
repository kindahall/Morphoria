# Morphoria - Prochaine tranche de production

## Objectif

Transformer le shell jouable actuel en experience presentable de 10 a 15 minutes, en gardant strictement la direction visuelle des references et en remplacant progressivement les formes temporaires par des assets conformes.

## Priorites

1. Playtest dans Unity depuis `MainMenu`.
2. Corriger les collisions, placements de ponts, timings de boss et lisibilite des portails.
3. Raffiner la camera troisieme personne, notamment collisions camera et recadrage automatique.
4. Importer les visuels de reference dans `Assets/Morphoria/Art/References`.
5. Creer une premiere passe de prefabs propres pour Rokko, Luma, Papyra, Cizo et Noctar.
6. Remplacer les primitives importantes par des meshes temporaires propres, sans changer les silhouettes validees.
7. Ajouter transitions audio, feedbacks visuels, animations simples et effets de collection.
8. Verifier chaque scene de progression : menu, village, carte, six niveaux, retour hub.

## Definition de fini

- Le jeu demarre sur `MainMenu`.
- Nouvelle partie ouvre le village.
- La carte du monde charge les niveaux debloques.
- Chaque niveau peut etre termine et sauvegarde la progression.
- La fin d'un niveau debloque le suivant.
- Les reglages et la pause fonctionnent dans les scenes jouables.
- Les formes temporaires respectent les couleurs, formes et intentions des visuels Morphoria.

## Validation Unity

Commande de validation scene par scene :

```bash
/Applications/Unity/Hub/Editor/6000.4.4f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -quit \
  -projectPath /Users/Artisaul/Desktop/Morphoria/UnityProject \
  -executeMethod MorphoriaProductionValidator.ValidateProductionScenes \
  -logFile /tmp/morphoria_unity_validate.log
```
