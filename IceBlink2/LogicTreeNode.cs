using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IceBlink2
{
    /*public class LogicTreeNode
    {
        public int idNum = -1;
        public int linkTo = 0;
        public string nodeText = "Continue";
        public bool IsExpanded = true;
        public List<LogicTreeNode> subNodes = new List<LogicTreeNode>();
        public List<Action> actions = new List<Action>();
        public List<Condition> conditions = new List<Condition>();
        public bool isLink = false;

        public LogicTreeNode()
        {

        }
        public LogicTreeNode SearchContentNodeById(int checkIdNum)
        {
            LogicTreeNode tempNode = null;
            if (idNum == checkIdNum)
            {
                return this;
            }
            foreach (LogicTreeNode subNode in subNodes)
            {
                tempNode = subNode.SearchContentNodeById(checkIdNum);
                if (tempNode != null)
                {
                    return tempNode;
                }
            }
            return null;
        }
    }*/
}
