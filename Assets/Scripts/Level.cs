using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level {

    public List<ServiceType> servicesAtLevel = new List<ServiceType>();
    public float time = 1;
    public Transform spawnPoint;
    public Transform door;
    public List<Transform> seats = new List<Transform>();
    public List<bool> occupied = new List<bool>();

}
