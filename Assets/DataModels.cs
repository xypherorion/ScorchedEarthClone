using System;
using Assets.Scripts.Weapon;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    [Serializable]
    public class HitPoint
    {
        public Vector3 Vector3;
        public float Distance;
        public int Degree;

        public HitPoint(Vector3 vector, float distance = default(float), int degree = default(int))
        {
            this.Vector3 = vector;
            this.Distance = distance;
            this.Degree = degree;
        }
    }

    [Serializable]
    public class Trigger
    {
        public bool IsTrigger;
        public float TotalTicks;
        public float TimeBetweenTicks;

        public Trigger(bool isTrigger = false, float totalTicks = 0, float timeBetweenTicks = 0)
        {
            this.IsTrigger = isTrigger;
            this.TotalTicks = totalTicks;
            this.TimeBetweenTicks = timeBetweenTicks;
        }
    }

    [Serializable]
    public class CrosshairData
    {
        public int From;
        public int To;
        public int Start;

        public CrosshairData(int from = 0, int to = 360, int start = 45)
        {
            this.From = from;
            this.To = to;
            this.Start = start;
        }
    }

    [Serializable]
    public class PowerData
    {
        public int From;
        public int To;
        public int Start;

        public PowerData(int from = 0, int to = 100, int start = 50)
        {
            this.From = from;
            this.To = to;
            this.Start = start;
        }
    }

    [Serializable]
    public class InventoryItem
    {
        public WeaponType Weapon;
        public int Quantity;

        public InventoryItem(WeaponType weapon, int quantitiy = 0)
        {
            this.Weapon = weapon;
            this.Quantity = quantitiy;
        }
    }

    [Serializable]
    public class Score
    {
        public int Money;
        public int Points;
        public int Kills;

        public Score(int money, int points, int kills)
        {
            this.Money = money;
            this.Points = points;
            this.Kills = kills;
        }
    }

    [Serializable]
    public class Stats
    {
        public float MovementSpeed;
        public float JumpForce;
        public float FallFactor;
        public float DamageFactor;
        public bool ShowTrajectory;

        public Stats(float movementSpeed, float jumpForce, float fallFactor, float damageFactor, bool showTrajectory)
        {
            this.MovementSpeed = movementSpeed;
            this.JumpForce = jumpForce;
            this.FallFactor = fallFactor;
            this.DamageFactor = damageFactor;
            this.ShowTrajectory = showTrajectory;
        }

        public Stats(Stats stats)
        {
            this.MovementSpeed = stats.MovementSpeed;
            this.JumpForce = stats.JumpForce;
            this.FallFactor = stats.FallFactor;
            this.DamageFactor = stats.DamageFactor;
            this.ShowTrajectory = stats.ShowTrajectory;
        }
    }

    public class Item
    {
        public WeaponType Type { get; private set; }
        public IWeapon Weapon { get; private set; }
        public Button Button { get; private set; }
        public Sprite Sprite { get; private set; }
        public bool IsUtility { get; private set; }

        public Item(WeaponType type, IWeapon weapon, Button button, Sprite sprite)
        {
            Type = type;
            Weapon = weapon;
            Button = button;
            Sprite = sprite;
            IsUtility = weapon is IUtility || weapon is ITool;
        }
    }

    public interface IVisible
    {
        void IsVisible(bool enable);
        bool CheckVisibilityChanges();
    }

    public interface IUiControl { }

    public enum Direction
    {
        Up, Down
    }

    public enum PlayerType
    {
        Human,
        Npc,
        HumanViaNetwork,
        NpcViaNetwork
    }

    public enum PlayerState
    {
        Idle, HasLostHealth, HasTriggeredHealthLoss, IsActive, IsUsingUtility, IsFiring, IsFiringWithTrigger, IsEndingTurn, IsDone, IsDead, Inactive
    }

    public enum ExplosionType
    {
        WhiteExplosion, LightExplosion, EarthExplosion, HitExplosion, GreenExplosion, OrangeExplosion, PinkExplosion
    }
}
