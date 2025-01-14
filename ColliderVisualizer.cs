using MelonLoader;
using System.Linq;
using UnityEngine;

[assembly: MelonInfo(typeof(ColliderVisualizer), "Collider Visualizer", PluginInfo.PLUGIN_VERSION, "Kalico")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

public class ColliderVisualizer : MelonMod
{
    private Material redMaterial;
    private int peakBoundaryLayer;

    public override void OnInitializeMelon()
    {
        redMaterial = new Material(Shader.Find("Standard"));
        redMaterial.color = Color.red;
        peakBoundaryLayer = LayerMask.NameToLayer("PeakBoundary");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (peakBoundaryLayer == -1 
            || sceneName == "Cabin" 
            || sceneName == "Category4_1_Cabin" 
            || sceneName == "Alps_Main")
        {
            return;
        }

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        string[] resetBoxNames = new[] { "resetbox", "reset box" };

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer != peakBoundaryLayer 
                && !resetBoxNames.Any(name => obj.name.ToLower().StartsWith(name)))
            {
                continue;
            }

            Collider collider = obj.GetComponent<Collider>();

            if (collider == null)
            {
                continue;
            }

            switch (collider)
            {
                case BoxCollider boxCollider:
                    HandleBoxCollider(boxCollider);
                    break;

                case MeshCollider meshCollider:
                    HandleMeshCollider(meshCollider);
                    break;

                case CapsuleCollider capsuleCollider:
                    HandleCapsuleCollider(capsuleCollider);
                    break;

                case SphereCollider sphereCollider:
                    HandleSphereCollider(sphereCollider);
                    break;

                default:
                    break;
            }
        }
    }

    private void HandleBoxCollider(BoxCollider boxCollider)
    {
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);

        visual.name = $"ColliderVisual_{boxCollider.gameObject.name}";
        visual.transform.parent = boxCollider.transform;

        visual.transform.localPosition = boxCollider.center;
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = boxCollider.size;

        MeshRenderer visualRenderer = visual.GetComponent<MeshRenderer>();

        if (visualRenderer != null)
        {
            visualRenderer.material = redMaterial;
        }

        Collider visualCollider = visual.GetComponent<BoxCollider>();
        GameObject.DestroyImmediate(visualCollider);

        ApplyVisualMaterial(visual, true);
    }

    private void HandleMeshCollider(MeshCollider meshCollider)
    {
        GameObject obj = meshCollider.transform.gameObject;

        MeshFilter filter = obj.GetComponent<MeshFilter>();
        GameObject.DestroyImmediate(filter);

        MeshFilter newFilter = obj.AddComponent<MeshFilter>();
        newFilter.mesh = meshCollider.sharedMesh;

        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        GameObject.DestroyImmediate(renderer);

        obj.AddComponent<MeshRenderer>();
        ApplyVisualMaterial(obj, false);
    }

    private void HandleCapsuleCollider(CapsuleCollider capsuleCollider)
    {
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);

        float diameter = capsuleCollider.radius * 2;
        float cylinderHeight = Mathf.Max(0, capsuleCollider.height - (2 * capsuleCollider.radius));
        float totalHeight = cylinderHeight + (2 * capsuleCollider.radius);

        visual.name = $"ColliderVisual_{capsuleCollider.gameObject.name}";
        visual.transform.parent = capsuleCollider.transform;
        visual.transform.localPosition = capsuleCollider.center;
        visual.transform.localScale = new Vector3(diameter, totalHeight / 2, diameter);

        switch (capsuleCollider.direction)
        {
            case 0: // X-axis
                visual.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case 1: // Y-axis (default)
                visual.transform.localRotation = Quaternion.identity;
                break;
            case 2: // Z-axis
                visual.transform.localRotation = Quaternion.Euler(90, 0, 0);
                break;
        }

        MeshRenderer renderer = visual.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.material = redMaterial;
        }

        Collider colliderComponent = visual.GetComponent<CapsuleCollider>();
        GameObject.DestroyImmediate(colliderComponent);

        ApplyVisualMaterial(visual, true);
    }


    private void HandleSphereCollider(SphereCollider sphereCollider)
    {
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        float diameter = sphereCollider.radius * 2;

        visual.name = $"ColliderVisual_{sphereCollider.gameObject.name}";
        visual.transform.parent = sphereCollider.transform;

        visual.transform.localPosition = sphereCollider.center;
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = new Vector3(diameter, diameter, diameter);

        MeshRenderer renderer = visual.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.material = redMaterial;
        }

        Collider colliderComponent = visual.GetComponent<SphereCollider>();
        GameObject.DestroyImmediate(colliderComponent);

        ApplyVisualMaterial(visual, true);
    }

    private void ApplyVisualMaterial(GameObject visual, bool childVisual)
    {
        if (childVisual && visual.transform.parent != null)
        {
            MeshRenderer parentRenderer = visual.transform.parent.GetComponent<MeshRenderer>();
            GameObject.DestroyImmediate(parentRenderer);
        }

        MeshRenderer renderer = visual.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.material = redMaterial;
        }
    }
}
