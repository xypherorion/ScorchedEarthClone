using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapon.WeaponInteraction
{
    public class WeaponExplosionLogic : HitLogic
    {
        protected override void HandleHit()
        {
            base.HandleHit();
            InitiateHitUpdate();
        }

        protected override void InitiateHitUpdate()
        {
            foreach (var obj in this.CollisionObjects.Where(x => x != this.Player))
                obj.SendMessage("HasBeenHit", new Hit() { Sender = this, HitPoints = this.HitPoints, HitCenter = this._worldPosition, HitDamage = this._hitDamage, HitForce = this.HitForce, HitRadius = this.HitRadius}, SendMessageOptions.DontRequireReceiver);

            base.InitiateHitUpdate();
        }

        public override void AddPushbackForce(Rigidbody2D rigidBody)
        {
            rigidBody.transform.position = new Vector3(rigidBody.transform.position.x, rigidBody.transform.position.y + (rigidBody.transform.localScale.y / 2));

            var direction = (rigidBody.transform.position - this._worldPosition);
            var wearoff = 1 - (direction.magnitude / this.HitRadius);
            var baseForce = direction.normalized * this.HitForce * wearoff;

            rigidBody.AddForce(baseForce);

            if (this._worldPosition.y <= (rigidBody.position.y + (rigidBody.transform.localScale.y/2)))
            {
                var upliftWearoff = 1 - this.HitUplift / this.HitRadius;
                var upliftForce = Vector2.up * this.HitForce * upliftWearoff;
                rigidBody.AddForce(upliftForce);
            }
        }
    }
}
