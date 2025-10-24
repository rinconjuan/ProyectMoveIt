using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnItemDelivered(ItemDefinition def, int earnedValue)
    {
        Debug.Log($"Item entregado: {def.displayName} -> Valor recibido: {earnedValue}");
        // Aqu� actualizar puntaje, energ�a, objetivos, etc.
    }
}
