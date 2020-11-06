using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Critter2FG.Character
{
    public class Feature
    {
        public int Locked { get; set; }
        public string name { get; set; }
        public string source { get; set; }
        public string text { get; set; }

        public Feature(int locked, string Name, string Source, string Text)
        {
            Locked = locked;
            name = Name;
            source = Source;
            text = Text;
        }
    }
}
