using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Critter2FG.Character
{


    public class Item
    {
        public int quantity { get; set; }
        public int damagecategory { get; set; }
        public string category { get; set; }
        public int ac { get; set; }
        public int bonus { get; set; }
        public int carried { get; set; }
        public int cost { get; set; }
        public string description { get; set; }
        public string dexbonus { get; set; }
        public int isIdentified { get; set; }
        public int locked { get; set; }
        public string name { get; set; }
        public string stealth { get; set; }
        public string strength { get; set; }
        public string subtype { get; set; }
        public string type { get; set; }
        public int weight { get; set; }
        public string damage { get; set; }
        public string diceString { get; set; }
        public string damageType { get; set; }
        public string rarity { get; set; }
        public string properties { get; set; }
        public string filtertype { get; set; }
        public string itemID { get; set; }

        public Item(string strName, int intCost, string strDescription, int itemquantity)
        {
            name = strName;
            description = strDescription;
            quantity = itemquantity;
            cost = intCost;
        }
        public Item()
        {

        }

    }

}
