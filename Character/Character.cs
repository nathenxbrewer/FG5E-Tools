using Critter2FG.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Critter2FG
{
    public class PlayableCharacter
    {
        public string name { get; set; }
        public string alignment { get; set; }
        public int age { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
        public string size { get; set; }
        public string personality { get; set; }
        public string bonds { get; set; }
        public string flaws { get; set; }
        public string ideals { get; set; }

        public string backgroundname { get; set; }
        public string race { get; set; }
        public string senses { get; set; }
        public int perception { get; set; }
        public int wounds { get; set; }
        public int maxhp { get; set; }
        public int temphp { get; set; }
       // public Class characterclass { get; set; }


        public List<Item> InventoryList { get; set; }
        public List<Skill> SkillList { get; set; }
        public List<AbilityScore> AbilityScoresList { get; set; }
        public PlayableCharacter()
        {
            InventoryList = new List<Item>();
            AbilityScore skillStrength = new AbilityScore("Strength", 4);

        }
    }
}
