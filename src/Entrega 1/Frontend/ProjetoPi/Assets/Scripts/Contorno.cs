using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 5f;

    private Renderer rend;
    private Material outlineMaterial;
    private Material originalMaterial;

    void Start()
    {
        rend = GetComponent<Renderer>();

        originalMaterial = rend.material;

        outlineMaterial = new Material(Shader.Find("Unlit/Color"));
        outlineMaterial.color = outlineColor;
    }

    public void EnableOutline()
    {
        rend.material = outlineMaterial;
    }

    public void DisableOutline()
    {
        rend.material = originalMaterial;
    }
}