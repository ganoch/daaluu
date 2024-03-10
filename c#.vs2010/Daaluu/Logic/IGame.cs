using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daaluu.Logic
{
    interface IGame
    {
        void Start();

        void setJanlii(DominoTypes type);
        void chooseStack(int index);

        void leadTrick(ADomino domino0, ADomino domino1, DominoColor color);
        void leadTrick(ADomino domino0);

        void playTrick(ADomino domino0, ADomino domino1);
        void playTrick(ADomino domino0);


        event EventHandler<GameStateChangedEventArgs> StateChanged;
    }
}
