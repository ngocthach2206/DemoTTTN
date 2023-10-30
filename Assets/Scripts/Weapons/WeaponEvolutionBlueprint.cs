using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponEvolutionBlueprint", menuName = "ScriptableObjects/WeaponEvoluionBlueprint")]
public class WeaponEvolutionBlueprint : ScriptableObject
{
    public WeaponScriptableObject baseWeaponData;
    public PassiveItemScriptableObject catalystPassiveItemData;
    public WeaponScriptableObject evolveWeaponData;
    public GameObject evolvedWeapon;
}
