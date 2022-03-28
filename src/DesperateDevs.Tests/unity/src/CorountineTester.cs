using System.Collections;
using DesperateDevs.Unity;
using UnityEngine;

public class CorountineTester : MonoBehaviour
{
    void Start()
    {
        CoroutineRunner.Run<string>(GreetRoutine(), Debug.Log);
        CoroutineRunner.Run(GreetRoutine());
    }

    IEnumerator GreetRoutine()
    {
        Debug.Log("GreetRoutine start");
        var i = 0;
        while (i < 10)
        {
            i += 1;
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("GreetRoutine done");
        yield return "Greetings " + i;
    }
}
