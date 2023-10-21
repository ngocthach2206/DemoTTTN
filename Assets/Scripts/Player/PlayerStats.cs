using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    CharacterScriptableObject characterData;

    //Current stats
    //[HideInInspector]
    public float currentHealth;
    //[HideInInspector]
    public float currentRecovery;
    //[HideInInspector]
    public float currentMoveSpeed;
    //[HideInInspector]
    public float currentMight;
    //[HideInInspector]
    public float currentProjectileSpeed;
    //[HideInInspector]
    public float currentMagnet;

    //Experence and level of player
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap = 100; //the max amount exp need to up to lv2 when start game
    public int experienceCapIncrease; //the max amount exp need for every level

    //i-frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    public GameObject firstPassiveItemTest, secondPassiveItemTest;

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();

        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentMagnet = characterData.Magnet;

        //spawn starting weapon
        SpawnWeapon(characterData.StartingWeapon);

        SpawnPassiveItem(firstPassiveItemTest);
        SpawnPassiveItem(secondPassiveItemTest);
    }

    void Update()
    {
        if(invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if(isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();
    }

    void LevelUpChecker()
    {
        if(experience >= experienceCap) //level up if the experience is meet required amount
        {
            //level up the player and deduct experience
            level++;
            experience -= experienceCap;
            experienceCap += experienceCapIncrease;
        }
    }

    public void TakeDamage(float dmg)
    {
        //if the player is not currently in invincible state, reduce health and start it
        if(!isInvincible)
        {
            currentHealth -= dmg;
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (currentHealth <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        Debug.Log("PLAYER IS DEAD !");
    }

    public void RestoreHealth(float amount)
    {
        //just health the player when current health is less than maxium health
        if(currentHealth < characterData.MaxHealth)
        {
            currentHealth += amount;

            if(currentHealth > characterData.MaxHealth)
            {
                currentHealth = characterData.MaxHealth;
            }
        }
    }

    void Recover()
    {
        if(currentHealth < characterData.MaxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;
            //make the player recovery doesn't exceed maxium health
            if(currentHealth > characterData.MaxHealth)
            {
                currentHealth = characterData.MaxHealth;
            }
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {
        if(weaponIndex >= inventory.weaponSlots.Count -1)
        {
            Debug.LogError("Inventory slots already full");
            return;
        }

        //spawn starting weapon
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); //set the weapon to be a child of the player
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); //add the weapon to inventory slot
        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("Inventory slots already full");
            return;
        }

        //spawn starting passive item
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); //set the passive item to be a child of the player
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); //add the passive item to inventory slot
        passiveItemIndex++;
    }
}
