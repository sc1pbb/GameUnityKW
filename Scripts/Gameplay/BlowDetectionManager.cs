using UnityEngine;
using System.Collections;
using System;

public class BlowDetectionManager : MonoBehaviour
{


    void Start()
    {

    }
 
 void OnCollisionEnter(Collision otherObj) 
   {
    if (otherObj.gameObject.tag == "GG") {
        Destroy(gameObject,.5f);
    }
}
 
 
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
          Debug.LogWarning ("Offset");
           if (GameManager.singleton_.gameOver_ == false)
            {
               GameManager.singleton_.DestroyVisibleEnemies ();
			 Destroy(gameObject);
			 
            }
        }
			
		
        //  {
        //    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        //   RaycastHit _hits;
        //  if (Physics.Raycast(ray, out _hits, Mathf.Infinity))
        //  {
        //      if (_hits.transform.tag == "war")
        //      {
        //           Destroy(_hits.transform.gameObject);
        //     }
        //     }

		}
}