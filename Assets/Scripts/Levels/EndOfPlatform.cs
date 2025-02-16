using UnityEngine;

public class EndOfPlatform : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 3)
        {
            other.gameObject.GetComponent<Enemy>().Flip();
        }
    }
}
