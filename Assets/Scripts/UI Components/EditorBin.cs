using UnityEngine;
using UnityEngine.UI;

public class EditorBin : MonoBehaviour
{
    [SerializeField] private Button bin;
    [SerializeField] private Transform LogicGates;

    void Start()
    {
        bin.onClick.AddListener(OnClick);
    }
    void Update()
    {
        // Only compute when the mouse button is up (i.e. no logic gate is being dragged)
        if (!Input.GetMouseButtonUp(0))
            return;

        // Get any collider that is touching the bin
        Collider2D other = Physics2D.OverlapCircle(transform.position, 0.75f);

        // Make sure there is a collider
        if (other == null)
            return;

        // Firstly, destroy any wires connected to the logic gate
        other.transform.parent.GetComponent<LogicGateComponent>().DestroyWires();
        // Then, destroy the logic gate itself
        Destroy(other.transform.parent.gameObject);
    }

    private void OnClick()
    {
        for (int i = 0; i < LogicGates.childCount; i++)
        {
            LogicGates.GetChild(i).GetComponent<LogicGateComponent>().DestroyWires();
            Destroy(LogicGates.GetChild(i).gameObject);
        }
    }
}
