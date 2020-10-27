using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Critter2FG.Character
{
    public class Skill
    {

        /*
         * proficiency id's:
         * 0 - not
         * 1 - full
         * 2 - double
         * 3 - half
         */

        public string name { get; set; }
        public int prof { get; set; }
        public string stat { get; set; }

        public Skill(string Name, string Stat)
        {
            name = Name;
            stat = Stat;
        }
    }
}
