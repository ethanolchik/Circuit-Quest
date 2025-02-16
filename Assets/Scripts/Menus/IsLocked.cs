using UnityEngine;

public class IsLocked : MonoBehaviour
{
    [SerializeField] private int levelNumber;

    void Start()
    {
        if (GameInfo.Instance.CurrentLevel() >= levelNumber)
        {
            // Unlock the level
            transform.parent.GetComponent<LoadLevel>().Unlock();

            Destroy(gameObject);
        }
    }
}
