using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing.Imaging;
using System.Drawing;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

public enum Face
{
    Front = 0,
    Back = 1,
    Left = 2,
    Right = 3,
    Top = 4,
    Bottom = 5
}

class Renderer
{
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private Texture _textureAtlas;
    //private int _shaderProgram;

    private Shader _shader;

    private readonly float[] _vertices = {
    // positions          // texture coords
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};

    private readonly uint[] _indices = {
        0,  1,  2,  2,  3,  0,  // Front face
        4,  5,  6,  6,  7,  4,  // Back face
        8,  9,  10, 10, 11, 8,  // Top face
        12, 13, 14, 14, 15, 12, // Bottom face
        16, 17, 18, 18, 19, 16, // Right face
        20, 21, 22, 22, 23, 20  // Left face
    };


    //private Dictionary<(BlockType, Face), Vector2[]> _textureMappings;

    public void Init()
    {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        _shader = new Shader("../../../shader.vert", "../../../shader.frag");
        _textureAtlas = Texture.LoadFromFile("../../../atlas.png");

    }

    public void Render(World world, Camera camera)
    {
        _shader.Use();

        _shader.SetInt("texture0", 0);
        _shader.SetMatrix4("view", camera.GetViewMatrix());
        _shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _shader.SetFloat("numRows", 2); // Assuming the texture atlas has 2x2 textures


        foreach (Chunk chunk in world.Chunks)
        {
            RenderChunk(chunk);
        }
    }

    private void RenderChunk(Chunk chunk)
    {
        for (int x = 0; x < World.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < World.CHUNK_HEIGHT; y++)
            {
                for (int z = 0; z < World.CHUNK_SIZE; z++)
                {
                    if (chunk.Blocks[x, y, z].type != BlockType.Air)
                    {

                        Block currBlock = chunk.Blocks[x, y, z];

                        if (IsFaceVisible(chunk, x, y, z, Face.Front))
                        {
                            DrawFace(chunk.Position + new Vector3(x, y, z), Face.Front, currBlock);
                        }
                        if (IsFaceVisible(chunk, x, y, z, Face.Back))
                        {
                            DrawFace(chunk.Position + new Vector3(x, y, z), Face.Back, currBlock);
                        }
                        if (IsFaceVisible(chunk, x, y, z, Face.Top))
                        {
                            DrawFace(chunk.Position + new Vector3(x, y, z), Face.Top, currBlock);
                        }
                        if (IsFaceVisible(chunk, x, y, z, Face.Bottom))
                        {
                            DrawFace(chunk.Position + new Vector3(x, y, z), Face.Bottom, currBlock);
                        }
                        if (IsFaceVisible(chunk, x, y, z, Face.Right))
                        {
                            DrawFace(chunk.Position + new Vector3(x, y, z), Face.Right, currBlock);
                        }
                        if (IsFaceVisible(chunk, x, y, z, Face.Left))
                        {
                            DrawFace(chunk.Position + new Vector3(x, y, z), Face.Left, currBlock);
                        }
                    }
                }
            }
        }
    }

    private bool IsFaceVisible(Chunk chunk, int x, int y, int z, Face face)
    {
        switch (face)
        {
            case Face.Front:
                return IsBlockAir(GetBlock(chunk, x, y, z + 1));
            case Face.Back:
                return IsBlockAir(GetBlock(chunk, x, y, z - 1));
            case Face.Top:
                return IsBlockAir(GetBlock(chunk, x, y + 1, z));
            case Face.Bottom:
                return IsBlockAir(GetBlock(chunk, x, y - 1, z));
            case Face.Right:
                return IsBlockAir(GetBlock(chunk, x + 1, y, z));
            case Face.Left:
                return IsBlockAir(GetBlock(chunk, x - 1, y, z));
            default:
                return false;
        }
    }

    private Block GetBlock(Chunk chunk, int x, int y, int z)
    {
        // Check if the coordinates are within bounds
        if (x < 0 || x >= World.CHUNK_SIZE ||
            y < 0 || y >= World.CHUNK_HEIGHT ||
            z < 0 || z >= World.CHUNK_SIZE)
        {
            // Return air block if out of bounds
            return new Block { type = BlockType.Air };
        }

        // Return the actual block from the chunk
        return chunk.Blocks[x, y, z];
    }

    private bool IsBlockAir(Block block)
    {
        return block.type == BlockType.Air;
    }

    private void DrawFace(Vector3 position, Face face, Block blockIn)
    {
        Matrix4 model = Matrix4.CreateTranslation(position);
        _shader.SetMatrix4("model", model);

        if(blockIn.type == BlockType.Stone)
        {
            _shader.SetVector2("texOffset", new Vector2(0.5f, 0.5f));
        }
        else if(blockIn.type == BlockType.Dirt)
        {
            _shader.SetVector2("texOffset", new Vector2(0, 0));
        }

        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

}
