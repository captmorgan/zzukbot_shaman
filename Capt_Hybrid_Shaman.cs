using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.Engines.CustomClass;

/*
Eat At 1%
Drink at 15%
Search mob range: 45
Roam from Waypoint: 120
Make sure Loot Units is On.

Talent spec: go enhancement, then go elemental. http://db.vanillagaming.org/?talent#hEcu0c0oZVVfzV0ux

Add Cure poison or Disease here: cureDisease() curePoison() 
Add any mob buffs to the purgeBuffs() routine

*/

namespace CaptShaman
{
    public class CaptShaman : CustomClass
    {
		/*
		Once health and mana are below these thresholds, it will try to use potions. Also, Racials.
		*/
		public int lowHealthP = 25;  // Point where we start healing
		public int lowManaP = 25;  // Percentage of mana reserved for Healing/adds/runners/silences etc, but will effect casting of DPS offensive abilities
		bool ShouldRest = true; // Prevent resting when pulling from range
		
		public string[] drinkNames = {
			"Refreshing Spring Water", 
			"Ice Cold Milk",
			"Melon Juice", 
			"Moonberry Juice",
            "Sweet Nectar", 
			"Morning Glory Dew", 
			"Conjured Purified Water", 
			"Conjured Spring Water", 
			"Conjured Mineral Water", 
			"Conjured Sparkling Water", 
			"Conjured Crystal Water",
			"Conjured Fresh Water"
		};

		
        public override byte DesignedForClass
        {
            get
            {
                return PlayerClass.Shaman;
            }
        }
		
        public override string CustomClassName
        {
            get
            {
                return "CaptShaman 2.0";
            }
        }
		
		
        public void SelectDrink()
        {
            if (this.Player.ItemCount("Morning Glory Dew") != 0)
                this.Player.Drink(drinkNames[5]);
            else if (this.Player.ItemCount("Sweet Nectar") != 0)
                this.Player.Drink(drinkNames[4]);
            else if (this.Player.ItemCount("Moonberry Juice") != 0)
                this.Player.Drink(drinkNames[3]);
            else if (this.Player.ItemCount("Melon Juice") != 0)
                this.Player.Drink(drinkNames[2]);
            else if (this.Player.ItemCount("Ice Cold Milk") != 0)
                this.Player.Drink(drinkNames[1]);
            else if (this.Player.ItemCount("Refreshing Spring Water") != 0)
                this.Player.Drink(drinkNames[0]);
            else if (this.Player.ItemCount("Conjured Purified Water") != 0)
                this.Player.Drink(drinkNames[6]);
            else if (this.Player.ItemCount("Conjured Spring Water") != 0)
                this.Player.Drink(drinkNames[7]);
            else if (this.Player.ItemCount("Conjured Mineral Water") != 0)
                this.Player.Drink(drinkNames[8]);
            else if (this.Player.ItemCount("Conjured Sparkling Water") != 0)
                this.Player.Drink(drinkNames[9]);
            else if (this.Player.ItemCount("Conjured Crystal Water") != 0)
                this.Player.Drink(drinkNames[10]);
			else if (this.Player.ItemCount("Conjured Fresh Water") != 0)
                this.Player.Drink(drinkNames[11]);
        }

        public void SelectHPotion()
        {
			if (this.Player.ManaPercent <= lowManaP)
			{
				if (this.Player.HealthPercent <= lowHealthP && this.Player.ItemCount("Major Healing Potion") != 0)
					this.Player.UseItem("Major Healing Potion");
				else if (this.Player.HealthPercent <= lowHealthP && this.Player.ItemCount("Superior Healing Potion") != 0)
					this.Player.UseItem("Superior Healing Potion");
				else if (this.Player.HealthPercent <= lowHealthP && this.Player.ItemCount("Greater Healing Potion") != 0)
					this.Player.UseItem("Greater Healing Potion");
				else if (this.Player.HealthPercent <= lowHealthP && this.Player.ItemCount("Healing Potion") != 0)
					this.Player.UseItem("Healing Potion");
				else if (this.Player.HealthPercent <= lowHealthP && this.Player.ItemCount("Discolored Healing Potion") != 0)
					this.Player.UseItem("Discolored Healing Potion");
				else if (this.Player.HealthPercent <= lowHealthP && this.Player.ItemCount("Lesser Healing Potion") != 0)
					this.Player.UseItem("Lesser Healing Potion");
				else if (this.Player.HealthPercent <= lowHealthP && this.Player.ItemCount("Minor Healing Potion") != 0)
					this.Player.UseItem("Minor Healing Potion");
			}
        }

