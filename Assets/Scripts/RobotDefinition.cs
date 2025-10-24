using UnityEngine;

[CreateAssetMenu(menuName = "Mudanzas/RobotDefinition")]
public class RobotDefinition : ScriptableObject
{
    public string id;
    public string displayName;
    public float energyMax = 100f;
    public float carryCapacity = 5f;
    public float baseSpeed = 5f;
    public float detectionRadius = 2f;
    public Sprite sprite;
    public enum Ability { None, ExtraGrip, ShockAbsorb, Turbo }
    public Ability ability = Ability.None;
}
