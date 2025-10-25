import json
import sys
import numpy as np
from scipy.spatial import ConvexHull

file_path = "scripts/mesh_input" 

def load_input(file_path):
    """Load 4D vertices and edges from a JSON file."""
    with open(file_path, "r") as f:
        data = json.load(f)
    vertices = np.array(data["vertices"])
    return vertices

def project_to_3d(vertices4d, d=5.0):
    """
    Project 4D vertices to 3D.
    d: distance for perspective projection
    """
    w = vertices4d[:, 3]
    factor = 1 / (d - w)
    return vertices4d[:, :3] * factor[:, np.newaxis]

def compute_convex_hull(vertices3d):
    """Compute convex hull of 3D vertices."""
    hull = ConvexHull(vertices3d)
    return hull

def save_output(vertices3d, hull, output_path):
    """Save 3D mesh (vertices + faces) to JSON."""
    mesh_data = {
        "vertices": vertices3d.tolist(),
        "faces": hull.simplices.tolist()
    }
    with open(output_path, "w") as f:
        json.dump(mesh_data, f)
    print(f"Saved 3D mesh to {output_path}")

def main():
    input_file = "mesh_input.json"
    output_file = "mesh_output.json"
    
    vertices4d = load_input(input_file)
    vertices3d = project_to_3d(vertices4d)
    hull = compute_convex_hull(vertices3d)
    save_output(vertices3d, hull, output_file)

if __name__ == "__main__":
    main()