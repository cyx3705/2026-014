import bpy
import math
from mathutils import Vector


OUTPUT_BLEND = r"C:\OneHistory\OneHistory-Projects\2026-014-Csharp学习\b-Codex-use\concept_car.blend"
OUTPUT_RENDER = r"C:\OneHistory\OneHistory-Projects\2026-014-Csharp学习\b-Codex-use\concept_car_preview.png"


def material(name, color, metallic=0.0, roughness=0.4, emission=None):
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = (*color, 1.0)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    bsdf.inputs["Base Color"].default_value = (*color, 1.0)
    bsdf.inputs["Metallic"].default_value = metallic
    bsdf.inputs["Roughness"].default_value = roughness
    if emission:
        bsdf.inputs["Emission Color"].default_value = (*emission, 1.0)
        bsdf.inputs["Emission Strength"].default_value = 7.0
    return mat


def smooth(obj):
    for polygon in obj.data.polygons:
        polygon.use_smooth = True


def bevelled_cube(name, location, scale, mat, bevel=0.2):
    bpy.ops.mesh.primitive_cube_add(location=location)
    obj = bpy.context.object
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    modifier = obj.modifiers.new("Soft industrial edges", "BEVEL")
    modifier.width = bevel
    modifier.segments = 5
    obj.data.materials.append(mat)
    return obj


def ellipsoid(name, location, scale, mat):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=64, ring_count=32, location=location)
    obj = bpy.context.object
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    smooth(obj)
    obj.data.materials.append(mat)
    return obj


def cylinder(name, location, radius, depth, rotation, mat, vertices=64):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=vertices,
        radius=radius,
        depth=depth,
        location=location,
        rotation=rotation,
    )
    obj = bpy.context.object
    obj.name = name
    smooth(obj)
    obj.data.materials.append(mat)
    return obj


def look_at(obj, target):
    obj.rotation_euler = (Vector(target) - obj.location).to_track_quat("-Z", "Y").to_euler()


# Clean scene.
bpy.ops.object.select_all(action="SELECT")
bpy.ops.object.delete(use_global=False)
for datablocks in (bpy.data.meshes, bpy.data.curves, bpy.data.materials, bpy.data.cameras, bpy.data.lights):
    pass

# Palette: warm white body, graphite lower structure, cyan lighting, copper details.
body_mat = material("Ceramic White", (0.72, 0.77, 0.78), metallic=0.55, roughness=0.2)
body_dark = material("Graphite Aero", (0.025, 0.035, 0.04), metallic=0.75, roughness=0.22)
glass_mat = material("Smoked Glass", (0.025, 0.075, 0.09), metallic=0.15, roughness=0.08)
tire_mat = material("Tire Rubber", (0.012, 0.014, 0.016), roughness=0.72)
rim_mat = material("Machined Rim", (0.2, 0.23, 0.24), metallic=0.95, roughness=0.16)
copper_mat = material("Copper Accent", (0.55, 0.16, 0.045), metallic=0.85, roughness=0.2)
light_mat = material("Cyan Light", (0.1, 0.8, 1.0), roughness=0.12, emission=(0.05, 0.75, 1.0))
rear_light_mat = material("Rear Light", (1.0, 0.025, 0.01), roughness=0.15, emission=(1.0, 0.015, 0.005))

# Main volumes. The front points toward negative X.
ellipsoid("Monocoque body", (0.0, 0.0, 0.92), (2.55, 0.96, 0.55), body_mat)
bevelled_cube("Flat battery floor", (0.18, 0.0, 0.55), (2.05, 0.83, 0.19), body_dark, 0.16)
ellipsoid("Cabin glass", (0.35, 0.0, 1.43), (1.28, 0.73, 0.59), glass_mat)

# Roof blade and centerline detail give the silhouette a deliberate concept-car character.
roof = bevelled_cube("Floating roof blade", (0.45, 0.0, 1.91), (0.78, 0.42, 0.045), body_dark, 0.06)
roof.rotation_euler[1] = math.radians(-2.5)
bevelled_cube("Copper roof spine", (0.45, 0.0, 1.975), (0.62, 0.035, 0.018), copper_mat, 0.015)

# Nose splitter, rear diffuser, and side sill accents.
bevelled_cube("Front splitter", (-2.37, 0.0, 0.52), (0.38, 0.82, 0.07), body_dark, 0.06)
bevelled_cube("Rear diffuser", (2.38, 0.0, 0.54), (0.30, 0.76, 0.09), body_dark, 0.05)
for side in (-1, 1):
    bevelled_cube(
        f"Copper side blade {side}",
        (0.25, side * 0.89, 0.61),
        (1.42, 0.035, 0.055),
        copper_mat,
        0.025,
    )

