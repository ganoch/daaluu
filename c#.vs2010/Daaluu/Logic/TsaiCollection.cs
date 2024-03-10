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

        public APlayer Owner { get; private set; }

        public int Sum { 
            get {
                int sum = 0;
                foreach(Tsai t in List)
                {
                    sum += t.Value;
                }
                return sum;
            }
        }

        public int Count
        {
            get
            {
                return this.List.Count;
            }
        }

        public void Populate()
        {
            this.Receive(new Tsai(this.Owner));
            this.Receive(new Tsai(this.Owner));
        }
        public Tsai giveTo(APlayer pl)
        {
            foreach (Tsai t in List)
            {
                // Өртэй бол өрөө дарах
                if (t.Value < 0 && t.Owner == pl)
                {
                    List.Remove(t);
                    t.Owner = this.Owner;
                    return t;
                }
            }

            foreach (Tsai t in List)
            {
                if (t.Value > 0)
                {
                    List.Remove(t);
                    return t;
                }
            }

            Tsai debt = new Tsai(-1, pl);
            List.Add(debt);

            Tsai iou = new Tsai(0, this.Owner);
            return iou;
        }

        public void Receive(Tsai incoming)
        {
            // өр дарагдах
            if (incoming.Value == 1)
            {
                foreach (Tsai T in this.List)
                {
                    if (T.Value == 0 && T.Owner == incoming.Owner)
                    {
                        this.List.Remove(T);
                        break;
                    }
                }
                incoming.Owner = this.Owner;
            }
            // өр дарах цай
            if(incoming.Value < 0)
            {
                incoming.Value = 0;
                if(!this.List.Remove(incoming))
                {
                    throw new InvalidOperationException("iou not found");
                }
                return;
            }

            this.List.Add(incoming);
        }

        public IEnumerator<Tsai> GetEnumerator()
        {
            return this.List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
