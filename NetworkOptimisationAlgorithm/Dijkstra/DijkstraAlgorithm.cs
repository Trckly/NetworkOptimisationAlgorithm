using System.Runtime.InteropServices.JavaScript;
using System.Collections.Generic;

namespace NetworkOptimisationAlgorithm.Dijkstra;

public class DijkstraAlgorithm
{
    private int [,] _weightMatrix;

    private Dictionary<int, bool> _markedNodes;
    private List<int> _weightArray;
    private List<int> _tracebackArray;

    public DijkstraAlgorithm(int[,] weightMatrix)
    {
        var nodesCount = weightMatrix.GetLength(0);
        
        _markedNodes = new Dictionary<int, bool>();
        _weightArray = new List<int>();
        _tracebackArray = new List<int>();
        _weightMatrix = new int[nodesCount, nodesCount];
        
        // Here negative values are being changed into max values and written into _weightMatrix
        // Also all extra arrays are being initialized with default values
        for (var i = 0; i < nodesCount; i++)
        {
            _weightArray.Add(int.MaxValue);
            _tracebackArray.Add(-1);
            
            for (var j = 0; j < nodesCount; j++)
            {
                _weightMatrix[i, j] = weightMatrix[i, j] < 0 ? int.MaxValue : weightMatrix[i, j];
            }
        }
    }

    public void Solve()
    {
        
    }

    private void FillPathTable()
    {
        if (_markedNodes.Count == 0)
        {
            _markedNodes.Add(0, false);
            _weightArray[0] = 0;
            _tracebackArray[0] = -1;
        }
    }

    private void CalculatePath()
    {
        // Get min path value of unmarked elements
        var min = int.MaxValue;
        var minIndex = -1;
        for (var i = 0; i < _weightArray.Count; i++)
        {
            if (_markedNodes.TryGetValue(i, out var isTraversed) && !isTraversed)
            {
                if (min <= _weightArray[i]) continue;
                min = _weightArray[i];
                minIndex = i;
            }
        }
        
        for (var i = 0; i < _weightArray.Count; i++)
        {
            if (_markedNodes.TryGetValue(i, out var isTraversed) && !isTraversed && i != minIndex)
            {
                var weight = Dijkstra(minIndex, i);
                if (weight != _weightArray[i])
                {
                    _weightArray[i] = weight;
                    _tracebackArray[i] = minIndex;
                }
            }
            else
            {
                
            }
        }
    }

    private int Dijkstra(int minIndex, int targetIndex)
    {
        return Math.Min(_weightArray[targetIndex], _weightArray[minIndex] + _weightMatrix[minIndex, targetIndex]);
    }
}