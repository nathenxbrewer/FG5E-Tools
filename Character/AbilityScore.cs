using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Critter2FG
{
    public class AbilityScore
    {

        public string abilityname { get; set; }
        private int Bonus { get; set; }
        public int save { get; set; }
        public int saveprof { get; set; }
        public int basescore { get; set; }
        public int totalscore { get; set; }
        public int modifier { get; set; }




        public AbilityScore(string scorename, int score)
        {
            abilityname = scorename;
            basescore = score;
            //CalculateTotal();




        }
        public void addBonus(int bonusamount)
        {
            basescore = basescore + bonusamount;
            //CalculateTotal();

        }

        public void CalculateTotal()
        {
            totalscore = basescore + Bonus;
        }
        public int CalculateModifier()
        {
            if (basescore == 1)
            {
                modifier = -5;
            }
            else if (IsWithin(basescore, 2, 3))
            {
                modifier = -4;
            }
            else if (IsWithin(basescore, 4, 5))
            {
                modifier = -3;
            }
            else if (IsWithin(basescore, 6, 7))
            {
                modifier = -2;
            }
            else if (IsWithin(basescore, 8, 9))
            {
                modifier = -1;
            }
            else if (IsWithin(basescore, 10, 11))
            {
                modifier = 0;
            }
            else if (IsWithin(basescore, 12, 13))
            {
                modifier = 1;
            }
            else if (IsWithin(basescore, 14, 15))
            {
                modifier = 2;
            }
            else if (IsWithin(basescore, 16,17))
            {
                modifier = 3;
            }
            else if (IsWithin(basescore, 18,19))
            {
                modifier = 4;
            }
            else if (IsWithin(basescore, 20,21))
            {
                modifier = 5;
            }
            else if (IsWithin(basescore, 22,23))
            {
                modifier = 6;
            }
            else if (IsWithin(basescore, 24,25))
            {
                modifier = 7;
            }
            else if (IsWithin(basescore, 26,27))
            {
                modifier = 8;
            }
            else if (IsWithin(basescore, 28,29))
            {
                modifier = 9;
            }
            else if (basescore == 30)
            {
                modifier = 10;
            }
            else
            {
                modifier = 0;
            }
            return modifier;
        }

        public static bool IsWithin(int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }
    }

}
