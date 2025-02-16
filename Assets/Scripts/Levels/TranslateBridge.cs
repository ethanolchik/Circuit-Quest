using System.Collections;
using UnityEngine;

public class TranslateBridge : MonoBehaviour
{
    [SerializeField] private GameObject solved;

    private bool isCoroutineRunning = true;
    public void InvokeMoveBridge()
    {
        if (GameInfo.Instance.GetPuzzleSolved())
        {
            // Set the puzzle to unsolved
            GameInfo.Instance.SetSolvePuzzle(false);
            BroadcastMessage("DestroyProximityMessage");

            // Use linear interpolation between gameObject.transform.position and solved.transform.position
            StartCoroutine(MoveBridge(gameObject.transform.position, solved.transform.position, 3f));
        }
    }

    void Update()
    {
        if (!isCoroutineRunning)
        {
            Destroy(this);
        }
    }

    IEnumerator MoveBridge(Vector3 start, Vector3 end, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            // Move the bridge from start to end over time elapsedTime
            gameObject.transform.position = Vector3.Lerp(start, end, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.position = end;

        isCoroutineRunning = false;
    }
}
