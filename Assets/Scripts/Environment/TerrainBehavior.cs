using System.Collections.Generic;
using Assets.Scripts.Extra;
using Assets.Scripts.Weapon;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class TerrainBehavior : MonoBehaviour
    {
        public GameObject Fractile;
        private HitPoint closestPoint;
        private HitPoint closestPointAbove;
        private HitPoint closestPointAboveTopPoint;


        public void HasBeenHit(Hit hit)
        {
            //var render = this.GetComponent<Renderer>();
            //render.material.color = new Color(100, 50, 50);
            SetTerrainSize(hit.HitPoints);
            CreateTerrainFractile(hit);
        }

        private void SetTerrainSize(List<HitPoint> explosionPoints)
        {
            bool isEmptyBelow;
            bool isEmptyAbove;
            float minimalNoise = 0.09f;
            SetClosestPoint(explosionPoints, out isEmptyBelow, out isEmptyAbove);
            
            
            if (isEmptyBelow)
            {
                var matterToRemove = closestPoint.Vector3.y - (transform.position.y - (transform.localScale.y/2));
                var newScale = Mathf.Clamp(transform.localScale.y - (transform.localScale.y - matterToRemove), 0, transform.localScale.y);

                if (newScale > minimalNoise)
                {
                    var newPosition = transform.position.y - (transform.localScale.y / 2) + newScale / 2;
                    transform.localScale = new Vector3(transform.localScale.x, newScale, transform.localScale.z);
                    transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);
                }
                else
                    Destroy(this.gameObject);
            }
            

            if (!isEmptyAbove && closestPointAboveTopPoint.Vector3.y - closestPointAbove.Vector3.y > minimalNoise)
                GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainGenerator>().GenerateSpecificTerrainObject(closestPointAbove.Vector3, closestPointAboveTopPoint.Vector3);

            if (!isEmptyBelow)
                Destroy(this.gameObject);
        }

        private void SetClosestPoint(List<HitPoint> points, out bool isEmptyBelow, out bool isEmptyAbove)
        {
            isEmptyAbove = true;
            float topPoint = transform.position.y + (transform.localScale.y/2);
            float bottomPoint = transform.position.y - (transform.localScale.y/2);

            closestPoint = new HitPoint(new Vector3(), 10000);

            foreach (var obj in points.GetRange(180, 180))
            {
                if (obj.Degree <= 180)
                    continue;

                var result = Mathf.Abs(transform.position.x - obj.Vector3.x);
                if (result <= closestPoint.Distance)
                {
                    closestPoint = new HitPoint(obj.Vector3, result);
                    var index = obj.Degree == 0 ? 0 : obj.Degree == 180 ? 180 : 360 - obj.Degree;

                    if (topPoint > points[index].Vector3.y)
                    {
                        closestPointAbove = new HitPoint(new Vector3(transform.position.x, points[index].Vector3.y));
                        closestPointAboveTopPoint = new HitPoint(new Vector3(transform.position.x, topPoint));
                        isEmptyAbove = false;
                    }    
                }
            }

            isEmptyBelow = !(closestPoint.Vector3.y <= bottomPoint);
        }

        private void CreateTerrainFractile(Hit hit)
        {
            if (Random.Range(1, 4) == 3)
            {
                var position = new Vector3(hit.HitCenter.x, hit.HitCenter.y, 0);
                var fractile = Instantiate(this.Fractile, position, Quaternion.identity) as GameObject;
                Util.AssignObjectParent("Environment", fractile);
            }
        }
    }
}