using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daaluu.Animation2D;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace Daaluu.Logic
{
    /* Class responsible for drawing all players
     * all player shall implement from this,
     *   the Hand is invisible
     */ 
    public abstract class APlayer
    {
        public APlayer()
        {
            this.MyTurn = false;
            this._hand = new List<ADomino>();
            this.Ger = new List<DominoTypes>();
            this.Tsai = new TsaiCollection(this);
        }

        public string handToString() {
            return String.Join(",", this.Hand);
        }
        private AGame _game;
        protected string _name;
        public List<DominoTypes> Ger { get; private set; }
        public TsaiCollection Tsai { get; private set; }
        public virtual AGame Game
        {
            get
            {
                return this._game;
            }
            set
            {
                this._game = value;
                this._game.StateChanged += new EventHandler<GameStateChangedEventArgs>(_game_StatusChanged);
            }
        }

        public override string ToString()
        {
            return this._name;
        }

        protected virtual void _game_StatusChanged(object sender, GameStateChangedEventArgs e)
        {
            
        }

        public void ReceiveStack(ShuffledStack stack)
        {
            AnimationProcessQueue a_queue = new AnimationProcessQueue();
            //double duration = 300;
            //double delay = queue * duration;
            //Debug.WriteLine("(" + stack.Coordinates.X + "," + stack.Coordinates.Y + ") -> (" + Graphix.player_domino_coordinates[this.Seat].X + "," + Graphix.player_domino_coordinates[this.Seat].Y + ")");
            AnimationProcess proc = DefaultAnimationProcess.CreateByDuration(
                stack,
                Graphix.player_domino_coordinates[this.Seat],
                Graphix.player_domino_angles[this.Seat],
                250);
            a_queue.Add(proc);
            proc.AnimationFinished += new EventHandler<AnimationEventArgs>(proc_AnimationFinished);
            a_queue.Start();
        }

        //public class TsaiReceivedEventArgs : EventArgs
        //{
        //    public TsaiReceivedEventArgs(Tsai tsai)
        //        : base()
        //    {
        //        this.Tsai = tsai;
        //    }
        //    public Tsai Tsai { get; private set; }
        //}

        //public event EventHandler<TsaiReceivedEventArgs> TsaiReceived;
        //public virtual void OnTsaiReceived(TsaiReceivedEventArgs e)
        //{
        //    EventHandler<TsaiReceivedEventArgs> handler = TsaiReceived;
        //    if (handler != null)
        //    {
        //        handler(this, e);
        //    }
        //}

        public void ReceiveTsai(Tsai tsai)
        {
            //this.OnTsaiReceived(new TsaiReceivedEventArgs(t));
            this.Tsai.Receive(tsai);

        }

        public bool hasDebts()
        {
            foreach(Tsai t in this.Tsai)
            {
                if (t.Value < 0) return true;
            }
            return false;
        }

        public void payDebts()
        {

            // get debts
            List<Tsai> debts = new List<Tsai>();
            List<Tsai> payable = new List<Tsai>();

            foreach (Tsai d in this.Tsai)
            {
                if (d.Value < 0)
                    debts.Add(d);
                else if (d.Value > 0)
                    payable.Add(d);
            }

            if (debts.Count > 0 && payable.Count > 0)
            {
                int i = 0;
                foreach (Tsai d in debts)
                {
                    if (payable.Count <= i) break;
                    this.Tsai.List.Remove(d);
                    this.Tsai.List.Remove(payable[i]);
                    d.Owner.ReceiveTsai(payable[i]);
                    d.Owner.payDebts();
                    i++;

                }
            }
        }


        void proc_AnimationFinished(object sender, AnimationEventArgs e)
        {
            ((ShuffledStack)e.Object).Coordinates = new PointF(-100, -100);
            this.Hand.AddRange(((ShuffledStack)e.Object).Contents);
            this.Hand.Sort();
        }


        public void ReceiveGer(DominoTypes type, bool is_double) {
            //Debug.WriteLine("*** ReceiveGer "+type+", is_double: " + is_double);
            AnimationProcessQueue a_queue = new AnimationProcessQueue();
            AnimationProcess proc = DefaultAnimationProcess.CreateByDuration(
                this._game.Trick.CurrentTop[0],
                Graphix.player_domino_coordinates[this.Seat],
                Graphix.player_domino_angles[this.Seat],
                250);
            a_queue.Add(proc);
            //proc.AnimationFinished += new EventHandler<AnimationEventArgs>(receiveGer_AnimationFinished);
            a_queue.Start();

            this.Ger.Add(type);
            if (is_double) {
                this.Ger.Add(type);
            }
        }

        public void RoundReset()
        {
            if (this.Hand.Count > 0)
                this.Hand.RemoveRange(0, this.Hand.Count);

            if (this.Ger.Count > 0)
                this.Ger.RemoveRange(0, this.Ger.Count);
        }

        public virtual bool MyTurn { get; set; }
        public virtual int Position { get; set; }
        public virtual int Seat { get; set; }
        public virtual string Name { get { return this._name; } set { this._name = value; } }

        public event EventHandler<DominoEventArgs> JanliiChosen;
        public virtual void OnJanliiChosen(DominoEventArgs e)
        {
            this._game.setJanlii(e.Domino);

            EventHandler<DominoEventArgs> handler = JanliiChosen;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<DominoEventArgs> NoMove;
        public virtual void OnNoMove(DominoEventArgs e)
        {
            EventHandler<DominoEventArgs> handler = NoMove;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void placeDomino(DominoEventArgs e) {
                
        }


        public event EventHandler<DominoEventArgs> PlacedDomino;
        public virtual void OnPlacedDomino(DominoEventArgs e) {

            ADomino domino = DominoFactory.getADomino(e.Domino);
            ADomino domino1 = DominoFactory.getADomino(e.Domino1);
            //List<ADomino> trick = new List<ADomino>();
            /*
            domino.Coordinates = Graphix.player_domino_coordinates[this.Seat];
            domino.Angle = Graphix.player_domino_angles[this.Seat];

            AnimationProcessQueue a_queue = new AnimationProcessQueue();
            AnimationProcess proc = DefaultAnimationProcess.CreateBySpeed(
                domino,
                Graphix.midpoint,
                0,
                300);

            a_queue.Add(proc);
            a_queue.Start();
            */

            if (this._game.Trick.Stack.Count == 0) {
                if (e.Domino1 == DominoTypes.None) {
                    this._game.leadTrick(domino);
                } else {
                    this._game.leadTrick(domino, domino1, e.Color);
                }
            } else {
                if (e.Domino1 == DominoTypes.None) {
                    this._game.playTrick(domino);
                } else {
                    this._game.playTrick(domino, domino1);
                }
            }
            EventHandler<DominoEventArgs> handler = PlacedDomino;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<StackEventArgs> StackChosen;
        public virtual void OnStackChosen(StackEventArgs e)
        {
            this._game.chooseStack(e.StackIndex);
            EventHandler<StackEventArgs> handler = StackChosen;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        private List<ADomino> _hand = new List<ADomino>();
        public List<ADomino> Hand { get { return this._hand; } }
        public static List<ADomino> getPairs(List<ADomino> collection)
        {
            List<ADomino> pairs = new List<ADomino>();
            ADomino[] copy = new ADomino[collection.Count];
            //collection.CopyTo(copy);
            List<ADomino> cloned = new List<ADomino>();

            foreach (ADomino domino in collection)
            {
                if (!cloned.Contains(domino))
                {
                    cloned.Add(domino);
                }
                else
                {
                    pairs.Add(domino);
                    cloned.Remove(domino);
                }
            }

            return pairs;
        }
    }
}
