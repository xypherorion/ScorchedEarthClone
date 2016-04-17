using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons.Utilities
{
    public class AimingPointLogic : UtilityLogic
    {
        public override void Fire()
        {
            base.Fire();
            this.PlayerBehavior.Stats.ShowTrajectory = true;
            this.PlayerBehavior.UtilityUsedController.ShowUtility(WeaponType.AimingPoint);
            this.WeaponController.SetTrajectory();
            Destroy(this.GameObject);
        }
    }
}
