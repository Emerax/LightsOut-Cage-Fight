using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool monster=false;
    void Update() 
    {
        if (monster){
            Vector3 camPos=Camera.main.transform.position;
            camPos.y=0;  //arenan
            transform.LookAt(camPos);
        }
        else{
             transform.rotation=Camera.main.transform.rotation;
        }
       
        
        
    }
}
