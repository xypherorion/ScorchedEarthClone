using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Environment;
using Assets.Scripts.Extra;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Weapon.WeaponInteraction
{
    public class HitLogic : MonoBehaviour
    {
        public List<HitPoint> HitPoints;
        public List<GameObject> CollisionObjects;
        public PlayerBehaviour Owner { get; set; }
        protected GameObject Player;

        public float HitUplift;
        public float HitForce;
        public float HitRadius;
        protected float _hitRadius;
        protected float _hitDamage;

        protected Vector3 _screenPosition;
        protected Vector3 _worldPosition;

        protected bool _hitInitialized = false;
        protected bool _currentlyHandlingHit = false;

        public virtual void StartHit(float hitDamage = 0, GameObject player = null)
        {
            Util.PreventCameraIntereference(false);
            Initialize(hitDamage, player);
            CalculateScreenRadius();
            this._hitInitialized = true;
        }

        public bool UpdateHit()
        {
            if (!this._currentlyHandlingHit && this._hitInitialized)
            {
                HandleHit();
                return true;
            }
            return false;
        }

        protected void Initialize(float hitDamage = 0, GameObject player = null)
        {
            this._screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            this._worldPosition = transform.position;
            this._hitDamage = hitDamage;
            this.Player = player;
        }

        protected virtual void HandleHit()
        {
            this._currentlyHandlingHit = true;

            CreateHitPoints();
            GetHitObjects();
        }

        protected void CalculateScreenRadius()
        {
            var tempVector = new Vector3(_worldPosition.x, (_worldPosition.y + this.HitRadius));
            var tempScreenVector = Camera.main.WorldToScreenPoint(tempVector);
            this._hitRadius = tempScreenVector.y - this._screenPosition.y;
        }

        protected void CreateHitPoints()
        {
            var tempList = new List<HitPoint>();
            for (int i = 0; i < 360; i++)
            {
                var resultY = _screenPosition.y + this._hitRadius * Mathf.Sin(i * (Mathf.PI / 180));
                var resultX = _screenPosition.x + this._hitRadius * Mathf.Cos(i * (Mathf.PI / 180));
                var result = Camera.main.ScreenToWorldPoint(new Vector3(resultX, resultY, 0));
                tempList.Add(new HitPoint(result, degree: i));
            }
            this.HitPoints = tempList;
        }

        protected void GetHitObjects()
        {
            var tempList = new List<GameObject>();
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(new Vector2(_worldPosition.x, _worldPosition.y), this.HitRadius);
            foreach (var obj in hitColliders)
                tempList.Add(obj.gameObject);

            this.CollisionObjects = tempList;
        }

        protected virtual void InitiateHitUpdate()
        {
            Util.PreventCameraIntereference(true);
            var playerObjects = this.CollisionObjects.Where(x => x.CompareTag("Player")).Select(x => x.GetComponent<PlayerBehaviour>());
            var ignorePlayerId = this.Player != null ? this.Player.GetComponent<PlayerBehaviour>().Id : 0;

            foreach (var player in playerObjects)
            {
                if (player.Id == ignorePlayerId || Global.PlayersHit.Any(x => x.Id == player.Id))
                    continue;

                Global.PlayersHit.Add(player);
            }
        }

        protected virtual ExplosionBehavior InitiateExplosion(ExplosionType explosionType, Vector3 position, float radius, float fadeSpeed = 0.1f, float startAlpha = 1f, float delay = 0)
        {
            var explosion = Instantiate(Util.LoadExplosion(explosionType), position, Quaternion.identity) as GameObject;
            var component = explosion.GetComponent<ExplosionBehavior>();
            explosion.transform.localScale = new Vector3(radius*2, radius*2);
            component.FadeSpeed = fadeSpeed;
            component.FadeDelay = delay;
            component.SetStartAlpha(startAlpha);
            Util.AssignObjectParent("Explosions", explosion);
            return component;
        }

        public void CreateExplosion(ExplosionType explosionType, Vector3 position = default(Vector3), float radius = default(float), float fadeSpeed = 0.1f, float startAlpha = 1f, float delay = 0)
        {
            position = position == default(Vector3) ? this._worldPosition : position;
            radius = radius == default(float) ? this.HitRadius*0.9f : radius;
            var explosion = InitiateExplosion(explosionType, position, radius, fadeSpeed, startAlpha, delay);
            explosion.StartFade();
        }

        public virtual void AddPushbackForce(Rigidbody2D rigidBody)
        {
        }

        public virtual float CalculateDamage(Vector3 position)
        {
            var distance = Mathf.Sqrt((position.x - this._worldPosition.x) * (position.x - this._worldPosition.x) + (position.y - this._worldPosition.y) * (position.y - this._worldPosition.y));
            var percent = (1 - (distance / this.HitRadius)) > 0.25f ? (1 - (distance / this.HitRadius)) : 0.25f;

            return this._hitDamage * percent * this.Owner.Stats.DamageFactor;
        }
    }
}
