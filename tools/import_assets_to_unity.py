#!/usr/bin/env python3
from __future__ import annotations

import shutil
from pathlib import Path

from _asset_manifest import ROOT


SOURCE_ROOTS = [
    ROOT / "generated_assets" / "images",
    ROOT / "generated_assets" / "models_3d",
    ROOT / "generated_assets" / "animations",
]
UNITY_TARGET = ROOT / "UnityProject" / "Assets" / "Morphoria" / "Art" / "Generated"
EXTENSIONS = {".png", ".jpg", ".jpeg", ".webp", ".glb", ".gltf", ".fbx", ".anim"}


def main() -> None:
    UNITY_TARGET.mkdir(parents=True, exist_ok=True)
    copied = 0

    for source_root in SOURCE_ROOTS:
        if not source_root.exists():
            continue
        for source in source_root.rglob("*"):
            if not source.is_file() or source.suffix.lower() not in EXTENSIONS:
                continue
            relative = source.relative_to(source_root)
            target = UNITY_TARGET / source_root.name / relative
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(source, target)
            copied += 1

    print(f"Imported {copied} generated asset files into {UNITY_TARGET}")


if __name__ == "__main__":
    main()
