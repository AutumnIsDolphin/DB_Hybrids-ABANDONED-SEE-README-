using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Hybrids;
[RegisterTypeInIl2Cpp]

public class Chaman : Minion
{
    public Chaman() : base(ClassInjector.DerivedConstructorPointer<Chaman>())
    {
        ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
    }

    public Chaman(IntPtr ptr) : base(ptr)
    {

    }

    public override string Description
    {
        get
        {
            return "Creates an outcast and duplicates it";
        }
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("");
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Start) return;

        Character pickedCharacter = new Character();

        // creating
        Il2CppSystem.Collections.Generic.List<Character> viableCharacters = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
        Il2CppSystem.Collections.Generic.List<Character> viableCharactersNew = new Il2CppSystem.Collections.Generic.List<Character>();

        foreach (Character character in viableCharacters)
            viableCharactersNew.Add(character);

        Il2CppSystem.Collections.Generic.List<CharacterData> notInPlayOutsiders = Gameplay.Instance.GetAscensionAllStartingCharacters();
        notInPlayOutsiders = Characters.Instance.FilterNotInDeckCharactersUnique(notInPlayOutsiders);
        notInPlayOutsiders = Characters.Instance.FilterRealCharacterType(notInPlayOutsiders, ECharacterType.Outcast);

        if (notInPlayOutsiders.Count == 0)
        {
            notInPlayOutsiders = Gameplay.Instance.GetAllAscensionCharacters();
            notInPlayOutsiders = Characters.Instance.FilterRealCharacterType(notInPlayOutsiders, ECharacterType.Outcast);
        }

        CharacterData pickedOutsider = notInPlayOutsiders[UnityEngine.Random.Range(0, notInPlayOutsiders.Count - 1)];

        if (notInPlayOutsiders.Count != 0)
        {
            Gameplay.Instance.AddScriptCharacter(ECharacterType.Outcast, pickedOutsider);

            viableCharactersNew = Characters.Instance.FilterAliveCharacters(viableCharactersNew);
            viableCharactersNew = Characters.Instance.FilterRealCharacterType(viableCharactersNew, ECharacterType.Villager);

            pickedCharacter = viableCharactersNew[UnityEngine.Random.Range(0, viableCharactersNew.Count)];
            pickedCharacter.Init(pickedOutsider);
            viableCharactersNew.Remove(pickedCharacter);
            notInPlayOutsiders.Remove(pickedOutsider);
        }

        // duplicating
        Il2CppSystem.Collections.Generic.List<Character> villagers = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
        Il2CppSystem.Collections.Generic.List<Character> villagersNew = new Il2CppSystem.Collections.Generic.List<Character>();

        foreach (Character character in villagers)
            villagersNew.Add(character);

        villagersNew = Characters.Instance.FilterCharacterType(villagers, ECharacterType.Villager);

        Character pickedVillager = villagersNew[UnityEngine.Random.Range(0, villagersNew.Count)];
        pickedVillager.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);

        villagersNew.Remove(pickedVillager);
        Character replacedVillager = villagersNew[UnityEngine.Random.Range(0, villagersNew.Count)];

        replacedVillager.InitWithNoReset(pickedCharacter.GetCharacterBluffIfAble());
        replacedVillager.Act(ETriggerPhase.Start);
        replacedVillager.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);
    }
}