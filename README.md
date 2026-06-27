# Morphoria

Morphoria is organized like a small production team:

- `01_GDD/` - production bible and source design
- `02_Lore/` to `07_UI/` - future GPT-5.5 design expansions
- `08_Assets/` - canonical visual references and style lock
- `09_Unity/` - Unity setup notes
- `10_Codex/` - Codex execution briefs and next prompts
- `11_QA/` - test checklists
- `12_Tasks/` - production backlog
- `UnityProject/` - playable Unity implementation
- `generated_assets/` - AI/generated output folders
- `tools/` - asset pipeline scripts

## Current State

The Unity foundation for the vertical slice is implemented:

`Le Pont des Quatre Formes`

It includes player movement, camera, four forms, form wheel, collectibles, obstacles, checkpoint, mini-boss, villager cages, HUD, and a scene builder.

## Important Setup Step

Unity is installed, but the editor license must be activated in Unity Hub before the project can be opened or generated from the command line.

Read:

`09_Unity/UNITY_ACTIVATION_REQUIRED.md`

## After Activating Unity

Open:

`UnityProject`

If the scene is not created automatically, run:

`Morphoria > Build Vertical Slice Scene`

Then press Play.

## Asset Pipeline

Read:

`docs/ASSET_PIPELINE.md`

The manifest is:

`ASSET_MANIFEST.yaml`
