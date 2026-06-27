# Vertical Slice QA Checklist

## Setup

- Unity opens `UnityProject` without compile errors.
- Scene `LePontDesQuatreFormes` exists.
- Play mode starts from the player spawn.

## Movement

- Player walks with WASD or arrows.
- Player runs with Shift.
- Player jumps with Space.
- Jump has coyote time and buffered input near platform edges.
- Short jump, long jump, and falling arcs feel controlled.
- Camera follows and rotates with mouse.
- Camera recenters, zooms, avoids geometry, and reacts subtly to jump/landing.
- Falling below the level returns to checkpoint.

## Forms

- `1` switches to Rokko / Pierre.
- `2` switches to Luma / Feuille.
- `3` switches to Papyra / Papier.
- `4` switches to Cizo / Ciseaux.
- Holding `Tab` opens the form wheel and slows time.
- Switching consumes a prism/choice star.

## Obstacles

- Rokko breaks cracked walls.
- Rokko activates heavy pressure plates.
- Luma uses wind and bounce sections.
- Papyra passes thin/fold gates and covers runes.
- Cizo cuts vines, ropes, cables, and nets.
- Wrong forms give clear feedback.

## Level

- 50 golden stars are reachable.
- 5 prism/choice stars are reachable.
- Checkpoint works.
- Mini-boss requires Stone or Scissors.
- Four villager cages can be opened after the mini-boss.
- Exit portal completes only after villagers are saved.

## Campaign Shell

- Main menu opens and can start a new game.
- Main menu shows save progression, disables continue without a save, and asks confirmation before replacing a save.
- Settings can change and restore volume, camera, color assist, feedback text, and reduced motion.
- Hub loads with player, HUD, pause menu, portals, and KO recovery.
- Hub shows damaged, repaired, garden, and final restoration states as campaign progress advances.
- World map shows the six level nodes in order.
- World map nodes and routes visibly switch between locked, open, and completed states.
- Completing a level unlocks the next level.
- Re-entering a level keeps previously collected stars hidden and previously rescued cages open while restoring the HUD counters.
- The last level opens the finale scene.
- Finale scene has the four heroes and Noctar redeemed.

## Recovery

- Enemy contact removes one heart.
- Falling below the level removes one heart and returns to checkpoint.
- At zero hearts, KO screen appears.
- KO `Checkpoint` restores hearts and returns to the last checkpoint.
- KO `Village`, `Carte`, and `Menu` navigate without leaving time paused.

## Objective Feedback

- HUD objective changes from boss to villagers to portal.
- Boss hit updates the boss HUD.
- Defeating Noctar uses Noctar-specific feedback.
- Saving the final villager shows portal-ready feedback.
- Blocked exit tells how many villagers remain.
- `Aide couleur` adds readable form markers above nearby gates, enemies, cages, exits, and hub portals.
- `Textes feedback` controls the temporary feedback captions without hiding prompts or objectives.

## Audio Feedback

- Collectibles have pickup tones.
- Important actions spawn readable particle bursts, pulse rings, and short light flashes.
- Form switching, dash, damage, checkpoint, boss hit, boss defeat, and level complete have cues.
- Each scene has a low procedural ambience loop.
- Pause, KO, result screen, and scene loads do not leave audio or time scale in a broken state.

## Visual Fidelity

- Level reads as floating-island adventure.
- Main route has visible rails, crystals, and form totems without feeling like graybox-only geometry.
- Playable scenes have world landmarks with vertical silhouettes matching their reference-card theme.
- Each form section keeps its reference-card color.
- HUD uses dark panels with colored accents.
- Cages and prism objects use blue/violet crystal language.

## Automated Validation

- Run `Morphoria/Build Game Shell Scenes` after generator changes.
- Run `Morphoria/Validate Production Scenes` before commits.
- Expected result: `Morphoria production validation passed for 10 scene(s).`
- Run `git diff --check` before committing.
