using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daaluu.Logic;

namespace TestDaaluu
{

    [TestClass()]
    public class LogicTest
    {
        [TestMethod()]
        public void distributeGersTest()
        {
            RandomPlayer pl0 = new RandomPlayer();
            RandomPlayer pl1 = new RandomPlayer();
            RandomPlayer pl2 = new RandomPlayer();

            pl0.Ger.Add(DominoTypes.Daaluu);
            pl0.Ger.Add(DominoTypes.Uuluu);
            pl0.Ger.Add(DominoTypes.Uuluu);

            pl1.Ger.Add(DominoTypes.Gaval9);

            pl2.Ger.Add(DominoTypes.Yooz);
            pl2.Ger.Add(DominoTypes.Yooz);

            LocalGame game= new LocalGame();
            game.seatNextPlayer(pl0);
            game.seatNextPlayer(pl1);
            game.seatNextPlayer(pl2);


            game.distributeGers();
            game.payDebts();


            Assert.AreEqual(3, pl0.Tsai.List.Count);
            Assert.AreEqual(1, pl1.Tsai.List.Count);
            Assert.AreEqual(2, pl2.Tsai.List.Count);

            game.ResetAndShuffle();

            pl0.Ger.Add(DominoTypes.Daaluu);
            pl0.Ger.Add(DominoTypes.Uuluu);
            pl0.Ger.Add(DominoTypes.Uuluu);

            pl1.Ger.Add(DominoTypes.Gaval9);

            pl2.Ger.Add(DominoTypes.Yooz);
            pl2.Ger.Add(DominoTypes.Yooz);
            game.distributeGers();
            game.payDebts();
            Assert.AreEqual(4, pl0.Tsai.List.Count);
            Assert.AreEqual(0, pl1.Tsai.List.Count);
            Assert.AreEqual(2, pl2.Tsai.List.Count);

            game.ResetAndShuffle();

            pl0.Ger.Add(DominoTypes.Daaluu);
            pl0.Ger.Add(DominoTypes.Uuluu);
            pl0.Ger.Add(DominoTypes.Uuluu);

            pl2.Ger.Add(DominoTypes.Gaval9);
            pl2.Ger.Add(DominoTypes.Yooz);
            pl2.Ger.Add(DominoTypes.Yooz);
            game.distributeGers();
            game.payDebts();
            
            Assert.AreEqual(5, pl0.Tsai.Count);
            Assert.AreEqual(4, pl0.Tsai.Sum);

            Assert.AreEqual(2, pl1.Tsai.Count);
            Assert.AreEqual(-2, pl1.Tsai.Sum);

            Assert.AreEqual(3, pl2.Tsai.Count);
            Assert.AreEqual(2, pl2.Tsai.Sum);
        }
        [TestMethod()]
        public void stackDebtToAllTest()
        {
            RandomPlayer pl = new RandomPlayer();
            RandomPlayer pl1 = new RandomPlayer();
            RandomPlayer pl2 = new RandomPlayer();
            RandomPlayer pl3 = new RandomPlayer();
            RandomPlayer pl4 = new RandomPlayer();

            LocalGame game = new LocalGame();
            game.seatNextPlayer(pl);
            game.seatNextPlayer(pl1);
            game.seatNextPlayer(pl2);
            game.seatNextPlayer(pl3);
            game.seatNextPlayer(pl4);

            pl1.ReceiveTsai(pl.Tsai.giveTo(pl1));
            pl2.ReceiveTsai(pl.Tsai.giveTo(pl2));
            pl3.ReceiveTsai(pl.Tsai.giveTo(pl3));
            pl4.ReceiveTsai(pl.Tsai.giveTo(pl4));
            pl1.ReceiveTsai(pl.Tsai.giveTo(pl1));
            pl2.ReceiveTsai(pl.Tsai.giveTo(pl2));

            Assert.AreEqual(4, pl.Tsai.Count);
            Assert.AreEqual(-4, pl.Tsai.Sum);

            Assert.AreEqual(4, pl1.Tsai.Count);
            Assert.AreEqual(3, pl1.Tsai.Sum);

            Assert.AreEqual(4, pl2.Tsai.Count);
            Assert.AreEqual(3, pl2.Tsai.Sum);

            Assert.AreEqual(3, pl3.Tsai.Count);
            Assert.AreEqual(2, pl3.Tsai.Sum);

            Assert.AreEqual(3, pl4.Tsai.Count);
            Assert.AreEqual(2, pl4.Tsai.Sum);

            pl1.Ger.Add(DominoTypes.Daaluu);
            pl1.Ger.Add(DominoTypes.Daaluu);

            pl2.Ger.Add(DominoTypes.Uuluu);
            pl2.Ger.Add(DominoTypes.Uuluu);

            pl3.Ger.Add(DominoTypes.Bajgar10);
            pl3.Ger.Add(DominoTypes.Bajgar10);
            pl3.Ger.Add(DominoTypes.Degee9);

            pl4.Ger.Add(DominoTypes.Siiluu10);
            pl4.Ger.Add(DominoTypes.Siiluu10);
            pl4.Ger.Add(DominoTypes.Gaval9);

            game.distributeGers();
            game.payDebts();

            Assert.AreEqual(6, pl.Tsai.Count);
            Assert.AreEqual(-6, pl.Tsai.Sum);

            Assert.AreEqual(4, pl1.Tsai.Count);
            Assert.AreEqual(3, pl1.Tsai.Sum);

            Assert.AreEqual(4, pl2.Tsai.Count);
            Assert.AreEqual(3, pl2.Tsai.Sum);

            Assert.AreEqual(4, pl3.Tsai.Count);
            Assert.AreEqual(2, pl3.Tsai.Sum);

            Assert.AreEqual(4, pl4.Tsai.Count);
            Assert.AreEqual(2, pl4.Tsai.Sum);

            Assert.AreEqual(6, pl.Tsai.Count);
            Assert.AreEqual(-6, pl.Tsai.Sum);

        }

