using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Daaluu.Logic;
using Daaluu.Animation2D;

using System.Drawing;
using Plasmoid.Extensions;
using System.Diagnostics;
using System.Resources;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;

namespace Daaluu
{
    public class Graphix
    {
        public static Point midpoint = new Point(286, 286);
        public static Point mouseLocation = new Point();
        public static PointF scaledMouseLocation = new PointF();
        private static AGame _game = null;
        private static UserPlayer _player = null;

        public static readonly SizeF DominoSize = new SizeF(24f, 52f);
        public static readonly float StackRadius = 119f;
        public static readonly SizeF Point = new SizeF(6.5f, 6.5f);
        public static readonly float PointRadius = 3.5f;

        public static readonly float BottomOffset = 49 / 2;

        public static readonly float Col2Spacing = 10f;
        public static readonly float Col20 = 3.5f;
        public static readonly float Col21 = Col20 + Col2Spacing;

        public static readonly float Row3Spacing = 7.5f;
        public static readonly float Row30 = 3;
        public static readonly float Row31 = Row30 + Row3Spacing;
        public static readonly float Row32 = Row31 + Row3Spacing;

        public static readonly float Col10 = 8.5f;

        public static readonly float Row2Spacing = 10.5f;
        public static readonly float Row20 = 5;
        public static readonly float Row21 = Row20 + Row2Spacing;
        public static readonly Color HoverBorderColor = Color.FromArgb(95, 142, 92);

        private static readonly int j_container_width = (30 + 10) * 8 + 20;
        private static readonly int j_container_height = 58 + 20;
        private static readonly int j_container_x = midpoint.X - j_container_width / 2;
        private static readonly int j_container_y = midpoint.Y - j_container_height / 2;

        private static readonly int c_container_width = 115 + 20;
        private static readonly int c_container_height = 50 + 20;

        private static Dictionary<string, Bitmap> _resources = new Dictionary<string, Bitmap>();

        public static bool WaitForColorSelect = false;

        static Graphix()
        {
            List<ADomino> set = DominoFactory.getSet();
        }

        public static void setGame(AGame game)
        {
            _game = game;
        }

        public static void setPlayer(UserPlayer player)
        {
            _player = player;
        }

        public static void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            setMouseLocation(e.Location);
            Point[] temp = { e.Location };
            PointF scaled_Location = e.Location;
            using (Matrix m = new Matrix())
            {
                m.Scale(1 / prev_scale, 1 / prev_scale);
                m.TransformPoints(temp);
                scaled_Location = temp[0];
                scaledMouseLocation = temp[0];
            }

            int index = 0;
            if (_game != null)
            {
                if (_player != null && _player.MyTurn)
                {
                    if (_game.State == GameState.ChoosingJanlii)
                    {
                        index = 0;
                        foreach (ADomino domino in _game.Janliis)
                        {
                            if (new RectangleF(j_container_x + 15 + index * (30 + 10), j_container_y + 10, 30, 58).Contains(scaled_Location))
                            {
                                domino.isHovered = true;
                            }
                            else
                            {
                                domino.isHovered = false;
                            }
                            index++;
                        }
                    }
                    else if (_game.State == GameState.ChoosingStacks)
                    {
                        if (new RectangleF(midpoint.X + 90, midpoint.Y - 15, 58, 30).Contains(scaled_Location))
                            _game.ShuffledStack[0].isHovered = true;
                        else
                            _game.ShuffledStack[0].isHovered = false;

                        using (Matrix reverse_m = new Matrix())
                        {
                            for (index = 1; index < 10; index++)
                            {
                                reverse_m.RotateAt(-360f / 10, midpoint, MatrixOrder.Append);
                                PointF[] temp1 = { scaled_Location };
                                reverse_m.TransformPoints(temp1);
                                PointF mlocation_relative_to_rect = temp1[0];
                                if (new RectangleF(midpoint.X + 90, midpoint.Y - 15, 58, 30).Contains(mlocation_relative_to_rect))
                                    _game.ShuffledStack[index].isHovered = true;
                                else
                                    _game.ShuffledStack[index].isHovered = false;
                            }
                        }
                    }

                }

                if (_player != null)
                {
                    if (_game.State == GameState.PlayTrick || _game.State == GameState.LeadTrick || _game.State == GameState.DistributingStacks)
                    {
                        int i = 0;
                        foreach (ADomino domino in _player.Hand)
                        {
                            domino.isHovered = new RectangleF(140 + 8 + 28 * (i % 10), (domino.Selected ? -10 : 0) + 500 + 56 * (i / 10), DominoSize.Width, DominoSize.Height).Contains(scaled_Location);
                            i++;
                        }
                    }
                }

                foreach (DObject obj in controls)
                {
                    obj.Hovered = new RectangleF(obj.Coordinates, obj.Size).Contains(scaled_Location);
                }
            }
        }

