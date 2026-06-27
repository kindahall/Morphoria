# Morphoria Production Bible

This folder is the source of truth for design decisions before they become Unity work.

## Production Rule

Codex executes. GPT-5.5 designs and reviews. Unity contains the playable implementation.

No gameplay, character, UI, level, boss, or asset decision should contradict:

- `01_GDD/00_GAME_DESIGN_DOCUMENT_SOURCE.md`
- the visual cards in `08_Assets/`
- the Unity execution brief in `10_Codex/CODEX_EXECUTION_BRIEF.md`

## Visual Canon

The current visual cards are canonical. Use the names and silhouettes visible in those cards:

- Rokko: Stone / Pierre
- Luma: Leaf / Feuille
- Papyra: Paper / Papier
- Cizo: Scissors / Ciseaux
- Noctar: main villain

If the text GDD uses older names, the visual-card names win unless the user decides otherwise.

## Development Strategy

1. Build a polished playable vertical slice.
2. Test controls, camera, form switching, obstacles, collectibles, cages, and mini-boss.
3. Replace placeholders with AI/3D assets only after the gameplay is stable.
4. Keep every task small enough to test.

## Bible Expansion Roadmap

GPT-5.5 can expand this bible into detailed documents for:

- characters and animation lists
- level maps and encounter beats
- enemy behavior specs
- boss phase scripts
- UI wireframes
- VFX and shader briefs
- music and SFX prompts
- QA plans
- production roadmap
