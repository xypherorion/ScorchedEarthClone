using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons
{
    public class BombLogic : ShooterLogic
    {
        protected override void Update()
        {
            this.WeaponExplosionLogic.CreateExplosion(ExplosionType.WhiteExplosion, position: transform.position, radius: transform.localScale.x/2, fadeSpeed: 0.2f, delay: 0.2f, startAlpha: 0.3f);
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }
                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.WhiteExplosion);
                Destroy(this.gameObject);
            }
        }
    }
}
