using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Daaluu.Logic
{
    public class UserPlayer : APlayer
    {
        public UserPlayer() : this("") { }

        public UserPlayer(string name)
            : base()
        {
            this.Name = name;
            this._selected_count = 0;
        }

        protected override void _game_StatusChanged(object sender, GameStateChangedEventArgs e) {
            if (e.NewStatus == GameState.PlayTrick || e.NewStatus == GameState.LeadTrick) {
                this.DisableSelect = false;
            }
        }

        public event EventHandler<EventArgs> HasSelectedChanged;
        public virtual void OnSelectionChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = HasSelectedChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        private int _selected_count = 0;
        public int SelectedCount
        {
            get { return this._selected_count; }
            private set
            {
                int temp = this._selected_count;
                this._selected_count = value;
                if (temp != this._selected_count)
                {
                    this.OnSelectionChanged(new EventArgs());
                }
            }
        }
        private int _last_selected = -1;
        public bool selectDomino(int index) {
            if (!this.MyTurn || this.DisableSelect)
                return false;
            List<ADomino> pairs = APlayer.getPairs(this.Hand);

            ADomino previous_selected = DominoFactory.getADomino(DominoTypes.None);
            if(this.SelectedCount > 0)
                previous_selected = this.Hand[this._last_selected];

            if (this.Game.Trick.Stack.Count > 0) {
                List<ADomino> current_top = this.Game.Trick.CurrentTop;
                if (this.Game.Trick.IsDouble) {
                    List<ADomino> possible_pairs = new List<ADomino>();

                    for (int i = pairs.Count - 1; i >= 0; i--) {
                        if (pairs[i].Color != this.Game.Trick.Color && !this.Game.Trick.isJanlii(pairs[i].DominoType)) {
                            pairs.RemoveAt(i);
                        }
                    }

                    if (pairs.Count > 0) {
                        foreach (ADomino pair in pairs) {
                            if (pair > current_top[0] || this.Game.Trick.isJanlii(pair.DominoType)) {
                                possible_pairs.Add(pair);
                            }
                        }
                    }

                    if (possible_pairs.Count > 0) {
                        if (!possible_pairs.Contains(this.Hand[index])) {
                            return false;
                        }
                    } else if (pairs.Count > 0) {
                        if (!pairs.Contains(this.Hand[index])) {
                            return false;
                        }
                    }

                    int same_color_count = 0;
                    foreach (ADomino d in this.Hand) {
                        if (d.Color.Equals(this.Game.Trick.Color) && !this.Game.Trick.isJanlii(d.DominoType)) {
                            same_color_count++;
                        }
                    }

                    if (this.SelectedCount > 0) {
                        if (pairs.Count > 0 && !previous_selected.Equals(this.Hand[index])) {
                            return false;
                        } else if(pairs.Count == 0){
                            if (same_color_count-1 > 0 && (this.Hand[index].Color != this.Game.Trick.Color))
                                return false;
                        }
                    } else {
                        if (pairs.Count > 0) {
                            if (possible_pairs.Count > 0 && !possible_pairs.Contains(this.Hand[index])) {
                                Debug.WriteLine("is first selection and has possible pairs, and is NOT of possible pair");
                                return false;
                            } else if(possible_pairs.Count == 0 && !pairs.Contains(this.Hand[index])){
                                Debug.WriteLine("is first selection and has no possible pairs but has colored pairs, and is NOT of colored pair");
                                return false;
                            }
                        } else { //if no pairs at all
                            if (same_color_count > 0 && this.Hand[index].Color != this.Game.Trick.Color) {
                                Debug.WriteLine("is first selection and has NO colored pairs, has same color and is same color");
                                return false;
                            }
                        }
                    }
                } else {
                    List<ADomino> possible = new List<ADomino>();
                    List<ADomino> same_color = new List<ADomino>();
                    foreach (ADomino d in this.Hand) {
                        if (!this.Game.Trick.Color.Equals(d.Color) && !this.Game.Trick.isJanlii(d.DominoType))
                            continue;
                        else if (!this.Game.Trick.isJanlii(d.DominoType))
                            same_color.Add(d);

                        if (d > current_top[0] || this.Game.Trick.isJanlii(d.DominoType))
                            possible.Add(d);
                    }

                    if (possible.Count == 1 && this.Game.Trick.isJanlii(current_top[0].DominoType) && this.Game.Trick.isJanlii(possible[0].DominoType)) {
                        possible.RemoveAt(0);
                    }

                    if (possible.Count > 0 && !possible.Contains(this.Hand[index])) {
                        return false;
                    } else if (possible.Count == 0 && same_color.Count > 0 && !same_color.Contains(this.Hand[index])) {
                        return false;
                    }
                }
            } else {
                /*
                if(this.SelectedCount > 0 && pairs.Count>0 && !this.Hand[_last_selected].Equals(this.Hand[index])){
                    return false;
                }

                /*
                if (this.SelectedCount == 0) {
                    this.SelectedCount++;
                    this.Hand[index].Selected = true;
                } else {
                    if (pairs.Count > 0 && this.Hand[_last_selected].Equals(this.Hand[index])) {
                        this.Hand[index].Selected = true;
                    } else
                        return false;
                }*/
            }


            foreach (ADomino domino in this.Hand) {
                domino.Selected = false;
            }

            this.SelectedCount++;
            this.Hand[index].Selected = true;
            if (((this.SelectedCount == 2 && this.Game.Trick.Stack.Count == 0 && this.Hand[_last_selected].Equals(this.Hand[index])) || this.Game.Trick.IsDouble) && _last_selected > -1 )
                this.Hand[this._last_selected].Selected = true;
            else if(this.SelectedCount == 2 && 
                ((this.Game.Trick.Stack.Count > 0 && !this.Game.Trick.IsDouble) || (this.Game.Trick.Stack.Count==0 && !this.Hand[_last_selected].Equals(this.Hand[index]))))
                this.SelectedCount--;

            if (this.SelectedCount > 2)
                this.SelectedCount = 2;
            this._last_selected = index;

            return true;
        }

        public bool deselectDomino(int index)
        {
            this.SelectedCount--;
            this.Hand[index].Selected = false;
            if (this.SelectedCount == 0)
            {
                _last_selected = -1;
            }
            return true;
        }

        public void placeSelected()
        {
            List<ADomino> selected = new List<ADomino>();
            foreach(ADomino domino in this.Hand){
                if(domino.Selected){
                    selected.Add(domino);
                }
            }
            if (selected.Count > 0) {

                this.DisableSelect = true;
                if (selected.Count > 1) {
                    if (selected[0].Equals(selected[1]) && this.Game.Trick.isJanlii(selected[0].DominoType) && this.Game.State == GameState.LeadTrick) {
                        Graphix.WaitForColorSelect = true;
                        return;
                    } else {
                        DominoEventArgs de = new DominoEventArgs(selected[0].DominoType, selected[1].DominoType, DominoColor.None, this.Position);
                        selected[0].Selected = false;
                        selected[1].Selected = false;
                        this.OnPlacedDomino(de);
                        this.Hand.Remove(selected[0]);
                        this.Hand.Remove(selected[1]);
                        this.SelectedCount = 0;
                    }
                } else {
                    DominoEventArgs de = new DominoEventArgs(selected[0].DominoType, this.Position);
                    selected[0].Selected = false;
                    this.OnPlacedDomino(de);
                    this.Hand.Remove(selected[0]);
                    this.SelectedCount = 0;
                }
                foreach (ADomino domino in this.Hand) {
                    domino.Selected = false;
                }
                _last_selected = -1;
            }
        }

        public void placeSelected(DominoColor color)
        {
            List<ADomino> selected = new List<ADomino>();
            foreach (ADomino domino in this.Hand)
            {
                if (domino.Selected)
                {
                    selected.Add(domino);
                }
            }
            DominoEventArgs de = new DominoEventArgs(selected[0].DominoType, selected[1].DominoType, color, this.Position);
            this.OnPlacedDomino(de);
            selected[0].Selected = false;
            selected[1].Selected = false;
            this.Hand.Remove(selected[0]);
            this.Hand.Remove(selected[1]);
            this.SelectedCount = 0;
            _last_selected = -1;
            Graphix.WaitForColorSelect = false;
            
        }

        public bool DisableSelect { get; set; }
    }
}
