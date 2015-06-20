using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IceBlink2
{
    [Serializable]
    public class ContentNode
    {        
        public int idNum = -1;
        public bool pcNode = true;
        public int linkTo = 0;
        public bool ShowOnlyOnce = false;
        public bool NodeIsActive = true;
        public string NodePortraitBitmap = "";
        public string NodeNpcName = "";
        public string NodeSound = "none";
        public string conversationText = "Continue";
        public bool IsExpanded = true;
        public List<ContentNode> subNodes = new List<ContentNode>();
        public List<Action> actions = new List<Action>();
        public List<Condition> conditions = new List<Condition>();
        public bool isLink = false;

        public ContentNode()
        {
        }

        public ContentNode SearchContentNodeById(int checkIdNum)
        {
            ContentNode tempNode = null;
            if (idNum == checkIdNum)
            {
                return this;
            }
            foreach (ContentNode subNode in subNodes)
            {
                tempNode = subNode.SearchContentNodeById(checkIdNum);
                if (tempNode != null)
                {
                    return tempNode;
                }
            }
            return null;
        }                
    }        
}