        [TestMethod()]
        public void debtPaymentTest()
        {
            RandomPlayer pl0 = new RandomPlayer();
            RandomPlayer pl1 = new RandomPlayer();
            RandomPlayer pl2 = new RandomPlayer();

            pl0.Tsai.List.Clear();
            pl1.ReceiveTsai(new Tsai(pl1));
            pl1.ReceiveTsai(pl0.Tsai.giveTo(pl1));
            pl2.ReceiveTsai(new Tsai(pl2));
            
            Assert.AreEqual(1, pl0.Tsai.List.Count);
            Assert.AreEqual(-1, pl0.Tsai.Sum);
            Assert.AreEqual(4, pl1.Tsai.List.Count);
            Assert.AreEqual(3, pl2.Tsai.List.Count);

            pl0.Ger.Add(DominoTypes.Daaluu);
            pl0.Ger.Add(DominoTypes.Uuluu);
            pl0.Ger.Add(DominoTypes.Uuluu);

            pl1.Ger.Add(DominoTypes.Gaval9);

            pl2.Ger.Add(DominoTypes.Yooz);
            pl2.Ger.Add(DominoTypes.Yooz);

            LocalGame game = new LocalGame();
            game.seatNextPlayer(pl0);
            game.seatNextPlayer(pl1);
            game.seatNextPlayer(pl2);

            game.distributeGers();

            Assert.AreEqual(2, pl0.Tsai.Count); //1 buten, 1 ur
            Assert.AreEqual(0, pl0.Tsai.Sum);
            Assert.AreEqual(3, pl1.Tsai.Count); //2 buten, 1 avlaga
            Assert.AreEqual(2, pl1.Tsai.Sum); 
            Assert.AreEqual(3, pl2.Tsai.Count); //3 buten


            game.payDebts();
            Assert.AreEqual(0, pl0.Tsai.List.Count); // hooson
            Assert.AreEqual(3, pl1.Tsai.List.Count); // 3 buten
            Assert.AreEqual(3, pl1.Tsai.Sum); // 3 buten
            Assert.AreEqual(3, pl2.Tsai.List.Count); // 3 buten
        }

        [TestMethod()]
        public void receiveTsaiWhenInDebt()
        {
            RandomPlayer pl = new RandomPlayer();
            RandomPlayer pl0 = new RandomPlayer();
            pl.Tsai.List.Clear();
            pl.Tsai.giveTo(pl0);

            Assert.AreEqual(1, pl.Tsai.List.Count);
            Assert.AreEqual(-1, pl.Tsai.Sum);


            pl.ReceiveTsai(new Tsai(pl0));
            Assert.AreEqual(2, pl.Tsai.List.Count);
            Assert.AreEqual(0, pl.Tsai.Sum);

            pl.payDebts();
            Assert.AreEqual(0, pl.Tsai.Count);
            Assert.AreEqual(0, pl.Tsai.Sum);
        }

