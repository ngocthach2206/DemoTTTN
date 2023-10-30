using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    //current stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    Transform player;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1); //color of damage flash
    public float damageFlashDuration = 0.2f; //the time flash last
    public float deathFadeTime = 0.6f; //time it takes for the enemy fade
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;

    void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        movement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if(Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        //create text pop up when enemies take damage
        if(dmg > 0)
        {
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
        }

        //apply knockback
        if(knockbackForce > 0)
        {
            //get direction of knockback
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if(currentHealth <= 0)
        {
            Kill();
        }
    }

    //make the enemies flash when taking damage
    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        //wait for a single frame
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, originalAlpha = sr.color.a;

        //loop that fires every frame
        while(t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            //set the colour fo this frame
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * originalAlpha);
        }
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        //reference the script from the collided collider and deal damage using TakeDamage()
        if(col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            //use currentDamage instead of weaponData.damage in case any damage multipliers in the future
            player.TakeDamage(currentDamage);
        }
    }

    private void OnDestroy()
    {
        EnemySpawner enemySp = FindObjectOfType<EnemySpawner>();
        enemySp.OnEnemyKilled();
    }

    void ReturnEnemy()
    {
        EnemySpawner enemySp = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + enemySp.relativeSpawnPoints[Random.Range(0, enemySp.relativeSpawnPoints.Count)].position;
    }
}
