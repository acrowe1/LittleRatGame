using UnityEngine;

// Logic for the damage indicator numbers that fall from the healtbar when hit by an enemy
// The red flashing that also happens is handled in the PlayerControls
public class PopUpDamage : MonoBehaviour
{
    public Vector2 InitialVelocity;
    public Rigidbody2D rb;
    public float lifetime = 1.5f;
    void Start()
    {
        rb.velocity = InitialVelocity;
        Destroy(gameObject, lifetime);
    }
}
