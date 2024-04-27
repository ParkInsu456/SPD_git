using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPD
{
    internal class Player
    {
        public string Name { get; }
        public string Job { get; }
        public int Level { get; }
        public int Atk { get; }
        public int Def { get; }
        public int Hp { get; set; }
        public int Gold { get; set; }
        public int BonusAtk {  get; set; }
        public int BonusDef { get; set; }
        public int BonusHp { get; set; }


        public Player(string name, string job, int level, int atk, int def, int hp, int gold, int bonusAtk = 0, int bonusDef = 0, int bonusHp = 0)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Hp = hp;
            Gold = gold;
        }

        
    }


}
