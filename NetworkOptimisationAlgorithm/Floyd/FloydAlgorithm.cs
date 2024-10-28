namespace NetworkOptimisationAlgorithm.Floyd;

public class FloydAlgorithm
{
    private int [,] _shortestPathMatrix;
    private int[,] _routeMatrix;
    
    public int [,] ShortestPathMatrix => _shortestPathMatrix;
    public int[,] RouteMatrix => _routeMatrix;
    
    public FloydAlgorithm(int[,] weightMatrix)
    {
        var nodesCount = weightMatrix.GetLength(0);
        
        _shortestPathMatrix = new int[nodesCount, nodesCount];
        _routeMatrix = new int[nodesCount, nodesCount];
        
        // Here negative values are being changed into max values and written into _shortestPathMatrix
        // _routeMatrix is being populated with column indexes. Same node path represents as -1 for convenience.
        for (var i = 0; i < nodesCount; i++)
        {
            for (var j = 0; j < nodesCount; j++)
            {
                _shortestPathMatrix[i, j] = weightMatrix[i, j] < 0 ? int.MaxValue : weightMatrix[i, j];
                _routeMatrix[i, j] = i != j ? j : -1;
            }
        }
    }

    public void Solve()
    {
        FloydLogger.OutFloydMatrices(_shortestPathMatrix, _routeMatrix, 0);
    }

    private void Floyd(int nodeIndex)
    {
        
    }
}