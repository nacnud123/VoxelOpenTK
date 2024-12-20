// DA. Holds all the functions needed for player movement.
// Known bugs - Sometimes the player can walk through blocks, but gravity works fine.

using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

public class Player
{
    private Vector3 position;
    private Vector3 velocity;
    private float speed = 5f;
    private float jumpStrength = 7f;
    private float gravity = -9.81f;
    private bool isGrounded = false;

    private const float PLAYER_WIDTH = .1f;
    private const float PLAYER_HEIGHT = 1.5f;


    public Player(Vector3 startPosition)
    {
        position = startPosition;
        velocity = Vector3.Zero;
    }

    public void Update(KeyboardState input, float deltaTime, Camera _camera)
    {
        Vector3 movement = Vector3.Zero;

        Vector3 forward = Vector3.Normalize(new Vector3(_camera.Front.X, 0, _camera.Front.Z));
        Vector3 right = Vector3.Normalize(Vector3.Cross(forward, _camera.Up));

        if (input.IsKeyDown(Keys.W)) movement += forward;
        if (input.IsKeyDown(Keys.S)) movement -= forward;
        if (input.IsKeyDown(Keys.A)) movement -= right;
        if (input.IsKeyDown(Keys.D)) movement += right;

        if (movement.LengthSquared > 1)
            movement.Normalize();

        movement *= speed * deltaTime;

        Move(movement);

        velocity.Y += gravity * deltaTime;
        Vector3 verticalMove = new Vector3(0, velocity.Y * deltaTime, 0);
        Move(verticalMove);


        isGrounded = IsBlockSolid(position - new Vector3(0, .5f, 0));

        if (isGrounded && input.IsKeyDown(Keys.Space))
        {
            Console.WriteLine("Jump!");
            velocity.Y = jumpStrength;
            isGrounded = false;
        }
        _camera.Position = position;
    }

    private void Move(Vector3 move)
    {
        Vector3 newPosition = position + move;
        if (!CheckCollision(new Vector3(newPosition.X, position.Y, position.Z)))
        {
            position.X = newPosition.X;
        }
        else
        {
            velocity.X = 0;
        }

        if (!CheckCollision(new Vector3(position.X, newPosition.Y, position.Z)))
        {
            position.Y = newPosition.Y;
        }
        else
        {
            velocity.Y = 0; 
            if (move.Y < 0) isGrounded = true;
        }

        if (!CheckCollision(new Vector3(position.X, position.Y, newPosition.Z)))
        {
            position.Z = newPosition.Z;
        }
        else
        {
            velocity.Z = 0;
        }
    }

    private bool CheckCollision(Vector3 pos)
    {
        Vector3 min = pos - new Vector3(PLAYER_WIDTH / 2, 0, PLAYER_WIDTH / 2);
        Vector3 max = pos + new Vector3(PLAYER_WIDTH / 2, PLAYER_HEIGHT, PLAYER_WIDTH / 2);

        for (float x = min.X; x <= max.X; x += PLAYER_WIDTH)
        {
            for (float y = min.Y; y <= max.Y; y += PLAYER_HEIGHT)
            {
                for (float z = min.Z; z <= max.Z; z += PLAYER_WIDTH)
                {
                    if (IsBlockSolid(new Vector3(x, y, z)))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool IsBlockSolid(Vector3 worldPosition)
    {
        Chunk chunk = World.init.GetChunkAtPosition(worldPosition);

        if (chunk == null) return false; // Treat out-of-bounds as solid

        Vector3 localPos = worldPosition - chunk.Position;
        int x = (int)MathF.Floor(localPos.X);
        int y = (int)MathF.Floor(localPos.Y);
        int z = (int)MathF.Floor(localPos.Z);

        // If outside current chunk, get the correct chunk
        if (x < 0 || x >= World.CHUNK_SIZE || z < 0 || z >= World.CHUNK_SIZE)
        {
            chunk = World.init.GetChunkAtPosition(worldPosition);
            if (chunk == null) return true; // Treat out-of-bounds as solid
            localPos = worldPosition - chunk.Position;
            x = (int)MathF.Floor(localPos.X);
            z = (int)MathF.Floor(localPos.Z);
        }

        if (y < 0 || y >= World.CHUNK_HEIGHT)
        {
            return y < 0; // Treat below world as solid, above world as air
        }

        return chunk.IsBlockSolid(new Vector3(x, y, z));
    }

    public void Render()
    {
        // Replace this with your voxel rendering logic
        Console.WriteLine($"Player Position: {position}");
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}