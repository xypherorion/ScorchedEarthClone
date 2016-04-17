using Assets.Scripts.Weapon;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class EnvironmentBehavior : MonoBehaviour
    {
        private Rigidbody2D RigidBody { get; set; }

        void Awake()
        {
            this.RigidBody = GetComponent<Rigidbody2D>();
        }

        public void HasBeenHit(Hit hit)
        {
            hit.Sender.AddPushbackForce(this.RigidBody);
        }
    }
}
