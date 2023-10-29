using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceItem : Pickup
{
    public int experienceGranted;

    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }

        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncreaseExperience(experienceGranted);
    }
}
