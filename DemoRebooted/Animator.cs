using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRebooted
{
    public class Animator
    {
        public float Value { get { return _value; } }
        float _value;
        float Target;
        float Duration;
        bool Repeat;
        float Speed;
        long ElapsedMillis = 0;

        public Animator(float start, float target, long durationMillis, bool repeat)
        {
            _value = start;
            Target = target;
            Duration = durationMillis;
            Repeat = repeat;
            Speed = (target - start) / durationMillis;
        }

        public void Update(long deltaMillis)
        {
            var durationToTarget = (Target - Value) / Speed;
            if (durationToTarget <= deltaMillis)
            {
                _value = Target;
            }
            else
            {
                _value += Speed * deltaMillis;
            }
        }

        

    }
}
