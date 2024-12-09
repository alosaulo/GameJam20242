using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;
    public int prefabindex = 0;
    public GameObject[] blockPrefabs; // Prefab do bloco a ser instanciado
    public float placementDistance = 5f; // Distância máxima para colocar o bloco
    public LayerMask buildableSurface; // Camadas que podem receber blocos
    public Transform weaponPosition;
    private GameObject previewBlock; // Bloco de visualização
    public Material[] blockMaterials; // Materiais disponíveis para os blocos
    private int materialIndex = 0; // Índice do material atual

    void Update()
    {
        UpdatePreviewBlock();

        if (Input.GetMouseButtonDown(0)) // Botão esquerdo do mouse para construir
        {
            if (prefabindex != 1 && prefabindex != 2)
                PlaceBlock();
            else if (prefabindex == 1)
                PaintBlock();
            else if (prefabindex == 2)
                GlueBlock();
           
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
        if (blockPrefabs[prefabindex] != null && prefabindex != 1 && prefabindex != 2)
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
        if (prefabindex == 1 || prefabindex == 2)
        {
            if (previewBlock == null)
            {
                previewBlock = Instantiate(blockPrefabs[prefabindex], weaponPosition);
                previewBlock.transform.localPosition = Vector3.zero; // Garante que o pincel esteja na posição correta em relação ao personagem
            }
            else
            {
                previewBlock.transform.position = weaponPosition.position; // Atualiza a posição do pincel para seguir o personagem
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
            audioSource.PlayOneShot(audioClip);
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
                audioSource.PlayOneShot(audioClip);
            }
        }
    }

    void PaintBlock()
    {
        // Lança um raycast a partir da câmera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, placementDistance, buildableSurface))
        {
            // Verifica se o objeto atingido é um bloco
            if (hit.collider.gameObject.CompareTag("Block"))
            {
                Renderer renderer = hit.collider.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    materialIndex = (materialIndex + 1) % blockMaterials.Length;
                    renderer.material = blockMaterials[materialIndex];
                    audioSource.PlayOneShot(audioClip);
                }
            }
        }
    }

    void GlueBlock() 
    {
        // Lança um raycast a partir da câmera
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, placementDistance, buildableSurface))
        {
            // Verifica se o objeto atingido é um bloco
            if (hit.collider.gameObject.CompareTag("Block"))
            {
                Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
                Debug.Log(rb);
                if (rb != null)
                {
                    if(rb.constraints == RigidbodyConstraints.FreezeAll)
                        rb.constraints = RigidbodyConstraints.None;
                    else
                        rb.constraints = RigidbodyConstraints.FreezeAll;
                    Debug.Log(rb.constraints);
                    audioSource.PlayOneShot(audioClip);
                }
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