using OpenTK.Mathematics;
using System.Collections.Generic;


public class Chunk
{
    private Vector3 position;
    private Block[,,] blocks;

    public Vector3 Position { get => position; set => position = value; }
    public Block[,,] Blocks { get => blocks; set => blocks = value; }



    public Chunk(Vector3 pos)
    {
        blocks = new Block[World.CHUNK_SIZE, World.CHUNK_HEIGHT, World.CHUNK_SIZE];
        position = pos;
    }

    public bool IsBlockSolid(Vector3 position)
    {
        int x = (int)Math.Floor(position.X);
        int y = (int)Math.Floor(position.Y);
        int z = (int)Math.Floor(position.Z);

        // Ensure we're within the bounds of the chunk
        if (x < 0 || y < 0 || z < 0 || x >= World.CHUNK_SIZE || y >= World.CHUNK_HEIGHT || z >= World.CHUNK_SIZE)
            return false;

        // Check if the block at this position is solid
        return blocks[x, y, z].type != BlockType.Air;
    }
}