        public void SelectMPotion()
        {
			if (this.Player.HealthPercent <= lowHealthP)
			{
				if (this.Player.ManaPercent <= lowManaP && this.Player.ItemCount("Major Mana Potion") != 0)
					this.Player.UseItem("Major Mana Potion");
				else if (this.Player.ManaPercent <= lowManaP && this.Player.ItemCount("Superior Mana Potion") != 0)
					this.Player.UseItem("Superior Mana Potion");
				else if (this.Player.ManaPercent <= lowManaP && this.Player.ItemCount("Greater Mana Potion") != 0)
					this.Player.UseItem("Greater Mana Potion");
				else if (this.Player.ManaPercent <= lowManaP && this.Player.ItemCount("Mana Potion") != 0)
					this.Player.UseItem("Mana Potion");
				else if (this.Player.ManaPercent <= lowManaP && this.Player.ItemCount("Lesser Mana Potion") != 0)
					this.Player.UseItem("Lesser Mana Potion");
				else if (this.Player.ManaPercent <= lowManaP && this.Player.ItemCount("Minor Healing Potion") != 0)
					this.Player.UseItem("Minor Mana Potion");
			}
		}
        public void EnchantWeapon()
        {
			//using Flametongue is superior to windfury when using fast 1-hand weapon
			if (this.Player.GetSpellRank("Flametongue Weapon") != 0)
            {
                if (!this.Player.IsMainhandEnchanted())
                    this.Player.Cast("Flametongue Weapon");
            }
            else if (this.Player.GetSpellRank("Rockbiter Weapon") != 0)
            {
                if (!this.Player.IsMainhandEnchanted())
                    this.Player.Cast("Rockbiter Weapon");
            }
        }
		
        public void Shield()
        {
            if (this.Player.GetSpellRank("Lightning Shield") != 0 && !this.Player.GotBuff("Lightning Shield") && this.Player.CanUse("Lightning Shield") && !this.Player.GotBuff("Ghost Wolf"))
                this.Player.Cast("Lightning Shield");
        }
		
        public void Shock()
		{
			/* Earth Shock */
			if (this.Target.IsCasting != "" || this.Target.IsChanneling != "") {
				if (this.Player.GetSpellRank("Earth Shock") != 0 && this.Player.CanUse("Earth Shock")) {
					this.Player.StopCasting();
					this.Player.Cast("Earth Shock");
					return;
				}
			}
			/* Frost Shock */
			else if (this.Target.DistanceToPlayer > 5 && this.Target.HealthPercent <= 85) {
				if (this.Player.GetSpellRank("Frost Shock") != 0 && this.Player.CanUse("Frost Shock")) {
					this.Player.Cast("Frost Shock");
					return;
				}
			}
			/* Flame Shock */
			else if (this.Player.GetSpellRank("Flame Shock") != 0 && !this.Target.GotDebuff("Flame Shock") && this.Target.HealthPercent >= 85 && this.Player.CanUse("Flame Shock")) {
				this.Player.Cast("Flame Shock");
				return;
			}
			/* Clearcasting use */
			else if (this.Player.GotBuff("Clearcasting") && this.Player.CanUse("Frost Shock")) this.Player.Cast("Frost Shock");
		}

        public void Totems()
        {
            if (this.Player.GetSpellRank("Searing Totem") != 0 && this.Target.HealthPercent >= 40 && this.Player.CanUse("Searing Totem"))
            {
                float searingRange = this.Player.IsTotemSpawned("Searing Totem");
                float fireNovaRange = this.Player.IsTotemSpawned("Fire Nova Totem");
                if ((searingRange == -1 || searingRange > 18) && (fireNovaRange == -1 || fireNovaRange > 18))
                    this.Player.Cast("Searing Totem");
            }
			if (this.Player.ManaPercent <= 15 && this.Target.HealthPercent <= 15 && this.Attackers.Count == 1 && this.Player.GetSpellRank("Mana Spring Totem") != 0 && this.Player.CanUse("Mana Spring Totem") && !this.Player.GotBuff("Mana Spring"))
				{
				float manaSpringRange = this.Player.IsTotemSpawned("Mana Spring Totem");
                float PoisonRange = this.Player.IsTotemSpawned("Poison Cleansing Totem");
				if ((manaSpringRange == -1 || manaSpringRange > 18) && (PoisonRange == -1 || PoisonRange > 18))
					this.Player.Cast("Mana Spring Totem");
				}
        }

