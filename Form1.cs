using Critter2FG.Character;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace Critter2FG
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string URL = "https://critterdb.com/api/publishedbestiaries/5ea4b8b463a0580dfd76d379/creatures/:page";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.ContentType = "application/json; charset=utf-8";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes("username:password"));
            request.PreAuthenticate = true;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            Console.WriteLine(reader.ReadToEnd());
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            // StatBlockDisplay test = new StatBlockDisplay();
            //test.name = "Aaric sucks";
            //panel1.Controls.Add(test);

            // MessageBox.Show(test.name);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StatBlockDisplay testdisplay = new StatBlockDisplay();
            panel1.Controls.Add(testdisplay);

            testdisplay.ExportToXML("test");
           // AbilityScore test = new AbilityScore();

        }

        public void ParseCharacter()
        {
            PlayableCharacter parsed_character = new PlayableCharacter();

            string inputjson = File.ReadAllText(@"dndbeyond.json");
            JObject input = JObject.Parse(inputjson);
            parsed_character.name = (string)input["data"]["name"];
            parsed_character.size = (string)input["data"]["name"];
            parsed_character.weight = (string)input["data"]["weight"];
            parsed_character.height = (string)input["data"]["height"];
            parsed_character.age = (string)input["data"]["age"];
            parsed_character.gender = (string)input["data"]["gender"];
            parsed_character.race = (string)input["data"]["race"]["baseRaceName"];
            parsed_character.xp = (string)input["data"]["race"]["currentXp"];

            //race


            //background
            parsed_character.backgroundname = (string)input["data"]["background"]["definition"]["name"];


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

                int quantity = (int)item["quantity"];
                string description = (string)itemdefinition["description"];
                int cost = (int)item["definition"]["cost"];
                string filtertype = (string)itemdefinition["filterType"];
                int weight = (int)itemdefinition["weight"];
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
            string backgroundfeaturename = (string)input["data"]["background"]["definition"]["featureName"];
            string backgroundfeaturetext = (string)input["data"]["background"]["definition"]["featureDescription"];
            string backgroundfeaturesource = (string)input["data"]["background"]["definition"]["name"];
            Feature backgroundfeature = new Feature(1, backgroundfeaturename, backgroundfeaturesource, backgroundfeaturetext);
            parsed_character.featurelist.Add(backgroundfeature);

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

            foreach (KeyValuePair<string, int> test in racialDictionary)
            {
               // MessageBox.Show(test.Key.ToString());
               // MessageBox.Show(test.Value.ToString());
            }
            var abilitynamesDictionary = abilitynames.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            foreach (JObject stat in stats)
            {
                int statid = (int)stat["id"];
                int value = (int)stat["value"];
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


                AbilityScore abilityscore = new AbilityScore(name, value+bonus);
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


            ExportCharacter(parsed_character);


        }
        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public void GenerateCharacter(PlayableCharacter character)
        {
           // File.WriteAllText("testcharacter.xml",PopulateInventory(character));
        }
        public void ExportCharacter(PlayableCharacter character)
        {


            // To convert an XML node contained in string xml into a JSON string   
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(xml);
            //string jsonText = JsonConvert.SerializeXmlNode(doc);

            // To convert JSON text contained in string json into an XML node
            XNode node = JsonConvert.DeserializeXNode(File.ReadAllText(@"wadereed.json"), "Root");

            File.WriteAllText("jsonconverter.xml",node.ToString());

            XmlWriterSettings settings = new XmlWriterSettings();


            using (FileStream fileStream = new FileStream(character.name + ".xml", FileMode.Create))
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
                    SingleAttribute(writer, "carried", "number", item.carried.ToString());
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
            }
        }
        public void SingleAttribute(XmlWriter writer, string attributename, string type, string value)
        {
            writer.WriteStartElement(attributename);
            writer.WriteAttributeString("type", type);
            writer.WriteString(value);
            writer.WriteEndElement();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnCharacterParse_Click(object sender, EventArgs e)
        {
            //AbilityScore score = new AbilityScore("testscore", 17);

            //score.addBonus(4);
            ParseCharacter();



        }
    }
}
