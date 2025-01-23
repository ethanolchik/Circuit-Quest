using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IDragHandler
{
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;

        var gate = transform.parent.GetComponent<LogicGateComponent>();

        // Update the wire positions
        foreach (var wire in GateEditor.Instance.wires)
        {
            if (wire.GetInput() == gate || wire.GetOutput() == gate)
            {
                wire.SetShouldUpdate(true);
            }
        }
    }
}