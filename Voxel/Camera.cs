using OpenTK.Mathematics;
using System;

public class Camera
{

    public static Camera init;

    private Vector3 front;

    public Vector3 Position { get; set; }
    public Vector3 Front { get => front; set => front = value; }
    public Vector3 Up { get; private set; }
    public float AspectRatio { get; set; }

    public float Yaw { get; set; } = -90f;
    public float Pitch { get; set; }

    public Camera(Vector3 position, float aspectRatio)
    {
        init = this;
        Position = position;
        AspectRatio = aspectRatio;
        Up = Vector3.UnitY;
        UpdateVectors();
    }

    public void UpdateVectors()
    {
        front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        front = Vector3.Normalize(front);
    }

    public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position + front, Up);

    public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), AspectRatio, 0.1f, 100f);


    public Vector3 getTargetBlock(bool existingBlock)
    {
        Vector3 rayStart = Position;
        Vector3 rayDir = Vector3.Normalize(front);
        Vector3 lastPos = rayStart;

        for(float distance = 0; distance < Game.MAX_DISTANCE; distance += Game.RAY_STEP)
        {
            Vector3 checkPos = rayStart + rayDir * distance;
            Chunk targetChunk = World.init.GetChunkAtPosition(checkPos);

            if (targetChunk != null)
            {
                Vector3 localPos = checkPos - targetChunk.Position;
                int x = (int)Math.Floor(localPos.X + .5f);
                int y = (int)Math.Floor(localPos.Y + .5f);
                int z = (int)Math.Floor(localPos.Z + .5f);
                if (x >= 0 && x < World.CHUNK_SIZE && y >= 0 && y < World.CHUNK_HEIGHT && z >= 0 && z < World.CHUNK_SIZE)
                {
                    if (targetChunk.Blocks[x, y, z].type != BlockType.Air)
                    {
                        if (existingBlock)
                        {
                            return new Vector3(x, y, z) + targetChunk.Position;
                        }
                        else
                        {
                            Vector3 normal = checkPos - lastPos;
                            int nx = Math.Abs(normal.X) > Math.Abs(normal.Y) && Math.Abs(normal.X) > Math.Abs(normal.Z) ? Math.Sign(normal.X) : 0;
                            int ny = Math.Abs(normal.Y) > Math.Abs(normal.X) && Math.Abs(normal.Y) > Math.Abs(normal.Z) ? Math.Sign(normal.Y) : 0;
                            int nz = Math.Abs(normal.Z) > Math.Abs(normal.X) && Math.Abs(normal.Z) > Math.Abs(normal.Y) ? Math.Sign(normal.Z) : 0;
                            return new Vector3(x - nx, y - ny, z - nz) + targetChunk.Position;
                        }
                    }
                }
            }
            lastPos = checkPos;
        }
        return Vector3.Zero;
    }
}
