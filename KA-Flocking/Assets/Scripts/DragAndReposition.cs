
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


//credit to YoungjaeKim, https://forum.unity.com/threads/drag-drop-game-objects-without-rigidbody-with-the-mouse.64169/
public class DragAndReposition : MonoBehaviour{
    private bool _mouseState;
    int troopLayer = 1 << 8;
    int planeLayer = 1 << 9;
    private GameObject target;
    public Vector3 screenSpace;
    public Vector3 offset;

    // Update is called once per frame
    void Update(){
        //mouse left click, and currently in setup 1 or setup 2 scene
        if (Input.GetMouseButtonDown(0) && (SceneManager.GetSceneByName("PlayerOneSetupScene").isLoaded || SceneManager.GetSceneByName("PlayerTwoSetupScene").isLoaded)){ 
            
            RaycastHit hit;
            target = RetrieveTroopObject(out hit);
            if (target != null) //if raycast found a troop, change its position based on mouse cursor
            {
                _mouseState = true;
                screenSpace = Camera.main.WorldToScreenPoint(target.transform.position);

                offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
            }
        }
        if (Input.GetMouseButtonUp(0)){
            _mouseState = false;
        }
        if (_mouseState)
        {
            //keep track of the mouse position
            var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);

            //convert the screen mouse position to world point and adjust with offset
            var curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;

            if (Physics.Raycast(new Vector3(curPosition.x, 100, curPosition.z), Vector3.down * 100f, out RaycastHit hit, Mathf.Infinity, planeLayer))
            {
                curPosition = hit.point;
            }
            //update the position of the object in the world
            target.transform.position = curPosition;
        }
    }


    GameObject RetrieveTroopObject(out RaycastHit hit){
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raytracing towards mouse cursor from camera
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, troopLayer))
        {
            target = hit.collider.gameObject; //if we hit a collider of a troop -> return its gameobject
        }
        return target;
    }
}
