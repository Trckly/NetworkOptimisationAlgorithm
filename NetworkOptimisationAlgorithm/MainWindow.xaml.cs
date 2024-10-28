﻿using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetworkOptimisationAlgorithm.Dijkstra;
using NetworkOptimisationAlgorithm.Floyd;

namespace NetworkOptimisationAlgorithm;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

public partial class MainWindow : Window
{
    private int _nodeCount;

    public int NodeCount
    {
        get => _nodeCount;
        set => _nodeCount = value > 2 ? value : 2;
    }

    public MainWindow()
    {
        NodeCount = 9;
        
        InitializeComponent();
        GenerateTransportGrid();
        PreDefineGraph();
    }
    
    private void GenerateTransportGrid()
    {
        TransportGrid ??= new Grid();
        TransportGrid.RowDefinitions.Clear();
        TransportGrid.ColumnDefinitions.Clear();
        TransportGrid.Children.Clear();
    
        // Create Row Definitions
        for (var i = 0; i <= NodeCount; i++)
        {
            TransportGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            TransportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }
    
        // Create headers (top row and left column)
        for (var i = 1; i <= NodeCount; i++)
        {
            var rowHeaders = new TextBlock
            {
                Text = ((char)('A' + i - 1)).ToString(),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(rowHeaders, i);
            Grid.SetColumn(rowHeaders, 0);
            TransportGrid.Children.Add(rowHeaders);
            
            var columnHeaders = new TextBlock
            {
                Text = ((char)('A' + i - 1)).ToString(),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(columnHeaders, 0);
            Grid.SetColumn(columnHeaders, i);
            TransportGrid.Children.Add(columnHeaders);
        }
    
        // Fill grid cells with TextBoxes for input (except the first row/column for headers)
        for (var i = 1; i <= NodeCount; i++)
        {
            for (var j = 1; j <= NodeCount; j++)
            {
                var cell = new TextBox
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    MinWidth = 30,
                    MinHeight = 30,
                };
    
                // Attach the validation event handler to each TextBox
                cell.PreviewTextInput += Cell_PreviewTextInput;
    
                Grid.SetRow(cell, i);
                Grid.SetColumn(cell, j);
                TransportGrid.Children.Add(cell);
            }
        }
    }
    
    private void PreDefineGraph()
    {
        int [,]weightMatrix =
        {
            {0, 7, 9, 4, -1, -1, -1, -1, -1},
            {7, 0, -1, 8, 1, -1, -1, 6, -1},
            {9, -1, 0, 5, -1, 8, 4, -1, -1},
            {4, 8, 5, 0, -1, -1, 9, 5, -1},
            {-1, 1, -1, -1, 0, -1, -1, 2, -1},
            {-1, -1, 8, -1, -1, 0, -1, -1, 3},
            {-1, -1, 4, 9, -1, -1, 0, 7, 6},
            {-1, 6, -1, 5, 2, -1, 7, 0, 8},
            {-1, -1, -1, -1, -1, 3, 6, 8, 0},
        };
        
        // int [,]weightMatrix =
        // {
        //     {0, 5, -1, 25, -1, -1, -1, -1, -1},
        //     {-1, 0, 1, -1, -1, -1, -1, -1, -1},
        //     {-1, -1, 0, -1, -1, -1, -1, -1, 1},
        //     {-1, 19, -1, 0, -1, -1, -1, -1, -1},
        //     {-1, -1, 7, -1, 0, -1, 1, -1, -1},
        //     {-1, -1, -1, -1, -1, 0, 3, -1, -1},
        //     {-1, -1, 4, -1, -1, -1, 0, 2, -1},
        //     {-1, -1, -1, 1, -1, -1, -1, 0, -1},
        //     {-1, -1, -1, -1, 1, 8, -1, -1, 0},
        // };
        
        for (var i = 1; i <= NodeCount; i++)
        {
            for (var j = 1; j <= NodeCount; j++)
            {
                ((TextBox)GetGridElement(TransportGrid, i, j)).Text = weightMatrix[i - 1, j - 1].ToString();
            }
        }
    }
    
    private UIElement GetGridElement(Grid grid, int row, int column)
    {
        foreach (UIElement element in grid.Children)
        {
            if (Grid.GetRow(element) == row && Grid.GetColumn(element) == column)
                return element;
        }

        return null;
    }
    
    // Event handler for TextBox validation: allows only numeric input
    private void Cell_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^-?0-9]+"); // Only positive and negative digits are allowed
        e.Handled = regex.IsMatch(e.Text);
    }

    private void PlusButton_OnClick(object sender, RoutedEventArgs e)
    {
        NodeCount++;
        GenerateTransportGrid();
    }

    private void SolveButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Read weight matrix from the grid
        var weightMatrix = new int[NodeCount, NodeCount];
        for (var i = 1; i <= NodeCount; i++)
        {
            for (var j = 1; j <= NodeCount; j++)
            {
                var cell = (TextBox)TransportGrid.Children
                    .Cast<UIElement>()
                    .FirstOrDefault(element => Grid.GetRow(element) == i && Grid.GetColumn(element) == j)!;
                weightMatrix[i - 1, j - 1] = int.Parse(cell?.Text ?? string.Empty);
            }
        }

        if ((string)AlgorithmComboBox.SelectedValue == "Dijkstra")
        {
            var dijkstra = new DijkstraAlgorithm(weightMatrix);
            
            dijkstra.Solve();
            
            DijkstraTableBuilder.BuildFinalTable(dijkstra.WeightArray, dijkstra.TracebackArray, 
                DynamicGridContainer);
        }
        else
        {
            var floyd = new FloydAlgorithm(weightMatrix);
            
            floyd.Solve();
            
            FloydTableBuilder.BuildFinalTable(floyd.ShortestPathMatrix, floyd.RouteMatrix, DynamicGridContainer);
        }
    }

    private void MinusButton_OnClick(object sender, RoutedEventArgs e)
    {
        NodeCount--;
        GenerateTransportGrid();
    }
}