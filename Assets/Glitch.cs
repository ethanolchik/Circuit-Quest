using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glitch : Enemy
{
    /// <summary>
    /// The sprite to be displayed when the enemy is stunned
    /// </summary>
    [SerializeField] private Sprite stunned;

    /// <summary>
    /// The normal sprite when the enemy is not stunned
    /// </summary>
    [SerializeField] private Sprite normal;

    /// <summary>
    /// The number of seconds the stun lasts for
    /// </summary>
    private float stunDuration = 2f;
    void LateUpdate()
    {
        if (Random.Range(0, 500) == 1f) {
            Stun();
        }
    }

    private void Stun()
    {
        sprite.sprite = stunned;
        // Freeze the x axis
        
        shouldFreeze = true;
        StartCoroutine(Recover());
    }

    private IEnumerator Recover()
    {
        // Wait for the stun duration, then reset the sprite and unfreeze the x axis
        yield return new WaitForSeconds(stunDuration);
        sprite.sprite = normal;
        shouldFreeze = false;
    }
}
