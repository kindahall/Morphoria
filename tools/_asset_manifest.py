from __future__ import annotations

from pathlib import Path
from typing import Any, Dict, Iterable, List, Optional, Tuple


ROOT = Path(__file__).resolve().parents[1]
MANIFEST_PATH = ROOT / "ASSET_MANIFEST.yaml"
JOBS_DIR = ROOT / "generated_assets" / "jobs"


def load_manifest() -> Dict[str, Any]:
    try:
        import yaml  # type: ignore
    except ImportError:
        return _load_manifest_without_pyyaml()

    with MANIFEST_PATH.open("r", encoding="utf-8") as handle:
        data = yaml.safe_load(handle) or {}

    if not isinstance(data, dict):
        raise SystemExit("ASSET_MANIFEST.yaml must contain a mapping at the top level.")

    return data


def _load_manifest_without_pyyaml() -> Dict[str, Any]:
    lines = MANIFEST_PATH.read_text(encoding="utf-8").splitlines()
    data: Dict[str, Any] = {}
    current_category: Optional[str] = None
    current_item: Optional[Dict[str, Any]] = None
    index = 0

    while index < len(lines):
        raw = lines[index]
        stripped = raw.strip()
        if not stripped or stripped.startswith("#"):
            index += 1
            continue

        if not raw.startswith(" ") and stripped.endswith(":"):
            current_category = stripped[:-1]
            data[current_category] = []
            current_item = None
            index += 1
            continue

        if raw.startswith("  - "):
            if current_category is None:
                raise SystemExit("Manifest item found before a category.")
            current_item = {}
            data[current_category].append(current_item)
            key, value = _split_key_value(raw[4:])
            current_item[key] = _parse_value(value)
            index += 1
            continue

        if raw.startswith("    ") and current_item is not None:
            key, value = _split_key_value(raw[4:])
            if value in {">", "|"}:
                block: List[str] = []
                index += 1
                while index < len(lines):
                    next_raw = lines[index]
                    if next_raw.startswith("      "):
                        block.append(next_raw[6:].strip())
                        index += 1
                        continue
                    if not next_raw.strip():
                        block.append("")
                        index += 1
                        continue
                    break
                current_item[key] = " ".join(part for part in block if part).strip()
                continue

            current_item[key] = _parse_value(value)
            index += 1
            continue

        raise SystemExit(f"Unsupported manifest YAML line: {raw}")

    return data


def _split_key_value(text: str) -> Tuple[str, str]:
    if ":" not in text:
        raise SystemExit(f"Expected key/value YAML line: {text}")
    key, value = text.split(":", 1)
    return key.strip(), value.strip()


def _parse_value(value: str) -> Any:
    value = value.strip()
    if not value:
        return ""
    if value.startswith('"') and value.endswith('"'):
        return value[1:-1]
    if value.startswith("'") and value.endswith("'"):
        return value[1:-1]
    return value


def iter_assets(manifest: Dict[str, Any]) -> Iterable[Dict[str, Any]]:
    for category, entries in manifest.items():
        if not isinstance(entries, list):
            continue
        for entry in entries:
            if isinstance(entry, dict):
                item = dict(entry)
                item["category"] = category
                yield item


def ensure_jobs_dir() -> Path:
    JOBS_DIR.mkdir(parents=True, exist_ok=True)
    return JOBS_DIR


def required(value: Dict[str, Any], key: str) -> Any:
    if key not in value or value[key] in (None, ""):
        raise SystemExit(f"Missing required manifest field: {key}")
    return value[key]


def as_list(items: Iterable[Dict[str, Any]]) -> List[Dict[str, Any]]:
    return list(items)
