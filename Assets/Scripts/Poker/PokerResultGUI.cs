using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KazukiTrumpGame
{
    public class PokerResultGUI : CutInEventBase
    {
        [SerializeField] GameObject cutInObj;
        public void MoveHandTypeText() { StartCoroutine( MoveCutInTextObject(cutInObj, 1.5f)); }
    }
}
