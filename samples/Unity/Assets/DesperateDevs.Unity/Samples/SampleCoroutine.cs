using System.Collections;
using UnityEngine;

namespace DesperateDevs.Unity.Samples
{
    public class SampleCoroutine : MonoBehaviour
    {
        void Start()
        {
            CoroutineRunner.Run<string>(SampleRoutine(), str => Debug.Log(str));
            CoroutineRunner.Run(SampleRoutine());
        }

        IEnumerator SampleRoutine()
        {
            Debug.Log("SampleRoutine start");
            var i = 0;
            while (i < 5)
            {
                i += 1;
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log("SampleRoutine done");
            yield return "Sample " + i;
        }

        void Update()
        {
            if (Input.anyKey)
                CoroutineRunner.StopAll();
        }
    }
}