        public void FightHeal()
        {
            if (this.Player.HealthPercent < lowHealthP && this.Player.GetSpellRank("Lesser Healing Wave") != 0 && this.Player.CanUse("Lesser Healing Wave"))
            {
				if (this.Player.CanUse("War Stomp")) this.Player.Cast("War Stomp");
                this.Player.Cast("Lesser Healing Wave");
            }
            else if (this.Player.HealthPercent < lowHealthP && this.Player.CanUse("Healing Wave"))
            {
				if (this.Player.CanUse("War Stomp")) this.Player.Cast("War Stomp");
                this.Player.Cast("Healing Wave");
            }
            return;
        }

        public void HandleAdds()
        {
			if (this.Attackers.Count >= 2)
			{
				if (this.Player.GetSpellRank("Grace of Air Totem") != 0 && this.Player.CanUse("Grace of Air Totem") && !this.Player.GotBuff("Grace of Air") && this.Player.ManaPercent >= lowManaP)
				{
                float graceOfAirRange = this.Player.IsTotemSpawned("Grace of Air Totem");
                if ((graceOfAirRange == -1 || graceOfAirRange > 18))
                    this.Player.Cast("Grace of Air Totem");
				}
				if (this.Player.GetSpellRank("Stoneclaw Totem") != 0 && this.Player.CanUse("Stoneclaw Totem") && this.Target.HealthPercent >= 50)
					this.Player.Cast("Stoneclaw Totem");
				else if (this.Player.GetSpellRank("Stoneskin Totem") != 0 && this.Player.CanUse("Stoneskin Totem") && !this.Player.GotBuff("Stoneskin"))
				{
					float StoneskinRange = this.Player.IsTotemSpawned("Stoneskin Totem");
					float StoneclawRange = this.Player.IsTotemSpawned("Stoneclaw Totem");
					if ((StoneskinRange == -1 || StoneskinRange > 18) && (StoneclawRange == -1 || StoneclawRange > 18))
                    this.Player.Cast("Stoneskin Totem");
				}
				if (this.Player.GetSpellRank("Fire Nova Totem") != 0 && this.Player.CanUse("Fire Nova Totem") && this.Player.ManaPercent >= 25)
					this.Player.Cast("Fire Nova Totem");
			}
		}

		public override bool Buff()
        {
			if (this.Player.IsCasting == "Ghost Wolf") return false;
			else if ((this.Player.IsCasting == "Healing Wave" || this.Player.IsCasting == "Lesser Healing Wave") && this.Player.HealthPercent < 75)
			{
                return false;
			}	
			else if (this.Player.HealthPercent < 65 && this.Player.CanUse("Healing Wave"))
            {
                this.Player.Cast("Healing Wave");
				return false;
            }
			else if (Player.NeedToLoot()) return true; // Disable if you turn off looting.
			else if (this.Player.HealthPercent < 75 && this.Player.GetSpellRank("Lesser Healing Wave") != 0 && this.Player.CanUse("Lesser Healing Wave"))
            {
                this.Player.Cast("Lesser Healing Wave");
				return false;
			}
			if (this.Player.ItemCount("Mighty Troll's Blood Potion") > 0 && !this.Player.GotBuff("Regeneration") && !this.Player.GotBuff("Ghost Wolf")) 
			{
				this.Player.UseItem("Mighty Troll's Blood Potion");
				return false;
			}
			if (this.Player.ItemCount("Elixir of Greater Agility") > 0 && !this.Player.GotBuff("Greater Agility") && !this.Player.GotBuff("Ghost Wolf")) 
			{
				this.Player.UseItem("Elixir of Greater Agility");
				return false;
			}
			cureDisease();
			Shield();
			
			if (this.Player.GetSpellRank("Ghost Wolf") != 0 && !this.Player.GotBuff("Ghost Wolf") && this.Player.CanUse("Ghost Wolf") 
				&& (this.Player.ManaPercent > (80 - this.Player.MaxMana/100)) && this.Player.GotBuff("Lightning Shield"))
			{
				this.Player.Cast("Ghost Wolf");
				return false;
			}
			
			return true;
        }

