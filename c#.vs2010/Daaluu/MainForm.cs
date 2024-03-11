using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Daaluu.Properties;
using Daaluu.Logic;
using System.Diagnostics;

namespace Daaluu
{
    public partial class Main : Form
    {
        private BufferedGraphics grafx;
        private BufferedGraphics grafx_stacks;
        private BufferedGraphicsContext context;
        private Timer frame_timer;
        private List<DObject> controls = new List<DObject>();

        public Main()
        {
            InitializeComponent();
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.splitContainer1.Panel1.Width*2 + 1, this.splitContainer1.Panel1.Height*2 + 1);
            grafx = context.Allocate(this.splitContainer1.Panel1.CreateGraphics(),
                 new Rectangle(0, 0, this.splitContainer1.Panel1.Width * 2, this.splitContainer1.Panel1.Height * 2));
            grafx_stacks = context.Allocate(this.pnlStacks.CreateGraphics(),
                 new Rectangle(0, 0, this.pnlStacks.Width, this.pnlStacks.Height));

            Dictionary<string, Bitmap> resources = new Dictionary<string, Bitmap>();
            resources.Add("table", Resources.table);
            resources.Add("crown", Resources.crown);
            Graphix.Initialize(resources);
            controls.Add(place);
            place.Click += new EventHandler<EventArgs>(place_Click);
            Graphix.controls = controls;

            frame_timer = new System.Windows.Forms.Timer();
            frame_timer.Interval = 30;
            frame_timer.Tick += new EventHandler(this.OnTimer);
            frame_timer.Start();


            this.splitContainer1.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(Graphix.panel1_MouseMove);
            this.splitContainer1.Panel1.MouseClick += new MouseEventHandler(Graphix.Panel1_MouseClick);
            this.btnReset.Click +=new EventHandler(btnReset_Click);

            this.pnlStacks.Paint += new PaintEventHandler(pnlStacks_Paint);

        }
        private DButton place = new DButton("гаргах", 450, 490, 100, 50);
        private void Main_Load(object sender, EventArgs e)
        {
            restart();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            //this.grafx.Render(e.Graphics);
        }

        void pnlStacks_Paint(object sender, PaintEventArgs e)
        {
            //this.grafx_stacks.Render(e.Graphics);
        }

        private void OnTimer(object sender, EventArgs e)
        {
            Graphix.DrawToBuffer(ref this.grafx, this.splitContainer1.Panel1.Size);
            Graphix.DrawStacksToBuffer(ref this.grafx_stacks, this.pnlStacks.Size);
            this.grafx.Render(Graphics.FromHwnd(this.splitContainer1.Panel1.Handle));
            this.grafx_stacks.Render(Graphics.FromHwnd(this.pnlStacks.Handle));
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            restart();
        }

        AGame game;
        UserPlayer user_player;
        private void restart()
        {
            place.Click -= new EventHandler<EventArgs>(place_Click);
            place.Click += new EventHandler<EventArgs>(place_Click);

            game = new LocalGame();
            user_player = new UserPlayer("Ган-Оч");
            Graphix.setGame(game);
            Graphix.setPlayer(user_player);
            game.StateChanged += new EventHandler<GameStateChangedEventArgs>(game_StatusChanged);
            user_player.HasSelectedChanged += new EventHandler<EventArgs>(user_player_HasSelectedChanged);

            //*
            int user_player_index = RandomExtensions.RandomInstance.Next(5);
            /*/
            int user_player_index = 0;
            //*/
            for (int i = 0; i < 5; i++)
            {
                if(user_player_index!=i)
                    game.seatNextPlayer(new RandomPlayer());
                else
                    game.seatNextPlayer(user_player);
            }

            // game.Start();

            #region debug
            (game as LocalGame).ResetAndShuffle();
            (game as LocalGame).setTurn(user_player_index);
            (game as LocalGame).State = GameState.ChoosingJanlii;
            #endregion
        }

        void user_player_HasSelectedChanged(object sender, EventArgs e)
        {
            if (user_player.MyTurn && (game.State == GameState.LeadTrick || (game.State == GameState.PlayTrick && ((user_player.SelectedCount == 1 && !game.Trick.IsDouble) || (user_player.SelectedCount == 2 && game.Trick.IsDouble)))))
            {
                place.Enabled = true;
            }
            else
                place.Enabled = false;
        }

        void place_Click(object sender, EventArgs e)
        {
            //Debug.WriteLine("clicked");
            user_player.placeSelected();
        }

        void game_StatusChanged(object sender, GameStateChangedEventArgs e)
        {
            if (user_player.MyTurn && (game.State == GameState.LeadTrick || game.State == GameState.PlayTrick) && user_player.SelectedCount > 0)
            {
                place.Enabled = true;
            }
            else
                place.Enabled = false;
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {

        }
    }
}
