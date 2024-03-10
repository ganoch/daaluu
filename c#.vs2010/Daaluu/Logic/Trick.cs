using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daaluu.Animation2D;
using System.Drawing;

namespace Daaluu.Logic
{
    public class Trick
    {
        public Trick(DominoTypes janlii)
        {
            this.Reset();
            this.Janlii = janlii;
        }
        public DominoTypes Janlii { get; private set; }
        public bool IsDouble { get; private set; }
        public int CurrentOwner { get; private set; }
        public bool setStack(List<ADomino> domino, DominoColor color, int player_index)
        {
            if (domino.Count == 0)
            {
                return false;
            }
            this.CurrentOwner = player_index;
            if (domino.Count > 1)
            {
                this.IsDouble = true;
            }

            if (this.IsDouble && this.isJanlii(domino[0].DominoType))
            {
                this.Color = color;
            }
            else
            {
                this.Color = domino[0].Color;
            }

            this.Stack.Add(domino);
            return true;
        }
        public bool placeDomino(List<ADomino> domino, int player_index)
        {
            if ((this.IsDouble && domino.Count > 1 && (!domino[0].Equals(domino[1]) || domino[0] < this.CurrentTop[0]))
                ||
                ( this.isJanlii(this.CurrentTop[0].DominoType)
                || ((!domino[0].Color.Equals(this.Color)
                    || domino[0] <= this.CurrentTop[0]) && !this.isJanlii(domino[0].DominoType))))
            {
                this.Stack.Insert(0, domino);
            }
            else
            {
                this.CurrentOwner = player_index;
                this.Stack.Add(domino);
            }

            return true;
        }
        public DominoColor Color { get; private set; }
        public List<List<ADomino>> Stack { get; set; }
        public List<ADomino> CurrentTop
        {
            get
            {
                if (this.Stack.Count > 0)
                {
                    return this.Stack[this.Stack.Count - 1];
                }
                else
                {
                    return new List<ADomino>();
                }
            }
        }
        public bool IsFinished { get { return this.Stack.Count == 5; } }

        public bool isJanlii(DominoTypes domino)
        {
            return domino == this.Janlii;
        }

        public void Reset()
        {
            this.Stack = new List<List<ADomino>>();
            this.CurrentOwner = -1;
            this.Color = DominoColor.None;
            this.IsDouble = false;
        }

        public void Empty()
        {
            if(this.Stack.Count > 4)
                this.Stack.RemoveRange(0, 4);
        }
    }

    public class PairDomino : IAnimatableObject
    {
        private SizeF _size = new SizeF(Graphix.DominoSize.Width * 2 + 1, Graphix.DominoSize.Height);
        public DominoTypes Type0 { get; set; }
        public DominoTypes Type1 { get; set; }
        /*
         * IAnimatableObject
         */
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

        /* end IAnimatableObject */
    }
}
