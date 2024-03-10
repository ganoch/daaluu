using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daaluu.Logic
{
    public class TsaiCollection : IEnumerable<Tsai>
    {
        public TsaiCollection(APlayer pl) {
            this.Owner= pl;
            this.List = new List<Tsai>();
            this.Populate();
        }
        public List<Tsai> List { get; set; }

        public APlayer Owner { get; set; }

        public int Count { 
            get {
                int sum = 0;
                foreach(Tsai t in List)
                {
                    sum += t.Value;
                }
                return sum;
            }
        }

        public Tsai giveTo(APlayer pl)
        {
            foreach(Tsai t in List)
            {
                if(t.Value > 0)
                {
                    List.Remove(t);
                    return t;
                }
            }

            Tsai debt = new Tsai(-1, pl);
            List.Add(debt);

            Tsai iou= new Tsai(0, this.Owner);
            return iou;
        }

        public void Populate()
        {
            this.Receive(new Tsai(this.Owner));
            this.Receive(new Tsai(this.Owner));
        }
        public void Receive(Tsai t)
        {
            // өрөө дарагдах
            foreach(Tsai T in this.List)
            {
                if(T.Value == 0 && T.Owner == t.Owner)
                {
                    this.List.Remove(T);
                    break;
                }
            }
            t.Owner = this.Owner;
            List.Add(t);
        }

        public IEnumerator<Tsai> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
