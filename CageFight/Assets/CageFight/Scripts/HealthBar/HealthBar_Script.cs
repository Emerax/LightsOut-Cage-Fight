using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_Script : MonoBehaviour
{
   private Image HealthBar;
   public MonsterBehaviour Monster;

   void Start(){
      HealthBar=GetComponent<Image>();
   }
   void Update(){
      //sDebug.Log(Monster.Data.health);
      HealthBar.fillAmount= Monster.Data.health/Monster.Data.MaxHealth;
   }
}
 
