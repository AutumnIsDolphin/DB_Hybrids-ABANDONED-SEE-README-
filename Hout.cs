using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;

namespace DB_Hybrids;
[RegisterTypeInIl2Cpp]

internal class Hout : Role
{
    public Hout() : base(ClassInjector.DerivedConstructorPointer<Hout>())
    {
        ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
    }

    public Hout(IntPtr ptr) : base(ptr)
    {

    }

    public override string Description
    {
        get
        {
            return "Tells how far a random evil is from my closest evil.";
        }
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        string info = "";

        Il2CppSystem.Collections.Generic.List<Character> allEvils = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
        Il2CppSystem.Collections.Generic.List<Character> allEvilsNew = new Il2CppSystem.Collections.Generic.List<Character>();

        foreach (Character character in allEvils)
            allEvilsNew.Add(character);

        allEvilsNew = Characters.Instance.FilterRealAlignmentCharacters(allEvilsNew, EAlignment.Evil);
        allEvilsNew = Characters.Instance.RemoveCharacterType<Recluse>(allEvilsNew);

        Character closestEvilCh = GetClosestEvil(charRef);
        Character pickedEvil = allEvilsNew[UnityEngine.Random.Range(0, allEvilsNew.Count)];

        int closestEvil = GetClosestEvilToEvil(pickedEvil, closestEvilCh);

        info = ConjourInfo(pickedEvil.GetRegisterAs().name, closestEvil);
        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
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

    public override ActedInfo GetBluffInfo(Character charRef)
    {
        float randomId = UnityEngine.Random.Range(0f, 1f);
        Il2CppSystem.Collections.Generic.List<Character> allEvils = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
        Il2CppSystem.Collections.Generic.List<Character> allEvilsNew = new Il2CppSystem.Collections.Generic.List<Character>();

        foreach (Character character in allEvils)
            allEvilsNew.Add(character);

        allEvilsNew = Characters.Instance.FilterRealAlignmentCharacters(allEvilsNew, EAlignment.Evil);
        allEvilsNew = Characters.Instance.RemoveCharacterType<Recluse>(allEvilsNew);

        Character closestEvilCh = GetClosestEvil(charRef);
        Character pickedEvil = allEvilsNew[UnityEngine.Random.Range(0, allEvilsNew.Count)];

        int id = GetClosestEvilToEvil(pickedEvil, closestEvilCh);
        id = Calculator.RemoveNumberAndGetRandomNumberFromList(id, 0, 3);

        string info = "";
        info = ConjourInfo(pickedEvil.dataRef.name, id);

        ActedInfo newInfo = new ActedInfo(info);
        return newInfo;
    }

    public int GetClosestEvilToEvil(Character pickedEvil, Character chRef)
    {
        int count = 0;
        int savedCount = 100;

        Il2CppSystem.Collections.Generic.List<Character> myList = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
        Il2CppSystem.Collections.Generic.List<Character> myListNew = new Il2CppSystem.Collections.Generic.List<Character>();

        foreach (Character character in myList)
            myListNew.Add(character);

        myListNew = CharactersHelper.GetSortedListWithCharacterFirst(myListNew, pickedEvil);
        myListNew.RemoveAt(0);

        for (int i = 0; i < myListNew.Count; i++)
        {
            if (myListNew[i].GetAlignment() == EAlignment.Evil)
            {
                savedCount = count;
                count = 0;
                break;
            }
            count++;
        }
        count = 0;

        for (int i = myListNew.Count - 1; i > 0; i--)
        {
            if (myListNew[i].GetAlignment() == EAlignment.Evil)
            {
                if (count < savedCount)
                {
                    savedCount = count;
                    count = 0;
                }
                break;
            }
            count++;
        }
        return savedCount;
    }

    public Character GetClosestEvil(Character charRef)
    {
        Il2CppSystem.Collections.Generic.List<Character> clockwise = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
        Il2CppSystem.Collections.Generic.List<Character> clockwiseNew = new Il2CppSystem.Collections.Generic.List<Character>();

        Il2CppSystem.Collections.Generic.List<Character> counterClockwise = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
        Il2CppSystem.Collections.Generic.List<Character> counterClockwiseNew = new Il2CppSystem.Collections.Generic.List<Character>();

        Character charRefC = new Character();
        Character charRefCC = new Character();

        foreach (Character character in clockwise)
            clockwiseNew.Add(character);

        foreach (Character character in counterClockwise)
            counterClockwiseNew.Add(character);

        foreach (Character ch in Gameplay.CurrentCharacters)
        {
            counterClockwiseNew.Remove(ch);
            if (charRef == ch)
            {
                counterClockwiseNew.Remove(ch);
                break;
            }
        }
        foreach (Character ch in Gameplay.CurrentCharacters)
        {
            if (charRef == ch)
                break;

            counterClockwiseNew.Add(ch);
        }

        clockwiseNew = new Il2CppSystem.Collections.Generic.List<Character>(counterClockwiseNew.Pointer);
        clockwiseNew.Reverse();

        int clockwiseNumber = 0;
        int counterClockwiseNumber = 0;

        foreach (Character c in counterClockwiseNew)
        {
            counterClockwiseNumber++;
            if (c.GetAlignment() == EAlignment.Evil)
                charRefCC = c;
        }

        foreach (Character c in clockwiseNew)
        {
            clockwiseNumber++;
            if (c.GetAlignment() == EAlignment.Evil)
                charRefC = c;
        }

        if (clockwiseNumber > counterClockwiseNumber)
            return charRefCC;
        if (clockwiseNumber < counterClockwiseNumber)
            return charRefC;

        return charRefC;
    }

    public string ConjourInfo(string charName, int steps)
    {
        if (steps > 20)
            return $"There is only 1 Evil";
        else if (steps == 0)
            return $"{charName} is\n{steps + 1} card away\nfrom my closest Evil";
        else
            return $"{charName} is\n{steps + 1} cards away\nfrom my closest Evil";
    }
}