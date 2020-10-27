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
        public int basescore { get; set; }
        public int modifier { get; set; }
        public string name { get; set; }
        public int proficient { get; set; }
        public string stattype { get; set; }
        public int total { get; set; }

    }
}
