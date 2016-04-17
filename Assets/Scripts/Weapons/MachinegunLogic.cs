using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons
{
    public class MachinegunLogic : ShooterLogic
    {
        protected override void Update()
        {
            this.WeaponExplosionLogic.CreateExplosion(ExplosionType.WhiteExplosion, position: transform.position, radius: transform.localScale.x/2, fadeSpeed: 0.2f);
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }
                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.WhiteExplosion, fadeSpeed: 0.2f);
                Destroy(this.gameObject);
            }
        }
    }
}
