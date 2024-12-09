using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    public int prefabindex = 0;
    public GameObject[] blockPrefabs; // Prefab do bloco a ser instanciado
    public float placementDistance = 5f; // Distância máxima para colocar o bloco
    public LayerMask buildableSurface; // Camadas que podem receber blocos
    private GameObject previewBlock; // Bloco de visualização

    void Update()
    {
        UpdatePreviewBlock();

        if (Input.GetMouseButtonDown(0)) // Botão esquerdo do mouse para construir
        {
            PlaceBlock();
        }
        else if (Input.GetMouseButtonDown(1)) // Botão direito do mouse para apagar
        {
            RemoveBlock();
        }

        // Aumenta o prefabindex quando usar o scroll do mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            prefabindex += scroll > 0f ? 1 : -1;
            if (prefabindex >= blockPrefabs.Length)
            {
                prefabindex = 0;
            }
            else if (prefabindex < 0)
            {
                prefabindex = blockPrefabs.Length - 1;
            }

            // Atualiza o bloco de visualização
            if (previewBlock != null)
            {
                Destroy(previewBlock);
                previewBlock = null;
            }
        }
    }

    void UpdatePreviewBlock()
    {
        if(blockPrefabs[prefabindex] != null)
        {
            if (previewBlock == null)
            {
                previewBlock = Instantiate(blockPrefabs[prefabindex]);
                previewBlock.GetComponentInChildren<Collider>().enabled = false; // Desativa o collider do bloco de visualização

                // Desativa a gravidade do Rigidbody do bloco de visualização
                Rigidbody rb = previewBlock.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true; // Torna o Rigidbody cinemático para evitar interações físicas
                }
            }

            // Lança um raycast a partir da câmera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, placementDistance, buildableSurface))
            {
                // Calcula a posição do bloco sem snapping
                Vector3 placementPosition = hit.point;
                previewBlock.transform.position = placementPosition;
            }
        }
    }

    void PlaceBlock()
    {
        if (previewBlock != null)
        {
            // Instancia o bloco na posição e rotação do bloco de visualização
            GameObject go = Instantiate(blockPrefabs[prefabindex], previewBlock.transform.position, previewBlock.transform.rotation);
            go.GetComponentInChildren<Collider>().enabled = true; // Ativa o collider do bloco
        }
    }

    void RemoveBlock()
    {
        // Lança um raycast a partir da câmera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, placementDistance, buildableSurface))
        {
            // Verifica se o objeto atingido é um bloco e não é o bloco de visualização, então o destrói
            if (hit.collider.gameObject.CompareTag("Block"))
            {
                Debug.Log("Destroying block");
                Destroy(hit.collider.transform.parent.gameObject);
            }
        }
    }

    Vector3 GetSnappedPosition(Vector3 rawPosition)
    {
        // Arredonda a posição para o grid
        float x = Mathf.Round(rawPosition.x);
        float y = Mathf.Round(rawPosition.y);
        float z = Mathf.Round(rawPosition.z);
        return new Vector3(x, y, z);
    }
}