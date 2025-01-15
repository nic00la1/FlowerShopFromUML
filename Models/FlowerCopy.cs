using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML;

public class FlowerCopy
{
    public string Name { get; set; }
    public string Color { get; set; }
    public int Count { get; set; }

    public FlowerCopy(string name, string color, int count)
    {
        Name = name;
        Color = color;
        Count = count;
    }

    public override bool Equals(object obj)
    {
        if (obj is FlowerCopy other)
            return Name == other.Name && Color == other.Color &&
                Count == other.Count;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Color, Count);
    }
}
