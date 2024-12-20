// DA - Main game window script, inits and all that.

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector3 = OpenTK.Mathematics.Vector3;

public class Game : GameWindow
{
    public static Game init;


    private Camera _camera;
    private Renderer _renderer;
    private World _world;
    private InputHandler _inputHandler;
    private Player _player;

    public const float RAY_STEP = 0.1f;
    public const float MAX_DISTANCE = 5f;

    public Game(NativeWindowSettings settings) : base(GameWindowSettings.Default, settings)
    {
        _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
        _renderer = new Renderer();
        _world = new World();
        _inputHandler = new InputHandler(this, _camera, _world);

        

        init = this;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(Color4.CornflowerBlue);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        //GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);

        _renderer.Init();
        _world.Generate();

        _player = new Player(new Vector3(20, World.CHUNK_HEIGHT + 5, 20));

        CursorState = CursorState.Grabbed;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


        _renderer.Render(_world, _camera);

        _player.Render();

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _inputHandler.HandleInput(args, KeyboardState, MouseState);

        float deltaTime = (float)args.Time;

        _player.Update(KeyboardState, (float)args.Time, _camera);


        if (KeyboardState.IsKeyDown(Keys.Escape))
            Close();
    }

    

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        _inputHandler.HandleMouseMove(e);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        _camera.AspectRatio = Size.X / (float)Size.Y;
    }

    protected override void OnUnload()
    {
        base.OnUnload();
    }


}
