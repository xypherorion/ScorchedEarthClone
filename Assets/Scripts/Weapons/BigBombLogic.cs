using Assets.Scripts.Weapon.WeaponTypes;

namespace Assets.Scripts.Weapons
{
    public class BigBombLogic : ShooterLogic
    {
        public int RainbowNumber = 3;
        protected override void Update()
        {
            this.WeaponExplosionLogic.CreateExplosion((ExplosionType)this.RainbowNumber, position: transform.position, radius: transform.localScale.x / 2, fadeSpeed: 0.2f, delay: 0.2f, startAlpha: 0.3f);
            this.RainbowNumber = this.RainbowNumber == 3 ? 5 : 3;
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                if (Util.OutOfBounds(this.gameObject.transform.position))
                {
                    Destroy(this.gameObject);
                    return;
                }
                this.WeaponExplosionLogic.CreateExplosion(ExplosionType.OrangeExplosion);
                Destroy(this.gameObject);
            }
        }
    }
}
