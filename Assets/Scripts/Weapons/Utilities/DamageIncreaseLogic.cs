using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons.Utilities
{
    public class DamageIncreaseLogic : UtilityLogic
    {
        public override void Fire()
        {
            base.Fire();
            this.PlayerBehavior.Stats.DamageFactor = this.PlayerBehavior.Stats.DamageFactor * this.IncreaseBy;
            this.PlayerBehavior.UtilityUsedController.ShowUtility(WeaponType.DamageIncrease);
            Destroy(this.GameObject);
        }
    }
}
