using UnityEngine;

namespace Assets.Scripts.Extra
{
    public class CameraBehaviour : MonoBehaviour
    {
        private GameObject _objectToTrack;
        public bool Locked;

        private float _minY;
        private float _maxY;
        private float _minX;
        private float _maxX;
        public float LerpSpeed;

        private float _x;
        private float _y;
        private float _nx;
        private float _ny;
        
        void Start()
        {
            _x = transform.position.x;
            _y = transform.position.y;
            _nx = transform.position.x;
            _ny = transform.position.y;
            GetMapBounds();
        }

        void Update()
        {
            if (!Global.IsTerrainGenerated)
                return;

            if (!this.Locked)
            {
                MovementByKey();
                MovementByMouse();
            }
            
            _x = Mathf.Lerp(transform.position.x, Mathf.Clamp(_nx, _minX, _maxX), LerpSpeed);
            _y = Mathf.Lerp(transform.position.y, Mathf.Clamp(_ny, _minY, _maxY), LerpSpeed);

            transform.position = new Vector3(_x, _y, -10);
        }

        public void SetTarget(Vector3 target)
        {
            _nx = target.x;
            _ny = target.y;
        }

        public void SetObjectToTrack(GameObject target)
        {
            _objectToTrack = target;
        }

        private bool IsKeyInput()
        {
            float xval = Input.GetAxis("Horizontal");


            return (xval < 0.0f || xval > 0.0f);
        }

        private void MovementByKey()
        {
            if (!IsKeyInput() || !Global.PlayersReady)
                return;

            _nx = _objectToTrack.transform.position.x;
            _ny = _objectToTrack.transform.position.y;
        }

        private void MovementByMouse()
        {
            if (IsKeyInput() || Global.IsInventoryOpen)
                return;

            float widthMargin = Screen.width * 0.1f;
            float heightMargin = Screen.height * 0.1f;

            Vector3 mouse = Input.mousePosition;

            if (mouse.x < Screen.width * 0.1f)
            {
                _nx = transform.position.x - Map(mouse.x, 0.0f, widthMargin, 1.0f, 0.0f);
            }
            else if (mouse.x > Screen.width * 0.9f)
            {
                _nx = transform.position.x + Map(mouse.x, Screen.width - widthMargin, Screen.width, 0.0f, 1.0f);
            }

            if (mouse.y < Screen.height * 0.1f)
            {
                _ny = transform.position.y - Map(mouse.y, 0.0f, heightMargin, 1.0f, 0.0f);
            }
            else if (mouse.y > Screen.height * 0.9f)
            {
                _ny = transform.position.y + Map(mouse.y, Screen.height - heightMargin, Screen.height, 0.0f, 1.0f);
            }

        }

        private void GetMapBounds()
        {
            _minX = ((Global.TerrainNullPoint.x + Util.OrthographicBounds(Camera.main).extents.x));
            _minY = (Global.TerrainNullPoint.y + Util.OrthographicBounds(Camera.main).extents.y);
            _maxX = Global.TerrainEndPoint.x - Util.OrthographicBounds(Camera.main).extents.x;
            _maxY = (Global.TerrainEndPoint.y - Util.OrthographicBounds(Camera.main).extents.y);
        }

        private float Map(float value, float min1, float max1, float min2, float max2)
        {
            return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
        }
    }
}
