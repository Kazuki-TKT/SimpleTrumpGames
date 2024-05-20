using UnityEngine;

namespace KazukiTrumpGame.Memory
{
    public class MemoryCutInGUI : CutInEventBase
    {
       
        [SerializeField]
        GameObject startObject, allOpenObject;
        public void CutInObject(TurnType obj)
        {
            switch (obj)
            {
                case TurnType.PLAYER:
                    StartCoroutine(MoveCutInTextObject(startObject, 1.5f));
                    break;
                case TurnType.RESULT:
                    StartCoroutine(MoveCutInTextObject(allOpenObject, 1.5f));
                    break;
            }
        }
    }
}
