using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using Plasmoid.Extensions;
using Daaluu.Animation2D;

namespace Daaluu.Logic
{
    public enum DominoTypes { None = 0, Daaluu, Uuluu, Yooz, Nohoi6, 
        Other, Gaval9, Tsahilgaan6, Shanaga7, Sarlag7, Chu5, Vand4, Buluu6, 
        Gozgor7, Hunid8, Murii8, Hana8, Degee9, Siiluu10, Bajgar10, Buhuun4 }

    public enum DominoColor { None = 0, Red, White }

    public static class DominoFactory
    {
        private static List<ADomino> set;
        private static List<DominoTypes> type_set;
        private static List<DominoTypes> playable_type_set;
        private static List<DominoTypes> callable_type_set;

        //NOT TO BE USED
        static DominoFactory()
        {
            type_set = new List<DominoTypes>();
            playable_type_set = new List<DominoTypes>();
            callable_type_set = new List<DominoTypes>();
            set = new List<ADomino>();

            type_set.Add(DominoTypes.Daaluu);
            type_set.Add(DominoTypes.Daaluu);
            type_set.Add(DominoTypes.Daaluu);
            type_set.Add(DominoTypes.Daaluu);
            type_set.Add(DominoTypes.Uuluu);
            type_set.Add(DominoTypes.Uuluu);
            type_set.Add(DominoTypes.Uuluu);
            type_set.Add(DominoTypes.Uuluu);
            type_set.Add(DominoTypes.Siiluu10);
            type_set.Add(DominoTypes.Siiluu10);
            type_set.Add(DominoTypes.Siiluu10);
            type_set.Add(DominoTypes.Siiluu10);
            type_set.Add(DominoTypes.Bajgar10);
            type_set.Add(DominoTypes.Bajgar10);
            type_set.Add(DominoTypes.Bajgar10);
            type_set.Add(DominoTypes.Bajgar10);
            type_set.Add(DominoTypes.Gaval9);
            type_set.Add(DominoTypes.Gaval9);
            type_set.Add(DominoTypes.Degee9);
            type_set.Add(DominoTypes.Degee9);
            type_set.Add(DominoTypes.Hana8);
            type_set.Add(DominoTypes.Hana8);
            type_set.Add(DominoTypes.Murii8);
            type_set.Add(DominoTypes.Murii8);
            type_set.Add(DominoTypes.Hunid8);
            type_set.Add(DominoTypes.Hunid8);
            type_set.Add(DominoTypes.Hunid8);
            type_set.Add(DominoTypes.Hunid8);
            type_set.Add(DominoTypes.Shanaga7);
            type_set.Add(DominoTypes.Shanaga7);
            type_set.Add(DominoTypes.Sarlag7);
            type_set.Add(DominoTypes.Sarlag7);
            type_set.Add(DominoTypes.Gozgor7);
            type_set.Add(DominoTypes.Gozgor7);
            type_set.Add(DominoTypes.Gozgor7);
            type_set.Add(DominoTypes.Gozgor7);
            type_set.Add(DominoTypes.Nohoi6);
            type_set.Add(DominoTypes.Nohoi6);
            type_set.Add(DominoTypes.Buluu6);
            type_set.Add(DominoTypes.Buluu6);
            type_set.Add(DominoTypes.Buluu6);
            type_set.Add(DominoTypes.Buluu6);
            type_set.Add(DominoTypes.Tsahilgaan6);
            type_set.Add(DominoTypes.Tsahilgaan6);
            type_set.Add(DominoTypes.Tsahilgaan6);
            type_set.Add(DominoTypes.Tsahilgaan6);
            type_set.Add(DominoTypes.Chu5);
            type_set.Add(DominoTypes.Chu5);
            type_set.Add(DominoTypes.Vand4);
            type_set.Add(DominoTypes.Vand4);
            type_set.Add(DominoTypes.Vand4);
            type_set.Add(DominoTypes.Vand4);
            type_set.Add(DominoTypes.Buhuun4);
            type_set.Add(DominoTypes.Buhuun4);
            type_set.Add(DominoTypes.Buhuun4);
            type_set.Add(DominoTypes.Buhuun4);
            type_set.Add(DominoTypes.Yooz);
            type_set.Add(DominoTypes.Yooz);
            type_set.Add(DominoTypes.Yooz);
            type_set.Add(DominoTypes.Yooz);

            foreach (DominoTypes type in type_set)
            {
                set.Add(getADomino(type));
            }
            DominoFactory.set.Sort();

            playable_type_set.Add(DominoTypes.Daaluu);
            playable_type_set.Add(DominoTypes.Daaluu);
            playable_type_set.Add(DominoTypes.Daaluu);
            playable_type_set.Add(DominoTypes.Uuluu);
            playable_type_set.Add(DominoTypes.Uuluu);
            playable_type_set.Add(DominoTypes.Uuluu);
            playable_type_set.Add(DominoTypes.Siiluu10);
            playable_type_set.Add(DominoTypes.Siiluu10);
            playable_type_set.Add(DominoTypes.Siiluu10);
            playable_type_set.Add(DominoTypes.Bajgar10);
            playable_type_set.Add(DominoTypes.Bajgar10);
            playable_type_set.Add(DominoTypes.Bajgar10);
            playable_type_set.Add(DominoTypes.Gaval9);
            playable_type_set.Add(DominoTypes.Gaval9);
            playable_type_set.Add(DominoTypes.Degee9);
            playable_type_set.Add(DominoTypes.Degee9);
            playable_type_set.Add(DominoTypes.Hana8);
            playable_type_set.Add(DominoTypes.Hana8);
            playable_type_set.Add(DominoTypes.Murii8);
            playable_type_set.Add(DominoTypes.Murii8);
            playable_type_set.Add(DominoTypes.Hunid8);
            playable_type_set.Add(DominoTypes.Hunid8);
            playable_type_set.Add(DominoTypes.Hunid8);
            playable_type_set.Add(DominoTypes.Shanaga7);
            playable_type_set.Add(DominoTypes.Shanaga7);
            playable_type_set.Add(DominoTypes.Sarlag7);
            playable_type_set.Add(DominoTypes.Sarlag7);
            playable_type_set.Add(DominoTypes.Gozgor7);
            playable_type_set.Add(DominoTypes.Gozgor7);
            playable_type_set.Add(DominoTypes.Gozgor7);
            playable_type_set.Add(DominoTypes.Nohoi6);
            playable_type_set.Add(DominoTypes.Nohoi6);
            playable_type_set.Add(DominoTypes.Buluu6);
            playable_type_set.Add(DominoTypes.Buluu6);
            playable_type_set.Add(DominoTypes.Buluu6);
            playable_type_set.Add(DominoTypes.Tsahilgaan6);
            playable_type_set.Add(DominoTypes.Tsahilgaan6);
            playable_type_set.Add(DominoTypes.Tsahilgaan6);
            playable_type_set.Add(DominoTypes.Chu5);
            playable_type_set.Add(DominoTypes.Chu5);
            playable_type_set.Add(DominoTypes.Vand4);
            playable_type_set.Add(DominoTypes.Vand4);
            playable_type_set.Add(DominoTypes.Vand4);
            playable_type_set.Add(DominoTypes.Buhuun4);
            playable_type_set.Add(DominoTypes.Buhuun4);
            playable_type_set.Add(DominoTypes.Buhuun4);
            playable_type_set.Add(DominoTypes.Yooz);
            playable_type_set.Add(DominoTypes.Yooz);
            playable_type_set.Add(DominoTypes.Yooz);
            playable_type_set.Add(DominoTypes.Yooz);

            callable_type_set.Add(DominoTypes.Gaval9);
            callable_type_set.Add(DominoTypes.Degee9);
            callable_type_set.Add(DominoTypes.Murii8);
            callable_type_set.Add(DominoTypes.Hana8);
            callable_type_set.Add(DominoTypes.Shanaga7);
            callable_type_set.Add(DominoTypes.Sarlag7);
            callable_type_set.Add(DominoTypes.Nohoi6);
            callable_type_set.Add(DominoTypes.Chu5);
        }

