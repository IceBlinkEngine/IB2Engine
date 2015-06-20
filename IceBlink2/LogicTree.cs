using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace IceBlink2
{
    public class LogicTree
    {
        public string Filename = "";
        public List<LogicTreeNode> SubNodes = new List<LogicTreeNode>();

        public LogicTree()
        {

        }
        public LogicTreeNode GetContentNodeById(int idNum)
        {
		    LogicTreeNode tempNode = null;
            foreach (LogicTreeNode subNode in SubNodes)
            {
                tempNode = subNode.SearchContentNodeById(idNum);
                if (tempNode != null)
                {
                    return tempNode;
                }
            }
            return null;
        }
    }
}