# Wheels and five-spoke rims.
wheel_x = (-1.62, 1.62)
wheel_y = (-0.92, 0.92)
for x in wheel_x:
    for y in wheel_y:
        side_name = "L" if y < 0 else "R"
        end_name = "Front" if x < 0 else "Rear"
        cylinder(
            f"{end_name} {side_name} tire",
            (x, y, 0.62),
            0.55,
            0.28,
            (math.radians(90), 0, 0),
            tire_mat,
        )
        cylinder(
            f"{end_name} {side_name} rim",
            (x, y + (-0.01 if y < 0 else 0.01), 0.62),
            0.36,
            0.30,
            (math.radians(90), 0, 0),
            rim_mat,
        )
        cylinder(
            f"{end_name} {side_name} hub",
            (x, y + (-0.17 if y < 0 else 0.17), 0.62),
            0.105,
            0.045,
            (math.radians(90), 0, 0),
            copper_mat,
            vertices=32,
        )
        for spoke in range(5):
            angle = spoke * math.tau / 5
            spoke_obj = bevelled_cube(
                f"{end_name} {side_name} spoke {spoke + 1}",
                (x + math.cos(angle) * 0.19, y + (-0.18 if y < 0 else 0.18), 0.62 + math.sin(angle) * 0.19),
                (0.21, 0.025, 0.035),
                body_dark,
                0.025,
            )
            spoke_obj.rotation_euler[1] = -angle

# Lighting signatures.
for side in (-1, 1):
    lamp = bevelled_cube(
        f"Front pixel lamp {side}",
        (-2.37, side * 0.48, 1.04),
        (0.10, 0.27, 0.045),
        light_mat,
        0.035,
    )
    lamp.rotation_euler[1] = math.radians(-18)
bevelled_cube("Rear light bar", (2.47, 0.0, 1.01), (0.055, 0.68, 0.035), rear_light_mat, 0.025)

# Door graphics and small sensor module.
for side in (-1, 1):
    bevelled_cube(f"Door inset {side}", (0.28, side * 0.905, 1.03), (0.75, 0.018, 0.17), body_dark, 0.08)
    bevelled_cube(f"Door accent {side}", (0.15, side * 0.935, 0.96), (0.37, 0.012, 0.025), copper_mat, 0.012)
cylinder("Roof lidar", (-0.28, 0.0, 2.03), 0.095, 0.08, (0, 0, 0), body_dark, vertices=32)

# Studio floor.
bpy.ops.mesh.primitive_plane_add(size=30, location=(0, 0, 0.02))
floor = bpy.context.object
floor.name = "Studio Floor"
floor.data.materials.append(material("Studio Floor", (0.055, 0.06, 0.065), metallic=0.1, roughness=0.38))

# Camera.
bpy.ops.object.camera_add(location=(-6.7, -6.7, 3.7))
camera = bpy.context.object
camera.name = "Hero Camera"
camera.data.lens = 58
look_at(camera, (0.05, 0.0, 0.95))
bpy.context.scene.camera = camera

# Three-point studio lighting.
def area_light(name, location, energy, size, color):
    bpy.ops.object.light_add(type="AREA", location=location)
    lamp = bpy.context.object
    lamp.name = name
    lamp.data.energy = energy
    lamp.data.shape = "DISK"
    lamp.data.size = size
    lamp.data.color = color
    look_at(lamp, (0.0, 0.0, 0.8))
    return lamp


area_light("Key softbox", (-3.8, -4.2, 6.8), 1500, 5.0, (1.0, 0.82, 0.67))
area_light("Cool rim", (3.8, 3.6, 4.2), 1200, 4.0, (0.45, 0.75, 1.0))
area_light("Front fill", (-4.5, 3.0, 2.5), 900, 3.0, (0.75, 0.9, 1.0))

scene = bpy.context.scene
scene.render.engine = "BLENDER_EEVEE"
scene.render.resolution_x = 900
scene.render.resolution_y = 650
scene.render.resolution_percentage = 100
scene.render.image_settings.file_format = "PNG"
scene.render.filepath = OUTPUT_RENDER
scene.render.film_transparent = False
scene.render.image_settings.color_mode = "RGBA"
scene.world.color = (0.018, 0.022, 0.028)
scene.view_settings.look = "AgX - Medium High Contrast"

# Keep all generated assets editable and save before rendering.
bpy.ops.wm.save_as_mainfile(filepath=OUTPUT_BLEND)
bpy.ops.render.render(write_still=True)
