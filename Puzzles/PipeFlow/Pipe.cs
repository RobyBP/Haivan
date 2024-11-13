using System;
using Godot;

namespace Haivan.Puzzles.PipeFlow
{
    [Tool]
    public partial class Pipe : Control
    {
        [Export]
        private bool randomizeShapeOnReady = true;

        [Export]
        private bool randomizeRotationOnReady = true;

        private PipeShape _shape;
        [Export]
        private PipeShape Shape
        {
            get => _shape;
            set
            {
                _shape = value;
                if (IsInsideTree())
                {
                    DetermineTexture(_shape);
                    DetermineInitialConnectionsForShape();
                }
            }
        }

        private int _pipeRotation = 0;
        [Export(PropertyHint.Enum, "0:0,90:90,180:180,270:270")]
        private int PipeRotation
        {
            get => _pipeRotation;
            set
            {
                int delta = value - _pipeRotation;
                _pipeRotation = value;
                AdjustConnections(delta);
            }
        }

        public bool ConnectedTop { get; private set; }

        public bool ConnectedRight { get; private set; }

        public bool ConnectedBottom { get; private set; }

        public bool ConnectedLeft { get; private set; }

        private TextureRect TextureRect { get => GetNode<TextureRect>("PipeTexture"); }

        public override void _Ready()
        {
            if (randomizeShapeOnReady)
            {
                Shape = (PipeShape)new Random().Next(Enum.GetValues(typeof(PipeShape)).Length);
            }
            DetermineInitialConnectionsForShape();
            if (randomizeRotationOnReady)
            {
                PipeRotation = new Random().Next(4) * 90;
            }
            DetermineTexture(Shape);
        }

        public override void _Process(double delta)
        {
            // The Rotation field doesn't update inside the editor from the Ready method
            if (RotationDegrees != PipeRotation)
            {
                RotationDegrees = PipeRotation;
            }
        }

        public void RotateClockwise()
        {
            PipeRotation = (PipeRotation + 90) % 360;
        }

        public bool IsConnectedTo(Pipe other, Direction direction)
        {
            return direction switch
            {
                Direction.Top => ConnectedTop && other.ConnectedBottom,
                Direction.Right => ConnectedRight && other.ConnectedLeft,
                Direction.Bottom => ConnectedBottom && other.ConnectedTop,
                Direction.Left => ConnectedLeft && other.ConnectedRight,
                _ => false,
            };
        }

        private void DetermineInitialConnectionsForShape()
        {
            switch (_shape)
            {
                case PipeShape.Straight:
                    ConnectedLeft = false;
                    ConnectedRight = false;
                    ConnectedTop = true;
                    ConnectedBottom = true;
                    break;
                case PipeShape.Elbow:
                    ConnectedRight = true;
                    ConnectedBottom = true;
                    ConnectedTop = false;
                    ConnectedLeft = false;
                    break;
                case PipeShape.T:
                    ConnectedTop = false;
                    ConnectedRight = true;
                    ConnectedLeft = true;
                    ConnectedBottom = true;
                    break;
                case PipeShape.Cross:
                    ConnectedTop = true;
                    ConnectedRight = true;
                    ConnectedBottom = true;
                    ConnectedLeft = true;
                    break;
            }
            AdjustConnections(PipeRotation);
        }

        private void AdjustConnections(int rotationDelta)
        {
            if (rotationDelta == 0)
            {
                return;
            }
            else if (rotationDelta < 0)
            {
                rotationDelta += 360;
            }
            for (int i = 0; i < rotationDelta / 90; i++)
            {
                bool temp = ConnectedTop;
                ConnectedTop = ConnectedLeft;
                ConnectedLeft = ConnectedBottom;
                ConnectedBottom = ConnectedRight;
                ConnectedRight = temp;
            }
        }

        private void DetermineTexture(PipeShape shape)
        {
            switch (shape)
            {
                case PipeShape.Straight:
                    TextureRect.Texture = GD.Load<Texture2D>("res://Puzzles/PipeFlow/Assets/straight-pipe.png");
                    break;
                case PipeShape.Elbow:
                    TextureRect.Texture = GD.Load<Texture2D>("res://Puzzles/PipeFlow/Assets/elbow-pipe.png");
                    break;
                case PipeShape.T:
                    TextureRect.Texture = GD.Load<Texture2D>("res://Puzzles/PipeFlow/Assets/t-pipe.png");
                    break;
                case PipeShape.Cross:
                    TextureRect.Texture = GD.Load<Texture2D>("res://Puzzles/PipeFlow/Assets/cross-pipe.png");
                    break;
            }
        }

        public enum Direction
        {
            Top,
            Right,
            Bottom,
            Left
        }
    }
}

