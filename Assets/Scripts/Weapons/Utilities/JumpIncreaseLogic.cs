using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons.Utilities
{
    public class JumpIncreaseLogic : UtilityLogic
    {
        public override void Fire()
        {
            base.Fire();
            this.PlayerBehavior.Stats.JumpForce = this.PlayerBehavior.Stats.JumpForce*this.IncreaseBy;
            this.PlayerBehavior.Stats.FallFactor = this.PlayerBehavior.Stats.FallFactor * (((this.IncreaseBy - 1) / 3) + 1);
            this.PlayerBehavior.UtilityUsedController.ShowUtility(WeaponType.JumpIncrease);
            Destroy(this.GameObject);
        }
    }
}
