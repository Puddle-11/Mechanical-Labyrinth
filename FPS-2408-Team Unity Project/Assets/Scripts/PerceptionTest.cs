using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[ExecuteInEditMode]
public class PerceptionTest : MonoBehaviour
{
    public string currentState;
    public test[] allStates;
    public IEnumerator currDelay;
    [System.Serializable]
    public struct test
    {
        public string name;
        public test2 t;
    }
    public enum test2
    {
        checkNone,
        checkSight,
        checkHearing,
        checkBoth,
    }

    public void Update()
    {
        for (int i = 0; i < allStates.Length; i++)
        {
            if (allStates[i].name == currentState)
            {
                switch (allStates[i].t)
                {
                    case test2.checkNone:
                        Debug.Log("none");
                        break;
                    case test2.checkSight:
                        Debug.Log("Sight");
                        break;
                    case test2.checkHearing:
                        Debug.Log("Hearing");
                        break;
                    case test2.checkBoth:
                        Debug.Log("Both");

                        break;
                    default:
                        break;
                }
            }
        }

    }
    public void ChangeState()
    {
        //resets


    }
    public IEnumerator delay()
    {

        yield return new WaitForSeconds(1);
    }

}
