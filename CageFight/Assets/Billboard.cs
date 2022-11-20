using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool monster=false;
    void Update() 
    {
        if (monster){
            transform.LookAt(transform.position - Camera.main.transform.forward);
        }
        else{
             transform.rotation=Camera.main.transform.rotation;
        }
       
        
        
    }
}
