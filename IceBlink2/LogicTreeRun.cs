using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    /*public class LogicTreeRun
    {
        public Module mod;
        public GameView gv;
        public List<string> parmsList = new List<string>();

        //LogicTree STUFF
        public LogicTree currentLogicTree = new LogicTree();
        public int parentIdNum = 0;

        public LogicTreeRun(Module m, GameView g)
        {
            mod = m;
            gv = g;
        }

        //methods
        public void startLogicTree(string parms)
        {
            //convert the parms into a List<String> by comma delimination and remove white space
            parmsList = parms.Split(',').Select(x => x.Trim()).ToList();

            if (mod.playerList[mod.selectedPartyLeader].charStatus.Equals("Dead"))
            {
                gv.cc.SwitchToNextAvailablePartyLeader();
            }
            parentIdNum = getParentIdNum(0);
            doLogicTree(parentIdNum);
        }

        private string replaceParmsIfListItem(string parm)
        {
            if (parm == null) { return parm; }
            try
            {
                if (parm.StartsWith("parm("))
                {
                    string indx = parm.Split('(',')')[1];
                    int index = Convert.ToInt32(indx);
                    if (index < parmsList.Count)
                    {
                        if (gv.mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>Replaced " + parm + " with " + parmsList[index] + "</font><BR>");
                        }
                        return parmsList[index];
                    }
                    else
                    {
                        if (gv.mod.debugMode)
                        {
                            gv.cc.addLogText("<font color='yellow'>index given in " + parm + " is outside range</font><BR>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (gv.mod.debugMode)
                {
                    gv.cc.addLogText("<font color='yellow'>failed to replace " + parm + "</font><BR>");
                }
            }
            return parm;
        }
        private bool nodePassesConditional(LogicTreeNode pnode)
	    {
		    //check to see if passes conditional
            bool check = true;
            LogicTreeNode chkNode = new LogicTreeNode();
            chkNode = pnode;
            // cycle through the conditions of each node and check for true
            // if one node is a link, then go to the linked node and check its conditional
            if (pnode.isLink)
            {
                chkNode = currentLogicTree.GetContentNodeById(pnode.linkTo);
            }
            bool AndStatments = true;
            foreach (Condition conditional in chkNode.conditions)
            {
                if (!conditional.c_and)
                {
                    AndStatments = false;
                    break;
                }
            }
            foreach (Condition conditional in chkNode.conditions)
            {
        	    string p1 = replaceParmsIfListItem(conditional.c_parameter_1);
        	    string p2 = replaceParmsIfListItem(conditional.c_parameter_2);
        	    string p3 = replaceParmsIfListItem(conditional.c_parameter_3);
        	    string p4 = replaceParmsIfListItem(conditional.c_parameter_4);
        	
        	    gv.cc.doScriptBasedOnFilename(conditional.c_script, p1, p2, p3, p4);
                if (AndStatments) //this is an "and" set
                {
                    if ((mod.returnCheck == false) && (conditional.c_not == false))
                    {
                        check = false;
                    }
                    if ((mod.returnCheck == true) && (conditional.c_not == true))
                    {
                        check = false;
                    }
                }
                else //this is an "or" set
                {
                    if ((mod.returnCheck == false) && (conditional.c_not == false))
                    {
                        check = false;
                    }
                    else if ((mod.returnCheck == true) && (conditional.c_not == true))
                    {
                        check = false;
                    }
                    else //in "or" statement, if find one true then done
                    {
                        check = true;
                        break;
                    }
                }
                //MessageBox.Show("script: " + conditional.c_script + "  variable: " + conditional.c_parameter_1 + "  value: " + conditional.c_parameter_2);
            }
            return check;
	    }

        private int getParentIdNum(int childIdNum) // Gets the first node idNum that returns a true conditional
        {
            //first determine which subNode to use by cycling through all children of parentNode until one returns true from conditionals
            foreach (LogicTreeNode npcNode in currentLogicTree.GetContentNodeById(childIdNum).subNodes)
            {            
                bool check = nodePassesConditional(npcNode);
                if (check == true)
                {
                    return npcNode.idNum;
                }
            }
            return currentLogicTree.GetContentNodeById(childIdNum).subNodes[0].idNum;
        }

        private void doLogicTree(int prntIdNum) // load up the text for the NPC node and all PC responses
        {
            //if the NPC node is a link, move to the actual node
            if (currentLogicTree.GetContentNodeById(prntIdNum).isLink)
            {
        	    parentIdNum = currentLogicTree.GetContentNodeById(prntIdNum).linkTo;
        	    prntIdNum = currentLogicTree.GetContentNodeById(prntIdNum).linkTo;
            }
        
            foreach (Action action in currentLogicTree.GetContentNodeById(prntIdNum).actions)
            {
        	    string p1 = replaceParmsIfListItem(action.a_parameter_1);
        	    string p2 = replaceParmsIfListItem(action.a_parameter_2);
        	    string p3 = replaceParmsIfListItem(action.a_parameter_3);
        	    string p4 = replaceParmsIfListItem(action.a_parameter_4);
        	
        	    gv.cc.doScriptBasedOnFilename(action.a_script, p1, p2, p3, p4);
            }
        
            if (currentLogicTree.GetContentNodeById(prntIdNum).subNodes.Count < 1)
            {
                //gv.cc.doTrigger();        	            
            }
            else
            {
                parentIdNum = getParentIdNum(prntIdNum);
                doLogicTree(parentIdNum);
            }   
        }
    }
    */
}
