public struct Vector2i
{
	public int x, y;

	public Vector2i( int x, int y )
	{
		this.x = x;
		this.y = y;
	}

	// Get that Hash Bro
	public override int GetHashCode()
	{
		int hash = 17;

		hash = hash * 23 + x.GetHashCode();
		hash = hash * 23 + y.GetHashCode();

		return hash;    // blaze it, lol ;)
	}
}
