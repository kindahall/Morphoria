#!/usr/bin/env python3
from __future__ import annotations

import json
import os
from pathlib import Path

from _asset_manifest import ensure_jobs_dir, iter_assets, load_manifest, required


def has_higgsfield_credentials() -> bool:
    return bool(os.getenv("HF_KEY") or (os.getenv("HF_API_KEY") and os.getenv("HF_API_SECRET")))


def main() -> None:
    manifest = load_manifest()
    jobs_dir = ensure_jobs_dir()
    queue_path = jobs_dir / "higgsfield_image_video_jobs.jsonl"

    count = 0
    with queue_path.open("w", encoding="utf-8") as handle:
        for asset in iter_assets(manifest):
            asset_type = str(asset.get("type", ""))
            if asset_type not in {"character_3d", "environment_3d", "ui_concept", "concept_image", "video"}:
                continue

            job = {
                "provider": "higgsfield",
                "id": required(asset, "id"),
                "category": asset.get("category"),
                "type": asset_type,
                "reference_image": asset.get("reference_image"),
                "prompt": required(asset, "prompt"),
                "output_image": asset.get("output_image"),
            }
            handle.write(json.dumps(job, ensure_ascii=False) + "\n")
            count += 1

    print(f"Wrote {count} Higgsfield prompt jobs to {queue_path}")
    if not has_higgsfield_credentials():
        print("No Higgsfield credentials found. Set HF_KEY or HF_API_KEY/HF_API_SECRET before live generation.")
    else:
        print("Credentials detected. Connect the official Higgsfield CLI/SDK call in this script when ready.")


if __name__ == "__main__":
    main()
