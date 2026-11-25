"""Distribution contracts for the post-publication consumer gate; no network or SDK process is started."""
import importlib.util
import io
import json
from pathlib import Path
import unittest
from unittest.mock import patch
import urllib.error
import xml.etree.ElementTree as ET

BUILD = Path(__file__).resolve().parents[1]

def load(name, filename):
    spec = importlib.util.spec_from_file_location(name, BUILD / filename)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module

consumers = load("public_consumers", "verify_sample_packages.py")
release = load("release_packages", "release_packages.py")
VERSION = "0.4.0-preview.4"

class StopBeforeRestore(Exception):
    pass

class PublicFeedContracts(unittest.TestCase):
    def test_fresh_consumers_have_no_local_source_or_project_reference(self):
        self.assertEqual(set(consumers.PUBLIC_PACKAGES), set(release.PACKAGE_IDS))
        for runtime in ("10", "11"):
            with self.subTest(runtime=runtime):
                def inspect(arguments, directory, log, executable=False):
                    self.assertEqual(arguments[0], "restore")
                    config = ET.parse(directory / "NuGet.Config")
                    sources = config.findall("./packageSources/add")
                    self.assertEqual([(s.get("key"), s.get("value")) for s in sources],
                                     [("nuget.org", "https://api.nuget.org/v3/index.json")])
                    self.assertEqual([s.get("key") for s in config.findall("./packageSourceMapping/packageSource")], ["nuget.org"])
                    package_root = Path(arguments[arguments.index("--packages") + 1])
                    self.assertEqual(package_root, directory / "packages")
                    self.assertFalse(package_root.exists())
                    projects = ET.parse(directory / "Samples.slnx").findall("./Project")
                    self.assertEqual(len(projects), 18)  # 17 real samples plus a build-task consumer.
                    for item in projects:
                        self.assertFalse(ET.parse(directory / item.get("Path")).findall(".//ProjectReference"))
                    shader = ET.parse(directory / "tools/ShaderConsumer/ShaderConsumer.csproj")
                    self.assertEqual(shader.findtext("./PropertyGroup/TargetFramework"), f"net{runtime}.0-windows10.0.26100.0")
                    self.assertEqual(shader.find("./ItemGroup/PackageReference").get("Include"), "DXSharp.Tools.Builds")
                    versions = ET.parse(directory / "Directory.Packages.props")
                    for package in release.PACKAGE_IDS:
                        matches = [p for p in versions.findall(".//PackageVersion") if p.get("Include") == package]
                        self.assertEqual([p.get("Version") for p in matches], [f"[{VERSION}]"])
                    raise StopBeforeRestore()
                with patch.object(consumers, "wait_for_public_feed") as feed, patch.object(consumers, "run", side_effect=inspect):
                    with self.assertRaises(StopBeforeRestore):
                        consumers.qualify("Release", runtime, VERSION, public_feed=True)
                    feed.assert_called_once_with(VERSION)

    def test_indexing_retries_until_every_release_package_is_visible(self):
        requests = {}
        def get(url, timeout):
            self.assertGreater(timeout, 0)
            requests[url] = requests.get(url, 0) + 1
            if requests[url] == 1:
                raise urllib.error.HTTPError(url, 404, "Not indexed", None, None)
            return io.BytesIO(json.dumps({"versions": [VERSION]}).encode())
        with patch.object(consumers.urllib.request, "urlopen", side_effect=get), patch.object(consumers.time, "sleep") as sleep:
            consumers.wait_for_public_feed(VERSION)
        self.assertEqual(len(requests), len(release.PACKAGE_IDS))
        self.assertTrue(all(count == 2 for count in requests.values()))
        sleep.assert_called_once()

    def test_indexing_wait_has_a_finite_deadline(self):
        clock = [0]
        def advance(seconds):
            clock[0] += seconds
        failure = urllib.error.HTTPError("https://api.nuget.org", 404, "Not indexed", None, None)
        with patch.object(consumers.time, "monotonic", side_effect=lambda: clock[0]), \
             patch.object(consumers.time, "sleep", side_effect=advance), \
             patch.object(consumers.urllib.request, "urlopen", side_effect=failure):
            with self.assertRaisesRegex(TimeoutError, "indexing timed out"):
                consumers.wait_for_public_feed(VERSION, timeout=30)
        self.assertEqual(clock[0], 30)

if __name__ == "__main__":
    unittest.main()
