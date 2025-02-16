using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Serializable]
public class TraceTable
{
    /// <summary>
    /// Each element is an int[]: [in0, in1, ..., in(n-1), output].
    /// </summary>
    private List<int[]> sets;

    public TraceTable()
    {
        sets = new List<int[]>();
    }

    public void Test()
    {
        int[] l = new int[] {0, 0, 0, 0};
        var y = PermuteBits(3, l, 3);
    }

    /// <summary>
    /// Adds a row of data to the trace table, where the last element is the output
    /// and the rest are input bits.
    /// Example: If n=2 inputs, array length should be 3: [in0, in1, out].
    /// </summary>
    public void AddSet(int[] set)
    {
        if (set.Length > 0)
        {
            sets.Add(set);
        }
    }

    /// <summary>
    /// Returns all rows in the table.
    /// </summary>
    public List<int[]> GetSets()
    {
        return sets;
    }

    /// <summary>
    /// Populates the trace table by simulating all combinations of inputs for
    /// the given logic gates (initialGates) and final output (lastGate).
    /// </summary>
    public void Populate(List<LogicGateComponent> initialGates, LogicGateComponent lastGate)
    {
        // Save initial states
        Dictionary<LogicGateComponent, int> initialStates = new Dictionary<LogicGateComponent, int>();
        foreach (var gate in initialGates)
        {
            initialStates[gate] = gate.GetState();
        }

        try
        {
            initialStates[lastGate] = lastGate.GetState();
        } catch
        {
            Debug.Log("The circuit is not complete! Please make sure all logic gates have been connected, including inputs and outputs.");
            return;
        }

        int numInputs = initialGates.Count;
        int combinations = 1 << numInputs; // 2^numInputs

        for (int i = 0; i < combinations; i++)
        {
            // Assign each input gate's state based on the bits of i
            for (int j = 0; j < numInputs; j++)
            {
                int bit = ((i >> j) & 1); // 0 or 1
                initialGates[j].SetState(bit);
            }

            // Compute final output
            int output = Simulate(lastGate);

            // Build the row: [in0, in1, ..., out]
            int[] row = new int[numInputs + 1];
            for (int k = 0; k < numInputs; k++)
            {
                row[k] = initialGates[k].GetState();
            }
            row[numInputs] = output;
            AddSet(row);

            // Reset states
            foreach (var gate in initialGates)
            {
                gate.SetState(initialStates[gate]);
            }
            lastGate.SetState(initialStates[lastGate]);
        }
    }

    /// <summary>
    /// Recursively simulates the logic circuit to compute the output of 'gate'.
    /// </summary>
    private int Simulate(LogicGateComponent gate)
    {
        if (gate == null) return 0;

        var inputs = gate.GetInputs();
        int input1 = (inputs.Length > 0 && inputs[0] != null)
            ? Simulate(inputs[0])
            : gate.GetState();

        int input2 = (inputs.Length > 1 && inputs[1] != null)
            ? Simulate(inputs[1])
            : gate.GetState();

        return gate.ComputeOutput(input1, input2);
    }

    /// <summary>
    /// Checks if this trace table is functionally equivalent to 'other' trace table,
    /// allowing any permutation of the input columns.
    /// </summary>
    public bool Equals(TraceTable other)
    {
        if (other == null) return false;

        // Both must have the same number of rows (2^n) to be comparable
        if (this.sets.Count != other.sets.Count) return false;

        // Determine the input count from the first row (last element is output)
        int inputCount = GetInputCount();
        int otherInputCount = other.GetInputCount();
        if (inputCount != otherInputCount) return false;

        // Build a map from "input combination" -> "output" for each table
        Dictionary<int, int> thisMap = BuildInputOutputMap();
        Dictionary<int, int> otherMap = other.BuildInputOutputMap();

        // Try all permutations of [0..inputCount-1]
        foreach (var permutation in GeneratePermutations(inputCount))
        {
            if (CheckPermutationMatch(thisMap, otherMap, permutation, inputCount))
            {
                return true; // Found a permutation that matches exactly
            }
        }

        return false; // No permutation matched
    }

    /// <summary>
    /// Returns the number of inputs, assuming each row is [in0, in1, ..., out].
    /// If no rows, returns 0.
    /// </summary>
    private int GetInputCount()
    {
        if (sets.Count == 0) return 0;
        // Each row has (inputCount + 1) elements => last is output
        return sets[0].Length - 1;
    }