		public void cureDisease()
		{
			if (this.Player.GotDebuff("Infected Wound") || this.Player.GotDebuff("Rabies"))
            {
				if (this.Player.CanUse("Cure Disease")) 
					this.Player.Cast("Cure Disease");
			}
		}

		public void curePoison()
       {
			if (this.Player.GotDebuff("Deadly Poison") || this.Player.GotDebuff("Venom Sting") || this.Player.GotDebuff("Poison") || this.Player.GotDebuff("Slowing Poison") 
				|| this.Player.GotDebuff("Bloodpetal Poison") || this.Player.GotDebuff("Localized Toxin"))
            {
				if (this.Player.GetSpellRank("Poison Cleansing Totem") != 0 && this.Player.CanUse("Poison Cleansing Totem"))
				{
                float PoisonRange = this.Player.IsTotemSpawned("Poison Cleansing Totem");
                if ((PoisonRange == -1 || PoisonRange > 18))
                    this.Player.Cast("Poison Cleansing Totem");
				}
            }
       }

	   public void purgeBuffs()
       {
			if (this.Target.GotBuff("Wild Regeneration") || this.Target.GotBuff("Rejuvenation"))
			{
				this.Player.Cast("Purge");
			}
       }

        public override void PreFight()
        {
			bool ShouldRest = false;
			if (this.Player.GotBuff("Ghost Wolf"))
			{
				this.SetCombatDistance(4);
                this.Player.Attack();				
			}
			else if (this.Target.DistanceToPlayer > 18 && this.Player.ManaPercent >= lowManaP && (this.Target.CreatureType != CreatureType.Elemental))
            {
                this.SetCombatDistance(28);
				if (this.Player.GotBuff("Clearcasting") && this.Player.CanUse("Chain Lightning")) 
					this.Player.Cast("Chain Lightning");
				else this.Player.Cast("Lightning Bolt");
            }
			else if (this.Target.DistanceToPlayer <= 18 && (this.Target.CreatureType != CreatureType.Elemental))
			{
				Shock();
			}
			else
			{
				this.SetCombatDistance(4);
                this.Player.Attack();
			}
        }
		
        public override void Fight()
        {
			if (this.Player.IsCasting == "Lesser Healing Wave" || this.Player.IsCasting == "Healing Wave")
            {
                return;
            }
			if (this.Player.GotBuff("Ghost Wolf"))
			{
				this.Player.Cast("Ghost Wolf");
			}

			EnchantWeapon();
			HandleAdds();
			FightHeal();
			SelectMPotion();
			SelectHPotion();
			purgeBuffs();
			curePoison();
			
			if (this.Target.DistanceToPlayer <= 30 && this.Target.DistanceToPlayer > 18 && this.Player.ManaPercent >= lowManaP)
            {
                this.SetCombatDistance(28);
				this.Player.CastWait("Lightning Bolt", 1000);
				return;
            }
			else if (this.Target.DistanceToPlayer <= 18 && this.Target.DistanceToPlayer >= 5 && this.Player.ManaPercent >= lowManaP)
            {                
                this.SetCombatDistance(18);
				Shock();
				this.Player.CastWait("Lightning Bolt", 1000);
				return;
			}
			else
			{
				if (this.Player.IsCasting == "Lightning Bolt") this.Player.StopCasting();
				this.SetCombatDistance(4);
                this.Player.Attack();
				Shock();
				Totems();

				if (this.Player.HealthPercent < 50 && this.Player.CanUse("Berserking")) this.Player.Cast("Berserking");
				if (this.Player.HealthPercent > 90 && this.Player.CanUse("Blood Fury") && this.Target.HealthPercent >= 85) this.Player.Cast("Blood Fury");
			}
			bool ShouldRest = true;
		}

        public override void Rest()
        {
			if (ShouldRest)
			{
				if (this.Player.NeedToDrink && !this.Player.GotBuff("Drink"))
				{
					if (this.Player.GetSpellRank("Mana Spring Totem") != 0 && this.Player.CanUse("Mana Spring Totem") && !this.Player.GotBuff("Mana Spring"))
					{
					float manaSpringRange = this.Player.IsTotemSpawned("Mana Spring Totem");
					if ((manaSpringRange == -1 || manaSpringRange > 18))
						this.Player.Cast("Mana Spring Totem");
					}
					else
					{
						this.Player.DoString("DoEmote('Sit')");
						SelectDrink();
						return;
					}
				}
			}
		}	
	}
}

