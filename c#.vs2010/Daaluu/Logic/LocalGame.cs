using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Daaluu.Animation2D;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace Daaluu.Logic
{
    public class LocalGame : AGame
    {
        public LocalGame()
            : base() {
        }

        public override void Start()
        {
            Debug.WriteLine("******** STARTING GAME **********");

            this.Trick = new Trick(DominoTypes.None);
            this.newRound(0);
        }

        protected override void newRound(int start_player_id)
        {
            this.ResetAndShuffle();
            this.setTurn(start_player_id);
            //this.players[this.CurrentPlayerIndex].JanliiChosen += new EventHandler<DominoEventArgs>(AGame_JanliiChosen);
            this.Trick.Reset();
            this.State = GameState.ChoosingJanlii;
            //base.newRound(start_player_id);
        }

        protected bool playerCanLead(int index)
        {
            if (APlayer.getPairs(this.Players[index].Hand).Count > 0)
                return true;
            else
            {
                foreach(ADomino domino in this.Players[index].Hand){
                    if (domino.Value >= 8 || this.Trick.isJanlii(domino.DominoType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ResetAndShuffle()
        {
            RandomExtensions.RandomInstance.Shuffle(this.dominoset);
            for (int i = 0; i < 10; i++)
            {
                for (int k = 0; k < 5; k++)
                {
                    this.ShuffledStack[i].Contents[k] = this.dominoset[i * 5 + k];
                    this.ShuffledStack[i].Reset();
                }
            }
            foreach (APlayer pl in this.players)
            {
                pl.RoundReset();
            }
        }

        public override void setJanlii(DominoTypes type) {
            Debug.WriteLine("*** LocalGame.setJanlii: "+type);
            this.LastCaller = this.CurrentPlayerIndex;
            this.Trick = new Trick(type);
            this.State = GameState.ChoosingStacks;
        }

        protected override void evaluateTrick() {
            Debug.WriteLine("*** LocalGame.evaluateTrick");
            if (this.Trick.IsFinished) {
                this.Trick.Empty();
                if (this.Trick.IsDouble) {
                    this.players[this.Trick.CurrentOwner].ReceiveGer(this.Trick.CurrentTop[1].DominoType, true);
                    this.TrickNumber++;
                } else {
                    this.players[this.Trick.CurrentOwner].ReceiveGer(this.Trick.CurrentTop[0].DominoType, false);
                }

                AnimationProcessQueue a_queue = new AnimationProcessQueue();
                SleepProcess proc = new SleepProcess(300, this.Trick.CurrentTop[0]);
                a_queue.QueueFinished += new EventHandler<EventArgs>(sendGerFinished);
                a_queue.Add(proc);
                a_queue.Start();
            } else {
                this.setNextPlayerTurn();
                this.State = GameState.PlayTrick;
            }
        }

        protected virtual void sendGerFinished(object sender, EventArgs e) {
            Debug.WriteLine("*** LocalGame.sendGerFinished");
            if (TrickNumber == 10) {
                TrickNumber = 0;
                this.distributeGers();
                this.payDebts();
                this.newRound((this.LastCaller + 1) % this.players.Count); //a_queue.QueueFinished += new EventHandler<EventArgs>(a_queue_QueueFinished_trick_new_round);
            } else {
                this.setTurn(this.Trick.CurrentOwner);
                this.Trick.Empty();
                this.newTrick();
            }
        }

        public void distributeGers()
        {
            List<APlayer> tsai_uguh = new List<APlayer>();

            foreach (APlayer pl in this.Players)
            {
                if (pl.Ger.Count < 2)
                {
                    for (int i = 2; i > pl.Ger.Count; i--)
                    {
                        tsai_uguh.Add(pl);
                    }
                }
            }


            foreach (APlayer pl in this.Players)
            {
                if (pl.Ger.Count > 2)
                {
                    for (int i = pl.Ger.Count; i > 2; i--)
                    {
                        bool allocated = false;
                        foreach (Tsai t in pl.Tsai.List)
                        {
                            if (t.Value < 0 && tsai_uguh.Contains(t.Owner))
                            {
                                pl.ReceiveTsai(t.Owner.Tsai.giveTo(pl));
                                tsai_uguh.Remove(t.Owner);
                                allocated = true;
                                break;
                            }
                        }
                        //өр дарагдаагүй бол
                        if(!allocated)
                        {
                            pl.ReceiveTsai(tsai_uguh[tsai_uguh.Count - 1].Tsai.giveTo(pl));
                            tsai_uguh.RemoveAt(tsai_uguh.Count - 1);
                        }
                    }

                }
            }



        }
        public void payDebts() { 
            foreach (APlayer pl in this.Players)
            {
                pl.payDebts();
            }
        }

        public void offerPlayerGer(DominoTypes type, APlayer from, APlayer to)
        {

        }

        public void acceptGer(DominoTypes type, APlayer from, APlayer to)
        {

        }

        public override void chooseStack(int stack_index) {
            Debug.WriteLine("*** LocalGame.Stack : " + stack_index);
            //this.players[this.CurrentPlayerIndex].StackChosen -= new EventHandler<StackEventArgs>(AGame_StackChosen);
            this.State = GameState.DistributingStacks; //start round

            AnimationProcessQueue a_queue = new AnimationProcessQueue();
            for (int i = stack_index; i < this.ShuffledStack.Length + stack_index; i++) {
                ShuffledStack stack = this.ShuffledStack[i % this.ShuffledStack.Length];
                int queue = (stack.Index - stack_index + 10) % 10;
                stack.ToUser = (queue + this.CurrentPlayerIndex) % 5;
                SleepProcess proc = new SleepProcess(300, stack);
                proc.AnimationFinished += new EventHandler<AnimationEventArgs>(proc_AnimationFinished);
                a_queue.Add(proc);
            }
            a_queue.Add(new SleepProcess(300, null));
            a_queue.QueueFinished += new EventHandler<EventArgs>(LeadTrickReady);
            a_queue.Start();
        }

        protected void LeadTrickReady(object sender, EventArgs e) {
            this.newTrick();
        }

        protected void newTrick() {
            this.Trick.Reset();
            while (!this.playerCanLead(this.CurrentPlayerIndex)) {
                Debug.WriteLine("player[" + this.CurrentPlayerIndex + "] cannot lead: " + this.players[this.CurrentPlayerIndex].handToString());
                this.setNextPlayerTurn();
            }
            this.State = GameState.LeadTrick;
            this.TrickNumber++;
        }
        protected virtual void proc_AnimationFinished(object sender, AnimationEventArgs e) {
            ShuffledStack stack = (ShuffledStack)e.Object;
            if (stack.ToUser < this.players.Count) {
                Debug.WriteLine("Stack " + stack.Index + " to player " + stack.ToUser);
                this.players[stack.ToUser].ReceiveStack(stack);
            }
        }
    }
}
