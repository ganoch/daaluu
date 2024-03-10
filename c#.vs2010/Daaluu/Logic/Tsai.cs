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

        
    }
}
