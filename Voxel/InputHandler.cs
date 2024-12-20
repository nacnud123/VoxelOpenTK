using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

public class InputHandler
{
    private Game _game;
    private Camera _camera;
    private World _world;
    private bool _firstMove = true;
    private Vector2 _lastPos;
    private readonly float _sensitivity = 0.2f;
    private readonly float _cameraSpeed = 1.5f;

    private bool _wireframeMode = false;

    public InputHandler(Game game, Camera camera, World world)
    {
        _game = game;
        _camera = camera;
        _world = world;
    }

    public void HandleInput(FrameEventArgs args, KeyboardState keyboard, MouseState mouse)
    {
        if (keyboard.IsKeyPressed(Keys.Escape))
            _game.Close();

        if (mouse.IsButtonPressed(MouseButton.Left))
            RemoveBlock(mouse);
        if (mouse.IsButtonPressed(MouseButton.Right))
            AddBlock(mouse);

        if (keyboard.IsKeyPressed(Keys.N))
        {
            _wireframeMode = !_wireframeMode;

            if(_wireframeMode)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }

    public void HandleMouseMove(MouseMoveEventArgs e)
    {
        if (_firstMove)
        {
            _lastPos = new Vector2(e.X, e.Y);
            _firstMove = false;
        }

        float xOffset = e.X - _lastPos.X;
        float yOffest = _lastPos.Y - e.Y;
        _lastPos = new Vector2(e.X, e.Y);

        xOffset *= _sensitivity;
        yOffest *= _sensitivity;

        _camera.Yaw += xOffset;
        _camera.Pitch += yOffest;

        _camera.Pitch = MathHelper.Clamp(_camera.Pitch, -89f, 89f);

        Vector3 direction;
        direction.X = MathF.Cos(MathHelper.DegreesToRadians(_camera.Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_camera.Pitch));
        direction.Y = MathF.Sin(MathHelper.DegreesToRadians(_camera.Pitch));
        direction.Z = MathF.Sin(MathHelper.DegreesToRadians(_camera.Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_camera.Pitch));

        _camera.Front = Vector3.Normalize(direction);
    }

    private void AddBlock(MouseState mouse)
    {
        Vector3 blockPos = Camera.init.getTargetBlock(false);
        if (blockPos != Vector3.Zero)
        {
            _world.AddBlock(blockPos);
        }
    }

    private void RemoveBlock(MouseState mouse)
    {
        Vector3 blockPos = Camera.init.getTargetBlock(true);
        if (blockPos != Vector3.Zero)
        {
            _world.RemoveBlock(blockPos);
        }
    }
}
