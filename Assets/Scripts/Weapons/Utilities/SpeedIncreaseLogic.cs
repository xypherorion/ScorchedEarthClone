using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons.Utilities
{
    public class SpeedIncreaseLogic : UtilityLogic
    {
        public override void Fire()
        {
            base.Fire();
            this.PlayerBehavior.Stats.MovementSpeed = this.PlayerBehavior.Stats.MovementSpeed * this.IncreaseBy;
            this.PlayerBehavior.UtilityUsedController.ShowUtility(WeaponType.SpeedIncrease);
            Destroy(this.GameObject);
        }
    }
}
