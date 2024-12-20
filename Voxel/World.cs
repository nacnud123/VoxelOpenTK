using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

public class World
{

    public static World init;
    public const int CHUNK_SIZE = 16;
    public const int CHUNK_HEIGHT = 64;
    public const int NUM_CHUNKS = 4;

    public List<Chunk> Chunks { get; private set; } = new List<Chunk>();

    public World()
    {
        init = this;
    }


    public void Generate()
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);

        for (int cx = 0; cx < NUM_CHUNKS / 2; cx++)
        {
            for (int cz = 0; cz < NUM_CHUNKS / 2; cz++)
            {
                Chunk chunk = new Chunk(new Vector3(cx * CHUNK_SIZE, 0, cz * CHUNK_SIZE));
                GenerateChunk(chunk, noise);
                Chunks.Add(chunk);
            }
        }
    }

    private void GenerateChunk(Chunk chunk, FastNoiseLite noise)
    {
        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int y = 0; y < CHUNK_HEIGHT; y++)
            {
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    if(y < CHUNK_HEIGHT - 15)
                    {
                        if(y == CHUNK_HEIGHT - 16)
                        {
                            chunk.Blocks[x, y, z] = new DirtBlock();
                        }
                        else
                        {
                            chunk.Blocks[x, y, z] = new StoneBlock();
                        }
                    }
                    else
                    {
                        chunk.Blocks[x, y, z] = new AirBlock();
                    }
                }
            }
        }
    }

    public Chunk GetChunkAtPosition(Vector3 position)
    {
        foreach (Chunk chunk in Chunks)
        {
            if (position.X >= chunk.Position.X && position.X < chunk.Position.X + CHUNK_SIZE &&
                position.Y >= chunk.Position.Y && position.Y < chunk.Position.Y + CHUNK_HEIGHT &&
                position.Z >= chunk.Position.Z && position.Z < chunk.Position.Z + CHUNK_SIZE)
            {
                return chunk;
            }
        }

        Console.WriteLine($"No Chunk found at: {position.X} {position.Y} {position.Z}");

        return null;
    }

    public void AddBlock(Vector3 position)
    {
        Chunk targetChunk = GetChunkAtPosition(position);
        if (targetChunk != null)
        {
            Vector3 localPos = position - targetChunk.Position;
            int x = (int)MathF.Round(localPos.X);
            int y = (int)MathF.Round(localPos.Y);
            int z = (int)MathF.Round(localPos.Z);
            if (x >= 0 && x < CHUNK_SIZE && y >= 0 && y < CHUNK_HEIGHT && z >= 0 && z < CHUNK_SIZE)
            {
                targetChunk.Blocks[x, y, z] = new StoneBlock();
                Console.WriteLine($"Added block at: {x} {y} {z} in chunk {targetChunk.Position.X} {targetChunk.Position.Y} {targetChunk.Position.Z}");
            }
        }
    }

    public void RemoveBlock(Vector3 position)
    {
        Chunk targetChunk = GetChunkAtPosition(position);
        if (targetChunk != null)
        {
            Vector3 localPos = position - targetChunk.Position;
            int x = (int)MathF.Round(localPos.X);
            int y = (int)MathF.Round(localPos.Y);
            int z = (int)MathF.Round(localPos.Z);
            if (x >= 0 && x < CHUNK_SIZE && y >= 0 && y < CHUNK_HEIGHT && z >= 0 && z < CHUNK_SIZE)
            {
                targetChunk.Blocks[x, y, z] = new AirBlock();
                Console.WriteLine($"Removed block at: {x} {y} {z} in chunk {targetChunk.Position.X} {targetChunk.Position.Y} {targetChunk.Position.Z}");
            }
        }
    }
}

