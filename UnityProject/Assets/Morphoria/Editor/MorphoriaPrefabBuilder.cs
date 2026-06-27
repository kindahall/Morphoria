using System.IO;
using UnityEditor;
using UnityEngine;

public static class MorphoriaPrefabBuilder
{
    private const string CharacterFolder = "Assets/Morphoria/Prefabs/Characters";
    private const string MaterialFolder = "Assets/Morphoria/Materials";

    [MenuItem("Morphoria/Build Character Prefabs")]
    public static void BuildCharacterPrefabs()
    {
        Directory.CreateDirectory(CharacterFolder);

        Material stone = LoadMaterial("M_Stone_Rokko_Ocher");
        Material leaf = LoadMaterial("M_Leaf_Luma_Green");
        Material paper = LoadMaterial("M_Paper_Papyra_Ivory");
        Material scissors = LoadMaterial("M_Scissors_Cizo_Steel");
        Material dark = LoadMaterial("M_Noctar_Arena_Dark");
        Material prism = LoadMaterial("M_Prism_Star_Violet");
        Material gold = LoadMaterial("M_Golden_Star_Crystal");

        SavePrefab(BuildRokko(stone, leaf, gold), "PF_Rokko.prefab");
        SavePrefab(BuildLuma(leaf, gold), "PF_Luma.prefab");
        SavePrefab(BuildPapyra(paper, prism), "PF_Papyra.prefab");
        SavePrefab(BuildCizo(scissors, dark), "PF_Cizo.prefab");
        SavePrefab(BuildNoctar(dark, prism, scissors), "PF_Noctar.prefab");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static GameObject BuildRokko(Material stone, Material leaf, Material gold)
    {
        GameObject root = Root("Rokko_Stone_Guardian");
        Part(root, "body", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.95f, 1.15f, 0.95f), Quaternion.identity, stone);
        Part(root, "head", PrimitiveType.Sphere, new Vector3(0f, 1.85f, 0f), new Vector3(0.82f, 0.58f, 0.74f), Quaternion.identity, stone);
        Part(root, "left_fist", PrimitiveType.Sphere, new Vector3(-0.78f, 0.95f, 0.05f), new Vector3(0.46f, 0.46f, 0.46f), Quaternion.identity, stone);
        Part(root, "right_fist", PrimitiveType.Sphere, new Vector3(0.78f, 0.95f, 0.05f), new Vector3(0.46f, 0.46f, 0.46f), Quaternion.identity, stone);
        Part(root, "left_foot", PrimitiveType.Cube, new Vector3(-0.28f, 0.16f, 0.08f), new Vector3(0.44f, 0.22f, 0.58f), Quaternion.identity, stone);
        Part(root, "right_foot", PrimitiveType.Cube, new Vector3(0.28f, 0.16f, 0.08f), new Vector3(0.44f, 0.22f, 0.58f), Quaternion.identity, stone);
        Part(root, "scarf", PrimitiveType.Cube, new Vector3(0f, 1.42f, 0.08f), new Vector3(1.1f, 0.12f, 0.12f), Quaternion.identity, leaf);
        Part(root, "amber_crack", PrimitiveType.Cube, new Vector3(0.02f, 0.96f, 0.49f), new Vector3(0.08f, 0.64f, 0.04f), Quaternion.identity, gold);
        Eyes(root, new Vector3(-0.18f, 1.9f, 0.34f), new Vector3(0.18f, 1.9f, 0.34f), gold);
        return root;
    }

