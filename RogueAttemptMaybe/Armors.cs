using System;

namespace RogueAttemptMaybe
{
    public class Armors
    {
        public float armorDMGR;
        public float armorAgilStat;
        public string armorDisc;
        public string armorName;
        public float armorMoneyMulti;
        public float armorSellMulti;

        public Armors(string name, float dmgR, float agil, string disc, float moneyMulti, float sellMulti)
        {
            armorDMGR = dmgR;
            armorAgilStat = agil;
            armorDisc = disc;
            armorName = name;
            armorMoneyMulti = moneyMulti;
            armorSellMulti = sellMulti;
        }
    }
}
