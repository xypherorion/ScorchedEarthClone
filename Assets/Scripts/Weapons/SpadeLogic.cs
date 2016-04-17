using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons
{
    public class SpadeLogic : MeleeLogic
    {
        public override void Initialize(WeaponController weaponController)
        {
            base.Initialize(weaponController);
            this.WeaponHitLogic.HitRadius = this.Player.transform.localScale.x;
        }

        public override void Fire()
        {
            base.Fire();
            this.WeaponHitLogic.StartHit(this.Damage, this.Player);
            this.WeaponHitLogic.CreateExplosion(ExplosionType.HitExplosion, position: transform.position, radius: this.WeaponHitLogic.HitRadius, fadeSpeed: 0.2f);
        }
    }
}
