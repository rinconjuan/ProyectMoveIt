using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerMovementTopDown playerMovement; // para leer lastMoveDir
    public Transform carryPoint;                 // punto desde donde se lanza el raycast y se "carga" el objeto

    [Header("Configuración")]
    public float pickupRange = 1f;               // distancia máxima del raycast
    public float pickupRadius = 1f;               // distancia máxima del raycast

    public KeyCode interactKey = KeyCode.F;      // tecla de interacción
    public int itemLayerMask;
    private GameObject currentCandidate; // Item actualmente más cercano y alineado

    private Rigidbody2D carriedObject;           // referencia al objeto que estamos llevando
    public bool isCollision = false;
    public bool isCarrying = false;

    void Start()
    {
        itemLayerMask = LayerMask.GetMask("Item");
        Time.fixedDeltaTime = 0.001f;
    }

    void Update()
    {
        DetectNearestItem();

        // Si presiona la tecla de interacción
        if (Input.GetKeyDown(interactKey))
        {
            if (carriedObject == null)
                TryPickup();
            else
                Drop();
        }

        // --- Actualiza posición del carryPoint según la dirección del jugador ---
        if (playerMovement.lastMoveDir.sqrMagnitude > 0.01f)
        {
            Vector2 dir = playerMovement.lastMoveDir.normalized;
            float offset = 0.5f; // distancia desde el centro del jugador
            float offsety = 1f;

            // mueve el carryPoint frente al jugador
            if (dir.y > 0f) { carryPoint.localPosition = dir * offsety; }
            else
            {
                carryPoint.localPosition = dir * offset;
            }

            //// Opcional: ajusta capa visual del objeto que llevas
            if (carriedObject != null)
            {
                SpriteRenderer objSprite = carriedObject.GetComponent<SpriteRenderer>();
                SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();

                //objSprite.sortingLayerID = playerSprite.sortingLayerID;

                if (objSprite != null && playerSprite != null)
                {
                    if (dir.y > 0)
                    {
                        objSprite.sortingOrder = 1; // detrás del jugador
                    }

                    else
                        objSprite.sortingOrder = playerSprite.sortingOrder + 2; // delante del jugador
                }
            }
        }
       

    }

    void FixedUpdate()
    {
        if (carriedObject != null)
        {
            Vector2 targetPos = carryPoint.position;
            Vector2 currentPos = carriedObject.position;
            //Vector2 newPos = Vector2.Lerp(currentPos, targetPos, 15f * Time.fixedDeltaTime);
            Vector2 newPos = targetPos;
            carriedObject.MovePosition(newPos);

            Collider2D itemCol = carriedObject.GetComponent<Collider2D>();
            if(itemCol != null)
            {
                isCollision = itemCol.IsTouchingLayers(LayerMask.GetMask("Default", "Item"));
                Debug.Log("Choco: " + isCollision);
            }
        }
    }


    void DetectNearestItem()
    {
        // --- CONFIGURACIÓN ---
        float pickupRadius = 1f; // ajusta según escala del jugador
        float alignmentWeight = 0.7f;
        float distanceWeight = 0.3f;

        // --- DETECCIÓN ---
        Collider2D[] hits = Physics2D.OverlapCircleAll(carryPoint.position, pickupRadius, itemLayerMask);

        if (hits.Length == 0)
        {
            currentCandidate = null;
            return;
        }

        // --- VARIABLES DE APOYO ---
        Vector2 origin = carryPoint.position;
        Vector2 dir = playerMovement.lastMoveDir.normalized;
        float bestScore = float.MinValue;
        Collider2D bestHit = null;

        foreach (var hit in hits)
        {
            Vector2 toItem = (Vector2)hit.transform.position - origin;
            float distance = toItem.magnitude;
            Vector2 toItemDir = toItem.normalized;

            // Cuán alineado está con la dirección del jugadora
            float alignment = Vector2.Dot(dir, toItemDir);
            // Cuán cerca está
            float invDistance = 1f / (distance + 0.001f);

            // Score total combinando alineación y distancia
            float score = alignmentWeight * alignment + distanceWeight * invDistance;

            if (score > bestScore)
            {
                bestScore = score;
                bestHit = hit;
            }

            // Debug visual para probar
            Debug.DrawLine(origin, hit.transform.position, Color.yellow, 0.1f);
            Debug.Log($"Item {hit.name} -> Alineación: {alignment:F2}, Distancia: {distance:F2}, Score: {score:F2}");
        }

        // --- ASIGNAR EL CANDIDATO ELEGIDO ---
        currentCandidate = bestHit != null ? bestHit.gameObject : null;

        // Línea verde hacia el candidato elegido
        if (currentCandidate != null)
        {
            Debug.DrawLine(origin, currentCandidate.transform.position, Color.green, 0.2f);
            Debug.Log($"→ Candidato elegido: {currentCandidate.name}");
        }
    }


    void TryPickup()
    {

        if (currentCandidate == null) return;

        Rigidbody2D itemRb = currentCandidate.GetComponent<Rigidbody2D>();
        if (itemRb == null)
        {
            Debug.LogWarning("El objeto no tiene Rigidbody2D, no se puede recoger.");
            return;
        }

        // Guardamos referencia y preparamos
        carriedObject = itemRb;

        // Si el objeto era estático, lo volvemos temporalmente cinemático
        if (carriedObject.bodyType == RigidbodyType2D.Static)
            carriedObject.bodyType = RigidbodyType2D.Kinematic;

        // Desactivar colisiones entre jugador y objeto
        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D itemCol = carriedObject.GetComponent<Collider2D>();
        if (playerCol && itemCol)
            Physics2D.IgnoreCollision(playerCol, itemCol, true);

        // Lo posicionamos en el carry point
        carriedObject.transform.position = carryPoint.position;
        carriedObject.constraints = RigidbodyConstraints2D.None;
        carriedObject.freezeRotation = true;

        isCarrying = true;
        Debug.Log($"Recogido: {currentCandidate.name}");
    }

    void Drop()
    {
        if(carriedObject == null) return;

        // Reactivar colisiones
        Collider2D playerCol = GetComponent<Collider2D>();
        Collider2D itemCol = carriedObject.GetComponent<Collider2D>();
        if (playerCol && itemCol)
            Physics2D.IgnoreCollision(playerCol, itemCol, false);

        // Volvemos el cuerpo a Static (no empujable)
        carriedObject.bodyType = RigidbodyType2D.Dynamic;
        carriedObject.freezeRotation = false;
        carriedObject.constraints = RigidbodyConstraints2D.FreezeAll;

        carriedObject = null;
        isCollision = false;
        isCarrying = false;
        Debug.Log("Objeto soltado.");
    }


}
