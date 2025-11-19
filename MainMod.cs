using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using Il2CppInterop.Runtime.Injection;
using Il2Cpp;
using Il2CppInterop.Runtime;
using static Il2CppSystem.Array;
using UnityEngine.Windows;
using static System.Formats.Asn1.AsnWriter;
using System.Drawing;
using System.Resources;
using DB_Hybrids;
using Color = UnityEngine.Color;

[assembly: MelonInfo(typeof(MainMod), "DB_Hybrids", "2.0.0", "AutumnIsDolphin")]
[assembly: MelonGame("UmiArt", "Demon Bluff")]

namespace DB_Hybrids
{
    public class MainMod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            ClassInjector.RegisterTypeInIl2Cpp<Scover>();
            ClassInjector.RegisterTypeInIl2Cpp<Hout>();
            ClassInjector.RegisterTypeInIl2Cpp<Barchitect>();
            ClassInjector.RegisterTypeInIl2Cpp<Chaman>();
            MelonLogger.Msg("Successfully loaded");
        }

        public override void OnLateInitializeMelon()
        {
            // Scover
            CharacterData scover = new CharacterData();
            scover.role = new Scover();
            scover.name = "Scover";
            scover.description = "Learn how many Evils are adjacent to another Evil.";
            scover.flavorText = "TBD";
            scover.hints = "";
            scover.ifLies = "";
            scover.startingAlignment = EAlignment.Good;
            scover.type = ECharacterType.Villager;
            scover.bluffable = true;
            scover.picking = false;
            scover.characterId = "scover_AID";
            scover.artBgColor = new Color(0.111f, 0.0833f, 0.1415f);
            scover.cardBgColor = new Color(0.26f, 0.1519f, 0.3396f);
            scover.cardBorderColor = new Color(0.7133f, 0.339f, 0.8679f);
            scover.color = new Color(1f, 0.935f, 0.7302f);

            // Hout
            CharacterData hout = new CharacterData();
            hout.role = new Hout();
            hout.name = "Hout";
            hout.description = "Learn how far a specific evil is from my closest evil.";
            hout.flavorText = "If you know, you know.";
            hout.hints = "I cannot target Wretch as the evil by name." +
                "\nEx: \"X is 2 cards away from my closest evil.\"" +
                "\nX will never be the Wretch. But 'My closest evil' can be the Wretch." +
                "\nIf two characters are equidistant from me, I will always select the character that is clockwise to me.";
            hout.ifLies = "";
            hout.startingAlignment = EAlignment.Good;
            hout.type = ECharacterType.Villager;
            hout.bluffable = true;
            hout.picking = false;
            hout.characterId = "hout_AID";
            hout.artBgColor = new Color(0.111f, 0.0833f, 0.1415f);
            hout.cardBgColor = new Color(0.26f, 0.1519f, 0.3396f);
            hout.cardBorderColor = new Color(0.7133f, 0.339f, 0.8679f);
            hout.color = new Color(1f, 0.935f, 0.7302f);

            // Barchitect
            CharacterData barchitect = new CharacterData();
            barchitect.role = new Barchitect();
            barchitect.name = "Barchitect";
            barchitect.description = "Learn which side is more corrupted.";
            barchitect.flavorText = "TBD";
            barchitect.hints = "";
            barchitect.ifLies = "";
            barchitect.startingAlignment = EAlignment.Good;
            barchitect.type = ECharacterType.Villager;
            barchitect.bluffable = true;
            barchitect.picking = false;
            barchitect.characterId = "barchitect_AID";
            barchitect.artBgColor = new Color(0.111f, 0.0833f, 0.1415f);
            barchitect.cardBgColor = new Color(0.26f, 0.1519f, 0.3396f);
            barchitect.cardBorderColor = new Color(0.7133f, 0.339f, 0.8679f);
            barchitect.color = new Color(1f, 0.935f, 0.7302f);

            // chaman
            CharacterData chaman = new CharacterData();
            chaman.role = new Chaman();
            chaman.name = "Chaman";
            chaman.description = "I create an outcast and then duplicate it." +
                "\nI Lie and Disguise.";
            chaman.flavorText = "TBD";
            chaman.hints = "";
            chaman.ifLies = "";
            chaman.startingAlignment = EAlignment.Evil;
            chaman.type = ECharacterType.Minion;
            chaman.bluffable = false;
            chaman.picking = false;
            chaman.characterId = "chaman_AID";
            chaman.artBgColor = new Color(1f, 0f, 0f);
            chaman.cardBgColor = new Color(0.0941f, 0.0431f, 0.0431f);
            chaman.cardBorderColor = new Color(0.8208f, 0f, 0.0241f);
            chaman.color = new Color(0.8491f, 0.4555f, 0f);
            Characters.Instance.startGameActOrder = insertAfterAct("Chancellor", chaman);

            AscensionsData advancedAscension = ProjectContext.Instance.gameData.advancedAscension;
            foreach (CustomScriptData scriptData in advancedAscension.possibleScriptsData)
            {
                ScriptInfo script = scriptData.scriptInfo;
                addRole(script.startingTownsfolks, scover);
                addRole(script.startingTownsfolks, hout);
                addRole(script.startingTownsfolks, barchitect);
                addRole(script.startingMinions, chaman);
            }
        }

        public CharacterData[] allDatas = Array.Empty<CharacterData>();
        public override void OnUpdate()
        {
            if (allDatas.Length == 0)
            {
                var loadedCharList = Resources.FindObjectsOfTypeAll(Il2CppType.Of<CharacterData>());
                if (loadedCharList != null)
                {
                    allDatas = new CharacterData[loadedCharList.Length];
                    for (int i = 0; i < loadedCharList.Length; i++)
                    {
                        allDatas[i] = loadedCharList[i]!.Cast<CharacterData>();
                    }
                }
            }
        }

        public void addRole(Il2CppSystem.Collections.Generic.List<CharacterData> list, CharacterData data)
        {
            if (list.Contains(data))
            {
                return;
            }
            list.Add(data);
        }

        public CharacterData[] insertAfterAct(string previous, CharacterData data)
        {
            CharacterData[] actList = Characters.Instance.startGameActOrder;
            int actSize = actList.Length;
            CharacterData[] newActList = new CharacterData[actSize + 1];
            bool inserted = false;
            for (int i = 0; i < actSize; i++)
            {
                if (inserted)
                {
                    newActList[i + 1] = actList[i];
                }
                else
                {
                    newActList[i] = actList[i];
                    if (actList[i].name == previous)
                    {
                        newActList[i + 1] = data;
                        inserted = true;
                    }
                }
            }

            if (!inserted)
            {
                LoggerInstance.Msg("");
            }
            return newActList;
        }
    }
}