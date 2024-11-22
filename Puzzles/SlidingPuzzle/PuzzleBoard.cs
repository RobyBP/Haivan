using System;
using System.Collections.Generic;
using Godot;
using Haivan.Utils;

namespace Haivan.Puzzles.SlidingPuzzle
{
	public partial class PuzzleBoard : Control
	{
		private int puzzleSize = 3;
		private readonly Tile[,] grid = new Tile[3, 3];
		private int tileSize = 256;
		private Texture2D backgroundTexture;

		public override void _Ready()
		{
			backgroundTexture = GD.Load<Texture2D>("res://Puzzles/SlidingPuzzle/Assets/chest-puzzle.png");
			GeneratePuzzle();
			ShufflePuzzle();
		}

		private void GeneratePuzzle()
		{
			for (int i = 0; i < puzzleSize; i++)
			{
				for (int j = 0; j < puzzleSize; j++)
				{
					if (i == puzzleSize - 1 && j == puzzleSize - 1)
					{
						break;
					}

					Tile tile = new(j + i * puzzleSize)
					{
						Texture = new AtlasTexture()
						{
							Atlas = backgroundTexture,
							Region = new Rect2(j * 64, i * 64, 64, 64),
						},
						TextureFilter = TextureFilterEnum.Nearest,
						Size = new Vector2(tileSize, tileSize),
						Position = new Vector2(j * tileSize, i * tileSize),
						GridPosition = (i, j),
					};

					tile.GuiInput += (@event) =>
					{
						if (InputUtils.IsLeftMouseButtonPressed(@event))
						{
							(int row, int col) = GetEmptyTile();
							if (Math.Abs(row - tile.GridPosition.row) + Math.Abs(col - tile.GridPosition.col) == 1)
							{
								SwapTiles(tile.GridPosition.row, tile.GridPosition.col, row, col);
								GD.Print(IsComplete());
							}
						}
					};
					AddChild(tile);
					grid[i, j] = tile;
				}
			}
		}

		private void ShufflePuzzle()
		{
			// Perform a number of random moves
			int numberOfShuffles = 10;
			for (int i = 0; i < numberOfShuffles; i++)
			{
				(int x, int y) = GetEmptyTile();
				(int newX, int newY) = GetRandomNeighbor(x, y);
				SwapTiles(x, y, newX, newY);
			}
		}

		private (int, int col) GetEmptyTile()
		{
			for (int i = 0; i < puzzleSize; i++)
			{
				for (int j = 0; j < puzzleSize; j++)
				{
					if (grid[i, j] == null)
					{
						return (i, j);
					}
				}
			}
			return (-1, -1);
		}

		private (int x, int y) GetRandomNeighbor(int x, int y)
		{
			Random random = new();
			List<(int x, int y)> neighbors = new();
			if (x > 0)
				neighbors.Add((x - 1, y));
			if (x < puzzleSize - 1)
				neighbors.Add((x + 1, y));
			if (y > 0)
				neighbors.Add((x, y - 1));
			if (y < puzzleSize - 1)
				neighbors.Add((x, y + 1));
			return neighbors[random.Next(neighbors.Count)];
		}

		private void SwapTiles(int x1, int y1, int x2, int y2)
		{
			(grid[x2, y2], grid[x1, y1]) = (grid[x1, y1], grid[x2, y2]);
			if (grid[x1, y1] != null)
			{
				grid[x1, y1].Position = new Vector2(y1 * tileSize, x1 * tileSize);
				grid[x1, y1].GridPosition = (x1, y1);
			}
			if (grid[x2, y2] != null)
			{
				grid[x2, y2].Position = new Vector2(y2 * tileSize, x2 * tileSize);
				grid[x2, y2].GridPosition = (x2, y2);
			}
		}

		private bool IsComplete()
		{
			int value = 0;
			for (int i = 0; i < puzzleSize; i++)
			{
				for (int j = 0; j < puzzleSize; j++)
				{
					if (grid[i, j] == null)
					{
						break;
					}
					if (grid[i, j].index != value)
					{
						return false;
					}
					value++;
				}
			}
			return value == puzzleSize * puzzleSize - 1;
		}
		private partial class Tile : TextureRect
		{
			public readonly int index;
			public (int row, int col) GridPosition { get; set; }

			public Tile(int index)
			{
				this.index = index;
			}

			public override void _Ready()
			{
				Label label = new()
				{
					Text = index.ToString(),
				};

				label.AddThemeColorOverride("font_color", new Color(0, 0, 0));
				AddChild(label);
			}
		}
	}
}
