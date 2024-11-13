using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Haivan.Puzzles.PipeFlow
{
	public partial class PipePuzzle : Control
	{
		private Pipe[,] grid;
		private (int row, int col) start;
		private (int row, int col) end;

		public override void _Ready()
		{
			VBoxContainer startingPipeColumn = GetNode<VBoxContainer>("HBoxContainer/StartContainer");
			VBoxContainer endingPipeColumn = GetNode<VBoxContainer>("HBoxContainer/EndContainer");
			GridContainer puzzleGridLayout = GetNode<GridContainer>("HBoxContainer/GridContainer");
			grid = GetPuzzleGrid(puzzleGridLayout);
			int startPipeIndex = GetPipeIndex(startingPipeColumn);
			start = (startPipeIndex, -1);
			int endPipeIndex = GetPipeIndex(endingPipeColumn);
			end = (endPipeIndex, grid.GetLength(1));
			puzzleGridLayout.GetChildren()
			.Cast<Pipe>()
			.ToList()
			.ForEach(pipe =>
			{
				pipe.GuiInput += (@event) =>
				{
					if (InputUtils.IsLeftMouseButtonPressed(@event))
					{
						pipe.RotateClockwise();
						GD.Print(IsComplete());
					}
				};
			});
		}

		private int GetPipeIndex(VBoxContainer pipeColumn)
		{
			for (int i = 0; i < pipeColumn.GetChildCount(); i++)
			{
				if (pipeColumn.GetChild(i) is Pipe)
				{
					return i;
				}
			}
			return -1;
		}

		private static Pipe[,] GetPuzzleGrid(GridContainer gridContainer)
		{
			int columns = gridContainer.Columns;
			int totalChildren = gridContainer.GetChildCount();
			int rows = (int)Math.Ceiling((double)totalChildren / columns);

			Pipe[,] matrix = new Pipe[rows, columns];

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					int index = i * columns + j;
					if (index < totalChildren)
					{
						matrix[i, j] = gridContainer.GetChild(index) as Pipe;
					}
					else
					{
						throw new Exception("The Pipe Grid must be fully populated");
					}
				}
			}

			return matrix;
		}

		public void RotatePipe((int row, int col) position)
		{
			grid[position.row, position.col].RotateClockwise();
		}

		public bool IsComplete()
		{
			return BFS(start, end);
		}

		private bool BFS((int row, int col) start, (int row, int col) end)
		{
			Dictionary<(int, int), bool> visited = new();
			var queue = new Queue<(int row, int col)>();
			queue.Enqueue(start);
			visited[start] = true;

			while (queue.Count > 0)
			{
				var (currentRow, currentCol) = queue.Dequeue();
				foreach (var (nextRow, nextCol) in GetConnectedNeighbors(currentRow, currentCol))
				{
					if (!visited.ContainsKey((nextRow, nextCol)))
					{
						if (nextRow == end.row && nextCol == end.col - 1)
						{
							return grid[nextRow, nextCol].ConnectedRight;
						}
						else
						{
							visited[(nextRow, nextCol)] = true;
							queue.Enqueue((nextRow, nextCol));
						}
					}
				}
			}

			return false;
		}

		private IEnumerable<(int row, int col)> GetConnectedNeighbors(int row, int col)
		{
			var directions = new List<(int dRow, int dCol, Pipe.Direction direction)>
			{
				(-1, 0, Pipe.Direction.Top),
                (1, 0, Pipe.Direction.Bottom), 
                (0, -1, Pipe.Direction.Left), 
                (0, 1, Pipe.Direction.Right),
            };

			foreach (var (dRow, dCol, direction) in directions)
			{
				int newRow = row + dRow;
				int newCol = col + dCol;

				if ((row, col) == end && grid[row, col - 1].ConnectedRight)
				{
					yield return (end.row, end.col - 1);
				}

				else if (newRow >= 0 && newRow < grid.GetLength(0) && newCol >= 0 && newCol < grid.GetLength(1))
				{
					if ((row, col) == start && grid[newRow, newCol].ConnectedLeft)
					{
						yield return (start.row, start.col + 1);
					}
					else if ((row, col) != start && grid[row, col].IsConnectedTo(grid[newRow, newCol], direction))
					{
						yield return (newRow, newCol);
					}
				}
			}
		}
	}
}
