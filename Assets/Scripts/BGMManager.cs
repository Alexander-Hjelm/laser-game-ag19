using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    //Singleton
    public static BGMManager instance = null;

    void Awake() {
        if(instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
