using Assets.Scripts.Weapon.WeaponInteraction;

namespace Assets.Scripts.Weapon.WeaponTypes
{
    public class ToolLogic : WeaponLogic, ITool
    {
        public float damage;
        public float Damage { get { return this.damage; } }

        protected WeaponExplosionLogic WeaponExplosionLogic { get; set; }

        protected virtual void Awake()
        {
            this.WeaponExplosionLogic = GetComponent<WeaponExplosionLogic>();
        }

        protected virtual void Update()
        {
            if (this.WeaponExplosionLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
            {
                Destroy(this.gameObject);
            }
        }

        public override void Fire()
        {
            if (this.PlayerBehavior.State != (this.Trigger.IsTrigger ? PlayerState.IsFiringWithTrigger : PlayerState.IsFiring))
                return;
        }
    }
}
