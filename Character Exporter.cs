using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Xml;
using Critter2FG.Character;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Critter2FG
{
    public partial class Character_Exporter : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        public bool isMaximized = false;
        public bool canDrag = true;

        public string json = "";


        //sizer stuff
        int Mx;
        int My;
        int Sw;
        int Sh;

        bool mov;


        void SizerMouseDown(object sender, MouseEventArgs e)
        {
            mov = true;
            My = MousePosition.Y;
            Mx = MousePosition.X;
            Sw = Width;
            Sh = Height;
        }

        void SizerMouseMove(object sender, MouseEventArgs e)
        {
            if (mov == true)
            {
                Width = MousePosition.X - Mx + Sw;
                Height = MousePosition.Y - My + Sh;
            }
        }

        void SizerMouseUp(object sender, MouseEventArgs e)
        {
            mov = false;
        }


        public Character_Exporter()
        {
            InitializeComponent();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public void textBox1_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                if (txtIP.Text != "")
                {
                    btnGo_Click(this, e);

                }
            }

        }


        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
            Close();
        }

        private void picJSON_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Browse .json Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "json",
                Filter = "json files (*.json)|*.json",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string selectedpath = Path.GetFullPath(openFileDialog1.FileName);
                lblPath.Text = Path.GetFullPath(openFileDialog1.FileName);
                string filepath = Path.GetFileName(openFileDialog1.FileName);
                string filename = openFileDialog1.FileName;
                openFileDialog1.Dispose();
                pnlID.Enabled = false;
                json = File.ReadAllText(filepath);
            }
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (canDrag)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }

            }
        }

        //


        public void ParseCharacter(string inputjson)
        {
            PlayableCharacter parsed_character = new PlayableCharacter();


            JObject input = JObject.Parse(inputjson);
            parsed_character.name = (string)input["data"]["name"];
            parsed_character.size = (string)input["data"]["size"];
            parsed_character.weight = (string)input["data"]["weight"];
            parsed_character.height = (string)input["data"]["height"];
            parsed_character.age = (string)input["data"]["age"];
            parsed_character.gender = (string)input["data"]["gender"];
            parsed_character.race = (string)input["data"]["race"]["baseRaceName"];
            parsed_character.xp = (string)input["data"]["race"]["currentXp"];
            parsed_character.ID = (int)input["data"]["id"];


            //race


            //background
            JToken backgroundname = input["data"]["background"]["definition"];
            if (backgroundname.HasValues)
            {
                parsed_character.backgroundname = (string)backgroundname["name"];
            }



            //traits
            JToken traits = input["data"]["traits"];
            parsed_character.personality = (string)traits["personalityTraits"];
            parsed_character.bonds = (string)traits["bonds"];
            parsed_character.flaws = (string)traits["flaws"];
            parsed_character.ideals = (string)traits["ideals"];

            //Classes
            foreach (JObject _class in input["data"]["classes"])
            {
                string name = (string)_class["definition"]["name"];
                int level = (int)_class["level"];
                string hitDie = "d" + (string)_class["definition"]["hitDice"];


                parsed_character.classlist.Add(new Class(name, level, hitDie));

            }

            #region Inventory
            //Inventory List
            foreach (JObject item in input["data"]["inventory"])
            {
                JToken itemdefinition = item["definition"];


                string name = (string)itemdefinition["name"];

                int.TryParse((string)item["quantity"], out int quantity);
                string description = (string)itemdefinition["description"];
                int.TryParse((string)item["definition"]["cost"], out int cost);
                string filtertype = (string)itemdefinition["filterType"];
                int.TryParse((string)itemdefinition["weight"], out int weight);
                string type = (string)itemdefinition["type"];
                string rarity = (string)itemdefinition["rarity"];
                string subtype = (string)itemdefinition["subType"];



                Item thisitem = new Item(name, cost, description, quantity);
                thisitem.weight = weight;
                thisitem.rarity = rarity;
                thisitem.subtype = subtype;
                thisitem.type = type;
                thisitem.filtertype = filtertype;
                thisitem.isIdentified = 1;
                thisitem.locked = 1;


                if (thisitem.filtertype == "Weapon")
                {
                    thisitem.damagecategory = 0;
                    string damagetype = (string)item["definition"]["damageType"];
                    string dicestring = (string)item["definition"]["damage"]["diceString"];
                    int category = (int)item["definition"]["categoryId"];

                    List<string> propertylist = new List<string>();
                    foreach (JObject property in item["definition"]["properties"])
                    {
                        string propertyname = (string)property["name"];
                        int longRange = (int)item["definition"]["longRange"];
                        int shortrange = (int)item["definition"]["range"];

                        if (propertyname == "Ammunition")
                        {
                            propertyname = "Ammunition (" + shortrange.ToString() + "/" + longRange.ToString() + ")";
                        }

                        if (longRange > 5)
                        {
                            thisitem.damagecategory = 1;
                            if (category == 1)
                            {
                                thisitem.category = "Simple Ranged Weapon";

                            }
                            else if (category == 2)
                            {
                                thisitem.category = "Martial Ranged Weapon";
                            }
                        }
                        else
                        {
                            if (category == 1)
                            {
                                thisitem.category = "Simple Melee Weapon";

                            }
                            else if (category == 2)
                            {
                                thisitem.category = "Martial Melee Weapon";
                            }
                        }

                        if (propertyname == "Thrown")
                        {
                            //Gets the ranges if the property is thrown, DndBeyond stores the values seperate from the 'thrown' name. 
                            string range = (string)item["definition"]["range"];
                            string longrange = (string)item["definition"]["longRange"];

                            propertyname = string.Format("thrown (range {0}/{1})", range, longrange);
                            thisitem.damagecategory = 2;


                        }



                        propertylist.Add(propertyname);
                    }

                    string properties = string.Join(", ", propertylist);
                    thisitem.diceString = dicestring;
                    thisitem.damageType = damagetype;
                    thisitem.damage = dicestring + " " + damagetype;
                    thisitem.properties = properties;
                }
                else if (thisitem.filtertype == "Armor")
                {
                    thisitem.ac = (int)itemdefinition["armorClass"];
                    thisitem.strength = (string)itemdefinition["strengthRequirement"];
                    //thisitem.stealth;
                }
                else
                {
                    //Other Gear

                }


                parsed_character.InventoryList.Add(thisitem);



            }
            //MessageBox.Show(parsed_character.InventoryList.Count.ToString());
            foreach (Item item in parsed_character.InventoryList)
            {
                //  MessageBox.Show(string.Format("Name: {0}\nDescription: {1}\nCost: {2}\nQuantity: {3}\nDamage: {4}\nProperties: {5}\nWeight: {6}", item.name, StripHTML(item.description), item.cost, item.quantity, item.damage, item.properties, item.weight));
            }
            #endregion

            //racial ability score bonuses.
            JToken racebonus = input["data"]["modifiers"]["race"];
            foreach (JObject bonus in racebonus)
            {
                string type = (string)bonus["type"];
                if (type == "bonus")
                {
                    string bonusname = (string)bonus["friendlySubtypeName"];
                    bonusname = bonusname.Replace(" Score", "");
                    int bonusamount = (int)bonus["value"];

                    KeyValuePair<string, int> bonuspair;
                    parsed_character.racialBonuses.Add(new KeyValuePair<string, int>(bonusname, bonusamount));
                }
                else if (type == "language")
                {
                    string language = (string)bonus["friendlySubtypeName"];
                    parsed_character.Languages.Add(language);


                }
            }
            //MessageBox.Show("Languages: " + string.Join(",", parsed_character.Languages.ToArray()));

            foreach (KeyValuePair<string, int> kvp in parsed_character.racialBonuses)
            {
                // MessageBox.Show(kvp.Key.ToLower() + " " + kvp.Value);
            }

            #region HP
            //HP & deathsaves. 
            parsed_character.maxhp = (int)input["data"]["baseHitPoints"];
            parsed_character.wounds = (int)input["data"]["temporaryHitPoints"];
            try
            {
                parsed_character.deathSaveSuccesses = (int)input["data"]["deathSaves"]["successCount"];
            }
            catch (Exception)
            {
            }
            try
            {
                parsed_character.deathSaveFails = (int)input["data"]["deathSaves"]["failCount"];
            }
            catch (Exception)
            {
            }

            #region FeatureList
            if (backgroundname.HasValues)
            {
                string backgroundfeaturename = (string)input["data"]["background"]["definition"]["featureName"];
                string backgroundfeaturetext = (string)input["data"]["background"]["definition"]["featureDescription"];
                string backgroundfeaturesource = (string)input["data"]["background"]["definition"]["name"];
                Feature backgroundfeature = new Feature(1, backgroundfeaturename, backgroundfeaturesource, backgroundfeaturetext);
                parsed_character.featurelist.Add(backgroundfeature);
            }


            //Class Features. 
            JToken classes = input["data"]["classes"];

            foreach (JObject _class in classes)
            {
                string classname = (string)_class["definition"]["name"];
                int classlevel = (int)_class["level"];
                JToken feature = _class["classFeatures"];
                foreach (JObject _feature in feature)
                {
                    //Only add feature if they are the correct level. 
                    int requiredlevel = (int)_feature["definition"]["requiredLevel"];
                    if (requiredlevel <= classlevel)
                    {
                        string snippet = (string)_feature["definition"]["snippet"];
                        if (snippet != "")
                        {
                            string name = (string)_feature["definition"]["name"];
                            string text = (string)_feature["definition"]["description"];

                            Feature newFeature = new Feature(1, name, classname.ToLower(), text);
                            parsed_character.featurelist.Add(newFeature);

                        }

                    }

                }
            }


            JToken classactions = input["data"]["actions"]["class"];
            foreach (JObject classaction in classactions)
            {
                string name = (string)classaction["name"];
                string text = (string)classaction["snippet"];


            }

            #endregion

            #endregion
            #region Proficiencies
            //Skill proficiencies (from background or class)
            JToken backgroundprof = input["data"]["modifiers"]["background"];
            JToken classprof = input["data"]["modifiers"]["class"];
            //background proficiencies
            foreach (JObject prof in backgroundprof)
            {
                string type = (string)prof["type"];
                if (type == "proficiency" || type == "expertise")
                {
                    string profname = (string)prof["friendlySubtypeName"];
                    // MessageBox.Show(profname);

                    int profvalue = 0;
                    if (type == "proficiency")
                    {
                        //standard proficiency
                        profvalue = 1;
                    }
                    else if (type == "expertise")
                    {
                        //double proficiency
                        profvalue = 2;
                    }

                    KeyValuePair<string, int> profpair;
                    parsed_character.backgroundProf.Add(new KeyValuePair<string, int>(profname, profvalue));
                }
            }
            List<string> skillstrings = new List<string>();
            foreach (Skill skills in parsed_character.SkillList)
            {
                skillstrings.Add(skills.name.ToLower());

            }


            //class proficiencies
            foreach (JObject prof in classprof)
            {
                string type = (string)prof["type"];
                if (type == "proficiency" || type == "expertise")
                {
                    string profname = (string)prof["friendlySubtypeName"];
                    // MessageBox.Show(profname);

                    int profvalue = 0;
                    if (type == "proficiency")
                    {
                        //standard proficiency
                        profvalue = 1;
                    }
                    else if (type == "expertise")
                    {
                        //double proficiency
                        profvalue = 2;
                    }

                    KeyValuePair<string, int> profpair;
                    parsed_character.classProf.Add(new KeyValuePair<string, int>(profname, profvalue));
                }
            }

            List<KeyValuePair<string, int>> otherbackprof = new List<KeyValuePair<string, int>>();
            //Getting Backround and Class proficiencies that pertain to the basic skill list. All others will be added under the proficiency list. 
            foreach (KeyValuePair<string, int> prof in parsed_character.backgroundProf)
            {
                List<Skill> skillprof = parsed_character.SkillList.FindAll(item => item.name.ToLower() == prof.Key.ToLower());
                otherbackprof = parsed_character.backgroundProf.FindAll(item => !skillstrings.Contains(item.Key.ToLower()));

                foreach (Skill activeprofskill in skillprof)
                {
                    activeprofskill.prof = prof.Value;
                }
            }



            List<KeyValuePair<string, int>> otherclassprof = new List<KeyValuePair<string, int>>();
            foreach (KeyValuePair<string, int> prof in parsed_character.classProf)
            {
                otherbackprof = parsed_character.classProf.FindAll(item => !skillstrings.Contains(item.Key.ToLower()));
                List<Skill> skillprof = parsed_character.SkillList.FindAll(item => item.name.ToLower() == prof.Key.ToLower());

                foreach (Skill activeprofskill in skillprof)
                {
                    activeprofskill.prof = prof.Value;
                }

            }
            List<KeyValuePair<string, int>> savingthrowprof = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> proftodelete = new List<KeyValuePair<string, int>>();


            foreach (KeyValuePair<string, int> test in otherbackprof)
            {
                if (test.Key.ToLower().Contains("throws"))
                {
                    savingthrowprof.Add(test);

                    proftodelete.Add(test);

                }
            }
            foreach (KeyValuePair<string, int> test in otherclassprof)
            {
                if (test.Key.ToLower().Contains("throws"))
                {
                    savingthrowprof.Add(test);
                    proftodelete.Add(test);

                }
            }
            foreach (KeyValuePair<string, int> todelete in proftodelete)
            {
                otherbackprof.Remove(todelete);
                otherclassprof.Remove(todelete);

            }
            foreach (KeyValuePair<string, int> savingthrow in savingthrowprof)
            {
                parsed_character.saveprof.Add(savingthrow.Key);
            }
            foreach (KeyValuePair<string, int> prof in otherbackprof)
            {
                parsed_character.proficiencylist.Add(prof.Key);
            }
            foreach (KeyValuePair<string, int> prof in otherclassprof)
            {
                parsed_character.proficiencylist.Add(prof.Key);
            }
            #endregion

            #region Abilities

            List<KeyValuePair<int, string>> abilitynames;
            abilitynames = new List<KeyValuePair<int, string>>();
            abilitynames.Add(new KeyValuePair<int, string>(1, "strength"));
            abilitynames.Add(new KeyValuePair<int, string>(2, "dexterity"));
            abilitynames.Add(new KeyValuePair<int, string>(3, "constitution"));
            abilitynames.Add(new KeyValuePair<int, string>(4, "inteligence"));
            abilitynames.Add(new KeyValuePair<int, string>(5, "wisdom"));
            abilitynames.Add(new KeyValuePair<int, string>(6, "charisma"));

            JToken stats = input["data"]["stats"];
                var racialDictionary = parsed_character.racialBonuses.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
                var abilitynamesDictionary = abilitynames.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

                foreach (JObject stat in stats)
                {
                    int statid = (int)stat["id"];
                JToken valuetoken = stat["value"];
                int value = 10;
                try
                {
                    value = (int)stat["value"];
                }
                catch (ArgumentException exc)
                {
                    //stat is null
                    value = 10;
                }

                    int bonus = 0;
                    string name = "";

                    if (abilitynamesDictionary.ContainsKey(statid))
                    {
                        string statname = abilitynamesDictionary[statid].ToString();
                        name = statname;
                        //MessageBox.Show(statname);
                        foreach (KeyValuePair<string, int> kvp in parsed_character.racialBonuses)
                        {
                            if (kvp.Key.ToLower() == statname)
                            {
                                bonus = kvp.Value;
                            }
                        }
                    }


                    AbilityScore abilityscore = new AbilityScore(name, value + bonus);
                    parsed_character.AbilityScoresList.Add(abilityscore);
                }

                foreach (AbilityScore score in parsed_character.AbilityScoresList)
                {
                    string othershit = score.abilityname.Replace(" Saving Throws", "").ToLower();
                    foreach (string test in parsed_character.saveprof)
                    {
                        string blah = test.Replace(" Saving Throws", "").ToLower();
                        if (blah == othershit)
                        {
                            score.saveprof = 1;
                        }
                    }
                }

            #endregion

            #region Speed
            //pulls walking speed out of racial traits. I don't need to parse each racial trait indiviually because by linking the sheet to the race, FG fills in the rest of that data. 
            JToken racialtraits = input["data"]["race"]["racialTraits"];
            foreach (JObject trait in racialtraits)
            {
                string traitName = (string)trait["definition"]["name"];
                if (traitName == "Speed")
                {
                    string speedDescription = (string)trait["definition"]["description"];
                    //uses Regex to pull the speed out of the speed strings. 
                    //"description": "<p>Your base walking speed is 30 feet.</p>",

                    int speed = Convert.ToInt32(Regex.Match(speedDescription, @"\d+").Value);
                    parsed_character.speed = speed;
                }
            }
            #endregion


            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "xml Document|*.xml";
            saveFileDialog1.Title = "Save Exported XML.";
            saveFileDialog1.FileName = parsed_character.name + ".xml";
            DialogResult result =  saveFileDialog1.ShowDialog();
            string savepath = "";


            if (result == DialogResult.OK)
            {
               savepath = Path.GetFullPath(saveFileDialog1.FileName);
                ExportCharacter(parsed_character, savepath);
            }
            else
            {
                MessageBox.Show("Save location not set!\nPlease try again.");
                txtIP.Text = "";
                pnlID.Enabled = true;
                pnlJSON.Enabled = true;
                lblPath.Text = "No file selected.";
            }




        }
        public void ExportCharacter(PlayableCharacter character, string savepath)
        {


            using (FileStream fileStream = new FileStream(savepath, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fileStream, Encoding.GetEncoding("iso-8859-1")))

            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 4;
                writer.WriteStartDocument();
                XmlRootAttribute root = new XmlRootAttribute("root");

                writer.WriteStartElement("root");
                writer.WriteAttributeString("version", "3.3");
                writer.WriteAttributeString("release", "8|CoreRPG:4");
                writer.WriteComment("CharacterID: " + character.ID);
                writer.WriteComment("Character Generated by FG5E Character Converter, created by Nathen Brewer");
                writer.WriteComment(@"Please consider donating :) https://www.buymeacoffee.com/NathenxBrewer ");

                writer.WriteStartElement("character");

                //Name
                SingleAttribute(writer, "name", "string", character.name);
                //Alignment ***Still need to complete, dndbeyond stores these as alignmentID's. 
                SingleAttribute(writer, "alignment", "string", "Neutral Good");
                SingleAttribute(writer, "personalitytraits", "string", character.personality);
                SingleAttribute(writer, "bonds", "string", character.bonds);
                SingleAttribute(writer, "ideals", "string", character.ideals);
                SingleAttribute(writer, "flaws", "string", character.flaws);

                //background
                SingleAttribute(writer, "background", "string", character.backgroundname);
                //backgroundlink
                writer.WriteStartElement("backgroundlink");
                writer.WriteAttributeString("type", "windowreference");
                writer.WriteElementString("class", "reference_background");
                writer.WriteElementString("recordname", "reference.backgrounddata." + character.backgroundname.ToLower() + "@*");
                writer.WriteEndElement();

                //race
                SingleAttribute(writer, "race", "string", character.race);
                //racelink
                writer.WriteStartElement("racelink");
                writer.WriteAttributeString("type", "windowreference");
                writer.WriteElementString("class", "reference_race");
                writer.WriteElementString("recordname", "reference.racedata." + character.race.ToLower() + "@*");
                writer.WriteEndElement();

                //Skilllist
                #region skilllist
                int skillid = 1;
                writer.WriteStartElement("skilllist");
                foreach (Skill skill in character.SkillList)
                {
                    string itemid = "id-" + skillid.ToString("D5").Trim();
                    writer.WriteStartElement(itemid);
                    SingleAttribute(writer, "misc", "number", "0");
                    SingleAttribute(writer, "name", "string", skill.name);
                    SingleAttribute(writer, "stat", "string", skill.stat);
                    SingleAttribute(writer, "prof", "number", skill.prof.ToString());
                    writer.WriteEndElement();
                    skillid++;
                }
                writer.WriteEndElement();
                #endregion
                #region classes
                writer.WriteStartElement("classes");
                int classid = 1;
                foreach (Class _class in character.classlist)
                {
                    string classname = _class.name;
                    int classlevel = _class.level;
                    string hitdie = _class.hitdie;

                    string itemid = "id-" + classid.ToString("D5").Trim();
                    writer.WriteStartElement(itemid);
                    SingleAttribute(writer, "hddie", "dice", hitdie);
                    SingleAttribute(writer, "name", "string", classname);
                    SingleAttribute(writer, "casterpactmagic", "number", "0");
                    SingleAttribute(writer, "level", "number", classlevel.ToString());


                    writer.WriteStartElement("shortcut");
                    writer.WriteAttributeString("type", "windowreference");
                    writer.WriteElementString("class", "reference_race");
                    writer.WriteElementString("recordname", "reference.classdata." + classname.ToLower() + "@*");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    classid++;
                }
                writer.WriteEndElement();



                #endregion

                //HP
                writer.WriteStartElement("hp");
                SingleAttribute(writer, "deathsavefail", "number", character.deathSaveFails.ToString());
                SingleAttribute(writer, "deathsavesuccess", "number", character.deathSaveSuccesses.ToString());
                SingleAttribute(writer, "total", "number", (character.maxhp - character.temphp).ToString());
                writer.WriteEndElement();


                //languagelist
                writer.WriteStartElement("languagelist");
                int languageid = 1;
                foreach (string language in character.Languages)
                {
                    string _id = "id-" + languageid.ToString("D5").Trim();
                    writer.WriteStartElement(_id);
                    SingleAttribute(writer, "name", "string", language);
                    writer.WriteEndElement();
                    languageid++;

                }
                writer.WriteEndElement();

                #region Featurelist
                writer.WriteStartElement("featurelist");
                int featureid = 1;
                foreach (Feature feature1 in character.featurelist)
                {
                    string _featureid = "id-" + featureid.ToString("D5").Trim();
                    writer.WriteStartElement(_featureid);
                    SingleAttribute(writer, "locked", "number", feature1.Locked.ToString());
                    SingleAttribute(writer, "name", "string", feature1.name);
                    SingleAttribute(writer, "source", "string", feature1.source);


                    writer.WriteStartElement("text");
                    writer.WriteAttributeString("type", "formattedtext");
                    writer.WriteRaw(@"<p>" + StripHTML(feature1.text) + @"</p>");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    featureid++;
                }
                writer.WriteEndElement();
                #endregion

                #region Abilities
                writer.WriteStartElement("abilities");
                foreach (AbilityScore ability in character.AbilityScoresList)
                {
                    writer.WriteStartElement(ability.abilityname);
                    SingleAttribute(writer, "bonus", "number", ability.CalculateModifier().ToString());
                    SingleAttribute(writer, "savemodifier", "number", "0");
                    SingleAttribute(writer, "saveprof", "number", ability.saveprof.ToString());
                    SingleAttribute(writer, "score", "number", ability.basescore.ToString());
                    writer.WriteEndElement();

                }
                writer.WriteEndElement();
                #endregion

                #region InventoryList
                //InventoryList
                writer.WriteStartElement("inventorylist");
                int id = 1;
                foreach (Item item in character.InventoryList)
                {
                    string itemid = "id-" + id.ToString("D5").Trim();
                    item.itemID = itemid;
                    writer.WriteStartElement(itemid);
                    SingleAttribute(writer, "count", "number", item.quantity.ToString());
                    SingleAttribute(writer, "name", "string", item.name);
                    SingleAttribute(writer, "weight", "number", item.weight.ToString());
                    SingleAttribute(writer, "locked", "number", item.locked.ToString());
                    SingleAttribute(writer, "isidentified", "number", item.isIdentified.ToString());
                    SingleAttribute(writer, "type", "string", item.filtertype);
                    SingleAttribute(writer, "subtype", "string", item.category);
                    SingleAttribute(writer, "ac", "number", item.ac.ToString());
                    SingleAttribute(writer, "stealth", "string", item.stealth);
                    SingleAttribute(writer, "strength", "string", item.strength);
                    SingleAttribute(writer, "cost", "string", item.cost.ToString());
                    SingleAttribute(writer, "rarity", "string", item.rarity);
                    SingleAttribute(writer, "carried", "number", "1");
                    if (item.filtertype == "Weapon")
                    {
                        SingleAttribute(writer, "damage", "string", item.damage.ToString());
                        SingleAttribute(writer, "properties", "string", item.properties.ToString());
                    }



                    //description
                    writer.WriteStartElement("description");
                    writer.WriteAttributeString("type", "formattedtext");
                    writer.WriteRaw(@"<p>" + StripHTML(item.description) + @"</p>");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.Flush();

                    id++;

                }

                writer.WriteEndElement();



                /*
                 *<id-00001>
                    <count type="number">1</count>
                    <name type="string">Shield</name>
                    <weight type="number">6</weight>
                    <locked type="number">1</locked>
                    <isidentified type="number">1</isidentified>
                    <type type="string">Armor</type>
                    <subtype type="string">Shield</subtype>
                    <ac type="number">2</ac>
                    <stealth type="string">-</stealth>
                    <strength type="string">-</strength>
                    <cost type="string">10 gp</cost>
                    <rarity type="string">Common</rarity>
                    <carried type="number">1</carried>
                    <description type="formattedtext">
                        <p>A shield is made from wood or metal and is carried in one hand. Wielding a shield increases your Armor Class by 2. You can benefit from only one shield at a time.</p>
                    </description>
                </id-00001>
                 * */
                #endregion

                #region WeaponList
                writer.WriteStartElement("weaponlist");
                int weaponid = 1;
                foreach (Item item in character.InventoryList)
                {
                    if (item.filtertype == "Weapon")
                    {
                        string weaponidstring = "id-" + weaponid.ToString("D5").Trim();
                        writer.WriteStartElement(weaponidstring);
                        //shortcut
                        writer.WriteStartElement("shortcut");
                        writer.WriteAttributeString("type", "windowreference");
                        writer.WriteElementString("class", "item");
                        writer.WriteElementString("recordname", "....inventorylist." + item.itemID);
                        writer.WriteEndElement();

                        SingleAttribute(writer, "name", "string", item.name);
                        SingleAttribute(writer, "properties", "string", item.properties);
                        SingleAttribute(writer, "type", "number", item.damagecategory.ToString());

                        //damagelist
                        writer.WriteStartElement("damagelist");
                        //***Some items have multiple damages..i have yet to figure out how to determine that with the json provides. Coming in a update. 
                        writer.WriteStartElement("id-00001");
                        SingleAttribute(writer, "bonus", "number", item.bonus.ToString());
                        SingleAttribute(writer, "dice", "dice", item.diceString.Substring(1));
                        //*Also can't figure out the damage stat. All weapons i've seen so far have been 'dexterity'
                        SingleAttribute(writer, "stat", "string", "dexterity");
                        SingleAttribute(writer, "type", "string", item.damageType);
                        writer.WriteEndElement();


                        if (item.properties.Contains("Ammunition"))
                        {
                            SingleAttribute(writer, "maxammo", "number", "20");
                        }
                        SingleAttribute(writer, "attackbonus", "number", "0");
                        SingleAttribute(writer, "attackstat", "string", "dexterity");
                        SingleAttribute(writer, "isidentified", "number", "1");
                        SingleAttribute(writer, "type", "number", item.damagecategory.ToString());
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        weaponid++;
                    }

                }
                writer.WriteEndElement();
                #endregion

                #region Speed
                writer.WriteStartElement("speed");
                SingleAttribute(writer, "base", "number", character.speed.ToString());
                SingleAttribute(writer, "total", "number", character.speed.ToString());
                writer.WriteEndElement();
                #endregion

                #region ProficiencyList
                //InventoryList
                writer.WriteStartElement("proficiencylist");
                int x = 1;
                foreach (string prof in character.proficiencylist)
                {
                    string profid = "id-" + x.ToString("D5").Trim();
                    writer.WriteStartElement(profid);
                    SingleAttribute(writer, "name", "string", prof);
                    writer.WriteEndElement();
                    x++;

                }
                writer.WriteEndElement();
                #endregion
                //size, weight, height, age, gender
                SingleAttribute(writer, "exp", "number", character.xp);
                SingleAttribute(writer, "size", "string", character.size);
                SingleAttribute(writer, "gender", "string", character.gender);
                SingleAttribute(writer, "height", "string", character.height);
                SingleAttribute(writer, "weight", "string", character.weight);

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                txtIP.Text = "";
                pnlID.Enabled = true;
                pnlJSON.Enabled = true;
                lblPath.Text = "No file selected.";
            }
        }
        public void SingleAttribute(XmlWriter writer, string attributename, string type, string value)
        {
            writer.WriteStartElement(attributename);
            writer.WriteAttributeString("type", type);
            writer.WriteString(value);
            writer.WriteEndElement();
        }
        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (txtIP.Text != "")
            {
                //get json string from URL
                //https://character-service.dndbeyond.com/character/v3/character/15609233/
                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        json = wc.DownloadString(@"https://character-service.dndbeyond.com/character/v3/character/" + txtIP.Text + @"/");
                        ParseCharacter(json);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Unable to fetch Character data!\nError returned: " + exc.Message + "\nPlease make sure that Character ID exists, and that it is set to Public.");
                        btnReset_Click(this, e);

                    }

                }
            }
            else
            {
                if (json == "")
                {
                    MessageBox.Show("You must enter a character ID or select a .json file!\nPlease enter a Character ID or select a .json file and try again!");
                    btnReset_Click(this, e);

                }
            }




        }

        private void picDonate_Click(object sender, EventArgs e)
        {
            Process.Start(@"https://www.buymeacoffee.com/NathenxBrewer");
        }

        private void txtIP_Leave(object sender, EventArgs e)
        {

        }

        private void txtIP_TextChanged(object sender, EventArgs e)
        {

        }

        private void picReset_Click(object sender, EventArgs e)
        {
            txtIP.Enabled = true;
            txtIP.Text = "";
            pnlJSON.Enabled = true;
            
        }

        private void picResetJSON_Click(object sender, EventArgs e)
        {
            lblPath.Text = "No file selected.";
            pnlJSON.Enabled = true;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtIP.Text = "";
            lblPath.Text = "No file selected.";

            pnlJSON.Enabled = true;
            pnlID.Enabled = true;
            json = "";
            txtIP.Enabled = true;

        }
    }
}
