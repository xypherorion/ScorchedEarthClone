namespace Assets.Scripts.Weapon.WeaponTypes
{
    public class UtilityLogic : WeaponLogic, IUtility
    {
        public float increaseBy;
        public float IncreaseBy { get { return this.increaseBy; } }

        public override void Fire()
        {
            if (this.PlayerBehavior.State != PlayerState.IsUsingUtility)
                return;
        }
    }
}
