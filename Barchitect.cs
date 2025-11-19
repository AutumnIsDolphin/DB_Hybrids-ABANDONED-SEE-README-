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

internal class Barchitect : Role
{
    public Barchitect() : base(ClassInjector.DerivedConstructorPointer<Barchitect>())
    {
        ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
    }

    public Barchitect(IntPtr ptr) : base(ptr)
    {

    }

    public override string Description
    {
        get
        {
            return "Learn which side is more corrupted.";
        }
    }

    public enum ECircleSide
    {
        Both = 0,
        Left = 10,
        Right = 20,
    }

    public class ArchitectInfo
    {
        public ECircleSide side;
        public Il2CppSystem.Collections.Generic.List<Character> characters = new Il2CppSystem.Collections.Generic.List<Character>();
    }

    public override ActedInfo GetInfo(Character charRef)
    {
        ArchitectInfo infos = GetSideOfCircle(charRef, true);

        string info = ConjourInfo(infos.side);

        ActedInfo newInfo = new ActedInfo(info, infos.characters);
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
        ArchitectInfo infos = GetSideOfCircle(charRef, false);

        string info = ConjourInfo(infos.side);

        ActedInfo newInfo = new ActedInfo(info, infos.characters);
        return newInfo;
    }

    public ArchitectInfo GetSideOfCircle(Character charRef, bool truth)
    {
        Il2CppSystem.Collections.Generic.List<Character> tempList = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
        Il2CppSystem.Collections.Generic.List<Character> tempListNew = new Il2CppSystem.Collections.Generic.List<Character>();

        foreach (Character character in tempList)
            tempListNew.Add(character);

        int circleSize = tempListNew.Count;

        tempListNew.Add(tempListNew[0]);

        int i = 0;
        int leftCorrupts = 0;
        int rightCorrupts = 0;
        Il2CppSystem.Collections.Generic.List<Character> leftChars = new Il2CppSystem.Collections.Generic.List<Character>();
        Il2CppSystem.Collections.Generic.List<Character> rightChars = new Il2CppSystem.Collections.Generic.List<Character>();
        foreach (Character c in tempListNew)
        {
            if (i <= circleSize / 2)
            {
                leftChars.Add(c);
                if (c.statuses.Contains(ECharacterStatus.Corrupted))
                    leftCorrupts++;
            }
            if (i >= (circleSize + 1) / 2)
            {
                rightChars.Add(c);
                if (c.statuses.Contains(ECharacterStatus.Corrupted))
                    rightCorrupts++;
            }
            i++;
        }

        ArchitectInfo infos = new ArchitectInfo();

        infos.side = ECircleSide.Both;
        if (leftCorrupts > rightCorrupts)
            infos.side = ECircleSide.Left;
        if (leftCorrupts < rightCorrupts)
            infos.side = ECircleSide.Right;

        if (!truth)
        {
            bool isBoth = Calculator.RollDice(10) > 9 ? true : false;

            if (infos.side == ECircleSide.Left)
                infos.side = ECircleSide.Right;
            else if (infos.side == ECircleSide.Right)
                infos.side = ECircleSide.Left;

            if (infos.side == ECircleSide.Both)
            {
                if (Calculator.RollDice(10) >= 5)
                    infos.side = ECircleSide.Left;
                else
                    infos.side = ECircleSide.Right;
            }
            else if (infos.side != ECircleSide.Both)
                if (isBoth)
                    infos.side = ECircleSide.Both;
        }

        if (infos.side == ECircleSide.Left)
            infos.characters = leftChars;
        if (infos.side == ECircleSide.Right)
            infos.characters = rightChars;

        return infos;
    }

    public string ConjourInfo(ECircleSide side)
    {
        string info = "";
        if (side == ECircleSide.Left)
            info = $"Left side is more Corrupted";
        if (side == ECircleSide.Right)
            info = $"Right side is more Corrupted";
        if (side == ECircleSide.Both)
            info = $"Both sides are equally Corrupted";
        return info;
    }
}