using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SPD
{

    internal class Dungeon
    {
        public string Name { get; }
        public int NeedDef {get;}       
        public int Reward { get; }

        public Dungeon (string name, int needDef, int needAtk, int reward)
        {
            Name = name;
            NeedDef = needDef;
            
            Reward = reward;
        }

        

        //다른 클래스 2개 이상에서 값을 가져와야 하는 경우
       
        
    }
}
