 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace IceBlink2
{
    public class IBScriptEngine
    {
        public GameView gv;
        public string[] lines;
        public Dictionary<string, float> localNumbers = new Dictionary<string, float>();
        public Dictionary<string, string> localStrings = new Dictionary<string, string>();
        public List<int> lastForLineNumber = new List<int>();
        public List<int> lastGosubLineNumber = new List<int>();
        public List<ForBlock> ForBlockStack = new List<ForBlock>();
        public List<IfBlock> IfBlockStack = new List<IfBlock>();
        public List<ForBlock> ForBlocksList = new List<ForBlock>();
        public List<IfBlock> IfBlocksList = new List<IfBlock>();
        public float helpResult;
       
        
        //The following are the keywords used:
        //if
        //else
        //endif
        //
        //for
        //continue
        //break
        //next
        //
        //goto
        //label
        //
        //gosub
        //subroutine
        //return
        //
        //msg

        //Variable types and the prefix used:
        //@ = float or integer (stored as float and converted to int if needed)
        //$ = string
        //% = the prefix for a data object property ( ex. %item[@i].name ) 
        //~ = the prefix for a function call ( ex. ~gaTakeItem() )
        //# = the prefix for the number of elements in a data object List ( ex. #ItemList )

        public IBScriptEngine(GameView g, string filename, string prm1, string prm2, string prm3, string prm4)
        {
            gv = g;
            //read in script file and create line numbered list
            lines = File.ReadAllLines(gv.cc.GetModulePath() + "\\ibscript\\" + filename + ".ibs");
            List<string> converttolist = lines.ToList();
            converttolist.Insert(0, "//line 0");
            lines = converttolist.ToArray();

            //set-up Block lists
            fillForBlocksList();
            fillIfBlocksList();

            //RunScript();
        }

        public void fillForBlocksList()
        {
            ForBlockStack.Clear();
            for (int i = 0; i < lines.Length; i++)
            {
                //remove all leading white space
                string line = lines[i].TrimStart(' ');
                if (line.StartsWith("for"))
                {
                    //get info
                    ForBlock newFB = CreateForBlock(line, i);
                    //push to stack
                    ForBlockStack.Add(newFB);
                }
                else if (line.StartsWith("next"))
                {
                    //add next location to last stack forBlock
                    ForBlockStack[ForBlockStack.Count - 1].nextLineNumber = i;
                    //copy to ForBlocksList
                    ForBlocksList.Add(ForBlockStack[ForBlockStack.Count - 1]);
                    //pull from stack
                    ForBlockStack.RemoveAt(ForBlockStack.Count - 1);
                }
            }
            ForBlockStack.Clear();
        }
        public void fillIfBlocksList()
        {
            IfBlockStack.Clear();
            for (int i = 0; i < lines.Length; i++)
            {
                //remove all leading white space
                string line = lines[i].TrimStart(' ');
                if (line.StartsWith("if"))
                {
                    //get info
                    IfBlock newIB = CreateIfBlock(line, i);
                    //push to stack
                    IfBlockStack.Add(newIB);
                }
                else if (line.StartsWith("else"))
                {
                    //add else location to current stack ifBlock
                    IfBlockStack[IfBlockStack.Count - 1].elseLineNumber = i;                    
                }
                else if (line.StartsWith("endif"))
                {
                    //add endif location to current stack ifBlock
                    IfBlockStack[IfBlockStack.Count - 1].endifLineNumber = i;
                    //copy to IfBlocksList
                    IfBlocksList.Add(IfBlockStack[IfBlockStack.Count - 1]);
                    //pull from stack
                    IfBlockStack.RemoveAt(IfBlockStack.Count - 1);
                }
            }
            IfBlockStack.Clear();
        }

        public void RunScript()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                //remove all leading white space
                string line = lines[i].TrimStart(' ');
                
                if (line.StartsWith("@"))
                {
                    DoNumberAssignment(line);
                }
                else if (line.StartsWith("$"))
                {
                    DoStringAssignment(line);
                }
                else if (line.StartsWith("~"))
                {
                    DoFunction(line); //not for ~gc functions, they use @ above
                }
                else if (line.StartsWith("%"))
                {
                    DoObjectPropertyAssignment(line);
                }
                else if (line.StartsWith("msg"))
                {
                    DoMessage(line);
                }
                else if (line.StartsWith("if"))
                {
                    i = DoIf(line, i);
                }
                else if (line.StartsWith("else"))
                {
                    i = DoElse(line, i);                   
                }
                else if (line.StartsWith("endif"))
                {
                    continue; //ignore and move to next line
                }                
                else if (line.StartsWith("for"))
                {
                    i = DoFor(line, i);
                }
                else if (line.StartsWith("continue"))
                {
                    i = DoContinue(line);
                }
                else if (line.StartsWith("break"))
                {
                    i = DoBreak(line);
                }
                else if (line.StartsWith("next"))
                {
                    i = DoNext(line, i);
                }
                else if (line.StartsWith("label"))
                {
                    continue; //ignore and move to next line
                }
                else if (line.StartsWith("goto"))
                {
                    i = DoGoto(line, i);
                }
                else if (line.StartsWith("gosub"))
                {
                    i = DoGosub(line, i);
                }
                else if (line.StartsWith("return"))
                {
                    i = DoReturn(line, i);                                       
                }
                else if ((line.StartsWith("end")) && (!line.StartsWith("endif")))
                {
                    break; //end of script so end the script
                }
            }
        }

        public void DoNumberAssignment(string line)
        {
            string[] element = GetLeftMiddleRightSides(line);

            float fValue = 0.0f;
            if (element.Length < 3) //@k ++
            {
                fValue = -1; //just set to -1 because fValue won't be used anyway
            }
            else if (element[2].StartsWith("~gc"))
            {
                fValue = (float)GetFunctionConditionalCheckReturnValue(element[2]);
            }            
            else //@k += 2 or @k = @i + 23 - @stuff
            {
                fValue = CalcualteNumberEquation(element[2]);
            }
            AddToLocalNumbers(element[0], fValue, element[1]);                                    
        }
        public void DoStringAssignment(string line)
        {
            string[] element = GetLeftMiddleRightSides(line);
            string sideR = ConcateString(element[2]);
            AddToLocalStrings(element[0], sideR, element[1]);            
        }

        public void DoFunction(string line)
        {
            string[] parms = GetParameters(line);

            try
            {
                string prm1 = parms[0];
                string prm2 = parms[1];
                string prm3 = parms[2];
                string prm4 = parms[3];

                string sub = line.Substring(1, 2);

                if (sub == "ga")
                {
                    string output = line.Split('~', '(')[1];
                    output += ".cs";
                    //yn1: added Gets for global ints and strings: prm1 is the searched global, prm2 is the name of the temporary local to store the value in
                    //for int: best use @ as first character of the local var to store in (like "~gaGetGlobalInt(questCounter21, @tempInt3)").
                    //for string: best use $ as first character of the local var to store in (like "~gaGetGlobalString(questCounter21, $tempString3)").
                    //form here on the temp local var can be used to e.g. assign its value to object properties via %... or just work with the value via $... or @... 
                    if (output == "gaGetGlobalInt.cs")
                    {
                        float temp = (int)gv.sf.GetGlobalInt(prm1);
                        AddToLocalNumbers(prm2, temp, "=");
                    }
                    else if (output == "gaGetGlobalString.cs")
                    {
                        string temp = gv.sf.GetGlobalString(prm1);
                        AddToLocalStrings(prm2, temp, "=");
                    }
                    //end of changes
                    else
                    {
                        gv.sf.gaController(output, prm1, prm2, prm3, prm4);
                    }
                }
                if (sub == "og")
                {
                    string output = line.Split('~', '(')[1];
                    output += ".cs";
                    gv.sf.ogController(output, prm1, prm2, prm3, prm4);
                }

                if (sub == "os")
                {

                    string output = line.Split('~', '(')[1];
                    output += ".cs";
                    gv.sf.osController(output, prm1, prm2, prm3, prm4);
                }
            }
            catch
            {
            }
            
            /*
                if (line.StartsWith("~gaTakeItem("))
                {  
                    gaTakeItem(parms);
                }
                else if (line.StartsWith("~gaShowFloatyTextOnMainMap("))
                {
                    gaShowFloatyTextOnMainMap(parms);
                }
             */

        }

        public void DoObjectPropertyAssignment(string line)
        {
            string[] element = GetLeftMiddleRightSides(line);

            //get index of object in its List
            string index = GetBetween(element[0], '[', ']');
            string indexReplaced = ReplaceParameter(index);
            int indexNum = (int)Convert.ToDouble(indexReplaced);

            int indexNum2 = 0;
            int indexNum3 = 0;
            int indexNum4 = 0;

            string index2 = GetBetween(element[0], '{', '}');
            if (index2 != element[0])
            {
                string indexReplaced2 = ReplaceParameter(index2);
                indexNum2 = (int)Convert.ToDouble(indexReplaced2);
            }

            string index3 = GetBetween(element[0], '|', '|');
            if (index3 != element[0])
            {
                string indexReplaced3 = ReplaceParameter(index3);
                indexNum3 = (int)Convert.ToDouble(indexReplaced3);
            }

            string index4 = GetBetween(element[0], '^', '^');
            if (index4 != element[0])
            {
                string indexReplaced4 = ReplaceParameter(index4);
                indexNum4 = (int)Convert.ToDouble(indexReplaced4);
            }

            
            if (element[0].StartsWith("%Mod"))
            {
                ModAssignment(element, indexNum);                
            }
            else if (element[0].StartsWith("%Player"))
            {
                PlayerAssignment(element, indexNum);
            }
            else if (element[0].StartsWith("%Prop"))
            {
                PropAssignment(element, indexNum, indexNum2);
            }
        }
        public void DoMessage(string line)
        {
            string removeParenth = GetBetween(line, '(', ')');
            string mesg = ConcateString(removeParenth);
            gv.log.AddHtmlTextToLog(mesg);
        }
        public int DoIf(string line, int i)
        {
            int lineNum = i;
            IfBlock ib = GetIfBlockByIfLineNumber(i);
            if (ib == null)
            {
                DoMessage("error: didn't find IfBlock for line number: " + i.ToString() + "  " + line);
            }
            //evaluate the statement
            bool retIf = EvaluateStatement(line);
            //if true, move to next line
            //if false, go to next else or endif
            if (!retIf)
            {
                if (ib.elseLineNumber > 0)
                {
                    lineNum = ib.elseLineNumber;
                }
                else
                {
                    lineNum = ib.endifLineNumber;
                }
            }
            return lineNum;
        }
        public int DoElse(string line, int i)
        {
            //hit the end of an if statement so go to next endif
            IfBlock ib = GetIfBlockByElseLineNumber(i);
            if (ib == null)
            {
                DoMessage("error: didn't find IfBlock for line number: " + i.ToString() + "  " + line);
            }
            return ib.endifLineNumber;
        }
        public int DoFor(string line, int i)
        {
            ForBlock fb = GetForBlockByForLineNumber(i);
            if (fb == null)
            {
                DoMessage("error: didn't find ForBlock for line number: " + i.ToString() + "  " + line);
            }
            //push forBlock to stack
            ForBlockStack.Add(fb);
            //set the indexer variable
            DoNumberAssignment(fb.indexInit);
            //check the conditional statement 
            bool retForEval = EvaluateStatement(fb.indexCond);
            //if true move to next line
            //if false go to the line just after 'next'
            if (!retForEval)
            {
                return fb.nextLineNumber;                
            }
            return i;
        }
        public int DoNext(string line, int i)
        {
            ForBlock fb = GetForBlockByNextLineNumber(i);
            if (fb == null)
            {
                DoMessage("error: didn't find ForBlock for line number: " + i.ToString() + "  " + line);
            }
            //do the for increment statement
            DoNumberAssignment(fb.indexStep);
            //then do the conditional check
            bool retForEval = EvaluateStatement(fb.indexCond);
            //if true, go to the line just after for line number
            if (retForEval)
            {
                return fb.forLineNumber;
            }
            //if false, pull from stack, move to next line
            else
            {
                ForBlockStack.Remove(fb);
            }
            return i;
        }
        public int DoContinue(string line)
        {
            //go to 'next' line number for the current stack ForBlock
            return ForBlockStack[ForBlockStack.Count - 1].nextLineNumber - 1;
        }
        public int DoBreak(string line)
        {
            //go to line after the 'next' line number for the current stack ForBlock
            int lineNum = ForBlockStack[ForBlockStack.Count - 1].nextLineNumber;
            //pull from stack
            ForBlockStack.RemoveAt(ForBlockStack.Count - 1);
            return lineNum;
        }
        public int DoGoto(string line, int i)
        {
            //find the index of the label
            string label = GetLabelName(line);
            //change 'i' to this value
            int index = GetLineNumberOfLabel(label);
            if (index == -1)
            {
                DoMessage("couldn't find label: " + label);
            }
            else
            {
                return index;
            }
            return i;
        }
        public int DoGosub(string line, int i)
        {
            //push this index, i, to the lastGosubLineNumber
            //find the index of the subroutine
            string subName = GetSubroutineName(line);
            //change 'i' to this value
            int index = GetLineNumberOfSubroutine(subName);
            if (index == -1)
            {
                DoMessage("couldn't find subroutine: " + subName);
            }
            else
            {
                //push this index, i, to the lastGosubLineNumber list
                lastGosubLineNumber.Add(i);
                //move to subroutine
                return index;
            }
            return i;
        }
        public int DoReturn(string line, int i)
        {
            if (lastGosubLineNumber.Count > 0)
            {
                //go to the line just after lastGosubLineNumber
                int lineNum = lastGosubLineNumber[lastGosubLineNumber.Count - 1];
                //pull the lastGosubLineNumber from list
                lastGosubLineNumber.RemoveAt(lastGosubLineNumber.Count - 1);
                return lineNum;
            }
            return i;
        }
        public int GetFunctionConditionalCheckReturnValue(string rightSide)
        {
            string[] parms = GetParameters(rightSide);

            try
            {
                string prm1 = parms[0];
                string prm2 = parms[1];
                string prm3 = parms[2];
                string prm4 = parms[3];

                string output = rightSide.Split('~', '(')[1];
                output += ".cs";
                gv.sf.gcController(output, prm1, prm2, prm3, prm4);
                if (gv.mod.returnCheck)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }

            /*if (rightSide.StartsWith("~gcCheckItem("))
            {
                gcCheckItem(parms);
            }
            return -1;*/
        }


        #region Object Property Assignment
        public void PlayerAssignment(string[] element, int indexNum)
        {
            if (element[0].EndsWith("hp"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].hp = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].hp += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].hp -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].hp *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].hp;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].hp = (int)helpResult;
                }
                    //yn1: avoided using % because I wasnt sure if this would lead to confusion with already reserved usage of % for object property identification
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].hp %= val;
                }
            }
            else if (element[0].EndsWith("sp"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].sp = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].sp += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].sp -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].sp *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].sp;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].sp = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].sp %= val;
                }

            }
            else if (element[0].EndsWith("combatLocX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].combatLocX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].combatLocX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].combatLocX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].combatLocX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].combatLocX;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].combatLocX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].combatLocX %= val;
                }

            }

            else if (element[0].EndsWith("combatLocY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].combatLocY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].combatLocY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].combatLocY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].combatLocY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].combatLocY;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].combatLocY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].combatLocY %= val;
                }
            }

            else if (element[0].EndsWith("classLevel"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].classLevel = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].classLevel += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].classLevel -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].classLevel *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].classLevel;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].classLevel = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].classLevel %= val;
                }
            }

            else if (element[0].EndsWith("baseFortitude"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseFortitude = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseFortitude += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseFortitude -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseFortitude *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseFortitude;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseFortitude = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseFortitude %= val;
                }
            }

            else if (element[0].EndsWith("baseWill"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseWill = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseWill += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseWill -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseWill *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseWill;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseWill = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseWill %= val;
                }
            }

            else if (element[0].EndsWith("baseReflex"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseReflex = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseReflex += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseReflex -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseReflex *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseReflex;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseReflex = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseReflex %= val;
                }
            }

            else if (element[0].EndsWith("fortitude"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].fortitude = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].fortitude += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].fortitude -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].fortitude *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].fortitude;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].fortitude = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].fortitude %= val;
                }
            }

            else if (element[0].EndsWith("will"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].will = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].will += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].will -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].will *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].will;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].will = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].will %= val;
                }
            }

            else if (element[0].EndsWith("reflex"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].reflex = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].reflex += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].reflex -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].reflex *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].reflex;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].reflex = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].reflex %= val;
                }
            }

            else if (element[0].EndsWith("reflex"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].reflex = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].reflex += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].reflex -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].reflex *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].reflex;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].reflex = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].reflex %= val;
                }
            }

            else if (element[0].EndsWith("strength"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].strength = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].strength += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].strength -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].strength *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].strength;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].strength = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].strength %= val;
                }
            }

            else if (element[0].EndsWith("dexterity"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].dexterity = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].dexterity += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].dexterity -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].dexterity *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].dexterity;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].dexterity = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].dexterity %= val;
                }
            }

            else if (element[0].EndsWith("intelligence"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].intelligence = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].intelligence += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].intelligence -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].intelligence *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].intelligence;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].intelligence = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].intelligence %= val;
                }
            }

            else if (element[0].EndsWith("charisma"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].charisma = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].charisma += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].charisma -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].charisma *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].charisma;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].charisma = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].charisma %= val;
                }
            }

            else if (element[0].EndsWith("baseStr"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseStr = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseStr += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseStr -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseStr *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseStr;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseStr = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseStr %= val;
                }
            }

            else if (element[0].EndsWith("baseDex"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseDex = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseDex += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseDex -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseDex *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseDex;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseDex = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseDex %= val;
                }
            }

            else if (element[0].EndsWith("baseInt"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseInt = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseInt += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseInt -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseInt *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseInt;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseInt = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseInt %= val;
                }
            }

            else if (element[0].EndsWith("baseCha"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseCha = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseCha += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseCha -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseCha *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseCha;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseCha = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseCha %= val;
                }
            }

            else if (element[0].EndsWith("ACBase"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].ACBase = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].ACBase += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].ACBase -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].ACBase *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].ACBase;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].ACBase = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].ACBase %= val;
                }
            }

            else if (element[0].EndsWith("AC"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].AC = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].AC += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].AC -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].AC *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].AC;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].AC = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].AC %= val;
                }
            }

            else if (element[0].EndsWith("classBonus"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].classBonus = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].classBonus += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].classBonus -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].classBonus *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].classBonus;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].classBonus = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].classBonus %= val;
                }
            }

            else if (element[0].EndsWith("baseAttBonus"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseAttBonus = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseAttBonus += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseAttBonus -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseAttBonus *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseAttBonus;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseAttBonus = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseAttBonus %= val;
                }
            }

            else if (element[0].EndsWith("baseAttBonusAdders"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseAttBonusAdders = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseAttBonusAdders += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseAttBonusAdders -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseAttBonusAdders *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseAttBonusAdders;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseAttBonusAdders = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseAttBonusAdders %= val;
                }
            }

            else if (element[0].EndsWith("hpMax"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].hpMax = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].hpMax += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].hpMax -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].hpMax *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].hpMax;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].hpMax = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].hpMax %= val;
                }
            }

            else if (element[0].EndsWith("spMax"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].spMax = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].spMax += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].spMax -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].spMax *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].spMax;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].spMax = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].spMax %= val;
                }
            }

            else if (element[0].EndsWith("XP"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].XP = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].XP += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].XP -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].XP *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].XP;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].XP = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].XP %= val;
                }
            }

            else if (element[0].EndsWith("XPNeeded"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].XPNeeded = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].XPNeeded += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].XPNeeded -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].XPNeeded *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].XPNeeded;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].XPNeeded = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].XPNeeded %= val;
                }
            }

            else if (element[0].EndsWith("hpRegenTimePassedCounter"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].hpRegenTimePassedCounter = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].hpRegenTimePassedCounter += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].hpRegenTimePassedCounter -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].hpRegenTimePassedCounter *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].hpRegenTimePassedCounter;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].hpRegenTimePassedCounter = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].hpRegenTimePassedCounter %= val;
                }
            }

            else if (element[0].EndsWith("spRegenTimePassedCounter"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].spRegenTimePassedCounter = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].spRegenTimePassedCounter += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].spRegenTimePassedCounter -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].spRegenTimePassedCounter *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].spRegenTimePassedCounter;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].spRegenTimePassedCounter = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].spRegenTimePassedCounter %= val;
                }
            }

            else if (element[0].EndsWith("damageTypeResistanceTotalAcid"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalAcid = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalAcid += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalAcid -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalAcid *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].damageTypeResistanceTotalAcid;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalAcid = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalAcid %= val;
                }
            }

            else if (element[0].EndsWith("damageTypeResistanceTotalCold"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalCold = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalCold += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalCold -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalCold *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].damageTypeResistanceTotalCold;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalCold = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalCold %= val;
                }
            }

            else if (element[0].EndsWith("damageTypeResistanceTotalNormal"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalNormal = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalNormal += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalNormal -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalNormal *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].damageTypeResistanceTotalNormal;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalNormal = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalNormal %= val;
                }
            }

            else if (element[0].EndsWith("damageTypeResistanceTotalElectricity"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalElectricity = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalElectricity += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalElectricity -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalElectricity *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].damageTypeResistanceTotalElectricity;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalElectricity = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalElectricity %= val;
                }
            }

            else if (element[0].EndsWith("damageTypeResistanceTotalFire"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalFire = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalFire += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalFire -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalFire *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].damageTypeResistanceTotalFire;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalFire = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalFire %= val;
                }
            }

            else if (element[0].EndsWith("damageTypeResistanceTotalMagic"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalMagic = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalMagic += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalMagic -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalMagic *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].damageTypeResistanceTotalMagic;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalMagic = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalMagic %= val;
                }
            }

            else if (element[0].EndsWith("damageTypeResistanceTotalPoison"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalPoison = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalPoison += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalPoison -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalPoison *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].damageTypeResistanceTotalPoison;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalPoison = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].damageTypeResistanceTotalPoison %= val;
                }
            }

        }
        public void ModAssignment(string[] element, int indexNum)
        {
            
        }
        public void PropAssignment(string[] element, int indexNum, int indexNum2)
        {
            if (element[0].EndsWith("isShown"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (val == 1)
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isShown = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isShown = false;
                }
            }
        }
        #endregion

        #region ga Functions
        public void gaTakeItem(string[] parms)
        {
           
        }
        public void gaShowFloatyTextOnMainMap(string[] parms)
        {
            
        }
        #endregion

        #region gc Functions
        public int gcCheckItem(string[] parms)
        {
            return -1;
        }
        #endregion

        public IfBlock GetIfBlockByIfLineNumber(int i)
        {
            foreach (IfBlock ib in IfBlocksList)
            {
                if (ib.ifLineNumber == i)
                {
                    return ib;
                }
            }
            return null;
        }
        public IfBlock GetIfBlockByElseLineNumber(int i)
        {
            foreach (IfBlock ib in IfBlocksList)
            {
                if (ib.elseLineNumber == i)
                {
                    return ib;
                }
            }
            return null;
        }
        public ForBlock GetForBlockByForLineNumber(int i)
        {
            foreach (ForBlock fb in ForBlocksList)
            {
                if (fb.forLineNumber == i)
                {
                    return fb;
                }
            }
            return null;
        }
        public ForBlock GetForBlockByNextLineNumber(int i)
        {
            foreach (ForBlock fb in ForBlocksList)
            {
                if (fb.nextLineNumber == i)
                {
                    return fb;
                }
            }
            return null;
        }
        public ForBlock CreateForBlock(string line, int i)
        {
            string removeParenth = GetBetween(line, '(', ')');
            string[] element = removeParenth.Split(';');

            List<string> trimmedList = new List<string>();
            foreach (string s in element)
            {
                string sTrimmed = s.Trim();
                trimmedList.Add(sTrimmed);
            }

            ForBlock newF = new ForBlock();
            newF.forLineNumber = i;
            newF.indexInit = trimmedList[0];
            newF.indexCond = trimmedList[1];
            newF.indexStep = trimmedList[2];
            return newF;
        }
        public IfBlock CreateIfBlock(string line, int i)
        {
            string conditional = GetBetween(line, '(', ')');
                        
            IfBlock newIB = new IfBlock();
            newIB.ifLineNumber = i;
            newIB.ifConditional = conditional;
            return newIB;
        }
        public string GetLabelName(string line)
        {
            string[] elements = line.Split(' ');
            return elements[1];
        }
        public int GetLineNumberOfLabel(string label)
        {
            for (int x = 0; x < lines.Length; x++)
            {
                string line = lines[x].TrimStart(' ');
                if (line.StartsWith("label"))
                {
                    string[] elements = line.Split(' ');
                    if (elements[1] == label)
                    {
                        return x;
                    }
                }
            }
            return -1;
        }
        public string GetSubroutineName(string line)
        {
            string[] elements = line.Split(' ');
            return elements[1];
        }
        public int GetLineNumberOfSubroutine(string subName)
        {
            for (int x = 0; x < lines.Length; x++)
            {
                string line = lines[x].TrimStart(' ');
                if (line.StartsWith("subroutine"))
                {
                    string[] elements = line.Split(' ');
                    if (elements[1] == subName)
                    {
                        return x;
                    }
                }
            }
            return -1;
        }

        public bool EvaluateStatement(string line)
        {
            string removeParenth = GetBetween(line, '(', ')');
            //string[] element = removeParenth.Split(' ');
            string[] element = GetLeftMiddleRightSides(removeParenth);

            string sLeft = ReplaceParameter(element[0]);
            
            //check to see if it is a number
            double d;
            bool testNumeric = double.TryParse(sLeft, out d);

            if (testNumeric)
            {
                //it is a number comparison
                float leftSide = (float)Convert.ToDouble(sLeft);
                float rightSide = CalcualteNumberEquation(element[2]);

                //compare left and right side
                if (element[1] == "=")
                {
                    if (leftSide == rightSide)
                    {
                        return true;
                    }
                }
                else if (element[1] == "!=")
                {
                    if (leftSide != rightSide)
                    {
                        return true;
                    }
                }
                else if (element[1] == "<")
                {
                    if (leftSide < rightSide)
                    {
                        return true;
                    }
                }
                else if (element[1] == ">")
                {
                    if (leftSide > rightSide)
                    {
                        return true;
                    }
                }
            }
            else
            {
                //it is a string comparison
                string sRight = ConcateString(element[2]);

                //compare left and right side
                if (element[1] == "=")
                {
                    if (sLeft == sRight)
                    {
                        return true;
                    }
                }
                else if (element[1] == "!=")
                {
                    if (sLeft != sRight)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }        
        public string ConcateString(string startString)
        {
            List<string> stringParts = new List<string>();
            string part = "";
            string retString = "";
            bool inQuote = false;

            foreach (char c in startString)
            {
                if ((!inQuote) && (c == '"')) //entering a quote area
                {
                    inQuote = true;
                    part += c;
                    continue;
                }
                if (c == '"') //exiting a quote area
                { 
                    inQuote = false;
                    part += c;
                    continue;
                }
                if (inQuote) //in a quote so record everything
                {
                    part += c;
                    continue;
                }
                if (c == '+') //+ outside a quote area
                {
                    stringParts.Add(part);
                    part = "";
                }
                else
                {
                    part += c;
                    continue;
                }                
            }
            //add what ever was left over after the last '+' to the List
            stringParts.Add(part);

            foreach (string str in stringParts)
            {
                string strTrimmed = str.Trim();
                if (strTrimmed.StartsWith("\""))
                {
                    string withoutQuotes = GetBetween(strTrimmed, '"', '"');
                    retString += withoutQuotes;
                }
                //else if ((strTrimmed.StartsWith("@")) || (strTrimmed.StartsWith("$")) || (strTrimmed.StartsWith("#")))
                else
                {
                    retString += ReplaceParameter(strTrimmed);
                }
            }

            return retString;
        }
        public float CalcualteNumberEquation(string sRightSide)
        {
            string[] element = SplitTrimRemoveBlanks(sRightSide);
            float retValue = 0;

            for (int x = 0; x < element.Length;  x++)
            {
                if ((element[x] != "+") && (element[x] != "-") && (element[x] != "*") && (element[x] != "/") && (element[x] != "./."))
                {
                    //check to see if any elements are variables (@...) and replace as needed
                    float numR = (float)Convert.ToDouble(ReplaceParameter(element[x]));
                    if (x > 1) //use last operator and calculate new value
                    {
                        if (element[x-1] == "+")
                        {
                            retValue += numR;
                        }
                        else if (element[x - 1] == "-")
                        {
                            retValue -= numR;
                        }
                        else if (element[x - 1] == "*")
                        {
                            retValue *= numR;
                        }
                        else if (element[x - 1] == "/")
                        {
                            retValue /= numR;
                        }
                        else if (element[x - 1] == "./.")
                        {
                            retValue = retValue % numR;
                        }
                    }
                    else
                    {
                        retValue = numR;
                    }
                }
            }            
            return retValue;
        }
        
        public void AddToLocalNumbers(string varName, float varNum, string assignType)
        {            
            //check to see if already in list and replace
            if (localNumbers.ContainsKey(varName))
            {                
                //check to see if it is = or += or -=
                if (assignType == "=")
                {
                    localNumbers[varName] = varNum;
                }
                else if (assignType == "+=")
                {
                    localNumbers[varName] += varNum;
                }
                else if (assignType == "-=")
                {
                    localNumbers[varName] -= varNum;
                }
                else if (assignType == "++")
                {
                    localNumbers[varName]++;
                }
                else if (assignType == "--")
                {
                    localNumbers[varName]--;
                }
                else if (assignType == "*=")
                {
                    localNumbers[varName] *= varNum;
                }
                //yn1: using instead of % to aovid confuion with already reserved meaning of %
                else if (assignType == "./.=")
                {
                    localNumbers[varName] %= varNum;
                }
                else if (assignType == "/=")
                {
                    helpResult = localNumbers[varName];
                    helpResult /= varNum;
                    localNumbers[varName] = (int)helpResult;
                }
               
            }
            else //if not in list then add
            {
                if (assignType != "=")
                {
                    DoMessage("can't use that assignment type for initializing a variable");
                }
                else
                {
                    localNumbers.Add(varName, varNum);
                }
            }
        }
        public void AddToLocalStrings(string varName, string varString, string assignType)
        {
            //check to see if already in list and replace or add to
            if (localStrings.ContainsKey(varName))
            {
                //check to see if it is = or +=
                if (assignType == "=")
                {
                    localStrings[varName] = varString;
                }
                else if (assignType == "+=")
                {
                    localStrings[varName] += varString;
                }                
            }
            else //if not in list then add
            {
                localStrings.Add(varName, varString);                
            }
        }
        public string[] GetLeftMiddleRightSides(string line)
        {
            //this needs to be coded still
            List<string> elementList = new List<string>();

            //get the left element(variable to assign to) and middle element (= or !=)
            string[] allelement = SplitTrimRemoveBlanks(line);
            elementList.Add(allelement[0]);
            elementList.Add(allelement[1]);
            //if only two elements (ex @k ++) return array
            if (allelement.Length < 3) { return elementList.ToArray(); }
            //get the right side concated
            string rightSide = line.Substring(line.IndexOf(allelement[1]) + allelement[1].Length);
            //string rightSideConcated = ConcateString(rightSide);
            elementList.Add(rightSide);
            //convert the list to an array
            return elementList.ToArray();
        }
        public string GetBetween(string str, char start, char end)
        {
            string retString = "";
            bool startRecording = false;

            foreach (char c in str)
            {
                if ((!startRecording) && (c == start)) 
                {
                    startRecording = true;
                    continue;
                }
                if (c == end) { return retString; }
                if (startRecording)
                {
                    retString += c;
                }
            }
            if (!startRecording)
            {
                return str;
            }
            return retString;
        }
        public string[] GetParameters(string line)
        {
            string lineJustParms = GetBetween(line, '(', ')');
            return lineJustParms.Split(',').Select(p => p.Trim()).ToArray();
        }
        public string ReplaceParameter(string parm)
        {
            //if parm is a variable, grab the value from the proper Dictionary
            if (parm.StartsWith("@"))
            {
                //grab the value from the Dictionary
                if (localNumbers.ContainsKey(parm))
                {
                    float val = localNumbers[parm];
                    return val.ToString();
                }
                else
                {
                    //not is the Dictionary so return 0
                    return "0";
                }
            }
            else if (parm.StartsWith("$"))
            {
                //grab the value from the Dictionary
                if (localStrings.ContainsKey(parm))
                {
                    return localStrings[parm];
                }
                else
                {
                    //not in Dictionary so return ""
                    return "";
                }
            }
            else if (parm.StartsWith("%"))
            {
                string index = GetBetween(parm, '[', ']');
                string indexTrimmed = index.Trim();
                string indexReplaced = ReplaceParameter(indexTrimmed);
                int indexNum = (int)Convert.ToDouble(indexReplaced);

                int indexNum2 = 0;
                int indexNum3 = 0;
                int indexNum4 = 0;


                string index2 = GetBetween(parm, '{', '}');
                if (index2 != parm)
                {
                    string indexTrimmed2 = index2.Trim();
                    string indexReplaced2 = ReplaceParameter(indexTrimmed2);
                    indexNum2 = (int)Convert.ToDouble(indexReplaced2);
                }
                

                string index3 = GetBetween(parm, '|', '|');
                if (index3 != parm)
                {
                    string indexTrimmed3 = index3.Trim();
                    string indexReplaced3 = ReplaceParameter(indexTrimmed3);
                    indexNum3 = (int)Convert.ToDouble(indexReplaced3);
                }

                string index4 = GetBetween(parm, '^', '^');
                    if (index4 != parm)
                    {
                    string indexTrimmed4 = index4.Trim();
                    string indexReplaced4 = ReplaceParameter(indexTrimmed4);
                    indexNum4 = (int)Convert.ToDouble(indexReplaced4);
                    }

                #region Player
                if (parm.StartsWith("%Player"))
                {
                    if (parm.EndsWith("hp"))
                    {
                        return gv.mod.playerList[indexNum].hp.ToString();
                    }
                    else if (parm.EndsWith("sp"))
                    {
                        return gv.mod.playerList[indexNum].sp.ToString();
                    }
                    else if (parm.EndsWith("combatLocX"))
                    {
                        return gv.mod.playerList[indexNum].combatLocX.ToString();
                    }
                    else if (parm.EndsWith("combatLocY"))
                    {
                        return gv.mod.playerList[indexNum].combatLocY.ToString();
                    }
                    else if (parm.EndsWith("classLevel"))
                    {
                        return gv.mod.playerList[indexNum].classLevel.ToString();
                    }
                    else if (parm.EndsWith("baseFortitude"))
                    {
                        return gv.mod.playerList[indexNum].baseFortitude.ToString();
                    }
                    else if (parm.EndsWith("baseWill"))
                    {
                        return gv.mod.playerList[indexNum].baseWill.ToString();
                    }
                    else if (parm.EndsWith("baseReflex"))
                    {
                        return gv.mod.playerList[indexNum].baseReflex.ToString();
                    }
                    else if (parm.EndsWith("fortitude"))
                    {
                        return gv.mod.playerList[indexNum].fortitude.ToString();
                    }
                    else if (parm.EndsWith("will"))
                    {
                        return gv.mod.playerList[indexNum].will.ToString();
                    }
                    else if (parm.EndsWith("reflex"))
                    {
                        return gv.mod.playerList[indexNum].reflex.ToString();
                    }
                    else if (parm.EndsWith("strength"))
                    {
                        return gv.mod.playerList[indexNum].strength.ToString();
                    }
                    else if (parm.EndsWith("dexterity"))
                    {
                        return gv.mod.playerList[indexNum].dexterity.ToString();
                    }
                    else if (parm.EndsWith("intelligence"))
                    {
                        return gv.mod.playerList[indexNum].intelligence.ToString();
                    }
                    else if (parm.EndsWith("charisma"))
                    {
                        return gv.mod.playerList[indexNum].charisma.ToString();
                    }
                    else if (parm.EndsWith("baseStr"))
                    {
                        return gv.mod.playerList[indexNum].baseStr.ToString();
                    }
                    else if (parm.EndsWith("baseDex"))
                    {
                        return gv.mod.playerList[indexNum].baseDex.ToString();
                    }
                    else if (parm.EndsWith("baseInt"))
                    {
                        return gv.mod.playerList[indexNum].baseInt.ToString();
                    }
                    else if (parm.EndsWith("baseCha"))
                    {
                        return gv.mod.playerList[indexNum].baseCha.ToString();
                    }
                    else if (parm.EndsWith("ACBase"))
                    {
                        return gv.mod.playerList[indexNum].ACBase.ToString();
                    }
                    else if (parm.EndsWith("AC"))
                    {
                        return gv.mod.playerList[indexNum].AC.ToString();
                    }
                    else if (parm.EndsWith("classBonus"))
                    {
                        return gv.mod.playerList[indexNum].classBonus.ToString();
                    }
                    else if (parm.EndsWith("baseAttBonus"))
                    {
                        return gv.mod.playerList[indexNum].baseAttBonus.ToString();
                    }
                    else if (parm.EndsWith("baseAttBonusAdders"))
                    {
                        return gv.mod.playerList[indexNum].baseAttBonusAdders.ToString();
                    }
                    else if (parm.EndsWith("hpMax"))
                    {
                        return gv.mod.playerList[indexNum].hpMax.ToString();
                    }
                    else if (parm.EndsWith("spMax"))
                    {
                        return gv.mod.playerList[indexNum].spMax.ToString();
                    }
                    else if (parm.EndsWith("XP"))
                    {
                        return gv.mod.playerList[indexNum].XP.ToString();
                    }
                    else if (parm.EndsWith("XPNeeded"))
                    {
                        return gv.mod.playerList[indexNum].XPNeeded.ToString();
                    }
                    else if (parm.EndsWith("hpRegenTimePassedCounter"))
                    {
                        return gv.mod.playerList[indexNum].hpRegenTimePassedCounter.ToString();
                    }
                    else if (parm.EndsWith("spRegenTimePassedCounter"))
                    {
                        return gv.mod.playerList[indexNum].spRegenTimePassedCounter.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceTotalAcid"))
                    {
                        return gv.mod.playerList[indexNum].damageTypeResistanceTotalAcid.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceTotalCold"))
                    {
                        return gv.mod.playerList[indexNum].damageTypeResistanceTotalCold.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceTotalNormal"))
                    {
                        return gv.mod.playerList[indexNum].damageTypeResistanceTotalNormal.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceTotalElectricity"))
                    {
                        return gv.mod.playerList[indexNum].damageTypeResistanceTotalElectricity.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceTotalFire"))
                    {
                        return gv.mod.playerList[indexNum].damageTypeResistanceTotalFire.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceTotalMagic"))
                    {
                        return gv.mod.playerList[indexNum].damageTypeResistanceTotalMagic.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceTotalPoison"))
                    {
                        return gv.mod.playerList[indexNum].damageTypeResistanceTotalPoison.ToString();
                    }
                    
                }
                #endregion

                #region Area
                else if (parm.StartsWith("%Area"))
                {
                    if (parm.EndsWith("numberOfProps"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props.Count.ToString();
                    }
                    else if (parm.EndsWith("Filename"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Filename.ToString();
                    }
                }

                #endregion

                #region Prop
                if (parm.StartsWith("%Prop"))
                {
                    if (parm.EndsWith("PropTag"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropTag.ToString();
                    }
                }

                #endregion
            }
            else if (parm.StartsWith("#"))
            {
                if (parm.StartsWith("#numModAreas"))
                {
                    return gv.mod.moduleAreasObjects.Count.ToString();
                }
                else if (parm.StartsWith("#numModEncounters"))
                {
                    return gv.mod.moduleEncountersList.Count.ToString();
                }
                else if (parm.StartsWith("#numAreaProps"))
                {
                    return gv.mod.currentArea.Props.Count.ToString();
                }
                else if (parm.StartsWith("#numPlayers"))
                {
                    return gv.mod.playerList.Count.ToString();
                }
                else if (parm.StartsWith("#numPlayersInRoster"))
                {
                    return gv.mod.partyRosterList.Count.ToString();
                }
                else if (parm.StartsWith("#numPartyInventoryItems"))
                {
                    return gv.mod.partyInventoryRefsList.Count.ToString();
                }
            }
            return parm;
        }
        public string[] SplitTrimRemoveBlanks(string text)
        {
            List<string> parts = text.Split(' ').Select(p => p.Trim()).ToList();
            for (int x = parts.Count - 1; x >= 0; x--)
            {
                if (parts[x] == "")
                {
                    parts.RemoveAt(x);
                }
            }
            return parts.ToArray();
        }
    }

    public class IfBlock
    {
        public int ifLineNumber = 0;
        public int elseLineNumber = -1;
        public int endifLineNumber = 0;
        public string ifConditional = "";

        public IfBlock()
        {

        }
    }

    public class ForBlock
    {
        public int forLineNumber = 0;
        public int nextLineNumber = 0;
        public string indexInit = "";
        public string indexCond = "";
        public string indexStep = "";

        public ForBlock()
        {

        }
    }
}
