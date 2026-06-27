# Morphoria Asset Pipeline

This pipeline keeps AI assets organized without putting API keys in code or Git.

## Inputs

- `ASSET_MANIFEST.yaml`
- reference images in `references/`
- visual style lock in `08_Assets/VISUAL_STYLE_LOCK.md`

## Outputs

- PNG concepts: `generated_assets/images/`
- GLB models: `generated_assets/models_3d/`
- videos: `generated_assets/videos/`
- Unity imports: `UnityProject/Assets/Morphoria/Art/Generated/`

## Environment Variables

Use environment variables only:

- Higgsfield: `HF_KEY` or `HF_API_KEY` plus `HF_API_SECRET`
- Tripo: `TRIPO_API_KEY`
- Meshy: `MESHY_API_KEY`

Never paste API keys into prompts, Markdown, Unity files, or Git.

## Scripts

Install Python dependencies if needed:

```bash
python3 -m pip install -r tools/requirements.txt
```

Prepare Higgsfield image/video jobs:

```bash
python3 tools/generate_higgsfield_assets.py
```

Prepare Tripo 3D jobs:

```bash
python3 tools/generate_3d_assets_tripo.py
```

Prepare Meshy 3D jobs:

```bash
python3 tools/generate_3d_assets_meshy.py
```

Download generated URLs listed in `generated_assets/jobs/generated_jobs.json`:

```bash
python3 tools/download_generated_assets.py
```

Import generated assets into Unity:

```bash
python3 tools/import_assets_to_unity.py
```

## Production Flow

1. GPT-5.5 expands prompts and acceptance criteria.
2. Codex updates `ASSET_MANIFEST.yaml`.
3. Asset scripts create provider job queues or call APIs once configured.
4. Blender cleanup happens outside Unity for GLB files that need repair.
5. Codex imports final assets into Unity and swaps placeholders.
