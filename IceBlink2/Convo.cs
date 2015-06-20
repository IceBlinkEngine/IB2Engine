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
    public class Convo
    {
        public string ConvoFileName = "";
        public bool Narration = false;
        public bool PartyChat = false;
        public bool SpeakToMainPcOnly = false;
        public String NpcPortraitBitmap = "";
        public String DefaultNpcName = "";
        public List<ContentNode> subNodes = new List<ContentNode>();

        public Convo()
        {

        }
        public ContentNode GetContentNodeById(int idNum)
        {
            ContentNode tempNode = null;
            foreach (ContentNode subNode in subNodes)
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
