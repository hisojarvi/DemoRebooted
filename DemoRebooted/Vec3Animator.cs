﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace DemoRebooted
{
    public class Vec3Animator
    {
        public Vector3 Value { get { return _value; } }
        Vector3 _value;
        Vector3 Target;
        Vector3 Start;
        long Duration;
        bool Repeat;
        long ElapsedMillis = 0;
        public BezierCurveCubic Curve;

        public Vec3Animator(Vector3 start, Vector3 target, long durationMillis, bool repeat)
        {
            Curve = new BezierCurveCubic(new Vector2(0.0f, 0.0f),
                            new Vector2(1.0f, 1.0f),
                            new Vector2(0.5f, 0.0f),
                            new Vector2(0.5f, 1.0f));

            _value = start;
            Start = start;
            Target = target;
            Duration = durationMillis;
            Repeat = repeat;
        }

        public void Update(long deltaMillis)
        {
            ElapsedMillis += deltaMillis;            
            if (ElapsedMillis >= Duration)
            {
                _value = Target;
            }
            else
            {
                var progress = Curve.CalculatePoint(ElapsedMillis / (float)Duration).Y;
                _value = Start + progress * (Target - Start);
            }
        }       
    }
}