    private static GameObject BuildLuma(Material leaf, Material gold)
    {
        GameObject root = Root("Luma_Leaf_Guardian");
        Part(root, "body", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.56f, 1f, 0.56f), Quaternion.identity, leaf);
        Part(root, "head", PrimitiveType.Sphere, new Vector3(0f, 1.75f, 0f), new Vector3(0.62f, 0.58f, 0.62f), Quaternion.identity, leaf);
        Part(root, "left_wing", PrimitiveType.Cube, new Vector3(-0.68f, 1.18f, -0.05f), new Vector3(0.12f, 0.68f, 1f), Quaternion.Euler(0f, -18f, -8f), gold);
        Part(root, "right_wing", PrimitiveType.Cube, new Vector3(0.68f, 1.18f, -0.05f), new Vector3(0.12f, 0.68f, 1f), Quaternion.Euler(0f, 18f, 8f), gold);
        Part(root, "orange_scarf", PrimitiveType.Cube, new Vector3(0f, 1.36f, 0.08f), new Vector3(0.86f, 0.1f, 0.1f), Quaternion.identity, gold);
        Part(root, "leaf_crown", PrimitiveType.Cube, new Vector3(0f, 2.12f, 0f), new Vector3(0.26f, 0.55f, 0.08f), Quaternion.Euler(0f, 0f, 36f), gold);
        Eyes(root, new Vector3(-0.15f, 1.8f, 0.28f), new Vector3(0.15f, 1.8f, 0.28f), gold);
        return root;
    }

    private static GameObject BuildPapyra(Material paper, Material prism)
    {
        GameObject root = Root("Papyra_Paper_Guardian");
        Part(root, "body", PrimitiveType.Cube, new Vector3(0f, 0.95f, 0f), new Vector3(0.78f, 1.25f, 0.18f), Quaternion.identity, paper);
        Part(root, "head", PrimitiveType.Cube, new Vector3(0f, 1.78f, 0f), new Vector3(0.7f, 0.54f, 0.22f), Quaternion.identity, paper);
        Part(root, "fold_left", PrimitiveType.Cube, new Vector3(-0.48f, 1.2f, 0.03f), new Vector3(0.18f, 0.8f, 0.18f), Quaternion.Euler(0f, -18f, 0f), prism);
        Part(root, "fold_right", PrimitiveType.Cube, new Vector3(0.48f, 1.2f, 0.03f), new Vector3(0.18f, 0.8f, 0.18f), Quaternion.Euler(0f, 18f, 0f), prism);
        Part(root, "paper_rune", PrimitiveType.Cube, new Vector3(0f, 1.13f, 0.13f), new Vector3(0.42f, 0.06f, 0.04f), Quaternion.identity, prism);
        Part(root, "paper_hat", PrimitiveType.Cube, new Vector3(0f, 2.12f, 0f), new Vector3(0.76f, 0.2f, 0.2f), Quaternion.Euler(0f, 0f, 45f), paper);
        Eyes(root, new Vector3(-0.15f, 1.84f, 0.13f), new Vector3(0.15f, 1.84f, 0.13f), prism);
        return root;
    }

    private static GameObject BuildCizo(Material scissors, Material dark)
    {
        GameObject root = Root("Cizo_Scissors_Guardian");
        Part(root, "body", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.58f, 1.08f, 0.58f), Quaternion.identity, scissors);
        Part(root, "head", PrimitiveType.Sphere, new Vector3(0f, 1.76f, 0f), new Vector3(0.6f, 0.52f, 0.6f), Quaternion.identity, scissors);
        Part(root, "left_blade", PrimitiveType.Cube, new Vector3(-0.66f, 1.05f, 0.12f), new Vector3(0.16f, 0.9f, 0.18f), Quaternion.Euler(0f, 0f, 18f), scissors);
        Part(root, "right_blade", PrimitiveType.Cube, new Vector3(0.66f, 1.05f, 0.12f), new Vector3(0.16f, 0.9f, 0.18f), Quaternion.Euler(0f, 0f, -18f), scissors);
        Part(root, "left_handle", PrimitiveType.Cylinder, new Vector3(-0.28f, 0.36f, 0.12f), new Vector3(0.22f, 0.06f, 0.22f), Quaternion.Euler(90f, 0f, 0f), dark);
        Part(root, "right_handle", PrimitiveType.Cylinder, new Vector3(0.28f, 0.36f, 0.12f), new Vector3(0.22f, 0.06f, 0.22f), Quaternion.Euler(90f, 0f, 0f), dark);
        Part(root, "blue_scarf", PrimitiveType.Cube, new Vector3(0f, 1.35f, 0.08f), new Vector3(0.84f, 0.1f, 0.1f), Quaternion.identity, dark);
        Eyes(root, new Vector3(-0.14f, 1.8f, 0.28f), new Vector3(0.14f, 1.8f, 0.28f), scissors);
        return root;
    }

    private static GameObject BuildNoctar(Material dark, Material prism, Material scissors)
    {
        GameObject root = Root("Noctar_Prism_Warden");
        Part(root, "body", PrimitiveType.Capsule, new Vector3(0f, 1.08f, 0f), new Vector3(0.9f, 1.45f, 0.9f), Quaternion.identity, dark);
        Part(root, "head", PrimitiveType.Sphere, new Vector3(0f, 2.23f, 0f), new Vector3(0.68f, 0.58f, 0.68f), Quaternion.identity, dark);
        Part(root, "left_shoulder", PrimitiveType.Cube, new Vector3(-0.74f, 1.64f, 0f), new Vector3(0.34f, 0.34f, 0.66f), Quaternion.Euler(0f, 0f, 20f), prism);
        Part(root, "right_shoulder", PrimitiveType.Cube, new Vector3(0.74f, 1.64f, 0f), new Vector3(0.34f, 0.34f, 0.66f), Quaternion.Euler(0f, 0f, -20f), prism);
        Part(root, "crown_left", PrimitiveType.Cube, new Vector3(-0.24f, 2.68f, 0f), new Vector3(0.18f, 0.42f, 0.18f), Quaternion.Euler(0f, 0f, -24f), prism);
        Part(root, "crown_mid", PrimitiveType.Cube, new Vector3(0f, 2.74f, 0f), new Vector3(0.18f, 0.52f, 0.18f), Quaternion.identity, prism);
        Part(root, "crown_right", PrimitiveType.Cube, new Vector3(0.24f, 2.68f, 0f), new Vector3(0.18f, 0.42f, 0.18f), Quaternion.Euler(0f, 0f, 24f), prism);
        Part(root, "left_chain", PrimitiveType.Cylinder, new Vector3(-0.82f, 0.86f, -0.12f), new Vector3(0.08f, 0.76f, 0.08f), Quaternion.Euler(16f, 0f, 26f), scissors);
        Part(root, "right_chain", PrimitiveType.Cylinder, new Vector3(0.82f, 0.86f, -0.12f), new Vector3(0.08f, 0.76f, 0.08f), Quaternion.Euler(16f, 0f, -26f), scissors);
        Eyes(root, new Vector3(-0.16f, 2.25f, 0.34f), new Vector3(0.16f, 2.25f, 0.34f), prism);
        return root;
    }

    private static GameObject Root(string name)
    {
        GameObject root = new GameObject(name);
        root.transform.position = Vector3.zero;
        return root;
    }

    private static void Eyes(GameObject root, Vector3 left, Vector3 right, Material material)
    {
        Part(root, "left_eye", PrimitiveType.Sphere, left, new Vector3(0.1f, 0.1f, 0.05f), Quaternion.identity, material);
        Part(root, "right_eye", PrimitiveType.Sphere, right, new Vector3(0.1f, 0.1f, 0.05f), Quaternion.identity, material);
    }

    private static GameObject Part(GameObject root, string name, PrimitiveType primitive, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = name;
        part.transform.SetParent(root.transform, false);
        part.transform.localPosition = position;
        part.transform.localRotation = rotation;
        part.transform.localScale = scale;

        Renderer renderer = part.GetComponent<Renderer>();
        renderer.sharedMaterial = material;

        Collider collider = part.GetComponent<Collider>();
        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }

        return part;
    }

    private static Material LoadMaterial(string name)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Debug.LogWarning("Morphoria prefab material missing: " + path);
            material = new Material(Shader.Find("Standard"));
        }

        return material;
    }

    private static void SavePrefab(GameObject root, string fileName)
    {
        string path = CharacterFolder + "/" + fileName;
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }
}
