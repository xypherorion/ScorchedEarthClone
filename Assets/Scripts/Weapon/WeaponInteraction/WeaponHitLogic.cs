using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Weapon.WeaponInteraction
{
    public class WeaponHitLogic : HitLogic
    {
        private WeaponController WeaponController { get { return this.Player.GetComponent<WeaponController>(); } }
        private GameObject ClosestObject { get; set; }

        protected override void HandleHit()
        {
            base.HandleHit();
            GetClosestObject();
            InitiateHitUpdate();
        }

        private void GetClosestObject()
        {
            foreach(var obj in this.CollisionObjects.Where(x => x != this.Player))
                if (!obj.CompareTag("TerrainObject"))
                    if (this.ClosestObject == null)
                        this.ClosestObject = obj;
                    else if (Vector2.Distance(this._worldPosition, obj.transform.position) < Vector2.Distance(this._worldPosition, this.ClosestObject.transform.position))
                        this.ClosestObject = obj;         
        }

        private void CalculateHitPoint()
        {
            this._worldPosition = this.WeaponController.CalculateFirePoint(this.ClosestObject.transform.localScale, ClosestObject.transform.position, true);
        }

        protected override void InitiateHitUpdate()
        {
            if (this.ClosestObject != null)
            {
                CalculateHitPoint();
                this.ClosestObject.SendMessage("HasBeenHit", new Hit() { Sender = this, HitPoints = this.HitPoints, HitCenter = this._worldPosition, HitDamage = this._hitDamage, HitForce = this.HitForce, HitRadius = this.HitRadius }, SendMessageOptions.DontRequireReceiver);
            }

            base.InitiateHitUpdate();
        }

        public override void AddPushbackForce(Rigidbody2D rigidBody)
        {
            rigidBody.transform.position = new Vector3(rigidBody.transform.position.x, rigidBody.transform.position.y+(rigidBody.transform.localScale.y/2));

            var direction = (rigidBody.transform.position - this._worldPosition);
            var baseForce = direction.normalized * this.HitForce * 2;
            rigidBody.AddForce(baseForce);
        }

        public override float CalculateDamage(Vector3 position)
        {
            return this._hitDamage*this.Owner.Stats.DamageFactor;
        }

        private void DrawHitEffect() { }
    }
}
