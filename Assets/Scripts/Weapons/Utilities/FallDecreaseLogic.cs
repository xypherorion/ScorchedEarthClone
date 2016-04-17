using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons.Utilities
{
    public class FallDecreaseLogic : UtilityLogic
    {
        public override void Fire()
        {
            base.Fire();
            this.PlayerBehavior.Stats.FallFactor = this.PlayerBehavior.Stats.FallFactor * this.IncreaseBy;
            this.PlayerBehavior.UtilityUsedController.ShowUtility(WeaponType.FallDecrease);
            Destroy(this.GameObject);
        }
    }
}
