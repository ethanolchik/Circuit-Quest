using UnityEngine;
using UnityEngine.UI;

public class LogicGateSpawner : MonoBehaviour
{
    /// <summary>
    /// The button that spawns the gate
    /// </summary>
    private Button gate;
    [SerializeField] private GameObject gateTemplate; // Set in unity editor, specific to the gate type
    [SerializeField] private Transform canvas;
    [SerializeField] private Transform LogicGates;

    // Start is called before the first frame update
    void Start()
    {
        gate = gameObject.GetComponent<Button>();
        gate.onClick.AddListener(() => {
            // Spawn the gate at (0, 0, 0)
            GameObject spawned = Instantiate(gateTemplate, Vector3.zero, Quaternion.identity);
            // Activate the logic gate
            spawned.transform.SetParent(LogicGates);
            spawned.transform.localScale = Vector3.one;
            spawned.SetActive(true);
        });
    }
}
