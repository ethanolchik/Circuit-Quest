using System.Collections.Generic;
using UnityEngine;

public class Explanations : MonoBehaviour
{
    [SerializeField] private List<Sprite> explanations;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the sprite to the first explanation
        spriteRenderer.sprite = explanations[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Get the current index of the sprite
            int currentIndex = explanations.IndexOf(spriteRenderer.sprite);

            // If the current index is the last index, destroy the object
            if (currentIndex == explanations.Count - 1)
            {
                Destroy(gameObject);
            }
            else
            {
                // Set the sprite to the next explanation
                spriteRenderer.sprite = explanations[currentIndex + 1];
            }
        }        
    }
}