        public static List<DObject> controls;
        public static void Panel1_MouseClick(object sender, MouseEventArgs e)
        {

            setMouseLocation(e.Location);
            Point[] temp = { e.Location };
            PointF scaled_Location = e.Location;
            using (Matrix m = new Matrix())
            {
                m.Scale(1 / prev_scale, 1 / prev_scale);
                m.TransformPoints(temp);
                scaled_Location = temp[0];
            }

            if (_game != null && _player != null && _player.MyTurn)
            {

                if (_game.State == GameState.ChoosingJanlii)
                {
                    foreach (ADomino domino in _game.Janliis)
                    {
                        if (domino.isHovered)
                        {
                            DominoEventArgs de = new DominoEventArgs(domino.DominoType, -1);
                            _player.OnJanliiChosen(de);
                            domino.isHovered = false;
                            break;
                        }
                    }
                }
                else if (_game.State == GameState.ChoosingStacks)
                {
                    int i = 0;
                    foreach (ShuffledStack stack in _game.ShuffledStack)
                    {
                        if (stack.isHovered)
                        {
                            StackEventArgs se = new StackEventArgs(i);
                            _player.OnStackChosen(se);
                            stack.isHovered = false;
                            break;
                        }
                        i++;
                    }
                }
                else if (WaitForColorSelect && _game.State == GameState.LeadTrick)
                {
                    if (new RectangleF(midpoint.X - c_container_width / 2 + 10, midpoint.Y - c_container_height / 2 + 10, 50, 50).Contains(scaled_Location))
                    {
                        _player.placeSelected(DominoColor.Red);
                    }
                    else if (new RectangleF(midpoint.X - c_container_width / 2 + 5 + c_container_width / 2, midpoint.Y - c_container_height / 2 + 10, 50, 50).Contains(scaled_Location))
                    {
                        _player.placeSelected(DominoColor.White);
                    }

                }
            }

            if (_game != null && _player != null)
            {
                if (_game.State == GameState.PlayTrick || _game.State == GameState.LeadTrick)
                {
                    int i = 0;
                    foreach (ADomino domino in _player.Hand)
                    {
                        if (domino.isHovered)
                        {
                            if (!domino.Selected)
                            {
                                _player.selectDomino(i);
                            }
                            else
                            {
                                _player.deselectDomino(i);
                            }
                            break;
                        }
                        i++;
                    }
                }
            }



            foreach (DObject obj in controls)
            {
                if (obj.Hovered && obj.Enabled)
                {
                    obj.Clicked();
                }
            }
        }

        private static void setMouseLocation(Point mlocation) {
            mouseLocation = mlocation;
        }

        public static void Initialize(Dictionary<string, Bitmap> resources)
        {
            _resources = resources;
            //rng.Shuffle(set);
        }


        public static void DrawStacksToBuffer(ref BufferedGraphics bg, Size size)
        {
            Graphics g = bg.Graphics;
            //PointF scaled_midpoint = new PointF(midpoint.X, midpoint.Y);
            g.FillRectangle(Brushes.White, 0, 0, size.Width, size.Height);
            for (int i = 0; i < _game.ShuffledStack.Length; i++)
            {
                //Debug.WriteLine("i: " + i);
                if (_game.ShuffledStack[i].isHovered)
                {
                    g.FillRoundedRectangle(new SolidBrush(Color.FromArgb(59, 196, 255)), 7, 7 + 56 * i, (DominoSize.Width + 2) * 5 + 4, (DominoSize.Height + 6), 5);
                }
                else if (i < 5 && _game.ShuffledStack[i + 5].isHovered || i > 4 && _game.ShuffledStack[i - 5].isHovered)
                {
                    g.FillRoundedRectangle(new SolidBrush(Color.FromArgb(161, 227, 255)), 7, 7 + 56 * i, (DominoSize.Width + 2) * 5 + 4, (DominoSize.Height + 6), 5);
                    g.DrawRoundedRectangle(new Pen(Brushes.Black, 0.5f), 7, 7 + 56 * i, ((DominoSize.Width + 2) * 5) + 4, DominoSize.Height + 6, 5);
                }
                for (int k = 0; k < _game.ShuffledStack[i].Contents.Length; k++)
                {
                    DrawLargeDomino(ref g, _game.ShuffledStack[i].Contents[k].DominoType, 10 + (DominoSize.Width + 2) * (k % 10), 10 + 56 * i);

                    DominoTypes type = _game.ShuffledStack[i].Contents[k].DominoType;

                    //highlight hovered selection Janlii
                    for (int j = 0; j < _game.Janliis.Length; j++)
                    {
                        ADomino domino = _game.Janliis[j];
                        if (domino.DominoType == type && domino.isHovered)
                        {
                            g.FillRoundedRectangle(new SolidBrush(Color.FromArgb(127, 161, 227, 255)),
                                8 + (DominoSize.Width + 2) * (k % 10), 8 + 56 * i,
                                (DominoSize.Width + 4), (DominoSize.Height + 5), 5);
                            break;
                        }
                    }
                }
            }
        }

