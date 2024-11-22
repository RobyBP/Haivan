using System;
using System.Collections.Generic;

namespace Haivan.Puzzles.SlidingPuzzle
{
    partial class SlidingPuzzle
    {
        private int[,] grid;
        private int size;
        private (int x, int y) emptyTile;

        public SlidingPuzzle(int size)
        {
            this.size = size;
            grid = new int[size, size];
            InitializeGrid();
            Shuffle();
        }

        // Initialize the grid in a solved state
        private void InitializeGrid()
        {
            int value = 1;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    grid[i, j] = value++;
                }
            }
            grid[size - 1, size - 1] = 0; // Empty tile
            emptyTile = (size - 1, size - 1);
        }

        // Shuffle the puzzle
        private void Shuffle()
        {
            Random random = new();
            int moves = size * size * 10; // Perform a number of random moves
            for (int i = 0; i < moves; i++)
            {
                List<(int x, int y)> neighbors = GetNeighbors(emptyTile.x, emptyTile.y);
                (int newX, int newY) = neighbors[random.Next(neighbors.Count)];
                SwapTiles(emptyTile.x, emptyTile.y, newX, newY);
                emptyTile = (newX, newY);
            }
        }

        // Get neighbors of the empty tile
        private List<(int x, int y)> GetNeighbors(int x, int y)
        {
            List<(int x, int y)> neighbors = new List<(int x, int y)>();
            if (x > 0)
                neighbors.Add((x - 1, y));
            if (x < size - 1)
                neighbors.Add((x + 1, y));
            if (y > 0)
                neighbors.Add((x, y - 1));
            if (y < size - 1)
                neighbors.Add((x, y + 1));
            return neighbors;
        }

        // Swap two tiles
        private void SwapTiles(int x1, int y1, int x2, int y2)
        {
            int temp = grid[x1, y1];
            grid[x1, y1] = grid[x2, y2];
            grid[x2, y2] = temp;
        }

        // Check if the puzzle is solved
        public bool IsSolved()
        {
            int value = 1;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == size - 1 && j == size - 1)
                        return grid[i, j] == 0;
                    if (grid[i, j] != value++)
                        return false;
                }
            }
            return true;
        }

        // Display the grid
        public void DisplayGrid()
        {

        }

        // Slide a tile into the empty space
        public bool SlideTile(int x, int y)
        {
            if (Math.Abs(emptyTile.x - x) + Math.Abs(emptyTile.y - y) == 1) // Ensure it's a valid move
            {
                SwapTiles(emptyTile.x, emptyTile.y, x, y);
                emptyTile = (x, y);
                return true;
            }
            return false;
        }
    }
}
