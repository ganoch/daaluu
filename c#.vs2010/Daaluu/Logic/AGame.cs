using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

using Daaluu;
using Daaluu.Animation2D;
using System.Threading;

namespace Daaluu.Logic
{
    public enum GameState
    {
        Null = 0,
        WaitingForSettings,
        ChoosingJanlii,
        ChoosingStacks,
        DistributingStacks,
        LeadTrick,
        PlayTrick
    }

    public class DominoEventArgs : EventArgs
    {
        public DominoColor Color { get; private set; }
        public DominoEventArgs(DominoTypes a, int player_index)
            : this(a, DominoTypes.None, DominoColor.Red, player_index) { }

        public DominoEventArgs(DominoTypes a, DominoTypes b, DominoColor color, int player_index)
            : base()
        {
            this.Domino = a;
            this.Domino1 = b;
            this.Color = color;
            this.PlayerIndex = player_index;
        }
        public DominoTypes Domino { get; set; }
        public DominoTypes Domino1 { get; set; }
        public int PlayerIndex { get; private set; }
    }

    public class GameStateChangedEventArgs : EventArgs
    {
        public GameStateChangedEventArgs(GameState prev, GameState _new)
            : base()
        {
            this.PreviousStatus = prev;
            this.NewStatus = _new;
        }
        public GameState PreviousStatus { get; set; }
        public GameState NewStatus { get; set; }
    }

    public class StackEventArgs : EventArgs
    {
        public StackEventArgs(int index)
            : base()
        {
            this.StackIndex = index;
        }
        public int StackIndex { get; set; }
    }
    /*
     * Universal Class for All IGame that will be run from this client
     * meaning all animations
     * does not include, host specific function such as shuffling, checking for valid moves of players, 
     *      or keeping track of stacks or players' hands, or controlling game states
     */

    public abstract class AGame : IGame 
    {
        public static AGame TheGame { get; set; }
        private static ADomino[] _janliis = { 
            DominoFactory.getADomino(DominoTypes.Gaval9),
            DominoFactory.getADomino(DominoTypes.Degee9),
            DominoFactory.getADomino(DominoTypes.Hana8),
            DominoFactory.getADomino(DominoTypes.Murii8),
            DominoFactory.getADomino(DominoTypes.Shanaga7),
            DominoFactory.getADomino(DominoTypes.Sarlag7),
            DominoFactory.getADomino(DominoTypes.Nohoi6),
            DominoFactory.getADomino(DominoTypes.Chu5)
        };
        protected ADomino[] dominoset = new ADomino[50];
        private ShuffledStack[] shuffledstack = new ShuffledStack[10];
        protected List<APlayer> players = new List<APlayer>();
        public GameState state = GameState.Null;
        public Trick Trick { get; set; }
        public int TrickNumber { get; protected set; }
        public int LastCaller { get; protected set; }
        public DominoTypes CurrentRoundJanlii
        {
            get
            {
                if (this.Trick != null)
                {
                    return this.Trick.Janlii;
                }
                else
                {
                    return DominoTypes.None;
                }
            }
        }

        public AGame()
        {
            AGame.TheGame = this;
            for (int i = 0; i < 50; i++)
            {
                dominoset[i] = DominoFactory.getADomino(DominoFactory.PlayableSet[i]);
            }

            shuffledstack[0] = new ShuffledStack(0);
            shuffledstack[1] = new ShuffledStack(1);
            shuffledstack[2] = new ShuffledStack(2);
            shuffledstack[3] = new ShuffledStack(3);
            shuffledstack[4] = new ShuffledStack(4);
            shuffledstack[5] = new ShuffledStack(5);
            shuffledstack[6] = new ShuffledStack(6);
            shuffledstack[7] = new ShuffledStack(7);
            shuffledstack[8] = new ShuffledStack(8);
            shuffledstack[9] = new ShuffledStack(9);
        }
        public ShuffledStack[] ShuffledStack { get { return this.shuffledstack; } }
        public int CurrentPlayerIndex { get; protected set; }
        public virtual ADomino[] Janliis { get { return _janliis; } }
        public virtual GameState State {
            get { return this.state; }
            set
            {
                if(value != GameState.LeadTrick && value != GameState.PlayTrick)
                    Debug.WriteLine("**~~ CHANGING STATUS **********: "+value);
                GameState old = this.state;
                this.state = value;
                OnStateChanged(new GameStateChangedEventArgs(old, value));
            }
        }
        public int PlayerCount { get { return this.players.Count; } }
        public virtual List<APlayer> Players { get { return players; } }


