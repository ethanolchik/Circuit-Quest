using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float power = 20f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<PlayerMovement>().JumpPadTrigger(power);
        }
    }
}
