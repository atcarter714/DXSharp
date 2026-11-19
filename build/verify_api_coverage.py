"""Exercise coverage failure paths with a missing source and a changed IID in a non-executable copy."""

import argparse
import json
from pathlib import Path
import subprocess
import tempfile


def verify(tool, metadata, assembly, report):
    baseline = json.loads(report.read_text(encoding="utf-8"))
    d3d12 = next(row for row in baseline if row["Namespace"] == "Windows.Win32.Graphics.Direct3D12")
    if d3d12["Status"] != "NameAndIidCoverageComplete":
        raise RuntimeError("A successful current coverage report is required")
    name = "Windows.Win32.Graphics.Direct3D12.ID3D12Device"
    guid = d3d12["ExpectedIids"][name].encode("ascii")
    original = assembly.read_bytes()
    index = original.lower().find(guid)
    if index < 0:
        raise RuntimeError("Unable to locate the fixture interface's serialized GUID attribute")
    modified = bytearray(original)
    modified[index] = ord("0") if modified[index] != ord("0") else ord("1")
    inputs = metadata.split(";")
    without_d3d12 = [item for item in inputs if Path(item).name != "Microsoft.Direct3D.D3D12.winmd"]
    if len(without_d3d12) == len(inputs) or not without_d3d12:
        raise RuntimeError("Expected modular Win32 and D3D12 reference metadata")

    with tempfile.TemporaryDirectory(prefix="dxsharp-coverage-") as directory:
        root = Path(directory)
        copied = root / "ChangedIid.dll"
        copied.write_bytes(modified)
        for label, sources, candidate in (
            ("missing-reference", ";".join(without_d3d12), assembly),
            ("changed-iid", metadata, copied),
            ("overlapping-reference", metadata + ";" + inputs[0], assembly),
        ):
            result_path = root / f"{label}.json"
            result = subprocess.run(["dotnet", str(tool), sources, str(candidate), str(result_path)],
                                    text=True, capture_output=True)
            if result.returncode == 0:
                raise RuntimeError(f"Coverage accepted {label}")
            if label != "overlapping-reference":
                rows = json.loads(result_path.read_text(encoding="utf-8"))
                family = next(row for row in rows if row["Namespace"] == d3d12["Namespace"])
                if label == "missing-reference" and family["Status"] != "MissingReference":
                    raise RuntimeError("A missing required metadata source was not reported explicitly")
                if label == "changed-iid" and name not in family["IidMismatches"]:
                    raise RuntimeError("The changed interface IID was not detected")
            elif "Overlapping metadata definition" not in result.stderr:
                raise RuntimeError("Overlapping inputs failed for an unexpected reason")
    print("PASS: required reference, IID mismatch and overlapping metadata failures.")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--tool", type=Path, required=True)
    parser.add_argument("--metadata", required=True)
    parser.add_argument("--assembly", type=Path, required=True)
    parser.add_argument("--report", type=Path, required=True)
    args = parser.parse_args()
    verify(args.tool, args.metadata, args.assembly, args.report)
