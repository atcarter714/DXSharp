"""Stage and validate the exact DXSharp packages approved for a public preview."""

import argparse
import hashlib
import json
from pathlib import Path
import re
import shutil
import xml.etree.ElementTree as ET
from zipfile import ZipFile

PACKAGE_IDS = ("DXSharp.Core", "DXSharp.DirectML", "DXSharp.Framework.Desktop")
PREVIEW = re.compile(r"(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)-net10-preview\.[1-9][0-9]*")


def fingerprint(path):
    with path.open("rb") as stream:
        return hashlib.file_digest(stream, "sha256").hexdigest()


def describe(directory, version, commit):
    packages = []
    for package_id in PACKAGE_IDS:
        for extension in ("nupkg", "snupkg"):
            name = f"{package_id}.{version}.{extension}"
            path = directory / name
            with ZipFile(path) as archive:
                metadata = ET.fromstring(archive.read(f"{package_id}.nuspec")).find("{*}metadata")
                if metadata is None or (
                    metadata.findtext("{*}id"), metadata.findtext("{*}version")
                ) != (package_id, version):
                    raise ValueError(f"Unexpected package identity in {name}")
                if extension == "nupkg":
                    for dependency in metadata.findall("{*}dependencies//{*}dependency"):
                        dependency_id = dependency.attrib["id"]
                        if dependency_id.casefold().startswith("dxsharp"):
                            if dependency_id not in PACKAGE_IDS or dependency.attrib["version"] not in (
                                version, f"[{version}]"
                            ):
                                raise ValueError(f"{name} has an unexpected SDK dependency: {dependency.attrib}")
            packages.append({"name": name, "sha256": fingerprint(path)})
    return {"commit": commit, "version": version, "packages": packages}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("directory", type=Path)
    parser.add_argument("--version", required=True)
    parser.add_argument("--commit", required=True)
    parser.add_argument("--stage-from", type=Path)
    args = parser.parse_args()
    if not PREVIEW.fullmatch(args.version) or not re.fullmatch(r"[0-9a-f]{40}", args.commit):
        parser.error("Expected a normalized net10-preview version and a full Git commit SHA.")

    manifest = args.directory / "release-manifest.json"
    if args.stage_from is not None:
        expected = describe(args.stage_from, args.version, args.commit)
        args.directory.mkdir(parents=True)
        for package in expected["packages"]:
            name = package["name"]
            shutil.copyfile(args.stage_from / name, args.directory / name)
        manifest.write_text(json.dumps(expected, indent=2) + "\n", encoding="utf-8")
    actual = describe(args.directory, args.version, args.commit)
    filenames = {"release-manifest.json", *(package["name"] for package in actual["packages"])}
    if json.loads(manifest.read_text(encoding="utf-8")) != actual or {
        path.name for path in args.directory.iterdir()
    } != filenames:
        raise ValueError("Release directory does not match the manifest or contains unexpected files.")
    print(f"Verified {len(actual['packages'])} package files for {args.version} at {args.commit}")


if __name__ == "__main__":
    main()
