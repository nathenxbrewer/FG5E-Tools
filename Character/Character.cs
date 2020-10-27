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

        public List<KeyValuePair<string, int>> racialBonuses;
        public List<KeyValuePair<string, int>> backgroundProf;
        public List<KeyValuePair<string, int>> classProf;
        public PlayableCharacter()
        {
            racialBonuses = new List<KeyValuePair<string, int>>();
            backgroundProf = new List<KeyValuePair<string, int>>();
            classProf = new List<KeyValuePair<string, int>>();
            SkillList = new List<Skill>();
            InventoryList = new List<Item>();
            AbilityScore skillStrength = new AbilityScore("Strength", 4);
            SkillList.Add(new Skill("Acrobatics", "dexterity"));
            SkillList.Add(new Skill("Animal Handling", "wisdom"));
            SkillList.Add(new Skill("Arcana", "intelligence"));
            SkillList.Add(new Skill("Athletics", "strength"));
            SkillList.Add(new Skill("Deception", "charisma"));
            SkillList.Add(new Skill("History", "intelligence"));
            SkillList.Add(new Skill("Insight", "wisdom"));
            SkillList.Add(new Skill("Intimidation", "charisma"));
            SkillList.Add(new Skill("Investigation", "intelligence"));
            SkillList.Add(new Skill("Medicine", "wisdom"));
            SkillList.Add(new Skill("Nature", "intelligence"));
            SkillList.Add(new Skill("Perception", "wisdom"));
            SkillList.Add(new Skill("Performance", "charisma"));
            SkillList.Add(new Skill("Persuasion", "charisma"));
            SkillList.Add(new Skill("Religion", "intelligence"));
            SkillList.Add(new Skill("Sleight of Hand", "dexterity"));
            SkillList.Add(new Skill("Stealth", "dexterity"));
            SkillList.Add(new Skill("Survival", "wisdom"));


        }
    }
}
