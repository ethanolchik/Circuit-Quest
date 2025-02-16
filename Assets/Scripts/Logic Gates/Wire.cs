using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    /// <summary>
    /// The input (the state is taken from here)
    /// </summary>
    private LogicGateComponent input;

    /// <summary>
    /// The output (the state is transferred to here)
    /// </summary>
    private LogicGateComponent output;

    /// <summary>
    /// Used to draw the wire
    /// </summary>
    private LineRenderer lineRenderer;

    /// <summary>
    /// The state of the wire (either 0 or 1)
    /// </summary>
    private int state = 0;

    /// <summary>
    /// Used to prevent the wire from updating when it is unecessary to.
    /// </summary>
    private bool shouldUpdate = true;

    /// <summary>
    /// The index of the input which the wire is connected to
    /// </summary>
    private int inputIndex;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 10;
    }

    void Update()
    {
        // If the input and output are set and the wire needs to be updated
        if (input != null && output != null && shouldUpdate)
        {
            // Calculate the points on the bezier curve
            Vector3[] points = CalculateCurve();

            // Set the positions of the line renderer
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);

            // Set the color of the wire
            if (state == 1)
            {
                lineRenderer.startColor = Color.blue;
            }
            else
            {
                lineRenderer.startColor = Color.black;
            }
            lineRenderer.endColor = lineRenderer.startColor;

            // The wire has been updated, shouldUpdate can be reset.
            shouldUpdate = false;
        }
    }

    /// <summary>
    /// Returns a list of points on a bezier curve using the positions of two different connections.
    /// </summary>
    /// <returns></returns>
    Vector3[] CalculateCurve()
    {
        if (input == null || output == null)
            return new Vector3[0];

        Vector3 P0 = output.GetOutputPosition();
        Vector3 P3 = input.GetInputPosition(inputIndex);

        float x = (P0.x + P3.x) / 2;
        Vector3 P1 = new Vector3(x, P0.y, 0);
        Vector3 P2 = new Vector3(x, P3.y, 0);

        List<Vector3> points = new List<Vector3>();
        for (float t = 0; t <= 1; t += 0.01f)
        {
            points.Add(CalculateBezierPoint(P0, P1, P2, P3, t));
        }

        return points.ToArray();
    }

    /// <summary>
    /// Calculates a point on a Bezier Curve.
    /// </summary>
    /// <returns></returns>
    Vector3 CalculateBezierPoint(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * P0;
        point += 3 * uu * t * P1;
        point += 3 * u * tt * P2;
        point += ttt * P3;

        return point;
    }

    /// <summary>
    /// Sets the state of the wire
    /// </summary>
    /// <param name="newState">The new state of the wire, either 0 or 1</param>
    public void SetState(int newState)
    {
        // Make sure that we are not setting the state to anything other than 0 or 1
        if (state == 0 || state == 1)
        {
            state = newState;
            shouldUpdate = true;
        }
    }

    /// <summary>
    /// Returns the current state of the wire
    /// </summary>
    /// <returns></returns>
    public int GetState()
    {
        return state;
    }

    /// <summary>
    /// Returns the current input gate of the wire
    /// </summary>
    /// <returns></returns>
    public LogicGateComponent GetInput()
    {
        return input;
    }

    /// <summary>
    /// Returns the current output gate of the wire
    /// </summary>
    /// <returns></returns>
    public LogicGateComponent GetOutput()
    {
        return output;
    }

    /// <summary>
    /// Set the input gate
    /// </summary>
    /// <param name="input">The logic gate which the input of the wire should be set to</param>
    /// <param name="inputIndex">The index of the input which the wire is connected to</param>
    public void SetInput(LogicGateComponent input, int inputIndex)
    {
        this.input = input;
        this.inputIndex = inputIndex;
    }

    /// <summary>
    /// Set the output gate
    /// </summary>
    /// <param name="output">The logic gate which the output of the wire should be set to</param>
    public void SetOutput(LogicGateComponent output)
    {
        this.output = output;
    }

    /// <summary>
    /// Get the index of the input which the wire is connected to
    /// </summary>
    /// <returns></returns>
    public int GetInputIndex()
    {
        return inputIndex;
    }

    public void SetShouldUpdate(bool a)
    {
        shouldUpdate = a;
    }
}