        private static float prev_scale = 1f;
        public static void DrawToBuffer(ref BufferedGraphics bg, Size size)
        {
            //591, 742
            SizeF original = new SizeF(591, 742);
            Graphics g = bg.Graphics;
            PointF scaled_midpoint = new PointF(midpoint.X, midpoint.Y);
            using (Matrix scale = new Matrix())
            {
                scale.Scale(size.Width / original.Width, size.Width / original.Width);
                if (prev_scale != (size.Width / original.Width))
                {
                    prev_scale = size.Width / original.Width;
                    PointF[] temp = { midpoint };
                    scale.TransformPoints(temp);
                    scaled_midpoint = temp[0];
                }
                prev_scale = size.Width / original.Width;

                g.ScaleTransform(prev_scale, prev_scale);
                GraphicsState scaledState = g.Save();
                g.FillRectangle(Brushes.White, 0, 0, size.Width, size.Height);
                g.DrawImage(_resources["table"], 70, 60);

                int max = 10;

                Pen color;// = new Pen(_game.ShuffledStack[0].isHovered ? Color.White : HoverBorderColor);
                //g.DrawRoundedRectangle(color, midpoint.X + 90, midpoint.Y - 15, 58, 30, 5);
                using (Matrix m = new Matrix())
                {
                    m.Multiply(scale); //must be set for new Matrix
                    for (int i = 0; i < max; i++)
                    {
                        color = new Pen(_game != null && i < _game.ShuffledStack.Length && _game.ShuffledStack[i].isHovered && _game.State == GameState.ChoosingStacks ? Color.White : HoverBorderColor);

                        g.DrawRoundedRectangle(color, midpoint.X + 90, midpoint.Y - 15, 58, 30, 5);
                        m.RotateAt((360f / max), scaled_midpoint, MatrixOrder.Append);
                        g.Transform = m;
                    }
                }
                g.ResetTransform();
                g.Restore(scaledState);
                scaledState = g.Save();

                if (_game != null)
                {
                    /***** DRAW PLAYERS ***/

                    int offset = 8 - _player.Position;

                    for (int player_index = 0; player_index < 5; player_index++)
                    {
                        DrawPlayer(ref g, _game.Players[player_index], player_coordinates[(player_index + offset) % 5].X, player_coordinates[(player_index + offset) % 5].Y, scale);

                        g.Restore(scaledState);
                        scaledState = g.Save();
                    }
                    //Тухайн тоглолтын Жанлий
                    using (Matrix m = new Matrix())
                    {
                        m.Multiply(scale); //must be set for new Matrix
                        m.Scale(.7f, .7f, MatrixOrder.Append);
                        g.Transform = m;
                        DrawLargeDomino(ref g, _game.CurrentRoundJanlii, midpoint.X * (1 / .7f) - 13, 115);

                        g.ResetTransform();
                        g.Restore(scaledState);
                        scaledState = g.Save();
                        g.DrawImage(_resources["crown"], midpoint.X - 2, 68);
                    }

                    /**
                     * Draw Stacks on table
                     */
                    if (_game.State == GameState.ChoosingStacks || _game.State == GameState.ChoosingJanlii || _game.State == GameState.DistributingStacks)
                    {
                        for (int i = 0; i < max && i < _game.ShuffledStack.Length; i++)
                        {
                            using (Matrix m = new Matrix())
                            {
                                PointF domino_mid = _game.ShuffledStack[i].Coordinates;
                                float angle = (float)_game.ShuffledStack[i].Angle;

                                PointF[] temp0 = { domino_mid };
                                scale.TransformPoints(temp0);
                                PointF scaled_domino_mid = temp0[0];

                                m.Multiply(scale); //must be set for new Matrix
                                m.RotateAt(angle, scaled_domino_mid, MatrixOrder.Append);
                                g.Transform = m;
                                DrawDominoBase(ref g, domino_mid.X - _game.ShuffledStack[i].Size.Width / 2, domino_mid.Y - _game.ShuffledStack[i].Size.Height / 2);
                                g.ResetTransform();
                            }
                        }
                        g.Restore(scaledState);
                        scaledState = g.Save();
                    }

                    if (_game.State == GameState.LeadTrick || _game.State == GameState.PlayTrick) {
                        if (_game.State == GameState.PlayTrick) {
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            // using (Matrix m = new Matrix()) {
                            //    m.Multiply(scale); //must be set for new Matrix
                            //    g.Transform = m;
                            g.FillEllipse((_game.Trick.Color == DominoColor.Red ? Brushes.Red : Brushes.White), midpoint.X - DominoSize.Width - Graphix.Point.Width / 2 - 2, midpoint.Y - DominoSize.Height / 2 - Graphix.Point.Height / 2 - 2, Graphix.Point.Width, Graphix.Point.Height);
                            //}
                            g.ResetTransform();
                            g.Restore(scaledState);
                            scaledState = g.Save();
                        }

                        for (int i = 0; i < _game.Trick.Stack.Count; i++)
                        {
                            using (Matrix m = new Matrix())
                            {
                                ADomino d = _game.Trick.Stack[i][0];
                                PointF domino_mid = d.Coordinates;
                                float angle = (float)_game.Trick.Stack[i][0].Angle;


                                PointF[] temp0 = { domino_mid };
                                scale.TransformPoints(temp0);
                                PointF scaled_domino_mid = temp0[0];

                                m.Multiply(scale); //must be set for new Matrix
                                m.RotateAt(angle, scaled_domino_mid, MatrixOrder.Append);
                                g.Transform = m;

                                //out of range walkaround


                                if (!_game.Trick.IsDouble)
                                {
                                    DrawLargeDomino(ref g, d.DominoType, domino_mid.X - d.Size.Width / 2, domino_mid.Y - d.Size.Height / 2);
                                }
                                else
                                {
                                    DrawLargeDomino(ref g, d.DominoType, domino_mid.X - d.Size.Width, domino_mid.Y - d.Size.Height / 2);
                                    if (_game.Trick.Stack.Count > i && _game.Trick.Stack[i].Count > 1)
                                    {
                                        DrawLargeDomino(ref g, _game.Trick.Stack[i][1].DominoType, domino_mid.X, domino_mid.Y - _game.Trick.Stack[i][0].Size.Height / 2);
                                    }
                                }
                                g.ResetTransform();
                            }
                        }

                        g.Restore(scaledState);
                        scaledState = g.Save();
                    }


                    // Draw Hand
                    if (_player != null)
                    {
                        int i = 0;
                        try
                        {
                            int doffset = 0;
                            foreach (ADomino domino in _player.Hand)
                            {
                                if (domino.Selected)
                                    doffset = -5;
                                else
                                    doffset = 0;
                                if (domino.isHovered)
                                {
                                    g.FillRoundedRectangle(Brushes.Gray, 140f + 6 + 28 * (i % 10), doffset + 478f + 56 * (i / 10), 28f, 57f, 5f);
                                }
                                DrawLargeDomino(ref g, domino.DominoType, 140 + 8 + 28 * (i % 10), doffset + 480 + 56 * (i / 10));
                                i++;
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            Debug.WriteLine("Invalid Operation!!!, READIN locked List<>");
                        }
                    }


                    foreach (DObject obj in controls)
                    {

                        if (obj.Type == DTypes.Button)
                        {
                            DButton btn = (DButton)obj;

                            g.DrawRoundedRectangle(new Pen(Brushes.LightGray), btn.Coordinates.X - 1, btn.Coordinates.Y - 1, btn.Size.Width + 2, btn.Size.Height + 2, 5);
                            g.FillRoundedRectangle(btn.Enabled ? Brushes.White : Brushes.LightGray, new Rectangle(btn.Coordinates, btn.Size), 5);
                            g.DrawString(btn.Text, new Font("Tahoma", 12), Brushes.Black, btn.Coordinates.X + 10, btn.Coordinates.Y + 5);
                        }
                    }

                    /** OVERLAYS***/
                    if ((_game.State == GameState.PlayTrick || _game.State == GameState.LeadTrick) && WaitForColorSelect && (_player != null && _player.MyTurn))
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(204, 0, 0, 0)), 0, 0, size.Width, size.Height);
                        g.FillRoundedRectangle(new SolidBrush(Color.FromArgb(255, 54, 100, 51)), new Rectangle(midpoint.X - c_container_width / 2, midpoint.Y - c_container_height / 2, c_container_width, c_container_height), 5);
                        g.DrawRoundedRectangle(new Pen(Color.FromArgb(95, 142, 92)), new Rectangle(midpoint.X - c_container_width / 2 - 1, midpoint.Y - c_container_height / 2 - 1, c_container_width + 2, c_container_height + 2), 6);

                        g.FillRoundedRectangle(Brushes.Red, new Rectangle(midpoint.X - c_container_width / 2 + 10, midpoint.Y - c_container_height / 2 + 10, 50, 50), 5);
                        g.FillRoundedRectangle(Brushes.White, new Rectangle(midpoint.X - c_container_width / 2 + 5 + c_container_width / 2, midpoint.Y - c_container_height / 2 + 10, 50, 50), 5);

                    }

                    if (_game.State == GameState.ChoosingJanlii && (_player != null && _player.MyTurn))
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(204, 0, 0, 0)), 0, 0, size.Width, size.Height);
                        g.FillRoundedRectangle(new SolidBrush(Color.FromArgb(255, 54, 100, 51)), new Rectangle(j_container_x, j_container_y, j_container_width, j_container_height), 5);
                        g.DrawRoundedRectangle(new Pen(Color.FromArgb(95, 142, 92)), new Rectangle(j_container_x - 1, j_container_y - 1, j_container_width + 2, j_container_height + 2), 6);

                        int index = 0;
                        foreach (ADomino domino in _game.Janliis)
                        {
                            color = new Pen(domino.isHovered ? Color.White : HoverBorderColor);
                            g.DrawRoundedRectangle(color, j_container_x + 15 + index * (30 + 10), j_container_y + 10, 30, 58, 5);
                            DrawLargeDomino(ref g, domino.DominoType, j_container_x + 18 + index * (30 + 10), j_container_y + 13);
                            index++;
                        }
                    }

                    /*
                    int offset = 2 - _player.Position;
                    for (int player_index = 0; player_index < _game.PlayerCount; player_index++)
                    {
                        DrawPlayer(ref g, _game.Players[player_index], player_coordinates[player_index + offset].X, player_coordinates[player_index].Y);
                    }
                     */
                }

                /*
                g.FillRectangle(new SolidBrush(Color.FromArgb(204, 0, 0, 0)), 0, 0, size.Width, size.Height);
                for (int i = 0; i < DominoFactory.PlayableSet.Count; i++)
                {
                    //int index = rng.Next(50)+10;
                    DrawLargeDomino(ref g, DominoFactory.PlayableSet[i], 140 + 8 + 28 * (i % 10), 350 + 56 * (i / 10));
                    //DominoFactory.getSet()[i].(ref g);
                }
                //*/

                /*
                DrawLargeDomino(ref g, DominoTypes.Yooz, 418 / 2 + 77 - 24 / 2 - 12, 80 + 418 / 2 - 29);
                DrawLargeDomino(ref g, DominoTypes.Yooz, 418 / 2 + 77 - 24 / 2 + 12, 80 + 418 / 2 - 29);
                //*/

                /*
#if DEBUG       
                g.FillEllipse(Brushes.Red, midpoint.X - 3, midpoint.Y - 3, 5, 5);

                Point top  = new Point(287, 100);


                using (Matrix m = new Matrix())
                {

                    m.Multiply(scale);

                    m.RotateAt(-36, scaled_midpoint, MatrixOrder.Append);
                    g.Transform = m;
                    g.DrawEllipse(Pens.Red, top.X - 2, top.Y - 2, 3, 3);

                    m.RotateAt(72, scaled_midpoint, MatrixOrder.Append);
                    g.Transform = m;
                    g.DrawEllipse(Pens.Red, top.X - 2, top.Y - 2, 3, 3);
                    //g.ResetTransform();

                    m.RotateAt(72, scaled_midpoint, MatrixOrder.Append);
                    g.Transform = m;
                    g.DrawEllipse(Pens.Red, top.X - 2, top.Y - 2, 3, 3);
                    //g.ResetTransform();

                    m.RotateAt(72, scaled_midpoint, MatrixOrder.Append);
                    g.Transform = m;
                    g.DrawEllipse(Pens.Red, top.X - 2, top.Y - 2, 3, 3);
                    //g.ResetTransform();

                    m.RotateAt(72, scaled_midpoint, MatrixOrder.Append);
                    g.Transform = m;
                    g.DrawEllipse(Pens.Red, top.X - 2, top.Y - 2, 3, 3);
                    //g.ResetTransform();
                }

                foreach (PointF pt in player_domino_coordinates)
                {
                    using (Matrix m = new Matrix())
                    {
                        m.Multiply(scale);

                        g.Transform = m;
                        g.DrawEllipse(Pens.Green, pt.X, pt.Y, 3, 3);
                    }
                }
                
#endif
                // * */
                g.ResetTransform();
            }

        }

        public static void DrawPlayer(ref Graphics g, APlayer player, int x, int y, Matrix scale)
        {
            DrawPlayer(ref g, player, (float)x, (float)y, scale);
        }

        public static void DrawPlayer(ref Graphics g, APlayer player, float x, float y, Matrix main)
        {
            if (player.MyTurn)
                g.FillRoundedRectangle(Brushes.Green, new RectangleF(x - 7, y - 7, 104f, 94f), 7);
            else
                g.DrawRoundedRectangle(Pens.Gray, new RectangleF(x - 5, y - 5, 100f, 90f), 5);
            g.FillRoundedRectangle(Brushes.White, new RectangleF(x - 5, y - 5, 100f, 90f), 5);
            g.FillRoundedRectangle(player.Position == _player.Position ? Brushes.Black : Brushes.Gray, new RectangleF(x, y, 50f, 50f), 5);
            g.DrawString(player.Name, new Font("Tahoma", 16), Brushes.Black, x, y + 55);


            DrawPlayerGers(ref g, player, main);

            //Draw Tsai
            int index = 0;
            using (Matrix m = new Matrix())
            {
                m.Scale(.6f, .6f);
                m.Multiply(main);
                g.Transform = m;

                using (Matrix reverse_m = new Matrix())
                {
                    reverse_m.Scale(1 / .6f, 1 / .6f);
                    PointF[] temp = { new PointF(x, y) };
                    //g.ScaleTransform(.7f, .7f);
                    reverse_m.TransformPoints(temp);
                    Tsai[] tmp = new Tsai[player.Tsai.List.Count];
                    player.Tsai.List.CopyTo(tmp);
                    foreach (Tsai t in tmp)
                    {
                        Brush b;
                        if (t.Value < 0)
                            b = Brushes.Red;
                        else if (t.Value == 0)
                            b = Brushes.Green;
                        else
                            b = Brushes.Black;
                        DrawDominoBase(ref g, b, temp[0].X - 10 + (DominoSize.Width + 2) * index, temp[0].Y - 67);
                        index++;
                    }
                }
            }
        }


        public static int gerOffsetRadius = 120;
        public static void DrawPlayerGers(ref Graphics g, APlayer player, Matrix main)
        {

            Double playerAngle = Graphix.player_domino_angles[player.Seat];

            // Draw Ger
            int index = 0;

            List<DominoTypes> tmp = new List<DominoTypes>(player.Ger);
            int width = (int)(DominoSize.Width + 1) * tmp.Count - 1;


            foreach (DominoTypes ger in tmp)
            {
                using (Matrix rotation = new Matrix())
                {
                    rotation.Multiply(main);
                    rotation.RotateAt((float)playerAngle, midpoint);
                    rotation.Translate(0,(float)gerOffsetRadius);
                    g.Transform = rotation;
                    DrawLargeDomino(ref g, ger, midpoint.X + (DominoSize.Width + 1) * index - width/2,midpoint.Y);
                }
                index++;
            }

            g.Transform = main;
        }

        public static void DrawDominoBase(ref Graphics g, int x, int y)
        {
            Graphix.DrawDominoBase(ref g, (float)x, (float)y);
        }
        public static void DrawDominoBase(ref Graphics g, float x, float y)
        {
            //48 x 104
            Graphix.DrawDominoBase(ref g, Brushes.Black, x, y);
        }
        public static void DrawDominoBase(ref Graphics g, Brush b, float x, float y)
        {
            //48 x 104
            g.FillRoundedRectangle(b, new RectangleF(x, y, DominoSize.Width, DominoSize.Height), 5);
        }

        public static void DrawLargeDomino(ref Graphics g, DominoTypes type, int x, int y)
        {
            Graphix.DrawLargeDomino(ref g, type, (float)x, (float)y);
        }

        public static void DrawLargeDomino(ref Graphics g, DominoTypes type, float x, float y)
        {
            DrawDominoBase(ref g, x, y);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            switch (type)
            {
                default:
                case DominoTypes.None:
                    break;

                case DominoTypes.Daaluu:
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);


                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Uuluu:
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Hunid8:
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Yooz:
                    g.FillEllipse(Brushes.Red, x + Graphix.Col10, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col10, y + Graphix.BottomOffset + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Nohoi6:
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);
                    break;
                
                case DominoTypes.Gaval9:
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    break;

                case DominoTypes.Tsahilgaan6:
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Shanaga7:
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Sarlag7:
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);


                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Chu5:
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Vand4:
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Bajgar10:
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Siiluu10:
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col21, y + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.Red, x + Graphix.Col20, y + Graphix.Row21, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Buluu6:
                    g.FillEllipse(Brushes.Red, x + Graphix.Col10, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Gozgor7:
                    g.FillEllipse(Brushes.Red, x + Graphix.Col10, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Buhuun4:
                    g.FillEllipse(Brushes.Red, x + Graphix.Col10, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Murii8:
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);


                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Hana8:
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row20, Graphix.Point.Width, Graphix.Point.Height);


                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;

                case DominoTypes.Degee9:
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col10, y + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col21, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);

                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row30, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row31, Graphix.Point.Width, Graphix.Point.Height);
                    g.FillEllipse(Brushes.White, x + Graphix.Col20, y + Graphix.BottomOffset + Graphix.Row32, Graphix.Point.Width, Graphix.Point.Height);
                    break;
            }
            g.SmoothingMode = SmoothingMode.Default;
        }


        public static void DrawSmallBlankDomino(ref Graphics g, int x, int y)
        {
            Graphix.DrawSmallBlankDomino(ref g, (float)x, (float)y);
        }
        public static void DrawSmallBlankDomino(ref Graphics g, float x, float y)
        {
            //48 x 104
            g.FillRoundedRectangle(Brushes.Black, new RectangleF(x, y, 12f, 26f), 2);
        }

        public static readonly Point[] player_coordinates = 
        {
            new Point(60,40),
            new Point(437,40),
            new Point(482,350),
            new Point(220,580),
            new Point(10,350)
        };

        public static readonly PointF[] player_domino_coordinates = 
        {
            new PointF(midpoint.X - 111, midpoint.Y - 152.5f),
            new PointF(midpoint.X + 108.5f, midpoint.Y - 152.5f),
            new PointF(midpoint.X + 175.7f, midpoint.Y + 56.5f),
            new PointF(midpoint.X - 2, midpoint.Y + 185),
            new PointF(midpoint.X - 179, midpoint.Y + 55.5f)
        };

        public static readonly double[] player_domino_angles = 
        {
            144d,
            -144d,
            -72d,
            0d,
            72d
        };
    }

    public enum DTypes { None = 0, Button };

    public class DObject
    {
        
        public bool Hovered { get; set; }
        public Point Coordinates { get; set; }
        public Size Size { get; set; }
        public virtual DTypes Type { get; set; }
        public bool Enabled { get; set; }
        public void Clicked()
        {
            this.OnClick(new EventArgs());
        }

        public event EventHandler<EventArgs> Click;
        public virtual void OnClick(EventArgs e)
        {
            EventHandler<EventArgs> handler = Click;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class DButton : DObject
    {
        public DButton(string text, int x, int y, int width, int height)
        {
            this.Text = text;
            this.Coordinates = new Point(x, y);
            this.Size = new Size(width, height);
        }
        public override DTypes Type { get { return DTypes.Button; } set { } }
        public string Text { get; set; }
        public Image Image { get; set; }
        
    }

    public static class NumericExtensions
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
    }

}

