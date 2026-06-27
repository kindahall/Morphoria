#!/usr/bin/env python3
from __future__ import annotations

import json
import urllib.request
from pathlib import Path

from _asset_manifest import ROOT


JOBS_PATH = ROOT / "generated_assets" / "jobs" / "generated_jobs.json"


def main() -> None:
    if not JOBS_PATH.exists():
        raise SystemExit(f"Missing {JOBS_PATH}. Add provider result URLs before downloading.")

    jobs = json.loads(JOBS_PATH.read_text(encoding="utf-8"))
    if not isinstance(jobs, list):
        raise SystemExit("generated_jobs.json must contain a list of download jobs.")

    for job in jobs:
        url = job.get("url")
        output = job.get("output")
        if not url or not output:
            raise SystemExit("Each generated job needs url and output.")

        output_path = ROOT / output
        output_path.parent.mkdir(parents=True, exist_ok=True)
        print(f"Downloading {url} -> {output_path}")
        urllib.request.urlretrieve(url, output_path)


if __name__ == "__main__":
    main()
