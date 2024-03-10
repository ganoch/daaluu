using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daaluu.Logic
{
    public class Tsai
    {
        public Tsai() : this(1, null){ 
            
        }

        public Tsai(int value, APlayer owner)
        {
            Value = value;
            Owner = owner;
        }

        public Tsai(APlayer owner) : this(1, owner)
        {

        }

        public int Value { get; set; }
        public APlayer Owner { get; set; }

        public static int operator +(Tsai a, Tsai b) { return a.Value + b.Value; }
        public static int operator +(int a, Tsai b) { return a + b.Value; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Tsai objAsPart = obj as Tsai;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public bool Equals(Tsai other)
        {
            if (other == null) return false;
            return (this.Value.Equals(other.Value) && this.Owner == other.Owner);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ this.Owner.GetHashCode();
        }
    }
}
