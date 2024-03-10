using System;
using System.Collections.Generic;
using System.Text;
using Daaluu.Logic;
using System.Diagnostics;

namespace Daaluu.Logic
{
    class RandomPlayer : APlayer
    {
        public override string Name { get { return this.Position.ToString(); } set { base.Name = value; } }

        public override AGame Game
        {
            get
            {
                return base.Game;
            }
            set
            {
                base.Game = value;
                base.Game.StateChanged += new EventHandler<GameStateChangedEventArgs>(_game_StatusChanged);
            }
        }

        void _game_StatusChanged(object sender, GameStateChangedEventArgs e)
        {
            this.execute();
        }

        private void execute()
        {
            if (this.MyTurn)
            {
                switch (this.Game.State)
                {
                    case GameState.ChoosingJanlii:
                        chooseJanlii();
                        break;

                    case GameState.ChoosingStacks:
                        chooseStack();
                        break;

                    case GameState.LeadTrick:
                        leadTrick();
                        break;

                    case GameState.PlayTrick:
                        playTrick();
                        break;
                }
            }
        }

        protected void chooseJanlii()
        {
            DominoEventArgs de = new DominoEventArgs(DominoFactory.CallableSet[RandomExtensions.RandomInstance.Next(DominoFactory.CallableSet.Count)], this.Position);

            Debug.WriteLine("Random player["+this.Position+"] choosing janlii: " + de.Domino);
            this.OnJanliiChosen(de);
        }

        protected void chooseStack()
        {
            StackEventArgs se = new StackEventArgs(RandomExtensions.RandomInstance.Next(10));

            Debug.WriteLine("Random player[" + this.Position + "] choosing stack: " + se.StackIndex);
            this.OnStackChosen(se);
        }

        protected void leadTrick()
        {
            
            List<ADomino> possible = new List<ADomino>();
            List<ADomino> pairs = APlayer.getPairs(this.Hand);
            bool canPair = pairs.Count > 0;
            
            foreach (ADomino d in this.Hand)
            {
                if (d.Value >= 8 || this.Game.Trick.isJanlii(d.DominoType))
                {
                    possible.Add(d);
                }
            }
            ADomino domino = DominoFactory.getADomino(DominoTypes.None);
            DominoEventArgs de = null;
            if (RandomExtensions.RandomInstance.Next(1) == 0 && canPair)
            {
                int index = RandomExtensions.RandomInstance.Next(pairs.Count);
                domino = pairs[index];
                ADomino domino1 = pairs[index];
                de = new DominoEventArgs(domino.DominoType, domino1.DominoType, (this.Game.Trick.isJanlii(domino.DominoType)&&RandomExtensions.RandomInstance.Next(1) == 0?ADomino.getOppositeColor(domino.Color):domino.Color), this.Position);
                this.Hand.Remove(domino);
                this.Hand.Remove(domino1);

            }
            else
            {
                int index = RandomExtensions.RandomInstance.Next(possible.Count);
                domino = possible[index];
                de = new DominoEventArgs(domino.DominoType, this.Position);
                this.Hand.Remove(domino);
            }

            this.OnPlacedDomino(de);
        }

        protected void playTrick()
        {
            ADomino domino = DominoFactory.getADomino(DominoTypes.None);
            List<ADomino> current_top = this.Game.Trick.CurrentTop;
            List<ADomino> possible = new List<ADomino>();
            List<ADomino> same_color = new List<ADomino>();
            foreach (ADomino d in this.Hand)
            {
                if (!this.Game.Trick.Color.Equals(d.Color) && !this.Game.Trick.isJanlii(d.DominoType))
                    continue;
                else if (!this.Game.Trick.isJanlii(d.DominoType))
                    same_color.Add(d);

                if (d > current_top[0] || this.Game.Trick.isJanlii(d.DominoType))
                    possible.Add(d);
            }


            if (possible.Count == 1 && this.Game.Trick.isJanlii(possible[0].DominoType) && this.Game.Trick.isJanlii(current_top[0].DominoType))
            {
                possible.RemoveAt(0);
            }

            if (possible.Count > 0)
            {
                domino = possible[RandomExtensions.RandomInstance.Next(possible.Count)];
            }
            else if (same_color.Count > 0)
            {
                same_color.Sort();
                domino = same_color[same_color.Count - 1];
            }
            else
            {
                this.Hand.Sort();
                domino = this.Hand[this.Hand.Count - 1];
            }



            if (this.Game.Trick.IsDouble)
            {
                ADomino domino1 = DominoFactory.getADomino(DominoTypes.None);
                List<ADomino> pairs = APlayer.getPairs(this.Hand);
                List<ADomino> same_color_pairs = new List<ADomino>();
                List<ADomino> possible_pairs = new List<ADomino>();
                if (pairs.Count > 0)
                {
                    foreach (ADomino pair in pairs)
                    {
                        if (!this.Game.Trick.Color.Equals(pair.Color))
                            continue;
                        else if (!this.Game.Trick.isJanlii(pair.DominoType))
                            same_color_pairs.Add(pair);

                        if (pair > current_top[0] || this.Game.Trick.isJanlii(pair.DominoType))
                            possible_pairs.Add(pair);
                    }
                }


                if (possible_pairs.Count > 0)
                {
                    domino = possible_pairs[RandomExtensions.RandomInstance.Next(possible_pairs.Count)];
                    domino1 = domino;
                }
                else if (same_color_pairs.Count > 0)
                {
                    same_color_pairs.Sort();
                    domino = same_color_pairs[same_color_pairs.Count - 1];
                    domino1 = domino;
                }
                else
                {
                    if (same_color.Count > 0)
                    {
                        same_color.Sort();
                        domino = same_color[same_color.Count - 1];
                        if (same_color.Count > 1)
                        {
                            domino1 = same_color[same_color.Count - 2];
                        }
                        else
                        {
                            this.Hand.Sort();
                            domino1 = this.Hand[this.Hand.Count - 1];
                        }
                    }
                    else
                    {
                        this.Hand.Sort();
                        domino = this.Hand[this.Hand.Count - 1];
                        if (this.Hand.Count > 1)
                        {
                            domino1 = this.Hand[this.Hand.Count - 2];
                        }
                    }
                }

                this.Hand.Remove(domino);
                this.Hand.Remove(domino1);
                DominoEventArgs de = new DominoEventArgs(domino.DominoType, domino1.DominoType, domino.Color, this.Position);
                this.OnPlacedDomino(de);
            }
            else
            {
                this.Hand.Remove(domino);
                DominoEventArgs de = new DominoEventArgs(domino.DominoType, this.Position);
                this.OnPlacedDomino(de);
            }
        }

    }
}
