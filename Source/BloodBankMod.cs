using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace BloodBank
{

    //This class holds settings for the mod and is constructed early, before defs are loaded.
    //Therefore it can't do anything that requires defs to be loaded already. This is where Harmony
    //should be initialised if modifications to def generation are required so that changes to the
    //code are actually used
    public class BloodBankMod : Mod
    {
        public static BloodBankMod Instance { get; private set; }
        public BloodBankModSettings Settings { get; }

        public Type AlienDefType { get; }

        public bool AlienFrameworkExists { get; }

        public BloodBankMod(ModContentPack content) : base(content)
        {
            if (Instance != null) 
                throw new Exception("BloodBankMod :: ERROR Static class instance already exists on ctor");

            Instance = this;
            Settings = GetSettings<BloodBankModSettings>();
            //alien races require special handling for a couple of things
            Debug.Log("Checking for Humanoid Alien Races...");
            var mod = LoadedModManager.RunningModsListForReading.FirstOrDefault(m => m.PackageId == "erdelf.humanoidalienraces");
            if (mod != null)
            {
                Debug.Log("Humanoid Alien Races found. Looking for Type AlienRace.ThingDef_AlienRace...");

                AlienDefType = mod.assemblies.loadedAssemblies.FirstOrDefault(a => a.GetName().Name == "AlienRace")
                                  ?.GetType("AlienRace.ThingDef_AlienRace");

                if (AlienDefType == null)
                    Debug.Warning("Can't find Type AlienRace.ThingDef_AlienRace");
                else
                {
                    Debug.Log("Found Type AlienRace.ThingDef_AlienRace");
                    AlienFrameworkExists = true;
                }
            }
            else
                Debug.Warning("Humanoid Alien Races not found. ");

            
            //Harmony.DEBUG = true; //Don't leave this uncommented. haha famous last words.
            Debug.Log("Processing Harmony Patches");
            var harmony = new Harmony("makeitso.bloodbank");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "BloodBankModName".Translate();
        }
    }

    //This class is constructed after defs have loaded, just before the main menu is shown. 
    //This is where any modifications to the def database can be processed. 
    [StaticConstructorOnStartup]
    public class BloodBankOnDefsLoaded
    {
        static BloodBankOnDefsLoaded()
        {
            FixAlienBloodSurgeries();
            if (!BloodBankMod.Instance.Settings.Empty)
                Debug.Log("Settings initialized. No additional work required.");
            else
            {
                Debug.Log($"Settings not initialized. Found {BloodDefCache.BloodProducts.Count()} Blood product entries after loading. Initializing mod settings now. ");
                BloodBankMod.Instance.Settings.InitializeWithDefaultData(BloodDefCache.BloodProducts.Select(o => o.defName).ToList());
            }

            //BloodDefCache.ApplySettings(BloodBankMod.Instance.Settings);
        }


        //Remove Blood Bank recipes added to aliens by the Alien Race Framework. ARF will copy all human recipes
        //to aliens with useHumanRecipes=true (sic). As my recipes are specifically dependent on what kind
        //of blood the pawn has, this is unwanted behaviour.
        //This runs after everything has loaded, just before menu. I have to do this here to ensure
        //Alien Race Framework and BloodBank have both done their Def manipulation.
        private static void FixAlienBloodSurgeries()
        {
            if (!BloodBankMod.Instance.AlienFrameworkExists)
            {
                Debug.Warning("Didn't find Alien Race Framework. Skipping patch. ");
                return;
            }

            int count = 0;
            Debug.Log("Removing BloodBank surgeries added by Alien Race Framework. ");
            DefDatabase<ThingDef>.AllDefsListForReading.ForEach(ar =>
            {
                //Debug.Log($"{ar} Type-{ar.GetType()}. ");
                if (ar.GetType() != BloodBankMod.Instance.AlienDefType || ar.defName == "Human")
                    return;

                Debug.Log("Removing surgeries for " + ar.defName);
                ar.recipes.RemoveAll(r => (r.Worker is Recipe_AdministerBloodTransfusion ||
                                           r.Worker is Recipe_AdministerBloodProduct ||
                                           r.Worker is Recipe_TakeBlood) &&
                                          !ar.RaceAllowsBloodSurgery(r));
                count++;
            });

            Debug.Log($"{count} {BloodBankMod.Instance.AlienDefType}s processed. ");
        }
    }
}
