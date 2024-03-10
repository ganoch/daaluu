using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Timers;
using System.Diagnostics;
using Daaluu.Logic;

namespace Daaluu.Animation2D
{
    class Animation
    {
        /*
        public static void AnimateFromCurrent(IAnimatableObject obj, PointF dest, double angle, float speed, double start_delay)
        {
            obj.IsAnimated = true;
            obj.StartAngle = (obj.Angle + 360) % 360;
            obj.StartCoordinates = obj.Coordinates;
            obj.AnimationStart = DateTime.Now.AddMilliseconds(start_delay);
            obj.DestinationAngle = (angle + 360) % 360;
            obj.DestinationCoordinates = dest;

            float distance = (float)Math.Sqrt(Math.Pow(obj.StartCoordinates.X - obj.DestinationCoordinates.X, 2) + Math.Pow(obj.StartCoordinates.Y - obj.DestinationCoordinates.Y, 2));
            double duration = distance / speed * 1000;
            obj.Duration = duration;
        }

        public static void AnimateFromCurrent(IAnimatableObject obj, PointF dest, double angle, double duration, double start_delay)
        {
            obj.IsAnimated = true;
            obj.StartAngle = (obj.Angle + 360) % 360;
            obj.StartCoordinates = obj.Coordinates;
            obj.AnimationStart = DateTime.Now.AddMilliseconds(start_delay);
            obj.DestinationAngle = (angle + 360) % 360;
            obj.DestinationCoordinates = dest;
            obj.Duration = duration;
        }

        public static void CalculateCurrentCoordinates(IAnimatableObject obj)
        {
            double msec_elapsed = DateTime.Now.Subtract(obj.AnimationStart).TotalMilliseconds;
        }
         */
    }
    public abstract class AnimationProcess
    {
        public abstract void Update(double elapsed);
        public bool Finished { get; set; }
        public DateTime StartTime { get; set; }
        public object Object { get; set; }

        protected Object _obj;

        public event EventHandler<AnimationEventArgs> AnimationFinished;
        public virtual void OnAnimationFinished(AnimationEventArgs e)
        {
            EventHandler<AnimationEventArgs> handler = AnimationFinished;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class AnimationEventArgs : EventArgs
    {
        public object Object { get; set; }
        public AnimationEventArgs(object obj)
        {
            this.Object = obj;
        }
    }

    public class AnimationProcessQueue
    {
        private DateTime startTime;
        Timer aTimer;
        private int _num_procs = 0;

        public void Add(AnimationProcess process)
        {
            this._num_procs++;
            process.StartTime = DateTime.MinValue;
            process.AnimationFinished += new EventHandler<AnimationEventArgs>(process_AnimationFinished);
            _processQueue.Add(process);
        }

        void process_AnimationFinished(object sender, AnimationEventArgs e)
        {
            if (--this._num_procs == 0)
            {
                EventArgs qe = new EventArgs();
                OnQueueFinished(qe);
            }
        }

        public void Update(double elapsed)
        {
            if (_processQueue.Count > 0)
            {
                double process_elapsed = 0;
                if (_processQueue[0].StartTime != DateTime.MinValue)
                {
                    process_elapsed = DateTime.Now.Subtract(_processQueue[0].StartTime).TotalMilliseconds;
                }
                else
                {
                    _processQueue[0].StartTime = DateTime.Now;
                    process_elapsed = 0;
                }
                _processQueue[0].Update(process_elapsed);
                if (_processQueue.Count > 0 && _processQueue[0].Finished)
                {
                    _processQueue.RemoveAt(0);
                }
            }
            else
            {
                aTimer.Stop();
                aTimer.Elapsed -= new ElapsedEventHandler(aTimer_Elapsed);
            }
        }

        public void Start()
        {
            this.startTime = DateTime.Now;
            aTimer = new Timer(10);
            aTimer.Elapsed += new ElapsedEventHandler(aTimer_Elapsed);
            aTimer.Enabled = true;
        }

        void aTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            double msec_elapsed = DateTime.Now.Subtract(this.startTime).TotalMilliseconds;
            this.Update(msec_elapsed);
        }

        private readonly List<AnimationProcess> _processQueue = new List<AnimationProcess>();



        public event EventHandler<EventArgs> QueueFinished;
        public virtual void OnQueueFinished(EventArgs e)
        {
            EventHandler<EventArgs> handler = QueueFinished;

            if (handler != null)
            {
                handler(this, e);
            }
        }

    }

