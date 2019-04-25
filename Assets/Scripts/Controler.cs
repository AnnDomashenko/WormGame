using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controler : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public MySnike snike;
    private int i=2;
    public void OnBeginDrag(PointerEventData eventData)
    {
      
        if ((Mathf.Abs(eventData.delta.x)) > (Mathf.Abs(eventData.delta.y))) {
          
            if (eventData.delta.x > 0 && i!=-1)
            {
                snike.MoveRight();
                i = 1;
           

            }
            else if(i!=1) { 
            snike.MoveLeft();
                i = -1;
            }
        }
        else
        {
          
            if (eventData.delta.y > 0 && i!=-2)
            {
                snike.MoveUp();
                i = 2;
            }
            else if (i!=2){ 
            snike.MoveDown();
                i = -2;  
            }
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
