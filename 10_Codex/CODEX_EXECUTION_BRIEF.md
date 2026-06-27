# Codex Execution Brief

## Role

Codex is the implementation team for Morphoria.

Codex should:

- create and organize the Unity project
- code gameplay systems
- create scenes, prefabs, materials, and test levels
- integrate generated assets
- run checks where possible
- document blockers clearly

Codex should not invent a new art direction. The existing visual cards are canonical.

## Current Target

Create a playable vertical slice:

`Le Pont des Quatre Formes`

The first implementation uses polished placeholders that match the visual cards. It should not wait for final 3D models.

## Required Vertical Slice Systems

- third-person player controller
- camera
- four forms: Rokko, Luma, Papyra, Cizo
- form wheel
- golden stars and prism/choice stars
- obstacles for Stone, Leaf, Paper, Scissors
- combined puzzle section
- checkpoint
- mini-boss Garde-Cage
- four villager cages
- exit portal
- temporary HUD matching the dark-card UI direction

## Unity Rules

- Keep code under `UnityProject/Assets/Morphoria/Scripts`.
- Keep scene tooling under `UnityProject/Assets/Morphoria/Editor`.
- Keep visual references under `UnityProject/Assets/Morphoria/Art/References`.
- Keep materials under `UnityProject/Assets/Morphoria/Materials`.
- Prefer data-driven checks by ability, not by hard-coded hero name.

## Current Blocker

Unity Editor is installed at:

`/Applications/Unity/Hub/Editor/6000.4.4f1/Unity.app/Contents/MacOS/Unity`

Batch project creation is blocked until Unity has an active license in Unity Hub.

After the license is active, open `UnityProject`. The editor script should create the scene automatically, or use:

`Morphoria > Build Vertical Slice Scene`
