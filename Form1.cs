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

            string inputjson = File.ReadAllText(@"wadereed.json");
            JObject input = JObject.Parse(inputjson);
            parsed_character.name = (string)input["data"]["name"];
            //background
            parsed_character.backgroundname = (string)input["data"]["background"]["definition"]["name"];


            //traits
            JToken traits = input["data"]["traits"];
            parsed_character.personality = (string)traits["personalityTraits"];
            parsed_character.bonds = (string)traits["bonds"];
            parsed_character.flaws = (string)traits["flaws"];
            parsed_character.ideals = (string)traits["ideals"];


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
                    string damagetype = (string)item["definition"]["damageType"];
                    string dicestring = (string)item["definition"]["damage"]["diceString"];
                    List<string> propertylist = new List<string>();
                    foreach (JObject property in item["definition"]["properties"])
                    {
                        string propertyname = (string)property["name"];
                        if (propertyname == "Thrown")
                        {
                            //Gets the ranges if the property is thrown, DndBeyond stores the values seperate from the 'thrown' name. 
                            string range = (string)item["definition"]["range"];
                            string longrange = (string)item["definition"]["longRange"];
                            propertyname = string.Format("thrown (range {0}/{1})", range, longrange);


                        }

                        propertylist.Add(propertyname);
                    }

                    string properties = string.Join(",", propertylist);

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
            JToken asBonuses = input["data"]["modifiers"]["race"];
            foreach (JObject bonus in asBonuses)
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
            }
            foreach (KeyValuePair<string, int> kvp in parsed_character.racialBonuses)
            {
               MessageBox.Show(kvp.Key.ToLower() + " " + kvp.Value);
            }

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

            //Getting Backround and Class proficiencies that pertain to the basic skill list. All others will be added under the proficiency list. 
            foreach (KeyValuePair<string, int> prof in parsed_character.backgroundProf)
            {
                List<Skill> skillprof = parsed_character.SkillList.FindAll(item => item.name.ToLower() == prof.Key.ToLower());
                foreach (Skill activeprofskill in skillprof)
                {
                    activeprofskill.prof = prof.Value;
                }
            }

            foreach (KeyValuePair<string, int> prof in parsed_character.classProf)
            {
                List<Skill> skillprof = parsed_character.SkillList.FindAll(item => item.name.ToLower() == prof.Key.ToLower());
              //--  List<Skill> otherprof = parsed_character.SkillList.FindAll(item => item.name.ToLower() != prof.Key.ToLower());
                foreach (Skill activeprofskill in skillprof)
                {
                    activeprofskill.prof = prof.Value;
                }
            }
            foreach (Skill skill in parsed_character.SkillList)
            {
                MessageBox.Show("Skill: " + skill.name + "\n" + "Proficiency: " + skill.prof.ToString());
            }

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
            XmlSerializer xmlSerial = new XmlSerializer(typeof(PlayableCharacter));

            using (var sww = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(sww))
                {
                    writer.Formatting = System.Xml.Formatting.Indented;
                    writer.Indentation = 4;
                    xmlSerial.Serialize(writer, character);
                    File.WriteAllText("serialized.xml", sww.ToString()); // Your XML
                }
            }


            // To convert an XML node contained in string xml into a JSON string   
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(xml);
            //string jsonText = JsonConvert.SerializeXmlNode(doc);

            // To convert JSON text contained in string json into an XML node
            XNode node = JsonConvert.DeserializeXNode(File.ReadAllText(@"wadereed.json"), "Root");

            File.WriteAllText("jsonconverter.xml",node.ToString());


            using (FileStream fileStream = new FileStream(character.name + ".xml", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fileStream))

            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = System.Xml.Formatting.Indented;
                writer.Indentation = 4;
                //writer.WriteStartDocument(true);
                XmlRootAttribute root = new XmlRootAttribute("root");


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
                writer.WriteElementString("class","reference_background");
                writer.WriteElementString("recordname","reference.backgrounddata." + character.backgroundname.ToLower() + "@*");
                writer.WriteEndElement();

                //race
                SingleAttribute(writer, "race", "string", character.race);
                //racelink
                writer.WriteStartElement("racelink");
                writer.WriteAttributeString("type", "windowreference");
                writer.WriteElementString("class", "reference_race");
                writer.WriteElementString("recordname", "reference.racedata." + character.race.ToLower() + "@*");
                writer.WriteEndElement();


                #region InventoryList
                //InventoryList
                writer.WriteStartElement("inventorylist");

                int id = 1;
                foreach (Item item in character.InventoryList)
                {
                    string itemid = "id-" + id.ToString("D5").Trim();
                    writer.WriteStartElement(itemid);
                    SingleAttribute(writer,"count", "number", item.quantity.ToString());
                    SingleAttribute(writer, "name", "string", item.name);
                    SingleAttribute(writer, "weight", "number", item.weight.ToString());
                    SingleAttribute(writer, "locked", "number", item.locked.ToString());
                    SingleAttribute(writer, "isidentified", "number", item.isIdentified.ToString());
                    SingleAttribute(writer, "type", "string", item.type);
                    SingleAttribute(writer, "subtype", "string", item.subtype);
                    SingleAttribute(writer, "ac", "number", item.ac.ToString());
                    SingleAttribute(writer, "stealth", "string", item.stealth);
                    SingleAttribute(writer, "strength", "string", item.strength);
                    SingleAttribute(writer, "cost", "string", item.cost.ToString());
                    SingleAttribute(writer, "rarity", "string", item.rarity);
                    SingleAttribute(writer, "carried", "number", item.carried.ToString());

                    //backgroundlink
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
