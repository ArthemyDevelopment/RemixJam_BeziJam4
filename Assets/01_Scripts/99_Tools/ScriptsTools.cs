using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable once CheckNamespace
namespace ArthemyDev.ScriptsTools
{
    public static class ScriptsTools
    {

        //Change a variable from one value to another with a delay
        public static IEnumerator DelayedVarChange<T>(Action<T> variable, float time, T startValue, T endValue)
        {
            variable(startValue);
            yield return GetWait(time);
            variable(endValue);
        }

        //Truncate text to short it for currency purposes, leaving 2 decimals and the letter.
        public static string TruncateCurrencyValueToText(int value)
        {
            string text = value.ToString();
            if (value >= 1000000)
            {
                float temp = (float)value / 1000000;
                float trunc = Mathf.Floor(temp * 100f) / 100f;
                text = trunc.ToString("#.00");
                text += "M";
            }
            else if (value >= 1000)
            {
                float temp = (float)value / 1000;
                float trunc = Mathf.Floor(temp * 100f) / 100f;
                text = trunc.ToString("#.00");
                text += "K";
            }

            return text;
        }

        //Map a value from one range of floats accordingly to another range of floats, useful for health bars and similar.
        public static float MapValues(float value, float fromTargetRange, float toTargetRange, float fromRefRange,
            float toRefRange)
        {
            return (value - fromTargetRange) / (toTargetRange - fromTargetRange) * (toRefRange - fromRefRange) +
                   fromRefRange;
        }


        //Dictionary with wait for seconds, with the time as keys
        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary =
            new Dictionary<float, WaitForSeconds>();


        //Get a wait for second element in case it was already used before, optimizing its use
        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }


        //Allow to use AddComponent with a parameter
        public static TComponent AddComponent<TComponent, TFirstArgument>
            (this GameObject gameObject, TFirstArgument firstArgument)
            where TComponent : MonoBehaviour
        {
            Arguments<TFirstArgument>.First = firstArgument;


            var component = gameObject.AddComponent<TComponent>();

            Arguments<TFirstArgument>.First = default;

            return component;
        }

        //Get the angle between 2 objects
        public static float GetRotation(Transform originObject, Transform target)
        {
            return Mathf.Atan2(target.position.x - originObject.position.x,
                target.position.z - originObject.position.z) * Mathf.Rad2Deg;
        }

        public static float GetRotation2D(Transform originObject, Transform target)
        {
            return Mathf.Atan2(target.position.y - originObject.position.y,
                target.position.x - originObject.position.x) * Mathf.Rad2Deg;
        }

        //Shuffle a list and returns it as queue
        public static Queue<T> ShuffleList<T>(List<T> thisList) //Shuffle a list and returns it as queue
        {
            Queue<T> temp = new Queue<T>(thisList.OrderBy(a => Guid.NewGuid()).ToList());

            return temp;
        }

        //Clonen a gameobject if needed
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static void DestroyAllChildrens(Transform target)
        {
            foreach (Transform child in target)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static int WrapInt(int input, int min, int max)
        {
            if (input < min) return max - (min - input) % (max - min);
            else return min + (input - min) % (max - min);
        }
        
        public static string FormatTime(int totalSeconds)
        {
            int minutes = Mathf.FloorToInt(totalSeconds / 60f);
            int seconds = Mathf.FloorToInt(totalSeconds % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        /// <summary>
        /// Get formatted time string with milliseconds (MM:SS.mmm)
        /// </summary>
        public static string FormatTimeWithMilliseconds(int totalSeconds)
        {
            int minutes = Mathf.FloorToInt(totalSeconds / 60f);
            int seconds = Mathf.FloorToInt(totalSeconds % 60f);
            int milliseconds = Mathf.FloorToInt((totalSeconds * 1000f) % 1000f);
            return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }
        
#if UNITY_EDITOR
        //Get all instances of a scriptableObject in the project, useful when setting up databases
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[]
                guids = AssetDatabase.FindAssets("t:" +
                                                 typeof(T)
                                                     .Name); //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++) //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;

        }
#endif

    }

//Used in add component with parameter
    public static class Arguments<TFirstArgument>
    {
        public static TFirstArgument First { get; internal set; }

    }


//Premade singleton class for managers, just call init in awake 
    public abstract class SingletonManager<T> : SerializedMonoBehaviour where T : SingletonManager<T>
    {
        public static T current;

        protected virtual void Awake()
        {
            init();
        }

        public virtual void init()
        {
            if (current == null)
                current = this as T;
            else if (current != this)
            {
                Destroy(this);
            }

        }
    }
}

