using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons.Utilities
{
    public class SmallHealthPotionLogic : UtilityLogic
    {
        public override void Fire()
        {
            base.Fire();
            this.PlayerBehavior.PlayerbarController.IsVisible = true;
            StartCoroutine(Util.WaitUntilContinueWithDelegate(0.5f, () =>
            {
                if (this.PlayerBehavior.PlayerbarController.IsCurrentlyVisible)
                {
                    this.PlayerBehavior.Health = this.PlayerBehavior.Health + this.IncreaseBy > 100 ? 100 : this.PlayerBehavior.Health + this.IncreaseBy;
                    return true;
                }
                return false;
            }, 1f, () =>
            {
                this.PlayerBehavior.PlayerbarController.IsVisible = false;
                Destroy(this.GameObject);
            }));
        }
    }
}
