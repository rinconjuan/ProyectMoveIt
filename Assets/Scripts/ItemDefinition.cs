using UnityEngine;

[CreateAssetMenu(menuName = "Mudanzas/ItemDefinition")]
public class ItemDefinition : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite sprite;
    public float weight = 1f;
    public float durabilityMax = 10f;
    public int value = 10;
    [Tooltip("Probabilidad relativa de aparecer")]
    public float spawnWeight = 1f;
    [Tooltip("Tags de habitaciones preferidas (ej: Kitchen, LivingRoom)")]
    public string[] preferredRoomTags;
}