        [TestMethod()]
        public void cancelDebtByDebtTest() {
            RandomPlayer pl0 = new RandomPlayer("p0");
            RandomPlayer pl1 = new RandomPlayer("p1");

            //pl0.Tsai.List.Clear();
            pl1.ReceiveTsai(pl0.Tsai.giveTo(pl1));
            pl1.ReceiveTsai(pl0.Tsai.giveTo(pl1));
            pl1.ReceiveTsai(pl0.Tsai.giveTo(pl1));
            Assert.AreEqual(1, pl0.Tsai.Count);
            Assert.AreEqual(-1, pl0.Tsai.Sum);

            Assert.AreEqual(5, pl1.Tsai.Count);
            Assert.AreEqual(4, pl1.Tsai.Sum);

            pl1.Tsai.List.Clear();
            pl1.ReceiveTsai(new Tsai(0, pl0)); //1гийн авлагатай
            pl1.ReceiveTsai(pl0.Tsai.giveTo(pl1));

            Assert.AreEqual(0, pl0.Tsai.Count);
            Assert.AreEqual(0, pl1.Tsai.Count);
        }

        [TestMethod()]
        public void distributeGersByDebtFirst()
        {
            RandomPlayer pl0 = new RandomPlayer();
            RandomPlayer pl1 = new RandomPlayer();
            RandomPlayer pl2 = new RandomPlayer();


            LocalGame game = new LocalGame();
            game.seatNextPlayer(pl0);
            game.seatNextPlayer(pl1);
            game.seatNextPlayer(pl2);

            pl2.ReceiveTsai(pl0.Tsai.giveTo(pl2));
            pl1.ReceiveTsai(pl0.Tsai.giveTo(pl1));


            Assert.AreEqual(0, pl0.Tsai.Count);
            Assert.AreEqual(0, pl0.Tsai.Sum);

            Assert.AreEqual(3, pl1.Tsai.Count);
            Assert.AreEqual(3, pl1.Tsai.Sum);


            Assert.AreEqual(3, pl2.Tsai.Count);
            Assert.AreEqual(3, pl2.Tsai.Sum);



            pl2.ReceiveTsai(pl0.Tsai.giveTo(pl2));


            Assert.AreEqual(1, pl0.Tsai.Count);
            Assert.AreEqual(-1, pl0.Tsai.Sum);

            Assert.AreEqual(4, pl2.Tsai.Count);
            Assert.AreEqual(3, pl2.Tsai.Sum);

            pl0.Ger.Add(DominoTypes.Daaluu);
            pl0.Ger.Add(DominoTypes.Daaluu);
            pl0.Ger.Add(DominoTypes.Uuluu);

            pl2.Ger.Add(DominoTypes.Uuluu);

            pl1.Ger.Add(DominoTypes.Daaluu);
            pl1.Ger.Add(DominoTypes.Uuluu);

            game.distributeGers();
            game.payDebts();


            Assert.AreEqual(0, pl0.Tsai.Sum);
            Assert.AreEqual(0, pl0.Tsai.Count);

            Assert.AreEqual(3, pl1.Tsai.Count);
            Assert.AreEqual(3, pl1.Tsai.Sum);

            Assert.AreEqual(3, pl2.Tsai.Count);
            Assert.AreEqual(3, pl2.Tsai.Sum);
        }

        [TestMethod()]
        public void receiveDebtPayment()
        {
            RandomPlayer pl = new RandomPlayer();
            RandomPlayer pl0 = new RandomPlayer();
            pl.Tsai.List.Clear();
            pl.ReceiveTsai(new Tsai(0, pl0));

            Assert.AreEqual(1, pl.Tsai.List.Count);
            Assert.AreEqual(0, pl.Tsai.Sum);
            Assert.ReferenceEquals(pl.Tsai.List[0].Owner, pl0);

            pl.ReceiveTsai(new Tsai(pl0));
            Assert.AreEqual(1, pl.Tsai.Sum);
            Assert.AreEqual(1, pl.Tsai.List.Count);
        }

        [TestMethod()]
        public void removeIouTest()
        {
            RandomPlayer pl = new RandomPlayer();
            RandomPlayer pl0 = new RandomPlayer();
            pl0.Name = "debtor";
            TsaiCollection tsais = new TsaiCollection(pl);
            tsais.List.Clear();

            tsais.Receive(new Tsai(0, pl0)); //add iou
            Assert.AreEqual(0, tsais.Sum);
            Assert.AreEqual(1, tsais.Count);

            Assert.ReferenceEquals(tsais.List[0].Owner, pl0);
            Assert.AreEqual(0, tsais.List[0].Value);

            tsais.Receive(new Tsai(pl0)); //should remove iou and set value to 1
            Assert.AreEqual(1, tsais.Sum);
            Assert.AreEqual(1, tsais.Count);
        }

        [TestMethod()]
        public void comparePlayersTest()
        {
            RandomPlayer pl = new RandomPlayer();

            List<APlayer> players0 = new List<APlayer>();
            List<APlayer> players1 = new List<APlayer>();

            players0.Add(pl);
            players1.Add(pl);

            Assert.IsTrue(players1[0] == players1[0]); 
        }
    }
}
