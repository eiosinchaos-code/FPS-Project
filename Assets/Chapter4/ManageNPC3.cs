using UnityEngine;

public class ManageNPC3 : MonoBehaviour
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
        // Gradual health degradation for testing features
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
}