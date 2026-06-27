using System;
using System.Collections.Generic;
using UnityEngine;

namespace Morphoria
{
    public sealed class MorphoriaAvatar : MonoBehaviour
    {
        private Transform visualRoot;

        public void ApplyForm(FormDefinition form)
        {
            if (visualRoot != null)
            {
                Destroy(visualRoot.gameObject);
            }

            visualRoot = new GameObject("Avatar_" + form.heroName).transform;
            visualRoot.SetParent(transform, false);
            visualRoot.localPosition = Vector3.zero;

            switch (form.form)
            {
                case MorphoriaForm.Stone:
                    BuildStone(form);
                    break;
                case MorphoriaForm.Leaf:
                    BuildLeaf(form);
                    break;
                case MorphoriaForm.Paper:
                    BuildPaper(form);
                    break;
                case MorphoriaForm.Scissors:
                    BuildScissors(form);
                    break;
            }
        }

        private void BuildStone(FormDefinition form)
        {
            Material body = RuntimeMaterial(form.color, form.accent);
            CreatePart("rock_body", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.95f, 1.15f, 0.95f), body);
            CreatePart("rock_head", PrimitiveType.Sphere, new Vector3(0f, 1.85f, 0f), new Vector3(0.82f, 0.58f, 0.74f), body);
            CreatePart("left_fist", PrimitiveType.Sphere, new Vector3(-0.78f, 0.95f, 0.05f), new Vector3(0.46f, 0.46f, 0.46f), body);
            CreatePart("right_fist", PrimitiveType.Sphere, new Vector3(0.78f, 0.95f, 0.05f), new Vector3(0.46f, 0.46f, 0.46f), body);
            CreatePart("scarf", PrimitiveType.Cube, new Vector3(0f, 1.42f, 0.08f), new Vector3(1.1f, 0.12f, 0.12f), RuntimeMaterial(new Color(0.14f, 0.36f, 0.16f), form.accent));
        }

        private void BuildLeaf(FormDefinition form)
        {
            Material body = RuntimeMaterial(form.color, form.accent);
            CreatePart("leaf_body", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.56f, 1.0f, 0.56f), body);
            CreatePart("leaf_head", PrimitiveType.Sphere, new Vector3(0f, 1.75f, 0f), new Vector3(0.62f, 0.58f, 0.62f), body);
            CreatePart("left_wing", PrimitiveType.Cube, new Vector3(-0.68f, 1.18f, -0.05f), new Vector3(0.12f, 0.68f, 1.0f), RuntimeMaterial(form.accent, form.color));
            CreatePart("right_wing", PrimitiveType.Cube, new Vector3(0.68f, 1.18f, -0.05f), new Vector3(0.12f, 0.68f, 1.0f), RuntimeMaterial(form.accent, form.color));
            CreatePart("orange_scarf", PrimitiveType.Cube, new Vector3(0f, 1.36f, 0.08f), new Vector3(0.86f, 0.1f, 0.1f), RuntimeMaterial(new Color(1f, 0.47f, 0.09f), form.accent));
        }

        private void BuildPaper(FormDefinition form)
        {
            Material body = RuntimeMaterial(form.color, form.accent);
            CreatePart("paper_body", PrimitiveType.Cube, new Vector3(0f, 0.95f, 0f), new Vector3(0.78f, 1.25f, 0.18f), body);
            CreatePart("paper_head", PrimitiveType.Cube, new Vector3(0f, 1.78f, 0f), new Vector3(0.7f, 0.54f, 0.22f), body);
            CreatePart("fold_left", PrimitiveType.Cube, new Vector3(-0.48f, 1.2f, 0.03f), new Vector3(0.18f, 0.8f, 0.18f), RuntimeMaterial(new Color(0.72f, 0.62f, 1f), form.accent));
            CreatePart("fold_right", PrimitiveType.Cube, new Vector3(0.48f, 1.2f, 0.03f), new Vector3(0.18f, 0.8f, 0.18f), RuntimeMaterial(new Color(0.72f, 0.62f, 1f), form.accent));
        }

        private void BuildScissors(FormDefinition form)
        {
            Material body = RuntimeMaterial(form.color, form.accent);
            CreatePart("scissors_body", PrimitiveType.Capsule, new Vector3(0f, 0.9f, 0f), new Vector3(0.58f, 1.08f, 0.58f), body);
            CreatePart("scissors_head", PrimitiveType.Sphere, new Vector3(0f, 1.76f, 0f), new Vector3(0.6f, 0.52f, 0.6f), body);
            GameObject left = CreatePart("left_blade", PrimitiveType.Cube, new Vector3(-0.66f, 1.05f, 0.12f), new Vector3(0.16f, 0.9f, 0.18f), RuntimeMaterial(new Color(0.9f, 0.95f, 1f), form.accent));
            GameObject right = CreatePart("right_blade", PrimitiveType.Cube, new Vector3(0.66f, 1.05f, 0.12f), new Vector3(0.16f, 0.9f, 0.18f), RuntimeMaterial(new Color(0.9f, 0.95f, 1f), form.accent));
            left.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
            right.transform.localRotation = Quaternion.Euler(0f, 0f, -18f);
            CreatePart("blue_scarf", PrimitiveType.Cube, new Vector3(0f, 1.35f, 0.08f), new Vector3(0.84f, 0.1f, 0.1f), RuntimeMaterial(new Color(0.05f, 0.2f, 0.42f), form.accent));
        }

        private GameObject CreatePart(string partName, PrimitiveType primitive, Vector3 position, Vector3 scale, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(primitive);
            part.name = partName;
            part.transform.SetParent(visualRoot, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;

            Collider partCollider = part.GetComponent<Collider>();
            if (partCollider != null)
            {
                Destroy(partCollider);
            }

            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            return part;
        }

        private static Material RuntimeMaterial(Color color, Color emission)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = color;
            material.SetColor("_EmissionColor", emission * 0.25f);
            material.EnableKeyword("_EMISSION");
            return material;
        }
    }
}
