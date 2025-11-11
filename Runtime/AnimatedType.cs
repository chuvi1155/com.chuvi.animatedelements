
[System.Flags]
public enum AnimatedType
{
    Color = 1 << 0,
    Transform = 1 << 1,
    Rotate = 1 << 2,
    Scale = 1 << 3,
    FrameAnimation = 1 << 4
}
[System.Flags]
public enum ColorAnimation
{
    Image = 1 << 0,
    Effect = 1 << 1,
    CanvasGroup = 1 << 2,
}
public enum FromCurrentPosition
{
    None,
    Local,
    World,
    Local2D
}
public enum FromCurrentSize
{
    None,
    Use
}
