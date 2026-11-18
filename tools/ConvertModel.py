"""Convert a static FBX scene to a self-contained glTF binary using Blender."""
import argparse
from pathlib import Path
import sys

import bpy


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("source", type=Path)
    parser.add_argument("destination", type=Path)
    parser.add_argument("--frame", type=int, default=1, help="Static pose frame to export")
    arguments = sys.argv[sys.argv.index("--") + 1:] if "--" in sys.argv else []
    options = parser.parse_args(arguments)

    source = options.source.resolve(strict=True)
    destination = options.destination.resolve()
    if source.suffix.lower() != ".fbx" or destination.suffix.lower() != ".glb":
        parser.error("Use an FBX source and a GLB destination.")

    bpy.ops.wm.read_factory_settings(use_empty=True)
    if bpy.ops.import_scene.fbx(filepath=str(source), use_anim=False) != {"FINISHED"}:
        raise RuntimeError(f"FBX import failed for {source}")

    meshes = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
    if not meshes:
        raise ValueError("The FBX contains no mesh objects.")
    if any(obj.find_armature() is not None or obj.data.shape_keys is not None for obj in meshes):
        raise ValueError("Bake skins and morphs to static meshes before conversion.")

    bpy.context.scene.frame_set(options.frame)
    destination.parent.mkdir(parents=True, exist_ok=True)
    result = bpy.ops.export_scene.gltf(filepath=str(destination), export_format="GLB",
        export_animations=False, export_skins=False, export_morph=False, export_yup=True,
        export_lights=False, export_cameras=False, export_apply=True)
    if result != {"FINISHED"} or not destination.is_file():
        raise RuntimeError(f"GLB export failed for {destination}")


if __name__ == "__main__":
    main()
