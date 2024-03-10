using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daaluu.Logic
{
    class LANGame : AGame
    {
        public override void seatNextPlayer(APlayer player)
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void chooseStack(int stack_index) {
            throw new NotImplementedException();
        }


        protected override void newRound(int start_player_id) {
            throw new NotImplementedException();
        }

        public override void setJanlii(DominoTypes type) {
            throw new NotImplementedException();
        }

        protected override void evaluateTrick() {
            throw new NotImplementedException();
        }
    }
}
