using Assets.Scripts.Weapon;
using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons
{
    public class DiggerLogic : ToolLogic
    {
        protected override void Update()
        {
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }
                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.EarthExplosion, fadeSpeed: 0.3f);
                Destroy(this.gameObject);
            }
        }

        public override void Initialize(WeaponController weaponController)
        {
            base.Initialize(weaponController);
            this.WeaponExplosionLogic.HitRadius = transform.localScale.x*2;
            transform.position = this.WeaponController.CalculateFirePoint(this.Player.transform.localScale);
        }

        public override void Fire()
        {
            base.Fire();
            this.WeaponExplosionLogic.StartHit(this.Damage, this.Player);
        }
    }
}