    /// <summary>
    /// Builds a mapping of input combinations (represented as integer bit patterns) to their corresponding output values.
    /// This method is used to simplify comparing trace tables (solution to row-by-row comparisons)
    ///
    /// For example:
    /// If inputCount = 2 and row = [0, 1, 0], the first two bits numbers are the inputs and the last is the output.
    /// The inputs can be combined to form the binary number 01 which represents the integer 1
    /// The output can either be 0 or 1 so no binary representation is needed.
    /// As a result, the dictionary entry is {1: 0}.
    /// 
    /// This allows trace tables to be compared regardless of the order of input columns.
    /// </summary>
    private Dictionary<int, int> BuildInputOutputMap()
    {
        // Initialize a dictionary to store the mapping of input patterns to outputs.
        Dictionary<int, int> map = new Dictionary<int, int>();

        // Determine the number of input gates (columns excluding the output column).
        int inputCount = GetInputCount();

        // Iterate over each row in the trace table.
        foreach (var row in sets)
        {
            // Initialize the integer representation of the input bit pattern.
            int bitPattern = 0;

            // Convert the input part of the row (first inputCount elements) into a bit pattern.
            for (int i = 0; i < inputCount; i++)
            {
                // If the bit at position i is 1, set the corresponding bit in the integer bitPattern.
                if (row[i] == 1)
                {
                    bitPattern |= (1 << i); // Set the i-th bit in the integer.
                }
            }

            // The last element in the row is the output corresponding to the input combination.
            int output = row[inputCount];

            // Add the mapping of input bit pattern to output value in the dictionary.
            map[bitPattern] = output;
        }

        // Return the completed map of input patterns to outputs.
        return map;
    }


    /// <summary>
    /// Checks if 'thisMap' matches 'otherMap' under a given permutation of the input bits.
    /// inputCount is the total number of input bits.
    /// </summary>
    private bool CheckPermutationMatch(
        Dictionary<int, int> thisMap,
        Dictionary<int, int> otherMap,
        int[] permutation,
        int inputCount)
    {
        // Total number of input combinations (2^inputCount).
        int combinations = 1 << inputCount;

        // Iterate through all possible input combinations.
        for (int i = 0; i < combinations; i++)
        {
            // Fetch the output for the current input combination from `thisMap`.
            int thisOutput = thisMap[i];

            // Apply the permutation to reorder the bits of the current input combination.
            int permutedInput = PermuteBits(i, permutation, inputCount);

            // Fetch the output for the permuted input combination from `otherMap`.
            int otherOutput = otherMap[permutedInput];

            // If the outputs do not match, the trace tables are not equivalent under this permutation.
            if (thisOutput != otherOutput)
            {
                return false;
            }
        }
        
        return true;
    }

    /// <summary>
    /// Rearranges the bits in 'value' according to 'permutation'.
    /// If permutation = [2,0,1], then the bit in position 0 of 'value'
    /// goes to position 2 in 'p', bit in position 1 goes to position 0, etc.
    /// </summary>
    private int PermuteBits(int value, int[] permutation, int inputCount)
    {
        int p = 0;
        for (int bitPos = 0; bitPos < inputCount; bitPos++)
        {
            // Extract the bit at bitPos
            int bit = (value >> bitPos) & 1;
            // Place it at permutation[bitPos] in p
            int newPos = permutation[bitPos];
            p |= (bit << newPos);
        }
        return p;
    }

    /// <summary>
    /// Generates all permutations of [0..n-1].
    /// For n=2, yields [0,1] and [1,0]; for n=3, yields [0,1,2], [0,2,1], etc.
    /// </summary>
    private IEnumerable<int[]> GeneratePermutations(int n)
    {
        int[] arr = new int[n];
        for (int i = 0; i < n; i++) arr[i] = i;

        return PermuteArray(arr, 0);
    }

    /// <summary>
    /// Recursively generates all permutations of the input array.
    /// </summary>
    /// <param name="arr">The array to permute.</param>
    /// <param name="start">The starting index for the current recursion.</param>
    /// <returns>An enumerable of all permutations of the array.</returns>
    /// <returns>An enumerable of all permutations of the array.</returns>
    private IEnumerable<int[]> PermuteArray(int[] arr, int start)
    {
        // Base case: If the recursion has reached the last index, the current arrangement of the array is a valid permutation.
        if (start == arr.Length - 1)
        {
            // Clone the array to avoid modifying the original as recursion unwinds.
            yield return (int[])arr.Clone();
        }
        else
        {
            // Recursive case: Generate permutations for each possible arrangement of elements starting from the current index.
            for (int i = start; i < arr.Length; i++)
            {
                // Swap the element at the current index (`start`) with the element at index `i`.
                // This brings a new element into the current position for this branch of recursion.
                Swap(arr, start, i);

                // Recursively generate permutations for the remaining sub-array.
                foreach (var perm in PermuteArray(arr, start + 1))
                {
                    // Yield each permutation generated by the deeper recursion levels.
                    yield return perm;
                }

                // Swap the elements back to restore the original order of the array.
                // This ensures that other branches of recursion work with the original state of the array.
                Swap(arr, start, i);
            }
        }
    }


    /// <summary>
    /// A simple method to swap the positions of two elements in an array of integers
    /// </summary>
    private void Swap(int[] arr, int i, int j)
    {
        int temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }
}
