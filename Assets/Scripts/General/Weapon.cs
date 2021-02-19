using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{

    public int range { get; }
    public int damage { get; }
    public int energyRequired { get; }
    public int maxAmmo { get; }
    public int currentAmmo { get; set; }

    public enum weaponType
    {
        Rifle = 0,
        Shotgun = 1,
        Sniper = 2
    };

    // Default weapon constrictor
    public Weapon()
    {
        this.range = 5;
        this.damage = 10;
        this.energyRequired = 25;
        this.maxAmmo = 100;
        this.currentAmmo = maxAmmo;
    }

    // Add specific weapon types here
    public Weapon(weaponType weaponType)
    {
        // Rifle
        if(weaponType == weaponType.Rifle)
        {
            this.range = 10;
            this.damage = 25;
            this.energyRequired = 34;
            this.maxAmmo = 100;
            this.currentAmmo = maxAmmo;
        }
        // Shotgun
        else if(weaponType == weaponType.Shotgun)
        {
            this.range = 3;
            this.damage = 50;
            this.energyRequired = 50;
            this.maxAmmo = 20;
            this.currentAmmo = maxAmmo;
        }
        // Sniper
        else if (weaponType == weaponType.Sniper)
        {
            this.range = 50;
            this.damage = 75;
            this.energyRequired = 150;
            this.maxAmmo = 10;
            this.currentAmmo = maxAmmo;
        }
        else
        {
            this.range = 5;
            this.damage = 10;
            this.energyRequired = 25;
            this.maxAmmo = 100;
            this.currentAmmo = maxAmmo;
        }
        
    }

}
