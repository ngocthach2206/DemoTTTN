using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
    }

    void Update()
    {
        if(Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if(currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
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
