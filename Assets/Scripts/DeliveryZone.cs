using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [Tooltip("Layers de items")]
    public LayerMask itemMask;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & itemMask) == 0) return;
        var runtime = other.GetComponent<ItemRuntime>();
        if (runtime == null) return;

        // Calcular valor entregado proporcional a durabilidad
        float ratio = runtime.definition != null && runtime.definition.durabilityMax > 0
            ? runtime.durability / runtime.definition.durabilityMax
            : 1f;
        int earned = Mathf.RoundToInt(runtime.definition != null ? runtime.definition.value * ratio : 0);

        // Notificar RunManager (puedes implementar con eventos o llamado directo)
        RunManager.Instance?.OnItemDelivered(runtime.definition, earned);

        // Destruir o desactivar item
        Destroy(other.gameObject);
    }
}
