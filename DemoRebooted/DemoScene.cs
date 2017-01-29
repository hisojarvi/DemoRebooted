using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRebooted
{
    public abstract class DemoScene
    {
        protected DemoEngine Engine;
        protected long ElapsedMillis = 0;

        public DemoScene(DemoEngine engine)
        {
            Engine = engine;
        }

        public virtual void Update(long deltaMillis)
        {
            ElapsedMillis += deltaMillis;
        }

        public abstract void Render();
      
    }
}
