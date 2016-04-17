using System;
using System.Collections;
using Assets.Scripts.Extra;
using Assets.Scripts.Weapon;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets
{
    public class Util
    {
        public static void AssignObjectParent(GameObject gameObject, string childTag)
        {
            var objects = GameObject.FindGameObjectsWithTag(childTag);
            foreach (var obj in objects)
                obj.transform.parent = gameObject.transform;
        }

        public static void AssignObjectParent(GameObject gameObject, GameObject childGameObject)
        {
            childGameObject.transform.parent = gameObject.transform;
        }

        public static void AssignObjectParent(string parentTag, GameObject childGameObject)
        {
            var parent = GameObject.FindGameObjectWithTag(parentTag);
            childGameObject.transform.parent = parent.transform;
        }

        public static void AssignObjectParent(string parentTag, string childTag)
        {
            var parent = GameObject.FindGameObjectWithTag(parentTag);
            var objects = GameObject.FindGameObjectsWithTag(childTag);
            foreach (var obj in objects)
                obj.transform.parent = parent.transform;
        }

        public static bool OutOfBounds(Vector3 position)
        {
            if (position.x > Global.TerrainEndPoint.x || position.x < Global.TerrainNullPoint.x)
                return true;

            if (position.y < Global.TerrainNullPoint.y || position.y > Global.TerrainEndPoint.y)
                return true;

            return false;
        }

        public static IEnumerator WaitUntilContinueWithDelegate(float tickDuration, Func<bool> toRun, float waitBetween, Action continueWith)
        {
            while (true)
            {
                if (toRun())
                {
                    break;
                }
                yield return new WaitForSeconds(tickDuration);
            }
            yield return new WaitForSeconds(waitBetween);
            continueWith();
        }

        public static IEnumerator WaitWithTicksEndByDelegate(float tickDuration, Func<bool> toRun, float maxDuration)
        {
            for (float i = 0; i < maxDuration; i++)
            {
                if (toRun())
                {
                    break;
                }
                yield return new WaitForSeconds(tickDuration);
            }
        }

        public static IEnumerator WaitWithTicksDelegate(float tickDuration, Action toRun, float maxDuration = Single.MaxValue)
        {
            for (float i = 0; i < maxDuration; i++)
            {
                toRun();
                yield return new WaitForSeconds(tickDuration);
            }
        }

        public static IEnumerator WaitWithTicksDelegateContinueWithDelegate(float tickDuration, Action toRun, Action continueWith, float maxDuration = Single.MaxValue)
        {
            for (float i = 0; i < maxDuration; i++)
            {
                toRun();
                yield return new WaitForSeconds(tickDuration);
            }
            continueWith();
        }

        public static IEnumerator WaitWithDelegateContinueWithDelegate(float durationToWait, Action toRun, float durationToBeforeWaitContinue, Action toRunAfter)
        {
            yield return new WaitForSeconds(durationToWait);
            toRun();
            yield return new WaitForSeconds(durationToBeforeWaitContinue);
            toRunAfter();
        }

        public static IEnumerator WaitWithDelegate(float durationToWait, Action toRun)
        {
            yield return new WaitForSeconds(durationToWait);
            toRun();
        }

        public static IEnumerator Wait(float durationToWait)
        {
            yield return new WaitForSeconds(durationToWait);
        }

        public static Object LoadWeapon(WeaponType weapon)
        {
            return Resources.Load("Weapons/" + ParseEnumToString(weapon), typeof(GameObject));
        }

        public static Object LoadExplosion(ExplosionType explosion)
        {
            return Resources.Load("Explosions/" + ParseEnumToString(explosion), typeof(GameObject));
        }

        public static void StopScene()
        {
            Time.timeScale = 0;
        }

        public static void StartScene()
        {
            Time.timeScale = 1;
        }

        public static T ParseStringToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string ParseEnumToString<T>(T enumValue)
        {
            return Enum.GetName(enumValue.GetType(), enumValue);
        }

        public static float UnitsPerPixel()
        {
            var p1 = Camera.main.ScreenToWorldPoint(Vector3.zero);
            var p2 = Camera.main.ScreenToWorldPoint(Vector3.right);
            return Vector3.Distance(p1, p2);
        }

        public static float PixelsPerUnit()
        {
            return 1 / UnitsPerPixel();
        }

        public static Bounds OrthographicBounds(Camera camera)
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            Bounds bounds = new Bounds(camera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }

        public static void PreventCameraIntereference(bool enable)
        {
            var component = Camera.main.GetComponent<CameraBehaviour>();
            component.enabled = enable;
        }

        public static Vector3 CalculateVelocity(float degreeOffset, float force = 20)
        {
            var radian = (Mathf.PI / 180) * degreeOffset;
            var velocity = new Vector3((float)Math.Cos(radian), (float)Math.Sin(radian));
            return new Vector3(velocity.x * ((force * 4) / 10), velocity.y * (force * 2 / 10));
        }
    }
}
