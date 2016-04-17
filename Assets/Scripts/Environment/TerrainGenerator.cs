using System;
using Assets.Scripts.Extra;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class TerrainGenerator : MonoBehaviour
    {
        public GameObject TerrainObject;
        private readonly float _terrainSizeX = Global.TerrainSize;
        private readonly float _terrainSizeY = 1000;

        private float _multiplier;
        private float _offset;

        public Vector3 TerrainSize
        {
            get
            {
                if (this._terrainSize == Vector3.zero)
                {
                    var x = new Vector3((Util.PixelsPerUnit())* ((_terrainSizeX/Util.PixelsPerUnit())/(1/TerrainObject.transform.localScale.x)), 0).x;
                    this._terrainRatio = ((decimal)x/(decimal)Camera.main.ScreenToWorldPoint(new Vector3(this._terrainSizeX, 0)).x);
                    var y = Camera.main.ScreenToWorldPoint(new Vector3(0, this._terrainSizeY)).y * (float)this._terrainRatio;
                    this._terrainSize = new Vector3(x, y);
                }  
                return this._terrainSize;
            }
        }
        private Vector3 _terrainSize = Vector3.zero;
        private decimal _terrainRatio;
        private bool _currentlyCheckingTopValue;
        private float _currentGenerationTopValue = 0;

        void Start()
        {
            Initialize();
            Global.TerrainNullPoint = this.gameObject.transform.position;
            Global.TerrainEndPoint = this.TerrainSize;
            Global.TerrainRatio = this._terrainRatio;
            Util.StopScene();
        }

        private void Initialize()
        {
            SetRandomSeeds();
            Global.InitializeGameProgressStatusText = "Generating Terrain.";
            Util.PreventCameraIntereference(false);
        }
	
        void Update ()
        {
            if (!Global.IsTerrainGenerated && !this._currentlyCheckingTopValue)
                GenerateTerrainOnUpdate();
        }

        public void GenerateSpecificTerrainObject(Vector3 pointFrom, Vector3 pointTo)
        {
            TerrainObject.gameObject.transform.localScale = new Vector3(TerrainObject.gameObject.transform.localScale.x, pointTo.y-pointFrom.y);
            var newTerrainObject = Instantiate(TerrainObject, new Vector3(pointFrom.x, 0 + (TerrainObject.gameObject.transform.localScale.y / 2) + pointFrom.y, this.transform.position.z), Quaternion.identity) as GameObject;
            Util.AssignObjectParent("Terrain", newTerrainObject);
        }

        private void GenerateTerrainOnUpdate()
        {
            if (Global.IsTerrainGenerated)
                return;

            this._currentlyCheckingTopValue = true;

            var iterations = 50;
            var lastTopValue = this._currentGenerationTopValue;
            
            this._currentGenerationTopValue = lastTopValue + iterations * (float)Math.Round((decimal)TerrainObject.gameObject.transform.localScale.x, 2);
            
            if ((Camera.main.WorldToScreenPoint(new Vector3(_currentGenerationTopValue, 0)).x >=
                 Camera.main.WorldToScreenPoint(this.TerrainSize).x))
            {
                Global.InitializeGameProgress = (this._currentGenerationTopValue / this.TerrainSize.x) * 0.90f;
                Global.IsTerrainGenerated = true;
                Util.PreventCameraIntereference(true);
                Util.StartScene();
            }
                

            this._currentlyCheckingTopValue = false;

            var currentTerrainSize = lastTopValue;
            for (int i = 0; i < iterations; i++)
            {
                if ((Camera.main.WorldToScreenPoint(new Vector3(currentTerrainSize, 0)).x >= Camera.main.WorldToScreenPoint(this.TerrainSize).x))
                    return;
                
                TerrainObject.gameObject.transform.localScale = AssignTerrainObjectSize(currentTerrainSize);
                currentTerrainSize += (float)Math.Round((decimal)TerrainObject.gameObject.transform.localScale.x, 2);
                if (TerrainObject.gameObject.transform.localScale.y / 2 >= 0)
                {
                    var newTerrainObject = Instantiate(TerrainObject, new Vector3(currentTerrainSize, 0 + TerrainObject.gameObject.transform.localScale.y / 2, this.transform.position.z), Quaternion.identity) as GameObject;
                    Util.AssignObjectParent("Terrain", newTerrainObject);
                }
            }
            Global.InitializeGameProgress = (this._currentGenerationTopValue / this.TerrainSize.x) * 0.90f;
        }

        private Vector3 AssignTerrainObjectSize(float currentTerrainSizeX)
        {
            var x = (float)Math.Round((decimal)TerrainObject.gameObject.transform.localScale.x, 2);
            var y = (this.TerrainSize.y / 2) *
                    Mathf.PerlinNoise((currentTerrainSizeX + this._offset) * this._multiplier,
                        Mathf.Sqrt(currentTerrainSizeX < 0 ? currentTerrainSizeX * -1 : currentTerrainSizeX) * this._multiplier);
            return new Vector3(x, y);
        }

        private void SetRandomSeeds()
        {
            var random = UnityEngine.Random.Range(0.01f, 0.4f);
            var multiplier = random > 0.2f ? UnityEngine.Random.Range(0.1f, 0.2f) : random;
            this._multiplier = UnityEngine.Random.Range(0.01f, multiplier);
            this._offset = UnityEngine.Random.Range(0, 100000);
        }
    }
}
