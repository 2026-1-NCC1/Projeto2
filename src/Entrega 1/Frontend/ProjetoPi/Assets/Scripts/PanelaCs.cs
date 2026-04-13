using UnityEngine;

public class Panela : MonoBehaviour
{
    public Transform ponto;

    private Renderer rend;
    private Color originalColor;

    public Color highlightColor = Color.green;

    private PickableItem itemAtual;
    private float tempoCozinhando = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    void Update()
    {
        if (itemAtual == null) return;

        tempoCozinhando += Time.deltaTime;

        // ?? VIRAR ASSADA (10s)
        if (tempoCozinhando >= 10f && itemAtual.carneAssadaPrefab != null)
        {
            Transform oldTransform = itemAtual.transform;

            GameObject novaCarne = Instantiate(
                itemAtual.carneAssadaPrefab,
                ponto.position,
                Quaternion.identity
            );

            novaCarne.transform.localScale = oldTransform.localScale;

            Destroy(itemAtual.gameObject);

            itemAtual = novaCarne.GetComponent<PickableItem>();
            tempoCozinhando = 10f; // trava pra não repetir
        }

        // ?? VIRAR QUEIMADA (15s)
        if (tempoCozinhando >= 15f && itemAtual.carneQueimadaPrefab != null)
        {
            Transform oldTransform = itemAtual.transform;

            GameObject novaCarne = Instantiate(
                itemAtual.carneQueimadaPrefab,
                ponto.position,
                Quaternion.identity
            );

            novaCarne.transform.localScale = oldTransform.localScale;

            Destroy(itemAtual.gameObject);

            itemAtual = novaCarne.GetComponent<PickableItem>();
            tempoCozinhando = 15f;
        }
    }

    void OnMouseEnter()
    {
        if (PointClickMovement.instance != null &&
            PointClickMovement.instance.EstaSegurandoItem())
        {
            if (rend != null)
                rend.material.color = highlightColor;
        }
    }

    void OnMouseExit()
    {
        if (rend != null)
            rend.material.color = originalColor;
    }

    public void ColocarItem(PickableItem item)
    {
        item.DropToPosition(ponto.position);

        itemAtual = item;
        tempoCozinhando = 0f;
    }
}