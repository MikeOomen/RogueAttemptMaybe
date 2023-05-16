using System;

public class Weapons
{
    public float weaponDmg;
    public float weaponCrit;
    public float weaponCritMulti;
    public string weaponName;

    public Weapons(string name, float dmg, float crit, float critMulti)
	{
        weaponDmg = dmg;
        weaponCrit = crit;
        weaponCritMulti = critMulti;
        weaponName = name;
	}
}