    public class DefaultAnimationProcess : AnimationProcess
    {
        private double _duration = 0;
        private double _angle = 0;
        private PointF _coor;

        private double _s_angle = 0;
        private PointF _s_coor;

        public static DefaultAnimationProcess CreateByDuration(IAnimatableObject obj, PointF dest, double angle, double duration)
        {
            return new DefaultAnimationProcess( obj, dest, angle, duration);
        }

        public static DefaultAnimationProcess CreateBySpeed(IAnimatableObject obj, PointF dest, double angle, int speed)
        {
            float distance = (float)Math.Sqrt(Math.Pow(obj.Coordinates.X - dest.X, 2) + Math.Pow(obj.Coordinates.Y - dest.Y, 2));
            double duration = distance / speed * 1000;
            return new DefaultAnimationProcess(obj, dest, angle, duration);
        }


        public DefaultAnimationProcess(IAnimatableObject obj, PointF dest, double angle, double duration)
        {
            this._duration = duration;
            this._angle = angle;
            this._coor = dest;

            this._s_coor = obj.Coordinates;
            this._s_angle = obj.Angle;

            this._obj = (Object)obj;
            ((IAnimatableObject)this._obj).IsAnimated = true;
        }

        public override void Update(double elapsed)
        {
            if (elapsed > 0)
            {
                double stage = (elapsed / this._duration);
                if (stage > 1)
                    stage = 1;
                float dx = this._coor.X - this._s_coor.X;
                float dy = this._coor.Y - this._s_coor.Y;
                dx *= (float)stage;
                dy *= (float)stage;

                double da = 0;
                if (((IAnimatableObject)this._obj).IsSymmetric)
                {
                    this._angle = this._angle % 180;
                    this._s_angle = this._s_angle % 180;

                    double maybe0 = this._angle - this._s_angle;
                    double maybe1 = maybe0 < 0 ? maybe0 + 180 : maybe0 - 180;

                    if (maybe1 != 0 && Math.Abs(maybe1) <= Math.Abs(maybe0))
                        da = maybe1;
                    else
                        da = maybe0;
                }
                else
                {
                    double maybe0 = this._angle - this._s_angle;
                    double maybe1 = maybe0 < 0 ? maybe0 + 360 : maybe0 - 360;

                    if (maybe1 != 0 && Math.Abs(maybe1) <= Math.Abs(maybe0))
                        da = maybe1;
                    else
                        da = maybe0;
                }

                ((IAnimatableObject)this._obj).Angle = this._s_angle + da * stage;
                ((IAnimatableObject)this._obj).Coordinates = new PointF(this._s_coor.X + dx, this._s_coor.Y + dy);

                if (((IAnimatableObject)this._obj).Coordinates.Equals(this._coor) || elapsed >= this._duration)
                {
                    ((IAnimatableObject)this._obj).IsAnimated = false;
                    this.Finished = true;

                    AnimationEventArgs e = new AnimationEventArgs(this._obj);
                    this.OnAnimationFinished(e);
                }
            }
        }
    }

    public class SleepProcess : AnimationProcess
    {
        public SleepProcess(float duration, Object obj)
        {
            this._duration = duration;
            this._obj = obj;
        }

        public override void Update(double elapsed)
        {
            if (elapsed >= this._duration) {
                Finished = true;

                AnimationEventArgs e = new AnimationEventArgs(this._obj);
                this.OnAnimationFinished(e);
            }
        }

        private float _duration = 0;
    }

    public interface IAnimatableObject
    {
        PointF Coordinates { get; set; }
        double Angle { get; set; }
        SizeF Size { get; }
        bool IsSymmetric { get; }
        bool IsAnimated { get; set; }
    }
}
