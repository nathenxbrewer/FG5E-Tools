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
        public int ID { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string alignment { get; set; }
        public string age { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public string xp { get; set; }
        public int speed { get; set; }
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
        public int deathSaveSuccesses { get; set; }
        public int deathSaveFails { get; set; }

       // public Class characterclass { get; set; }


        public List<Item> InventoryList { get; set; }
        public List<Skill> SkillList { get; set; }
        public List<AbilityScore> AbilityScoresList;

        public List<KeyValuePair<string, int>> racialBonuses;
        public List<KeyValuePair<string, int>> backgroundProf;
        public List<KeyValuePair<string, int>> classProf;
        public List<string> Languages;
        public List<string> saveprof;
        public List<string> proficiencylist;
        public List<Class> classlist;
        public List<Feature> featurelist;
        public PlayableCharacter()
        {


            name = "";
            gender = "";
            backgroundname = "";


            racialBonuses = new List<KeyValuePair<string, int>>();
            backgroundProf = new List<KeyValuePair<string, int>>();
            classProf = new List<KeyValuePair<string, int>>();
            SkillList = new List<Skill>();
            InventoryList = new List<Item>();
            Languages = new List<string>();
            proficiencylist = new List<string>();
            classlist = new List<Class>();
            saveprof = new List<string>();
            featurelist = new List<Feature>();
            AbilityScoresList = new List<AbilityScore>();




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
