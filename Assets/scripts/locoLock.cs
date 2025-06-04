using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class locoLock : MonoBehaviour
{
    [SerializeField] private GameObject loco;
    void OnTriggerEnter(Collider other)
    {
        switchLockLocomotion(loco.activeSelf);
    }
    public void switchLockLocomotion(bool active)
    {
        loco.SetActive(!active);
    }
}
