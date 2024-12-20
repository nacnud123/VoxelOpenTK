// DA - Holds the information used for blocks. Not really used right now.

using OpenTK.Mathematics;

public enum BlockType
{
    Air = 0,
    Stone,
    Grass,
    Dirt
}

public class Block
{
    public BlockType type;

    public Block()
    {

    }

}