        public static List<ADomino> getSet(){
            return DominoFactory.set;
        }

        public static List<DominoTypes> PlayableSet
        {
            get { return playable_type_set; }
        }

        public static List<DominoTypes> CallableSet
        {
            get { return callable_type_set; }
        }

        public static ADomino getADomino(DominoTypes type){
            switch(type){
                default:
                case DominoTypes.None:
                    return new ADomino(DominoTypes.None, 0, DominoColor.Red);
                case DominoTypes.Daaluu:
                    return new ADomino(DominoTypes.Daaluu, 12, DominoColor.Red);
                case DominoTypes.Uuluu:
                    return new ADomino(DominoTypes.Uuluu, 11, DominoColor.White);
                case DominoTypes.Siiluu10:
                    return new ADomino(DominoTypes.Siiluu10, 10, DominoColor.Red);
                case DominoTypes.Bajgar10:
                    return new ADomino(DominoTypes.Bajgar10, 10, DominoColor.White);
                case DominoTypes.Gaval9:
                    return new ADomino(DominoTypes.Gaval9, 9, DominoColor.Red, true);
                case DominoTypes.Degee9:
                    return new ADomino(DominoTypes.Degee9, 9, DominoColor.White, true);
                case DominoTypes.Hana8:
                    return new ADomino(DominoTypes.Hana8, 8, DominoColor.White, true);
                case DominoTypes.Murii8:
                    return new ADomino(DominoTypes.Murii8, 8, DominoColor.White, true);
                case DominoTypes.Hunid8:
                    return new ADomino(DominoTypes.Hunid8, 8, DominoColor.Red);
                case DominoTypes.Sarlag7:
                    return new ADomino(DominoTypes.Sarlag7, 7, DominoColor.White, true);
                case DominoTypes.Shanaga7:
                    return new ADomino(DominoTypes.Shanaga7, 7, DominoColor.Red, true);
                case DominoTypes.Gozgor7:
                    return new ADomino(DominoTypes.Gozgor7, 7, DominoColor.Red);
                case DominoTypes.Nohoi6:
                    return new ADomino(DominoTypes.Nohoi6, 6, DominoColor.Red, true);
                case DominoTypes.Buluu6:
                    return new ADomino(DominoTypes.Buluu6, 6, DominoColor.Red);
                case DominoTypes.Tsahilgaan6:
                    return new ADomino(DominoTypes.Tsahilgaan6, 6, DominoColor.White);
                case DominoTypes.Chu5:
                    return new ADomino(DominoTypes.Chu5, 5, DominoColor.White, true);
                case DominoTypes.Vand4:
                    return new ADomino(DominoTypes.Vand4, 4, DominoColor.White);
                case DominoTypes.Buhuun4:
                    return new ADomino(DominoTypes.Buhuun4, 4, DominoColor.Red);
                case DominoTypes.Yooz:
                    return new ADomino(DominoTypes.Yooz, 2, DominoColor.Red);
            }
        }
    }

