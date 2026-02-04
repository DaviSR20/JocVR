[System.Serializable] // Per veure'l a l'Inspector
public struct TokenID
{
    public int x;
    public int y;

    public TokenID(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}