        public event EventHandler<GameStateChangedEventArgs> StateChanged;
        public virtual void OnStateChanged(GameStateChangedEventArgs e)
        {
            EventHandler<GameStateChangedEventArgs> handler = StateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public abstract void setJanlii(DominoTypes type);

        public virtual void seatNextPlayer(APlayer player)
        {
            player.Game = this;
            player.Position = this.players.Count;
            this.players.Add(player);
            
            if (player is UserPlayer)
            {
                player.Seat = 3;
            }
            int offset = 0;
            foreach (APlayer pl in this.players)
            {
                if (pl is UserPlayer)
                {
                    offset = 8 - pl.Position;
                    break;
                }
            }

            foreach (APlayer pl in this.players)
            {
                if (!(pl is UserPlayer))
                {
                    pl.Seat = (pl.Position + offset) % 5;
                }
            }

        }

        public abstract void Start();

        public virtual void playTrick(ADomino domino0) {
            Debug.WriteLine("player[" + this.CurrentPlayerIndex + "].play: " + domino0.ToString());

            List<ADomino> trick = new List<ADomino>();
            trick.Add(domino0);
            this.playTrick(trick);
        }

        public virtual void playTrick(ADomino domino0, ADomino domino1) {
            Debug.WriteLine("player[" + this.CurrentPlayerIndex + "].play: " + domino0.ToString() + ", " + domino1.ToString());
            List<ADomino> trick = new List<ADomino>();
            trick.Add(domino0);
            trick.Add(domino1);
            this.playTrick(trick);

        }

        public virtual void leadTrick(ADomino domino0) {
            Debug.WriteLine("player[" + this.CurrentPlayerIndex + "].lead: " + domino0.ToString());
            List<ADomino> trick = new List<ADomino>();
            trick.Add(domino0);
            this.leadTrick(trick, domino0.Color);
        }

        public virtual void leadTrick(ADomino domino0, ADomino domino1, DominoColor color) {
            if (domino0.Equals(domino1)) {
                Debug.WriteLine("player[" + this.CurrentPlayerIndex + "].lead: " + domino0.ToString() + ", " + domino1.ToString() + ", ["+color.ToString()+"]");
                List<ADomino> trick = new List<ADomino>();
                trick.Add(domino0);
                trick.Add(domino1);
                this.leadTrick(trick, (color != DominoColor.None ? color : domino0.Color ));
            }
        }
        public virtual void leadTrick(List<ADomino> trick, DominoColor color) {
            this.Trick.setStack(trick, color, this.CurrentPlayerIndex);
            animateTrick(trick);
        }

        public virtual void playTrick(List<ADomino> trick) {
            this.Trick.placeDomino(trick, this.CurrentPlayerIndex);
            animateTrick(trick);
        }

        private void animateTrick(List<ADomino> trick) {
            //List<ADomino> trick = new List<ADomino>();
            trick[0].Coordinates = Graphix.player_domino_coordinates[this.players[this.CurrentPlayerIndex].Seat];
            trick[0].Angle = Graphix.player_domino_angles[this.players[this.CurrentPlayerIndex].Seat];

            AnimationProcessQueue a_queue = new AnimationProcessQueue();
            AnimationProcess proc = DefaultAnimationProcess.CreateBySpeed(
                trick[0],
                Graphix.midpoint,
                0,
                300);
            a_queue.QueueFinished += new EventHandler<EventArgs>(trickPlaced);
            a_queue.Add(proc);
            a_queue.Start();
        }

        void trickPlaced(object sender, EventArgs e) {
            this.evaluateTrick();
        }

        protected abstract void evaluateTrick();
        public abstract void chooseStack(int stack_index);

       
        protected abstract void newRound(int start_player_id);

        public void setTurn(int index)
        {
            this.players[this.CurrentPlayerIndex].MyTurn = false;
            this.players[index].MyTurn = true;
            this.CurrentPlayerIndex = index;
        }

        protected void setNextPlayerTurn()
        {
            this.setTurn((this.CurrentPlayerIndex + 1) % this.players.Count);
        }
    }

    public class ShuffledStack : IAnimatableObject
    {
        public int ToUser { get; set; }
        private int index = 0;
        private ADomino[] contents = new ADomino[5];
        public ShuffledStack(int pos){
            this.index = pos;
            this._size = Graphix.DominoSize;

            this.ToUser = 5;
            this.Reset();

        }
        public void Reset()
        {
            int max = 10;
            PointF domino_mid = new PointF(Graphix.midpoint.X + (float)Math.Cos(((double)this.index * 360f / max).ToRadians()) * Graphix.StackRadius, Graphix.midpoint.Y + (float)Math.Sin(((double)this.index * 360f / max).ToRadians()) * Graphix.StackRadius);
            double angle = -90f + this.index * 360f / max;
            this.Coordinates = domino_mid;
            this.Angle = angle;
        }
        public bool Selected { get; set; }
        public ADomino[] Contents { get { return this.contents; } }
        public int Index
        {
            get { return this.index; }
        }
        public RectangleF Rect { get; set; }

        public bool isHovered { get; set; }

        public event EventHandler<DominoEventArgs> Clicked;
        public virtual void OnClick(DominoEventArgs e)
        {
            EventHandler<DominoEventArgs> handler = Clicked;

            if (handler != null)
            {
                handler(this, e);
            }
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

    }
}
