using UnityEngine;

public class ManageNPC2 : MonoBehaviour
{
    private Animator anim;
    private float health = 100f;
    private int ammos = 100;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Gradual health degradation for testing features (you can adjust or comment this out if needed)
        SetHealth(health - Time.deltaTime * 0.5f);
        if (health < 0) SetHealth(0);
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
        anim.SetFloat("health", health);
    }

    public int GetAmmos()
    {
        return ammos;
    }

    public void SetAmmos(int newAmbos)
    {
        ammos = newAmbos;
        anim.SetFloat("ammos", ammos);
    }

    public void DecreaseAmmo()
    {
        ammos--;
        if (ammos <= 0) ammos = 0;
        anim.SetFloat("ammos", ammos);
    }

    // --- Added methods to handle incoming damage ---

    public void gotHit()
    {
        // Standard hit damage (e.g., bullets)
        SetHealth(health - 10f);
    }

    public void gotHit(float damage)
    {
        // Overload for custom damage amounts
        SetHealth(health - damage);
    }

    public void gotHitByGrenade()
    {
        // Higher damage from explosions/grenades
        SetHealth(health - 50f);
    }
}