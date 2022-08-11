using System.Collections;
using DesperateDevs.Unity;
using UnityEngine;

namespace Samples.DesperateDevs.Unity
{
    public class SampleCoroutine : MonoBehaviour
    {
        void Start()
        {
            CoroutineRunner.Run<string>(SampleRoutine(), Debug.Log);
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
