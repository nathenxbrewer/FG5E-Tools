using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace Critter2FG
{
    public partial class StatBlockDisplay : UserControl
    {
        public string name { get; set; }
        public string nonIDName { get; set; }
        public string size { get; set; }
        public string type { get; set; }
        public string alignment { get; set; }
        public int armorclass { get; set; }
        public string armorclasstext { get; set; }
        public int hitpoints { get; set; }
        public string hitpointstext { get; set; }
        public string speed { get; set; }
        public int statSTR { get; set; }
        public int modSTR { get; set; }
        public int statDEX { get; set; }
        public int modDEX { get; set; }
        public int statCON { get; set; }
        public int modCON { get; set; }
        public int statINT { get; set; }
        public int modINT { get; set; }
        public int statWIS { get; set; }
        public int modWIS { get; set; }
        public int statCHA { get; set; }
        public int modCHA { get; set; }
        public string savingthrows { get; set; }
        public string skills { get; set; }
        public string damagevulnerabilities { get; set; }
        public string damageresistences { get; set; }
        public string damageimmunities { get; set; }
        public string conditionimmunities { get; set; }
        public string senses { get; set; }
        public string languages { get; set; }
        public int challengerating { get; set; }
        public int xp { get; set; }
        public List<string[]> traits { get; set; }
        public List<string[]> actions { get; set; }
        public List<string[]> reactions { get; set; }
        public List<string[]> legendaryactions { get; set; }
        public List<string[]> lairactions { get; set; }
        public List<string[]> innatespells { get; set; }

        public StatBlockDisplay()
        {
            InitializeComponent();

            lblAC.Text = "";
            ConvertFromJSON("");

            lblAC.Text = armorclass.ToString() + " " + armorclasstext;
            lblHP.Text = hitpoints + " " + hitpointstext;
            lblSpeed.Text = speed;

            lblName.Text = name;
            lblFlavor.Text = size + " " + type + " ," + alignment;
            lblSTR.Text = abilityScore(statSTR, modSTR);
            lblDEX.Text = abilityScore(statDEX, modDEX);
            lblCON.Text = abilityScore(statCON, modCON);
            lblINT.Text = abilityScore(statINT, modINT);
            lblWIS.Text = abilityScore(statWIS, modWIS);
            lblCHA.Text = abilityScore(statCHA, modCHA);

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
        public string abilityScore(int score, int mod)
        {
            string modsign = "";
            
            if (mod > 0)
            {
                modsign = "+";
            }

            string abilityscore = score + " (" + modsign + mod + ")";
            return abilityscore;
        }
        public string mod(int mod)
        {
            string modsign = "";

            if (mod > 0)
            {
                modsign = "+";
            }

            string modstring = modsign + mod;
            return modstring;
        }
        public void ConvertFromJSON(string jsonpath)
        {
            string json = File.ReadAllText(@"D:\Downloads\Astral-Dreadnought.json");
            JObject o = JObject.Parse(json);
            name = (string)o["name"];
            type = (string)o["stats"]["race"];
            size = (string)o["stats"]["size"];
            alignment = (string)o["stats"]["alignment"];
            armorclass = (int)o["stats"]["armorClass"];
            armorclasstext = (string)o["stats"]["armorType"];

            hitpoints = (int)o["stats"]["hitPoints"];
            hitpointstext = (string)o["stats"]["hitPointsStr"];

            speed = (string)o["stats"]["speed"];
            //--------------------------Abilities------------------------------------//
            //--STR
            statSTR = (int)o["stats"]["abilityScores"]["strength"];
            modSTR = (int)o["stats"]["abilityScoreModifiers"]["strength"];
            //--DEX
            statDEX = (int)o["stats"]["abilityScores"]["dexterity"];
            modDEX = (int)o["stats"]["abilityScoreModifiers"]["dexterity"];
            //--CON
            statCON = (int)o["stats"]["abilityScores"]["constitution"];
            modCON = (int)o["stats"]["abilityScoreModifiers"]["constitution"];
            //--INT
            statINT = (int)o["stats"]["abilityScores"]["intelligence"];
            modINT = (int)o["stats"]["abilityScoreModifiers"]["intelligence"];
            //--WIS
            statWIS = (int)o["stats"]["abilityScores"]["wisdom"];
            modWIS = (int)o["stats"]["abilityScoreModifiers"]["wisdom"];
            //--CHA
            statCHA = (int)o["stats"]["abilityScores"]["charisma"];
            modCHA = (int)o["stats"]["abilityScoreModifiers"]["charisma"];
            //--------------------------------------------------------------------------//
            //Saving Throws
            List<string> savingthrowlist = new List<string>();
            foreach (JObject savingthrow in o["stats"]["savingThrows"])
            {
                savingthrowlist.Add((string)savingthrow["modifierStr"]);

            }
            savingthrows = string.Join(",", savingthrowlist);

            //Skills
            List<string> skillslist = new List<string>();
            foreach (JObject skills in o["stats"]["skills"])
            {
                skillslist.Add((string)skills["modifierStr"]);

            }
            skills = string.Join(",", skillslist);

            //Damage Resistances



        }

        public void SingleAttribute(XmlWriter writer, string attributename, string type, string value)
        {
            writer.WriteStartElement(attributename);
            writer.WriteAttributeString("type", type);
            writer.WriteString(value);
            writer.WriteEndElement();
        }

        public void ExportToXML(string name)
        {
            using (FileStream fileStream = new FileStream(name + ".xml", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fileStream))

            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                /*
                 * 			
            <name type="string">Astral Dreadnought</name>
			<savingthrows type="string">Dex +5, Wis +9</savingthrows>
			<senses type="string">darkvision 120 ft., passive Perception 19</senses>
			<size type="string">Gargantuan</size>
			<skills type="string">Perception +9</skills>
			<speed type="string">15 ft., fly 80 ft. (hover)</speed>
            <alignment type="string">unaligned</alignment>
			<conditionimmunities type="string">charmed, exhaustion, frightened, paralyzed, petrified, poisoned, prone, stunned</conditionimmunities>
			<cr type="string">21</cr>
			<damageresistances type="string">bludgeoning, piercing, and slashing from nonmagical weapons</damageresistances>
			<hd type="string">(17d20 + 119)</hd>
			<hp type="number">297</hp>
            <ac type="number">20</ac>
			<actext type="string">(natural armor)</actext>
            <type type="string">monstrosity (titan)</type>

                 * */
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartElement("id-00001");
                //name
                SingleAttribute(writer, "name", "string", name);
                //size
                writer.WriteStartElement("size");
                writer.WriteAttributeString("type", "string");
                writer.WriteString(size);
                writer.WriteEndElement();
                //type
                writer.WriteStartElement("type");
                writer.WriteAttributeString("type", "string");
                writer.WriteString(type);
                writer.WriteEndElement();
                //alignment
                writer.WriteStartElement("alignment");
                writer.WriteAttributeString("type", "string");
                writer.WriteString(alignment);
                writer.WriteEndElement();
                //

                writer.WriteStartElement("abilities");

                //STR
                writer.WriteStartElement("strength");
                writer.WriteStartElement("bonus");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(modSTR.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("modifier");
                writer.WriteAttributeString("type", "string");
                writer.WriteString(mod(modSTR));
                writer.WriteEndElement();
                writer.WriteStartElement("score");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(statSTR.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();
                //DEX
                writer.WriteStartElement("dexterity");
                writer.WriteStartElement("bonus");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(modDEX.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("modifier");
                writer.WriteAttributeString("type", "string");
                writer.WriteString(mod(modDEX));
                writer.WriteEndElement();
                writer.WriteStartElement("score");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(statDEX.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();
                //CON
                writer.WriteStartElement("constitution");
                writer.WriteStartElement("bonus");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(modCON.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("modifier");
                writer.WriteAttributeString("type", "string");
                writer.WriteString(mod(modCON));
                writer.WriteEndElement();
                writer.WriteStartElement("score");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(statCON.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();
                //INT
                writer.WriteStartElement("intelligence");
                writer.WriteStartElement("bonus");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(modINT.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("modifier");
                writer.WriteAttributeString("type", "string");
                writer.WriteString(mod(modINT));
                writer.WriteEndElement();
                writer.WriteStartElement("score");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(statINT.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();
                //WIS
                writer.WriteStartElement("wisdom");
                writer.WriteStartElement("bonus");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(modWIS.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("modifier");
                writer.WriteAttributeString("type", "string");
                writer.WriteString(mod(modWIS));
                writer.WriteEndElement();
                writer.WriteStartElement("score");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(statWIS.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();

                //CHA
                writer.WriteStartElement("charisma");
                writer.WriteStartElement("bonus");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(modCHA.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("modifier");
                writer.WriteAttributeString("type", "string");
                writer.WriteString(mod(modCHA));
                writer.WriteEndElement();
                writer.WriteStartElement("score");
                writer.WriteAttributeString("type", "number");
                writer.WriteString(statCHA.ToString());
                writer.WriteEndElement();
                //end abilities
                writer.WriteEndElement();


                /*
 * 			
<name type="string">Astral Dreadnought</name>
<savingthrows type="string">Dex +5, Wis +9</savingthrows>
<senses type="string">darkvision 120 ft., passive Perception 19</senses>
<size type="string">Gargantuan</size>
<skills type="string">Perception +9</skills>
<speed type="string">15 ft., fly 80 ft. (hover)</speed>
<alignment type="string">unaligned</alignment>
<conditionimmunities type="string">charmed, exhaustion, frightened, paralyzed, petrified, poisoned, prone, stunned</conditionimmunities>
<cr type="string">21</cr>
<damageresistances type="string">bludgeoning, piercing, and slashing from nonmagical weapons</damageresistances>
<hd type="string">(17d20 + 119)</hd>
<hp type="number">297</hp>
<ac type="number">20</ac>
<actext type="string">(natural armor)</actext>
<type type="string">monstrosity (titan)</type>

 * */
                //saving throws
                if (!string.IsNullOrEmpty(skills))
                {
                    writer.WriteStartElement("savingthrows");
                    writer.WriteAttributeString("type", "string");
                    writer.WriteString(savingthrows);
                    writer.WriteEndElement();
                }

                if (!string.IsNullOrEmpty(skills))
                {
                    //skills
                    writer.WriteStartElement("skills");
                    writer.WriteAttributeString("type", "string");
                    writer.WriteString(skills);
                    writer.WriteEndElement();
                }
                if (!string.IsNullOrEmpty(damageresistences))
                {
                    //skills
                    writer.WriteStartElement("damageresistances");
                    writer.WriteAttributeString("type", "string");
                    writer.WriteString(damageresistences);
                    writer.WriteEndElement();
                }
                if (!string.IsNullOrEmpty(conditionimmunities))
                {
                    //skills
                    writer.WriteStartElement("conditionimmunities");
                    writer.WriteAttributeString("type", "string");
                    writer.WriteString(conditionimmunities);
                    writer.WriteEndElement();
                }




                writer.Flush();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
