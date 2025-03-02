using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorielManager : MonoBehaviour
{
    [SerializeField] private GeneratorBehaviour generator;


    // Start is called before the first frame update
    void Start()
    {
        generator.RemoveEnergie(78);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
