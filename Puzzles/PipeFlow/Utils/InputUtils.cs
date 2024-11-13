using Godot;

public static class InputUtils
{
    public static bool IsLeftMouseButtonPressed(InputEvent @event)
    {
        return @event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed;
    }
}