    public class ADomino : IComparable, IAnimatableObject
    {
        private DominoTypes _type;
        private int _value;
        private DominoColor _color;
        private bool _is_callable;
        public ADomino(DominoTypes type, int value, DominoColor color) : this(type, value, color, false) { }
       
        public ADomino(DominoTypes type, int value, DominoColor color, bool janlii)
        {
            this._type = type;
            this._value = value;
            this._color = color;
            this._is_callable = janlii;
            this._size = Graphix.DominoSize;
        }

        public DominoTypes DominoType { get { return this._type; } }
        public int Value { get { return this._value; } }
        public DominoColor Color { get { return this._color; } }
        public bool isCallable { get { return this._is_callable; } }
        public bool isHovered { get; set; }
        public bool Selected { get; set; }

        public event EventHandler<DominoEventArgs> Clicked;
        public virtual void OnClick(DominoEventArgs e)
        {
            EventHandler<DominoEventArgs> handler = Clicked;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ADomino)) return false;
            return this.DominoType == ((ADomino)obj).DominoType;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            int retval = 0;
            ADomino otherDomino = obj as ADomino;
            if (otherDomino != null){
                if (AGame.TheGame != null)
                {
                    retval = (this.Value+(AGame.TheGame.CurrentRoundJanlii == this._type?12:0)).CompareTo(otherDomino.Value +(AGame.TheGame.CurrentRoundJanlii == otherDomino._type?12:0)) * -1;
                }

                if(retval == 0){
                    return this.Color.CompareTo(otherDomino.Color);
                }
                return retval;
            }

            else
                throw new ArgumentException("Object is not a Domino");
        }

        public static bool operator ==(ADomino a, ADomino b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.CompareTo(b) == 0;
        }

        public static bool operator !=(ADomino a, ADomino b)
        {
            return !(a == b);
        }


        public static bool operator <(ADomino a, ADomino b)
        {
            return a.CompareTo(b) > 0;
        }
        
        public static bool operator <=(ADomino a, ADomino b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator >(ADomino a, ADomino b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >=(ADomino a, ADomino b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static DominoColor getOppositeColor(DominoColor color) {
            if (color == DominoColor.Red)
                return DominoColor.White;
            else
                return DominoColor.Red;
        }

        /*
         * IAnimatableObject
         */
        private SizeF _size;
        public SizeF Size
        {
            get { return this._size; }
        }
        public bool IsSymmetric
        {
            get { return true; }
        }
        public PointF Coordinates { get; set; }
        public double Angle { get; set; }
        public bool IsAnimated { get; set; }

        public override string ToString() {
            return this.DominoType.ToString();
        }
        /* end IAnimatableObject */
    }

}
