using Assets.Scripts.Weapon.WeaponInteraction;

namespace Assets.Scripts.Weapon.WeaponTypes
{
    public class MeleeLogic : WeaponLogic, IMelee
    {
        public float damage;
        public float Damage { get { return this.damage; } }

        protected WeaponHitLogic WeaponHitLogic { get; set; }

        protected virtual void Awake()
        {
            this.WeaponHitLogic = GetComponent<WeaponHitLogic>();
        }

        protected virtual void Update()
        {
            if (this.WeaponHitLogic.UpdateHit() || Util.OutOfBounds(this.gameObject.transform.position))
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
