import json
import sys
import numpy as np
from scipy.spatial import Delaunay

file_path = "scripts/mesh_input" 

def load_input(file_path):
    """Load 4D vertices and edges from a JSON file."""
    with open(file_path, "r") as f:
        data = json.load(f)
    vertices = np.array(data["vertices"])
    return vertices

def compute_delaunay(vertices4d):
    """Compute convex hull of 3D vertices."""
    return Delaunay(vertices4d)

def simplices_4d_to_tetrahedra(simplices_4d):
    tets = []
    for s in simplices_4d:
        v0, v1, v2, v3, v4 = s
        tets.append([v0, v1, v2, v3])
        tets.append([v0, v1, v2, v4])
        tets.append([v0, v1, v3, v4])
        tets.append([v0, v2, v3, v4])
        tets.append([v1, v2, v3, v4])
    return np.array(tets)

def save_output(vertices3d, tets, output_path):
    mesh_data = {
        "vertices": vertices3d.tolist(),
        "tetrahedra": tets.tolist()
    }
    with open(output_path, "w") as f:
        json.dump(mesh_data, f)
    print(f"Saved 3D tetrahedral mesh to {output_path}")

def main():
    input_file = "mesh_input.json"
    output_file = "mesh_output.json"
    
    vertices4d = load_input(input_file)
    mesh = compute_delaunay(vertices4d)
    tets3d = simplices_4d_to_tetrahedra(mesh.simplices)
    save_output(vertices4d, tets3d, output_file)

if __name__ == "__main__":
    main()