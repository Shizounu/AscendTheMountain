using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battlefield.Controller
{
    [CreateAssetMenu(fileName = "State_Base", menuName = "BattlefieldController/Base")]
    public class State_Base : ControllerState
    {
        public LayerMask tileLayer;
        public override void onLeftClick(Controller controller)
        {
            Vector2 mousePosition = controller.inputActions.BattlefieldControl.MousePosition.ReadValue<Vector2>();
            Ray r = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, 100, tileLayer);

            if(rh.collider == null)
                return;

            controller.selectedTile = rh.transform.GetComponent<Tile>().gridPosition;
        }

        public override void onRightClick(Controller controller)
        {
            controller.selectedTile = new Vector2Int(-1, -1);
            controller.selectedCard = null;
        }
    }
    
}
