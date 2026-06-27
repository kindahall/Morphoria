#!/usr/bin/env python3
from __future__ import annotations

import json
import os

from _asset_manifest import ensure_jobs_dir, iter_assets, load_manifest, required


def main() -> None:
    manifest = load_manifest()
    jobs_dir = ensure_jobs_dir()
    queue_path = jobs_dir / "tripo_3d_jobs.jsonl"

    count = 0
    with queue_path.open("w", encoding="utf-8") as handle:
        for asset in iter_assets(manifest):
            if str(asset.get("type", "")) not in {"character_3d", "environment_3d", "prop_3d"}:
                continue

            job = {
                "provider": "tripo",
                "id": required(asset, "id"),
                "category": asset.get("category"),
                "reference_image": asset.get("reference_image"),
                "prompt": required(asset, "prompt"),
                "output_glb": asset.get("output_glb"),
            }
            handle.write(json.dumps(job, ensure_ascii=False) + "\n")
            count += 1

    print(f"Wrote {count} Tripo 3D jobs to {queue_path}")
    if not os.getenv("TRIPO_API_KEY"):
        print("No TRIPO_API_KEY found. Set it before live 3D generation.")


if __name__ == "__main__":
    main()
