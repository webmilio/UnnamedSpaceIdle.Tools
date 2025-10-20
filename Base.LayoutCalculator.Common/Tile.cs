namespace Base.LayoutCalculator.Common;

public abstract class Tile
{
    public static readonly Tile Slot = new EmptyTile("Slot", true);
    public static readonly Tile Block = new EmptyTile("Block", false);

    public abstract string Name { get; }
    public virtual string Category { get; } = string.Empty;

    public virtual char Display { get; } = ' ';

    public virtual bool Replacable { get; } = true;

    public abstract void ApplyMultipliers(double[,,] multipliers, int x, int y, int z);

    public abstract double GetProduction();

    public override string ToString()
    {
        return $"{Display}";
    }

    private class EmptyTile : Tile
    {
        public override string Name { get; }

        public override bool Replacable { get; }

        public EmptyTile(string name, bool replacable)
        {
            Name = name;
            Replacable = replacable;
        }

        public override void ApplyMultipliers(double[,,] multipliers, int x, int y, int z) { }
        public override double GetProduction() { return 0; }
    }
}
