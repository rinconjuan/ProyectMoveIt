using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ItemRuntime : MonoBehaviour
{
    public ItemDefinition definition;

    [HideInInspector] public float durability;
    [HideInInspector] public bool isCarried = false;
    [HideInInspector] public bool collidedWhileCarried = false;

    Rigidbody2D rb;
    Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        durability = definition != null ? definition.durabilityMax : 1f;
    }

    public void InitializeFromDefinition(ItemDefinition def)
    {
        definition = def;
        durability = def != null ? def.durabilityMax : durability;
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && def != null) sr.sprite = def.sprite;
    }

    public void SetCarried(bool on)
    {
        isCarried = on;
        collidedWhileCarried = false;
        // Ajustes físicos cuando se carga
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }
        if (col != null)
        {
            // Asegúrate que la capa del item permita colisiones con el mundo según necesites
            col.enabled = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCarried) return;

        // Ignorar colisión con el player (suponiendo capa o tag del player)
        if (collision.collider.CompareTag("Player")) return;

        // Marcar colisión mientras se lleva
        collidedWhileCarried = true;

        // Reducir durabilidad como ejemplo
        float impactDamage = collision.relativeVelocity.magnitude * 0.5f;
        ApplyDamage(impactDamage);
    }

    public void ApplyDamage(float amount)
    {
        durability -= amount;
        if (durability < 0f) durability = 0f;
    }
}
