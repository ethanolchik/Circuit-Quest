using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SubmissionMessage : MonoBehaviour
{
    public static SubmissionMessage Instance;
    [SerializeField] private Sprite correct;
    [SerializeField] private Sprite incorrect;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    public void ShowCorrect()
    {
        spriteRenderer.sprite = correct;
        spriteRenderer.enabled = true;

        // Wait until any input key is pressed async
        StartCoroutine(WaitForKeyDown(true));
    }

    public void ShowIncorrect()
    {
        spriteRenderer.sprite = incorrect;
        spriteRenderer.enabled = true;

        // Wait until any input key is pressed async
        StartCoroutine(WaitForKeyDown(false));
    }

    private IEnumerator WaitForKeyDown(bool isCorrect)
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        
        // Once a key has been pressed, set the sprite to invisible
        spriteRenderer.enabled = false;

        if (isCorrect)
            GateEditor.Instance.PostCorrect();
        else
            GateEditor.Instance.PostIncorrect();
    }
}
