using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class MoonBehavior : MonoBehaviour
    {
        private void Awake()
        {
            var render = GetComponent<Renderer>().bounds;
            var parallaxCamera = GameObject.FindGameObjectWithTag("Parallax").GetComponent<Camera>();
            this.transform.position = new Vector3(render.size.x/2*(parallaxCamera.fieldOfView/100),
                render.size.y * 2f * (parallaxCamera.fieldOfView/100)*0.8f, 20);
        }
    }
}
