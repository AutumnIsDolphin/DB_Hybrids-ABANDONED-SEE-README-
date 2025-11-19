using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using System;

namespace DB_Hybrids;
[RegisterTypeInIl2Cpp]

internal class Scover : Role
{
    public Scover() : base(ClassInjector.DerivedConstructorPointer<Scover>())
    {
        ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
    }
    public Scover(IntPtr ptr) : base(ptr)
    {

    }

    public override string Description
    {
        get
        {
            return "Tells how many evils are adjacent to another evil.";
        }
    }

    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetInfo(charRef));
    }

    public override void BluffAct(ETriggerPhase trigger, Character charRef)
    {
        if (trigger != ETriggerPhase.Day) return;
        onActed?.Invoke(GetBluffInfo(charRef));
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        charRef = GetEvilsName(charRef);
        int evils = CheckAdjacentEvils(charRef);

        string info = ConjourInfo(evils, charRef.GetRegisterAs().name);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        Il2CppSystem.Collections.Generic.List<int> possibleEvils = new Il2CppSystem.Collections.Generic.List<int>();
        int allEvils = Gameplay.CurrentScript.minion + Gameplay.CurrentScript.demon;

        for (int i = 0; i < allEvils + 1; i++)
        {
            if (i == 3) break;
            possibleEvils.Add(i);
        }

        charRef = GetEvilsName(charRef);
        int evils = CheckAdjacentEvils(charRef);

        possibleEvils.Remove(evils);
        int randomEvilNumber = possibleEvils[UnityEngine.Random.Range(0, possibleEvils.Count)];
        string info = ConjourInfo(randomEvilNumber, charRef.GetRegisterAs().name);
        ActedInfo newInfo = new ActedInfo(info);

        return newInfo;
    }

    public Character GetEvilsName(Character charRef)
    {
        Il2CppSystem.Collections.Generic.List<Character> evils = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
        evils = Characters.Instance.FilterRealAlignmentCharacters(evils, EAlignment.Evil);

        charRef = evils[UnityEngine.Random.Range(0, evils.Count - 1)];

        return charRef;
    }

    public int CheckAdjacentEvils(Character charRef)
    {
        Il2CppSystem.Collections.Generic.List<Character> adjacentCharacters = new Il2CppSystem.Collections.Generic.List<Character>();
        foreach (Character ch in Gameplay.CurrentCharacters)
            if (charRef == ch)
            {
                adjacentCharacters = Characters.Instance.GetAdjacentCharacters(ch);
                break;
            }

        int evils = 0;

        foreach (Character ch in adjacentCharacters)
        {
            if (ch.GetAlignment() == EAlignment.Evil)
                evils++;
        }

        return evils;
    }

    public string ConjourInfo(int evils, string evil)
    {
        string info = "";
        if (evils == 0)
            info = $"NO Evils\nadjacent to {evil}";
        else if (evils == 1)
            info = $"{evils} Evil\nadjacent to {evil}";
        else
            info = $"{evils} Evils\nadjacent to {evil}";

        return info;
    }
}