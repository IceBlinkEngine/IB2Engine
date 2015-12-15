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
        public List<string> parmsList = new List<string>();
        public string scriptFilename = "";
       
        
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
        //debug

        //Variable types and the prefix used:
        //@ = float or integer (stored as float and converted to int if needed)
        //$ = string
        //% = the prefix for a data object property ( ex. %item[@i].name ) 
        //~ = the prefix for a function call ( ex. ~gaTakeItem() )
        //# = the prefix for the number of elements in a data object List ( ex. #ItemList )

        public IBScriptEngine(GameView g, string filename, string parms)
        {
            gv = g;
            scriptFilename = filename;
            //read in script file and create line numbered list
            lines = File.ReadAllLines(gv.cc.GetModulePath() + "\\ibscript\\" + filename + ".ibs");
            List<string> converttolist = lines.ToList();
            converttolist.Insert(0, "//line 0");
            lines = converttolist.ToArray();

            //set-up Block lists
            fillForBlocksList();
            fillIfBlocksList();

            //convert the parms into a List<String> by comma delimination and remove white space
            parmsList = parms.Split(',').Select(x => x.Trim()).ToList();
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
            int currentLineNumber = 0;
            try
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    currentLineNumber = i;
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
                    else if (line.StartsWith("debug"))
                    {
                        DoDebugMessage(line);
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
            catch (Exception ex)
            {                
                if (gv.mod.debugMode)
                {
                    gv.log.AddHtmlTextToLog("IBScript " + scriptFilename + " failed at line: " + currentLineNumber.ToString() + " with this Message: " + ex.Message.ToString());
                }
                gv.errorLog(ex.ToString());
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
            sideR = ReplaceParameter(sideR);
            AddToLocalStrings(element[0], sideR, element[1]);            
        }

        public void DoFunction(string line)
        {
            string[] parms = GetParameters(line);

            try
            {
                string prm1 = "none";
                string prm2 = "none";
                string prm3 = "none";
                string prm4 = "none";
                if (parms.Length > 0)
                    prm1 = ReplaceParameter(parms[0]);
                if (parms.Length > 1)
                    prm2 = ReplaceParameter(parms[1]);
                if (parms.Length > 2)
                    prm3 = ReplaceParameter(parms[2]);
                if (parms.Length > 3)
                    prm4 = ReplaceParameter(parms[3]);

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
                        AddToLocalNumbers(parms[1], temp, "=");
                    }
                    else if (output == "gaGetGlobalString.cs")
                    {
                        string temp = gv.sf.GetGlobalString(prm1);
                        AddToLocalStrings(parms[1], temp, "=");
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
            else if (element[0].StartsWith("%CreatureInCurrentEncounter"))
            {
                CreatureInCurrentEncounterAssignment(element, indexNum);
            }
            else if (element[0].StartsWith("%CreatureResRef"))
            {
                CreatureResRefAssignment(element, indexNum, indexNum2);
            }
            else if (element[0].StartsWith("%Area"))
            {
                AreaAssignment(element, indexNum);
            }
            else if (element[0].StartsWith("%Encounter"))
            {
                EncounterAssignment(element, indexNum);
            }
            else if (element[0].StartsWith("%CurrentEncounter"))
            {
                CurrentEncounterAssignment(element);
            }

        }
        public void DoMessage(string line)
        {
            string removeParenth = GetBetween(line, '(', ')');
            string mesg = ConcateString(removeParenth);
            gv.log.AddHtmlTextToLog(mesg);
        }
        public void DoDebugMessage(string line)
        {
            if (gv.mod.debugMode)
            {
                string removeParenth = GetBetween(line, '(', ')');
                string mesg = ConcateString(removeParenth);
                gv.log.AddHtmlTextToLog(mesg);
            }
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
                string prm1 = "none";
                string prm2 = "none";
                string prm3 = "none";
                string prm4 = "none";
                if (parms.Length > 0)
                    prm1 = parms[0];
                if (parms.Length > 1)
                    prm2 = parms[1];
                if (parms.Length > 2)
                    prm3 = parms[2];
                if (parms.Length > 3)
                    prm4 = parms[3];

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

            else if (element[0].EndsWith("moveDistance"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].moveDistance = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].moveDistance += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].moveDistance -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].moveDistance *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].moveDistance;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].moveDistance = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].moveDistance %= val;
                }
            }

            else if (element[0].EndsWith("moveOrder"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].moveOrder = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].moveOrder += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].moveOrder -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].moveOrder *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].moveOrder;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].moveOrder = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].moveOrder %= val;
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

            else if (element[0].EndsWith("wisdom"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].wisdom = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].wisdom += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].wisdom -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].wisdom *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].wisdom;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].wisdom = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].wisdom %= val;
                }
            }

            else if (element[0].EndsWith("constitution"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].constitution = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].constitution += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].constitution -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].constitution *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].constitution;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].constitution = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].constitution %= val;
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

            else if (element[0].EndsWith("baseCon"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseCon = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseCon += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseCon -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseCon *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseCon;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseCon = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseCon %= val;
                }
            }

            else if (element[0].EndsWith("baseWis"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].baseWis = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].baseWis += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.playerList[indexNum].baseWis -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.playerList[indexNum].baseWis *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.playerList[indexNum].baseWis;
                    helpResult /= val;
                    gv.mod.playerList[indexNum].baseWis = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.playerList[indexNum].baseWis %= val;
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
            else if (element[0].EndsWith("combatFacingLeft"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.playerList[indexNum].combatFacingLeft = true;
                }
                else
                {
                    gv.mod.playerList[indexNum].combatFacingLeft = false;
                }
            }
            else if (element[0].EndsWith("steathModeOn"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.playerList[indexNum].steathModeOn = true;
                }
                else
                {
                    gv.mod.playerList[indexNum].steathModeOn = false;
                }
            }
            else if (element[0].EndsWith("mainPc"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.playerList[indexNum].mainPc = true;
                }
                else
                {
                    gv.mod.playerList[indexNum].mainPc = false;
                }
            }
            else if (element[0].EndsWith("nonRemoveablePc"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.playerList[indexNum].nonRemoveablePc = true;
                }
                else
                {
                    gv.mod.playerList[indexNum].nonRemoveablePc = false;
                }
            }
            else if (element[0].EndsWith("isMale"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.playerList[indexNum].isMale = true;
                }
                else
                {
                    gv.mod.playerList[indexNum].isMale = false;
                }
            }

            else if (element[0].EndsWith("tokenFilename"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].tokenFilename = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].tokenFilename += val;
                }
            }

            else if (element[0].EndsWith("name"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].name = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].name += val;
                }
            }

            else if (element[0].EndsWith("tag"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].tag = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].tag += val;
                }
            }

            else if (element[0].EndsWith("raceTag"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].raceTag = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].raceTag += val;
                }
            }

            else if (element[0].EndsWith("classTag"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].classTag = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].classTag += val;
                }
            }

            else if (element[0].EndsWith("charStatus"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.playerList[indexNum].charStatus = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.playerList[indexNum].charStatus += val;
                }
            }
            
        }
        public void ModAssignment(string[] element, int indexNum)
        {
            if (element[0].EndsWith("WorldTime"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.WorldTime = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.WorldTime += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.WorldTime -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.WorldTime *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.WorldTime;
                    helpResult /= val;
                    gv.mod.WorldTime = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.WorldTime %= val;
                }
            }

            else if (element[0].EndsWith("TimePerRound"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.TimePerRound = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.TimePerRound += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.TimePerRound -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.TimePerRound *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.TimePerRound;
                    helpResult /= val;
                    gv.mod.TimePerRound = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.TimePerRound %= val;
                }
            }

            else if (element[0].EndsWith("startingPlayerPositionX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.startingPlayerPositionX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.startingPlayerPositionX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.startingPlayerPositionX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.startingPlayerPositionX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.startingPlayerPositionX;
                    helpResult /= val;
                    gv.mod.startingPlayerPositionX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.startingPlayerPositionX %= val;
                }
            }

            else if (element[0].EndsWith("startingPlayerPositionY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.startingPlayerPositionY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.startingPlayerPositionY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.startingPlayerPositionY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.startingPlayerPositionY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.startingPlayerPositionY;
                    helpResult /= val;
                    gv.mod.startingPlayerPositionY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.startingPlayerPositionY %= val;
                }
            }

            else if (element[0].EndsWith("PlayerLocationX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.PlayerLocationX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.PlayerLocationX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.PlayerLocationX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.PlayerLocationX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.PlayerLocationX;
                    helpResult /= val;
                    gv.mod.PlayerLocationX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.PlayerLocationX %= val;
                }
            }

            else if (element[0].EndsWith("PlayerLocationY "))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.PlayerLocationY  = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.PlayerLocationY  += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.PlayerLocationY  -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.PlayerLocationY  *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.PlayerLocationY ;
                    helpResult /= val;
                    gv.mod.PlayerLocationY  = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.PlayerLocationY  %= val;
                }
            }

            else if (element[0].EndsWith("PlayerLastLocationX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.PlayerLastLocationX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.PlayerLastLocationX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.PlayerLastLocationX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.PlayerLastLocationX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.PlayerLastLocationX;
                    helpResult /= val;
                    gv.mod.PlayerLastLocationX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.PlayerLastLocationX %= val;
                }
            }

            else if (element[0].EndsWith("PlayerLastLocationY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.PlayerLastLocationY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.PlayerLastLocationY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.PlayerLastLocationY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.PlayerLastLocationY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.PlayerLastLocationY;
                    helpResult /= val;
                    gv.mod.PlayerLastLocationY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.PlayerLastLocationY %= val;
                }
            }

            else if (element[0].EndsWith("partyGold"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.partyGold = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.partyGold += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.partyGold -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.partyGold *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.partyGold;
                    helpResult /= val;
                    gv.mod.partyGold = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.partyGold %= val;
                }
            }

            else if (element[0].EndsWith("selectedPartyLeader"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.selectedPartyLeader = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.selectedPartyLeader += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.selectedPartyLeader -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.selectedPartyLeader *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.selectedPartyLeader;
                    helpResult /= val;
                    gv.mod.selectedPartyLeader = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.selectedPartyLeader %= val;
                }
            }

            else if (element[0].EndsWith("combatAnimationSpeed"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.combatAnimationSpeed = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.combatAnimationSpeed += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.combatAnimationSpeed -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.combatAnimationSpeed *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.combatAnimationSpeed;
                    helpResult /= val;
                    gv.mod.combatAnimationSpeed = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.combatAnimationSpeed %= val;
                }
            }

            else if (element[0].EndsWith("indexOfPCtoLastUseItem"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.indexOfPCtoLastUseItem = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.indexOfPCtoLastUseItem += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.indexOfPCtoLastUseItem -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.indexOfPCtoLastUseItem *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.indexOfPCtoLastUseItem;
                    helpResult /= val;
                    gv.mod.indexOfPCtoLastUseItem = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.indexOfPCtoLastUseItem %= val;
                }
            }

            else if (element[0].EndsWith("partyTokenFilename"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.partyTokenFilename = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.partyTokenFilename += val;
                }
            }
            /*else if (element[0].EndsWith("OnHeartBeatLogicTree"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.OnHeartBeatLogicTree = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.OnHeartBeatLogicTree += val;
                }
            }*/
            /*else if (element[0].EndsWith("OnHeartBeatParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.OnHeartBeatParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.OnHeartBeatParms += val;
                }
            }*/
            else if (element[0].EndsWith("OnHeartBeatIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.OnHeartBeatIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.OnHeartBeatIBScript += val;
                }
            }
            else if (element[0].EndsWith("OnHeartBeatIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.OnHeartBeatIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.OnHeartBeatIBScriptParms += val;
                }
            }
            else if (element[0].EndsWith("allowSave"))
            {

                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.allowSave = true;
                }
                else
                {
                    gv.mod.allowSave = false;
                }
            }
            else if (element[0].EndsWith("debugMode"))
            {

                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.debugMode = true;
                }
                else
                {
                    gv.mod.debugMode = false;
                }
            }
            else if (element[0].EndsWith("showPartyToken"))
            {

                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.showPartyToken = true;
                }
                else
                {
                    gv.mod.showPartyToken = false;
                }
            }
            else if (element[0].EndsWith("PlayerFacingLeft"))
            {

                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.PlayerFacingLeft = true;
                }
                else
                {
                    gv.mod.PlayerFacingLeft = false;
                }
            }
            else if (element[0].EndsWith("allowAutosave"))
            {

                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.allowAutosave = true;
                }
                else
                {
                    gv.mod.allowAutosave = false;
                }
            }

        }
        public void PropAssignment(string[] element, int indexNum, int indexNum2)
        {
            if (element[0].EndsWith("isShown"))
            {
                
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isShown = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isShown = false;
                }
            }

            else if (element[0].EndsWith("LocationX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationX;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationX %= val;
                }
            }

            else if (element[0].EndsWith("LocationY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationY;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationY %= val;
                }
            }

            else if (element[0].EndsWith("PostLocationX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationX;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationX %= val;
                }
            }

            else if (element[0].EndsWith("PostLocationY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationY;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationY %= val;
                }
            }

            else if (element[0].EndsWith("WayPointListCurrentIndex"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].WayPointListCurrentIndex = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].WayPointListCurrentIndex += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].WayPointListCurrentIndex -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].WayPointListCurrentIndex *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].WayPointListCurrentIndex;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].WayPointListCurrentIndex = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].WayPointListCurrentIndex %= val;
                }
            }

            else if (element[0].EndsWith("ChanceToMove2Squares"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove2Squares = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove2Squares += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove2Squares -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove2Squares *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove2Squares;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove2Squares = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove2Squares %= val;
                }
            }

            else if (element[0].EndsWith("ChanceToMove0Squares"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove0Squares = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove0Squares += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove0Squares -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove0Squares *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove0Squares;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove0Squares = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove0Squares %= val;
                }
            }

            else if (element[0].EndsWith("ChaserDetectRangeRadius"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserDetectRangeRadius = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserDetectRangeRadius += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserDetectRangeRadius -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserDetectRangeRadius *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserDetectRangeRadius;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserDetectRangeRadius = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserDetectRangeRadius %= val;
                }
            }

            else if (element[0].EndsWith("ChaserGiveUpChasingRangeRadius"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserGiveUpChasingRangeRadius = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserGiveUpChasingRangeRadius += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserGiveUpChasingRangeRadius -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserGiveUpChasingRangeRadius *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserGiveUpChasingRangeRadius;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserGiveUpChasingRangeRadius = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserGiveUpChasingRangeRadius %= val;
                }
            }

            else if (element[0].EndsWith("ChaserChaseDuration"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserChaseDuration = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserChaseDuration += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserChaseDuration -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserChaseDuration *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserChaseDuration;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserChaseDuration = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserChaseDuration %= val;
                }
            }

            else if (element[0].EndsWith("ChaserStartChasingTime"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserStartChasingTime = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserStartChasingTime += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserStartChasingTime -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserStartChasingTime *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserStartChasingTime;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserStartChasingTime = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserStartChasingTime %= val;
                }
            }

            else if (element[0].EndsWith("RandomMoverRadius"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].RandomMoverRadius = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].RandomMoverRadius += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].RandomMoverRadius -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].RandomMoverRadius *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].RandomMoverRadius;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].RandomMoverRadius = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].RandomMoverRadius %= val;
                }
            }
            else if (element[0].EndsWith("PropFacingLeft"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropFacingLeft = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropFacingLeft = false;
                }
            }

            else if (element[0].EndsWith("HasCollision"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].HasCollision = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].HasCollision = false;
                }
            }

            else if (element[0].EndsWith("isActive"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isActive = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isActive = false;
                }
            }

            else if (element[0].EndsWith("DeletePropWhenThisEncounterIsWon"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].DeletePropWhenThisEncounterIsWon = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].DeletePropWhenThisEncounterIsWon = false;
                }
            }

            else if (element[0].EndsWith("isMover"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isMover = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isMover = false;
                }
            }

            else if (element[0].EndsWith("isChaser"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isChaser = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isChaser = false;
                }
            }

            else if (element[0].EndsWith("isCurrentlyChasing"))
            {
                string val = ConcateString(element[2]);
                if (val == "val")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isCurrentlyChasing = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isCurrentlyChasing = false;
                }
            }

            else if (element[0].EndsWith("ReturningToPost"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ReturningToPost = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ReturningToPost = false;
                }
            }
            else if (element[0].EndsWith("ImageFileName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ImageFileName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ImageFileName += val;
                }
            }
            else if (element[0].EndsWith("MouseOverText"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].MouseOverText = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].MouseOverText += val;
                }
            }
            else if (element[0].EndsWith("PropTag"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropTag = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropTag += val;
                }
            }
            else if (element[0].EndsWith("PropCategoryName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropCategoryName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropCategoryName += val;
                }
            }
            else if (element[0].EndsWith("ConversationWhenOnPartySquare"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ConversationWhenOnPartySquare = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ConversationWhenOnPartySquare += val;
                }
            }
            else if (element[0].EndsWith("EncounterWhenOnPartySquare"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].EncounterWhenOnPartySquare = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].EncounterWhenOnPartySquare += val;
                }
            }
            else if (element[0].EndsWith("MoverType"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].MoverType = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].MoverType += val;
                }
            }
            /*else if (element[0].EndsWith("OnHeartBeatLogicTree"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatLogicTree = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatLogicTree += val;
                }
            }*/
            /*else if (element[0].EndsWith("OnHeartBeatParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatParms += val;
                }
            }*/
            else if (element[0].EndsWith("OnHeartBeatIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatIBScript += val;
                }
            }
            else if (element[0].EndsWith("OnHeartBeatIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatIBScriptParms += val;
                }
            }

        }
        public void CreatureInCurrentEncounterAssignment(string[] element, int indexNum)
        {
            if (element[0].EndsWith("combatLocX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocX;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocX %= val;
                }
            }
            else if (element[0].EndsWith("combatLocY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocY;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocY %= val;
                }
            }
            else if (element[0].EndsWith("moveDistance"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].moveDistance = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].moveDistance += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].moveDistance -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].moveDistance *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].moveDistance;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].moveDistance = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].moveDistance %= val;
                }
            }
            else if (element[0].EndsWith("cr_level"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_level = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_level += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_level -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_level *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_level;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_level = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_level %= val;
                }
            }
            else if (element[0].EndsWith("hp"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hp = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hp += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hp -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hp *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].hp;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hp = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hp %= val;
                }
            }
            else if (element[0].EndsWith("hpMax"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hpMax = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hpMax += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hpMax -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hpMax *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].hpMax;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hpMax = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].hpMax %= val;
                }
            }
            else if (element[0].EndsWith("sp"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].sp = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].sp += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].sp -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].sp *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].sp;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].sp = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].sp %= val;
                }
            }
            else if (element[0].EndsWith("cr_XP"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_XP = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_XP += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_XP -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_XP *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_XP;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_XP = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_XP %= val;
                }
            }
            else if (element[0].EndsWith("AC"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].AC = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].AC += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].AC -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].AC *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].AC;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].AC = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].AC %= val;
                }
            }
            else if (element[0].EndsWith("cr_att"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_att = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_att += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_att -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_att *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_att;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_att = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_att %= val;
                }
            }
            else if (element[0].EndsWith("cr_attRange"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attRange = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attRange += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attRange -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attRange *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attRange;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attRange = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attRange %= val;
                }
            }
            else if (element[0].EndsWith("cr_damageNumDice"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageNumDice = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageNumDice += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageNumDice -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageNumDice *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageNumDice;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageNumDice = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageNumDice %= val;
                }
            }
            else if (element[0].EndsWith("cr_damageDie"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageDie = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageDie += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageDie -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageDie *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageDie;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageDie = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageDie %= val;
                }
            }
            else if (element[0].EndsWith("cr_damageAdder"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageAdder = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageAdder += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageAdder -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageAdder *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageAdder;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageAdder = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageAdder %= val;
                }
            }
            else if (element[0].EndsWith("cr_numberOfAttacks"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_numberOfAttacks = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_numberOfAttacks += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_numberOfAttacks -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_numberOfAttacks *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_numberOfAttacks;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_numberOfAttacks = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_numberOfAttacks %= val;
                }
            }
            else if (element[0].EndsWith("fortitude"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].fortitude = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].fortitude += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].fortitude -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].fortitude *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].fortitude;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].fortitude = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].fortitude %= val;
                }
            }
            else if (element[0].EndsWith("will"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].will = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].will += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].will -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].will *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].will;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].will = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].will %= val;
                }
            }
            else if (element[0].EndsWith("reflex"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].reflex = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].reflex += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].reflex -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].reflex *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].reflex;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].reflex = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].reflex %= val;
                }
            }
            else if (element[0].EndsWith("damageTypeResistanceValueAcid"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueAcid = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueAcid += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueAcid -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueAcid *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueAcid;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueAcid = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueAcid %= val;
                }
            }
            else if (element[0].EndsWith("damageTypeResistanceValueNormal"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueNormal = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueNormal += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueNormal -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueNormal *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueNormal;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueNormal = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueNormal %= val;
                }
            }
            else if (element[0].EndsWith("damageTypeResistanceValueCold"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueCold = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueCold += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueCold -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueCold *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueCold;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueCold = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueCold %= val;
                }
            }
            else if (element[0].EndsWith("damageTypeResistanceValueElectricity"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueElectricity = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueElectricity += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueElectricity -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueElectricity *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueElectricity;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueElectricity = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueElectricity %= val;
                }
            }
            else if (element[0].EndsWith("damageTypeResistanceValueFire"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueFire = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueFire += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueFire -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueFire *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueFire;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueFire = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueFire %= val;
                }
            }
            else if (element[0].EndsWith("damageTypeResistanceValueMagic"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueMagic = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueMagic += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueMagic -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueMagic *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueMagic;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueMagic = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueMagic %= val;
                }
            }
            else if (element[0].EndsWith("damageTypeResistanceValuePoison"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValuePoison = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValuePoison += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValuePoison -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValuePoison *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValuePoison;
                    helpResult /= val;
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValuePoison = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValuePoison %= val;
                }
            }

            else if (element[0].EndsWith("cr_tokenFilename"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_tokenFilename = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_tokenFilename += val;
                }
            }
            else if (element[0].EndsWith("cr_name"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_name = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_name += val;
                }
            }
            else if (element[0].EndsWith("cr_tag"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_tag = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_tag += val;
                }
            }
            else if (element[0].EndsWith("cr_resref"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_resref = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_resref += val;
                }
            }
            else if (element[0].EndsWith("cr_desc"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_desc = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_desc += val;
                }
            }
            else if (element[0].EndsWith("cr_status"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_status = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_status += val;
                }
            }
            else if (element[0].EndsWith("cr_category"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_category = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_category += val;
                }
            }
            else if (element[0].EndsWith("cr_projSpriteFilename"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_projSpriteFilename = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_projSpriteFilename += val;
                }
            }
            else if (element[0].EndsWith("cr_spriteEndingFilename"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_spriteEndingFilename = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_spriteEndingFilename += val;
                }
            }
            else if (element[0].EndsWith("cr_attackSound"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attackSound = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attackSound += val;
                }
            }
            else if (element[0].EndsWith("cr_ai"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_ai = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_ai += val;
                }
            }
            else if (element[0].EndsWith("cr_typeOfDamage"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_typeOfDamage = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_typeOfDamage += val;
                }
            }
            else if (element[0].EndsWith("onScoringHit"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].onScoringHit = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].onScoringHit += val;
                }
            }
            else if (element[0].EndsWith("onScoringHitParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].onScoringHitParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].onScoringHitParms += val;
                }
            }
            /*else if (element[0].EndsWith("onDeathLogicTree"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].onDeathLogicTree = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].onDeathLogicTree += val;
                }
            }
            else if (element[0].EndsWith("onDeathParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].onDeathParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].onDeathParms += val;
                }
            }*/

            else if (element[0].EndsWith("combatFacingLeft"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatFacingLeft = true;
                }
                else
                {
                    gv.mod.currentEncounter.encounterCreatureList[indexNum].combatFacingLeft = false;
                }
            }
            
        }
        public void CreatureResRefAssignment(string[] element, int indexNum, int indexNum2)
        {
            if (element[0].EndsWith("creatureStartLocationX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX;
                    helpResult /= val;
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX %= val;
                }
            }
            else if (element[0].EndsWith("creatureStartLocationY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY;
                    helpResult /= val;
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY %= val;
                }
            }
            else if (element[0].EndsWith("creatureResRef"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureResRef = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureResRef += val;
                }
            }
            else if (element[0].EndsWith("creatureTag"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureTag = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureTag += val;
                }
            }
        }
        public void AreaAssignment(string[] element, int indexNum)
        {
            if (element[0].EndsWith("TimePerSquare"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].TimePerSquare = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].TimePerSquare += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].TimePerSquare -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].TimePerSquare *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].TimePerSquare;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].TimePerSquare = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].TimePerSquare %= val;
                }
            }

            else if (element[0].EndsWith("MapSizeX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].MapSizeX;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].MapSizeX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeX %= val;
                }
            }

            else if (element[0].EndsWith("MapSizeY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].MapSizeY;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].MapSizeY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MapSizeY %= val;
                }
            }

            else if (element[0].EndsWith("UseMiniMapFogOfWar"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].UseMiniMapFogOfWar = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].UseMiniMapFogOfWar = false;
                }
            }

            else if (element[0].EndsWith("areaDark"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].areaDark = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].areaDark = false;
                }
            }

            else if (element[0].EndsWith("UseDayNightCycle"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleAreasObjects[indexNum].UseDayNightCycle = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].UseDayNightCycle = false;
                }
            }

            else if (element[0].EndsWith("Filename"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Filename = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].Filename += val;
                }
            }

            else if (element[0].EndsWith("MusicFileName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MusicFileName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].MusicFileName += val;
                }
            }

            else if (element[0].EndsWith("ImageFileName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].ImageFileName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].ImageFileName += val;
                }
            }

            else if (element[0].EndsWith("AreaMusic"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].AreaMusic = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].AreaMusic += val;
                }
            }

            else if (element[0].EndsWith("AreaSounds"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].AreaSounds = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].AreaSounds += val;
                }
            }

            /*else if (element[0].EndsWith("OnHeartBeatLogicTree"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].OnHeartBeatLogicTree = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].OnHeartBeatLogicTree += val;
                }
            }

            else if (element[0].EndsWith("OnHeartBeatParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].OnHeartBeatParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].OnHeartBeatParms += val;
                }
            }*/

            else if (element[0].EndsWith("OnHeartBeatIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].OnHeartBeatIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].OnHeartBeatIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnHeartBeatIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].OnHeartBeatIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].OnHeartBeatIBScriptParms += val;
                }
            }
            else if (element[0].EndsWith("inGameAreaName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].inGameAreaName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].inGameAreaName += val;
                }
            }
        }
        public void EncounterAssignment(string[] element, int indexNum)
        {
            if (element[0].EndsWith("MapSizeX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleEncountersList[indexNum].MapSizeX;
                    helpResult /= val;
                    gv.mod.moduleEncountersList[indexNum].MapSizeX = (int)helpResult;
                }
                    //yn1: avoided using % because I wasnt sure if this would lead to confusion with already reserved usage of % for object property identification
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeX %= val;
                }
            }
            else if (element[0].EndsWith("MapSizeY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleEncountersList[indexNum].MapSizeY;
                    helpResult /= val;
                    gv.mod.moduleEncountersList[indexNum].MapSizeY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapSizeY %= val;
                }

            }
            else if (element[0].EndsWith("goldDrop"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].goldDrop = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].goldDrop += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleEncountersList[indexNum].goldDrop -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleEncountersList[indexNum].goldDrop *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleEncountersList[indexNum].goldDrop;
                    helpResult /= val;
                    gv.mod.moduleEncountersList[indexNum].goldDrop = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleEncountersList[indexNum].goldDrop %= val;
                }

            }

            else if (element[0].EndsWith("AreaMusicDelay"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelay = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelay += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelay -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelay *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleEncountersList[indexNum].AreaMusicDelay;
                    helpResult /= val;
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelay = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelay %= val;
                }
            }

            else if (element[0].EndsWith("AreaMusicDelayRandomAdder"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelayRandomAdder = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelayRandomAdder += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelayRandomAdder -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelayRandomAdder *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleEncountersList[indexNum].AreaMusicDelayRandomAdder;
                    helpResult /= val;
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelayRandomAdder = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusicDelayRandomAdder %= val;
                }
            }

            else if (element[0].EndsWith("UseMapImage"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleEncountersList[indexNum].UseMapImage = true;
                }
                else
                {
                    gv.mod.moduleEncountersList[indexNum].UseMapImage = false;
                }
            }
            else if (element[0].EndsWith("UseDayNightCycle"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.moduleEncountersList[indexNum].UseDayNightCycle = true;
                }
                else
                {
                    gv.mod.moduleEncountersList[indexNum].UseDayNightCycle = false;
                }
            }
            else if (element[0].EndsWith("encounterName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].encounterName += val;
                }
            }

            else if (element[0].EndsWith("MapImage"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapImage = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].MapImage += val;
                }
            }

            else if (element[0].EndsWith("AreaMusic"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusic = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].AreaMusic += val;
                }
            }

            else if (element[0].EndsWith("OnSetupCombatIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnSetupCombatIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnSetupCombatIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnSetupCombatIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnSetupCombatIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnSetupCombatIBScriptParms += val;
                }
            }

            else if (element[0].EndsWith("OnStartCombatRoundIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnStartCombatRoundIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnStartCombatRoundIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnStartCombatRoundIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnStartCombatRoundIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnStartCombatRoundIBScriptParms += val;
                }
            }

            else if (element[0].EndsWith("OnStartCombatTurnIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnStartCombatTurnIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnStartCombatTurnIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnStartCombatTurnIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnStartCombatTurnIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnStartCombatTurnIBScriptParms += val;
                }
            }

            else if (element[0].EndsWith("OnEndCombatIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnEndCombatIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnEndCombatIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnEndCombatIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnEndCombatIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleEncountersList[indexNum].OnEndCombatIBScriptParms += val;
                }
            }         
        }

        public void CurrentEncounterAssignment(string[] element)
        {
            if (element[0].EndsWith("MapSizeX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.MapSizeX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.MapSizeX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.MapSizeX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.MapSizeX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.MapSizeX;
                    helpResult /= val;
                    gv.mod.currentEncounter.MapSizeX = (int)helpResult;
                }
                //yn1: avoided using % because I wasnt sure if this would lead to confusion with already reserved usage of % for object property identification
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.MapSizeX %= val;
                }
            }
            else if (element[0].EndsWith("MapSizeY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.MapSizeY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.MapSizeY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.MapSizeY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.MapSizeY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.MapSizeY;
                    helpResult /= val;
                    gv.mod.currentEncounter.MapSizeY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.MapSizeY %= val;
                }

            }
            else if (element[0].EndsWith("goldDrop"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.goldDrop = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.goldDrop += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.goldDrop -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.goldDrop *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.goldDrop;
                    helpResult /= val;
                    gv.mod.currentEncounter.goldDrop = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.goldDrop %= val;
                }

            }

            else if (element[0].EndsWith("AreaMusicDelay"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.AreaMusicDelay = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.AreaMusicDelay += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.AreaMusicDelay -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.AreaMusicDelay *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.AreaMusicDelay;
                    helpResult /= val;
                    gv.mod.currentEncounter.AreaMusicDelay = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.AreaMusicDelay %= val;
                }
            }

            else if (element[0].EndsWith("AreaMusicDelayRandomAdder"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.AreaMusicDelayRandomAdder = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.AreaMusicDelayRandomAdder += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentEncounter.AreaMusicDelayRandomAdder -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentEncounter.AreaMusicDelayRandomAdder *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentEncounter.AreaMusicDelayRandomAdder;
                    helpResult /= val;
                    gv.mod.currentEncounter.AreaMusicDelayRandomAdder = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentEncounter.AreaMusicDelayRandomAdder %= val;
                }
            }

            else if (element[0].EndsWith("UseMapImage"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.currentEncounter.UseMapImage = true;
                }
                else
                {
                    gv.mod.currentEncounter.UseMapImage = false;
                }
            }
            else if (element[0].EndsWith("UseDayNightCycle"))
            {
                string val = ConcateString(element[2]);
                if (val == "true")
                {
                    gv.mod.currentEncounter.UseDayNightCycle = true;
                }
                else
                {
                    gv.mod.currentEncounter.UseDayNightCycle = false;
                }
            }
            else if (element[0].EndsWith("encounterName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.encounterName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.encounterName += val;
                }
            }

            else if (element[0].EndsWith("MapImage"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.MapImage = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.MapImage += val;
                }
            }

            else if (element[0].EndsWith("AreaMusic"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.AreaMusic = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.AreaMusic += val;
                }
            }

            else if (element[0].EndsWith("OnSetupCombatIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.OnSetupCombatIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.OnSetupCombatIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnSetupCombatIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.OnSetupCombatIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.OnSetupCombatIBScriptParms += val;
                }
            }

            else if (element[0].EndsWith("OnStartCombatRoundIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.OnStartCombatRoundIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.OnStartCombatRoundIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnStartCombatRoundIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.OnStartCombatRoundIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.OnStartCombatRoundIBScriptParms += val;
                }
            }

            else if (element[0].EndsWith("OnStartCombatTurnIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.OnStartCombatTurnIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.OnStartCombatTurnIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnStartCombatTurnIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.OnStartCombatTurnIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.OnStartCombatTurnIBScriptParms += val;
                }
            }

            else if (element[0].EndsWith("OnEndCombatIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.OnEndCombatIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.OnEndCombatIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnEndCombatIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentEncounter.OnEndCombatIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentEncounter.OnEndCombatIBScriptParms += val;
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
            rightSide = rightSide.Trim();
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

        public string GetBetweenFunctionParms(string str)
        {
            int posA = str.IndexOf("(");
            int posB = str.LastIndexOf(")");
            if (posA == -1)
            {
                return str;
            }
            if (posB == -1)
            {
                return str;
            }
            int adjustedPosA = posA + 1;
            if (adjustedPosA >= posB)
            {
                return str;
            }
            return str.Substring(adjustedPosA, posB - adjustedPosA);
        }
        public string[] GetParameters(string line)
        {
            string lineJustParms = GetBetweenFunctionParms(line);
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
            else if (parm.StartsWith("parm("))
            {
                string indx = parm.Split('(', ')')[1];
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
                    else if (parm.EndsWith("moveDistance"))
                    {
                        return gv.mod.playerList[indexNum].moveDistance.ToString();
                    }
                    else if (parm.EndsWith("moveOrder"))
                    {
                        return gv.mod.playerList[indexNum].moveOrder.ToString();
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
                    else if (parm.EndsWith("wisdom"))
                    {
                        return gv.mod.playerList[indexNum].wisdom.ToString();
                    }
                    else if (parm.EndsWith("constitution"))
                    {
                        return gv.mod.playerList[indexNum].constitution.ToString();
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
                    else if (parm.EndsWith("baseWis"))
                    {
                        return gv.mod.playerList[indexNum].baseWis.ToString();
                    }
                    else if (parm.EndsWith("baseCon"))
                    {
                        return gv.mod.playerList[indexNum].baseCon.ToString();
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
         
                    else if (parm.EndsWith("SizeOfKnownSpellsTags"))
                    {
                        return gv.mod.playerList[indexNum].knownSpellsTags.Count.ToString();
                    }
                    /*else if (parm.EndsWith("SizeOfKnownSpellsList"))
                    {
                        return gv.mod.playerList[indexNum].knownSpellsList.Count.ToString();
                    }*/
                    else if (parm.EndsWith("SizeOfKnownTraitsTags"))
                    {
                        return gv.mod.playerList[indexNum].knownTraitsTags.Count.ToString();
                    }
                    /*else if (parm.EndsWith("SizeOfKnownTraitsList"))
                    {
                        return gv.mod.playerList[indexNum].knownTraitsList.Count.ToString();
                    }*/
                    else if (parm.EndsWith("SizeOfEffectsList"))
                    {
                        return gv.mod.playerList[indexNum].effectsList.Count.ToString();
                    }
                    else if (parm.EndsWith("combatFacingLeft"))
                    {
                        return gv.mod.playerList[indexNum].combatFacingLeft.ToString();
                    }
                    else if (parm.EndsWith("steathModeOn"))
                    {
                        return gv.mod.playerList[indexNum].steathModeOn.ToString();
                    }
                    else if (parm.EndsWith("mainPc"))
                    {
                        return gv.mod.playerList[indexNum].mainPc.ToString();
                    }
                    else if (parm.EndsWith("nonRemoveablePc"))
                    {
                        return gv.mod.playerList[indexNum].nonRemoveablePc.ToString();
                    }
                    else if (parm.EndsWith("isMale"))
                    {
                        return gv.mod.playerList[indexNum].isMale.ToString();
                    }
                    else if (parm.EndsWith("tokenFilename"))
                    {
                        return gv.mod.playerList[indexNum].tokenFilename.ToString();
                    }
                    else if (parm.EndsWith("name"))
                    {
                        return gv.mod.playerList[indexNum].name.ToString();
                    }
                    else if (parm.EndsWith("tag"))
                    {
                        return gv.mod.playerList[indexNum].tag.ToString();
                    }
                    else if (parm.EndsWith("raceTag"))
                    {
                        return gv.mod.playerList[indexNum].raceTag.ToString();
                    }
                    else if (parm.EndsWith("classTag"))
                    {
                        return gv.mod.playerList[indexNum].classTag.ToString();
                    }
                    else if (parm.EndsWith("HeadRefsTag"))
                    {
                        return gv.mod.playerList[indexNum].HeadRefs.tag.ToString();
                    }
                    else if (parm.EndsWith("HeadRefsName"))
                    {
                        return gv.mod.playerList[indexNum].HeadRefs.name.ToString();
                    }
                    else if (parm.EndsWith("HeadRefsResRef"))
                    {
                        return gv.mod.playerList[indexNum].HeadRefs.resref.ToString();
                    }
                    else if (parm.EndsWith("HeadRefsCanNotBeUnequipped"))
                    {
                        return gv.mod.playerList[indexNum].HeadRefs.canNotBeUnequipped.ToString();
                    }
                    else if (parm.EndsWith("HeadRefsQuantity"))
                    {
                        return gv.mod.playerList[indexNum].HeadRefs.quantity.ToString();
                    }
                    else if (parm.EndsWith("NeckRefsTag"))
                    {
                        return gv.mod.playerList[indexNum].NeckRefs.tag.ToString();
                    }
                    else if (parm.EndsWith("NeckRefsName"))
                    {
                        return gv.mod.playerList[indexNum].NeckRefs.name.ToString();
                    }
                    else if (parm.EndsWith("NeckRefsResRef"))
                    {
                        return gv.mod.playerList[indexNum].NeckRefs.resref.ToString();
                    }
                    else if (parm.EndsWith("NeckRefsCanNotBeUnequipped"))
                    {
                        return gv.mod.playerList[indexNum].NeckRefs.canNotBeUnequipped.ToString();
                    }
                    else if (parm.EndsWith("NeckRefsQuantity"))
                    {
                        return gv.mod.playerList[indexNum].NeckRefs.quantity.ToString();
                    }
                    else if (parm.EndsWith("BodyRefsTag"))
                    {
                        return gv.mod.playerList[indexNum].BodyRefs.tag.ToString();
                    }
                    else if (parm.EndsWith("BodyRefsName"))
                    {
                        return gv.mod.playerList[indexNum].BodyRefs.name.ToString();
                    }
                    else if (parm.EndsWith("BodyRefsResRef"))
                    {
                        return gv.mod.playerList[indexNum].BodyRefs.resref.ToString();
                    }
                    else if (parm.EndsWith("BodyRefsCanNotBeUnequipped"))
                    {
                        return gv.mod.playerList[indexNum].BodyRefs.canNotBeUnequipped.ToString();
                    }
                    else if (parm.EndsWith("BodyRefsQuantity"))
                    {
                        return gv.mod.playerList[indexNum].BodyRefs.quantity.ToString();
                    }
                    else if (parm.EndsWith("MainHandRefsTag"))
                    {
                        return gv.mod.playerList[indexNum].MainHandRefs.tag.ToString();
                    }
                    else if (parm.EndsWith("MainHandRefsName"))
                    {
                        return gv.mod.playerList[indexNum].MainHandRefs.name.ToString();
                    }
                    else if (parm.EndsWith("MainHandRefsResRef"))
                    {
                        return gv.mod.playerList[indexNum].MainHandRefs.resref.ToString();
                    }
                    else if (parm.EndsWith("MainHandRefsCanNotBeUnequipped"))
                    {
                        return gv.mod.playerList[indexNum].MainHandRefs.canNotBeUnequipped.ToString();
                    }
                    else if (parm.EndsWith("MainHandRefsQuantity"))
                    {
                        return gv.mod.playerList[indexNum].MainHandRefs.quantity.ToString();
                    }
                    else if (parm.EndsWith("OffHandRefsTag"))
                    {
                        return gv.mod.playerList[indexNum].OffHandRefs.tag.ToString();
                    }
                    else if (parm.EndsWith("OffHandRefsName"))
                    {
                        return gv.mod.playerList[indexNum].OffHandRefs.name.ToString();
                    }
                    else if (parm.EndsWith("OffHandRefsResRef"))
                    {
                        return gv.mod.playerList[indexNum].OffHandRefs.resref.ToString();
                    }
                    else if (parm.EndsWith("OffHandRefsCanNotBeUnequipped"))
                    {
                        return gv.mod.playerList[indexNum].OffHandRefs.canNotBeUnequipped.ToString();
                    }
                    else if (parm.EndsWith("OffHandRefsQuantity"))
                    {
                        return gv.mod.playerList[indexNum].OffHandRefs.quantity.ToString();
                    }
                    else if (parm.EndsWith("RingRefsTag"))
                    {
                        return gv.mod.playerList[indexNum].RingRefs.tag.ToString();
                    }
                    else if (parm.EndsWith("RingRefsName"))
                    {
                        return gv.mod.playerList[indexNum].RingRefs.name.ToString();
                    }
                    else if (parm.EndsWith("RingRefsResRef"))
                    {
                        return gv.mod.playerList[indexNum].RingRefs.resref.ToString();
                    }
                    else if (parm.EndsWith("RingRefsCanNotBeUnequipped"))
                    {
                        return gv.mod.playerList[indexNum].RingRefs.canNotBeUnequipped.ToString();
                    }
                    else if (parm.EndsWith("RingRefsQuantity"))
                    {
                        return gv.mod.playerList[indexNum].RingRefs.quantity.ToString();
                    }
                    else if (parm.EndsWith("Ring2RefsTag"))
                    {
                        return gv.mod.playerList[indexNum].Ring2Refs.tag.ToString();
                    }
                    else if (parm.EndsWith("Ring2RefsName"))
                    {
                        return gv.mod.playerList[indexNum].Ring2Refs.name.ToString();
                    }
                    else if (parm.EndsWith("Ring2RefsResRef"))
                    {
                        return gv.mod.playerList[indexNum].Ring2Refs.resref.ToString();
                    }
                    else if (parm.EndsWith("Ring2RefsCanNotBeUnequipped"))
                    {
                        return gv.mod.playerList[indexNum].Ring2Refs.canNotBeUnequipped.ToString();
                    }
                    else if (parm.EndsWith("Ring2RefsQuantity"))
                    {
                        return gv.mod.playerList[indexNum].Ring2Refs.quantity.ToString();
                    }
                    else if (parm.EndsWith("FeetRefsTag"))
                    {
                        return gv.mod.playerList[indexNum].FeetRefs.tag.ToString();
                    }
                    else if (parm.EndsWith("FeetRefsName"))
                    {
                        return gv.mod.playerList[indexNum].FeetRefs.name.ToString();
                    }
                    else if (parm.EndsWith("FeetRefsResRef"))
                    {
                        return gv.mod.playerList[indexNum].FeetRefs.resref.ToString();
                    }
                    else if (parm.EndsWith("FeetRefsCanNotBeUnequipped"))
                    {
                        return gv.mod.playerList[indexNum].FeetRefs.canNotBeUnequipped.ToString();
                    }
                    else if (parm.EndsWith("FeetRefsQuantity"))
                    {
                        return gv.mod.playerList[indexNum].FeetRefs.quantity.ToString();
                    }
                    else if (parm.EndsWith("AmmoRefsTag"))
                    {
                        return gv.mod.playerList[indexNum].AmmoRefs.tag.ToString();
                    }
                    else if (parm.EndsWith("AmmoRefsName"))
                    {
                        return gv.mod.playerList[indexNum].AmmoRefs.name.ToString();
                    }
                    else if (parm.EndsWith("AmmoRefsResRef"))
                    {
                        return gv.mod.playerList[indexNum].AmmoRefs.resref.ToString();
                    }
                    else if (parm.EndsWith("AmmoRefsCanNotBeUnequipped"))
                    {
                        return gv.mod.playerList[indexNum].AmmoRefs.canNotBeUnequipped.ToString();
                    }
                    else if (parm.EndsWith("AmmoRefsQuantity"))
                    {
                        return gv.mod.playerList[indexNum].AmmoRefs.quantity.ToString();
                    }
                }
                #endregion

                #region Area
                else if (parm.StartsWith("%Area"))
                {
                    if (parm.EndsWith("SizeOfProps"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props.Count.ToString();
                    }
                    else if (parm.EndsWith("Filename"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Filename.ToString();
                    }
                    else if (parm.EndsWith("UseMiniMapFogOfWar"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].UseMiniMapFogOfWar.ToString();
                    }
                    else if (parm.EndsWith("areaDark"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].areaDark.ToString();
                    }
                    else if (parm.EndsWith("UseDayNightCycle"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].UseDayNightCycle.ToString();
                    }
                    else if (parm.EndsWith("TimePerSquare"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].TimePerSquare.ToString();
                    }
                    else if (parm.EndsWith("MusicFileName"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].MusicFileName.ToString();
                    }
                    else if (parm.EndsWith("ImageFileName"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].ImageFileName.ToString();
                    }
                    else if (parm.EndsWith("MapSizeX"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].MapSizeX.ToString();
                    }
                    else if (parm.EndsWith("MapSizeY"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].MapSizeY.ToString();
                    }
                    else if (parm.EndsWith("AreaMusic"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].AreaMusic.ToString();
                    }
                    else if (parm.EndsWith("AreaSounds"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].AreaSounds.ToString();
                    }
                    /*else if (parm.EndsWith("OnHeartBeatLogicTree"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].OnHeartBeatLogicTree.ToString();
                    }
                    else if (parm.EndsWith("OnHeartBeatParms"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].OnHeartBeatParms.ToString();
                    }*/
                    else if (parm.EndsWith("SizeOfTriggers"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Triggers.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOFAreaLocalInts"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].AreaLocalInts.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfAreaLocalStrings"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].AreaLocalStrings.Count.ToString();
                    }
                    else if (parm.EndsWith("OnHeartBeatIBScript"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].OnHeartBeatIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnHeartBeatIBScriptParms"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].OnHeartBeatIBScriptParms.ToString();
                    }
                     else if (parm.EndsWith("inGameAreaName"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].inGameAreaName.ToString();
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
                    else if (parm.EndsWith("LocationX"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationX.ToString();
                    }
                    else if (parm.EndsWith("LocationY"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].LocationY.ToString();
                    }
                    else if (parm.EndsWith("PropFacingLeft"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropFacingLeft.ToString();
                    }
                    else if (parm.EndsWith("HasCollision"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].HasCollision.ToString();
                    }
                    else if (parm.EndsWith("isShown"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isShown.ToString();
                    }
                    else if (parm.EndsWith("isActive"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isActive.ToString();
                    }
                    else if (parm.EndsWith("DeletePropWhenThisEncounterIsWon"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].DeletePropWhenThisEncounterIsWon.ToString();
                    }
                    else if (parm.EndsWith("PostLocationX"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationX.ToString();
                    }
                    else if (parm.EndsWith("PostLocationY"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PostLocationY.ToString();
                    }
                    else if (parm.EndsWith("WayPointListCurrentIndex"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].WayPointListCurrentIndex.ToString();
                    }
                    else if (parm.EndsWith("isMover"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isMover.ToString();
                    }
                    else if (parm.EndsWith("ChanceToMove2Squares"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove2Squares.ToString();
                    }
                    else if (parm.EndsWith("public int ChanceToMove0Squares"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChanceToMove0Squares.ToString();
                    }
                    else if (parm.EndsWith("isChaser"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isChaser.ToString();
                    }
                    else if (parm.EndsWith("isCurrentlyChasing"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].isCurrentlyChasing.ToString();
                    }
                    else if (parm.EndsWith("ChaserDetectRangeRadius"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserDetectRangeRadius.ToString();
                    }
                    else if (parm.EndsWith("ChaserGiveUpChasingRangeRadius"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserGiveUpChasingRangeRadius.ToString();
                    }
                    else if (parm.EndsWith("ChaserChaseDuration"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserChaseDuration.ToString();
                    }
                    else if (parm.EndsWith("ChaserStartChasingTime"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ChaserStartChasingTime.ToString();
                    }
                    else if (parm.EndsWith("RandomMoverRadius"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].RandomMoverRadius.ToString();
                    }
                    else if (parm.EndsWith("ReturningToPost"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ReturningToPost.ToString();
                    }
                    else if (parm.EndsWith("ImageFileName"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ImageFileName.ToString();
                    }
                    else if (parm.EndsWith("MouseOverText"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].MouseOverText.ToString();
                    }
                    else if (parm.EndsWith("PropCategoryName"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropCategoryName.ToString();
                    }
                    else if (parm.EndsWith("ConversationWhenOnPartySquare"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].ConversationWhenOnPartySquare.ToString();
                    }
                    else if (parm.EndsWith("EncounterWhenOnPartySquare"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].EncounterWhenOnPartySquare.ToString();
                    }
                    else if (parm.EndsWith("MoverType"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].MoverType.ToString();
                    }
                    /*else if (parm.EndsWith("OnHeartBeatLogicTree"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatLogicTree.ToString();
                    }*/
                    /*else if (parm.EndsWith("OnHeartBeatParms"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].OnHeartBeatParms.ToString();
                    }*/

                    else if (parm.EndsWith("SizeOfPropLocalInts"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropLocalInts.Count.ToString();
                    }
                    
                    else if (parm.EndsWith("SizeOfPropLocalStrings"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].PropLocalStrings.Count.ToString();
                    }

                    else if (parm.EndsWith("SizeOfWayPointList"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].Props[indexNum2].WayPointList.Count.ToString();
                    }   
                }

                #endregion

                #region CreatureInCurrentEncounter
                else if (parm.StartsWith("%CreatureInCurrrentEncounter"))
                {
                    if (parm.EndsWith("cr_tokenFilename"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_tokenFilename.ToString();
                    }
                    else if (parm.EndsWith("combatFacingLeft"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].combatFacingLeft.ToString();
                    }
                    else if (parm.EndsWith("combatLocX"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocX.ToString();
                    }
                    else if (parm.EndsWith("combatLocY"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].combatLocY.ToString();
                    }
                    else if (parm.EndsWith("moveDistance"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].moveDistance.ToString();
                    }
                    else if (parm.EndsWith("cr_name"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_name.ToString();
                    }
                    else if (parm.EndsWith("cr_tag"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_tag.ToString();
                    }
                    else if (parm.EndsWith("cr_resref"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_resref.ToString();
                    }
                    else if (parm.EndsWith("cr_desc"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_desc.ToString();
                    }
                    else if (parm.EndsWith("cr_level"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_level.ToString();
                    }
                    else if (parm.EndsWith("hpMax"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].hpMax.ToString();
                    }
                    else if (parm.EndsWith("sp"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].sp.ToString();
                    }
                    else if (parm.EndsWith("hp"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].hp.ToString();
                    }
                    else if (parm.EndsWith("cr_XP"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_XP.ToString();
                    }
                    else if (parm.EndsWith("AC"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].AC.ToString();
                    }
                    else if (parm.EndsWith("cr_status"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_status.ToString();
                    }
                    else if (parm.EndsWith("cr_att"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_att.ToString();
                    }
                    else if (parm.EndsWith("cr_attRange"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attRange.ToString();
                    }
                    else if (parm.EndsWith("cr_damageNumDice"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageNumDice.ToString();
                    }
                    else if (parm.EndsWith("cr_damageDie"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageDie.ToString();
                    }
                    else if (parm.EndsWith("cr_damageAdder"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_damageAdder.ToString();
                    }
                    else if (parm.EndsWith("cr_category"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_category.ToString();
                    }
                    else if (parm.EndsWith("cr_projSpriteFilename"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_projSpriteFilename.ToString();
                    }
                    else if (parm.EndsWith("cr_spriteEndingFilename"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_spriteEndingFilename.ToString();
                    }
                    else if (parm.EndsWith("cr_attackSound"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_attackSound.ToString();
                    }
                    else if (parm.EndsWith("cr_numberOfAttacks"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_numberOfAttacks.ToString();
                    }
                    else if (parm.EndsWith("cr_ai"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_ai.ToString();
                    }
                    else if (parm.EndsWith("fortitude"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].fortitude.ToString();
                    }
                    else if (parm.EndsWith("will"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].will.ToString();
                    }
                    else if (parm.EndsWith("reflex"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].reflex.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceValueAcid"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueAcid.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceValueNormal"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueNormal.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceValueCold"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueCold.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceValueElectricity"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueElectricity.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceValueFire"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueFire.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceValueMagic"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValueMagic.ToString();
                    }
                    else if (parm.EndsWith("damageTypeResistanceValuePoison"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].damageTypeResistanceValuePoison.ToString();
                    }
                    else if (parm.EndsWith("cr_typeOfDamage"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_typeOfDamage.ToString();
                    }
                    else if (parm.EndsWith("onScoringHit"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].onScoringHit.ToString();
                    }
                    else if (parm.EndsWith("onScoringHitParms"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].onScoringHitParms.ToString();
                    }
                    /*else if (parm.EndsWith("onDeathLogicTree"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].onDeathLogicTree.ToString();
                    }
                    else if (parm.EndsWith("onDeathParms"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].onDeathParms.ToString();
                    }*/
                    else if (parm.EndsWith("knownSpellsTags"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].knownSpellsTags.Count.ToString();
                    }
                    else if (parm.EndsWith("cr_effectsList"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].cr_effectsList.Count.ToString();
                    }
                    else if (parm.EndsWith("CreatureLocalInts"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].CreatureLocalInts.Count.ToString();
                    }
                    else if (parm.EndsWith("CreatureLocalStrings"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList[indexNum].CreatureLocalStrings.Count.ToString();
                    }
                }
                #endregion

                #region CreatureResRef
                else if (parm.StartsWith("%CreatureResRef"))
                {
                    if (parm.EndsWith("creatureResRef"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureResRef.ToString();
                    }
                    else if (parm.EndsWith("creatureTag"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureTag.ToString();
                    }
                    else if (parm.EndsWith("creatureStartLocationX"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX.ToString();
                    }
                    else if (parm.EndsWith("creatureStartLocationY"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY.ToString();
                    }
                }

                #endregion

                #region Mod
                else if (parm.StartsWith("%Mod"))
                {
                    if (parm.EndsWith("WorldTime"))
                    {
                        return gv.mod.WorldTime.ToString();
                    }
                    else if (parm.EndsWith("TimePerRound"))
                    {
                        return gv.mod.TimePerRound.ToString();
                    }
                    else if (parm.EndsWith("PlayerLocationX"))
                    {
                        return gv.mod.PlayerLocationX.ToString();
                    }
                    else if (parm.EndsWith("PlayerLocationY"))
                    {
                        return gv.mod.PlayerLocationY.ToString();
                    }
                    else if (parm.EndsWith("PlayerLastLocationX"))
                    {
                        return gv.mod.PlayerLastLocationX.ToString();
                    }
                    else if (parm.EndsWith("PlayerLastLocationY"))
                    {
                        return gv.mod.PlayerLastLocationY.ToString();
                    }
                    else if (parm.EndsWith("partyGold"))
                    {
                        return gv.mod.partyGold.ToString();
                    }
                    else if (parm.EndsWith("showPartyToken"))
                    {
                        return gv.mod.showPartyToken.ToString();
                    }
                    else if (parm.EndsWith("partyTokenFilename"))
                    {
                        return gv.mod.partyTokenFilename.ToString();
                    }
                    else if (parm.EndsWith("selectedPartyLeader"))
                    {
                        return gv.mod.selectedPartyLeader.ToString();
                    }
                    else if (parm.EndsWith("indexOfPCtoLastUseItem"))
                    {
                        return gv.mod.indexOfPCtoLastUseItem.ToString();
                    }
                    else if (parm.EndsWith("combatAnimationSpeed"))
                    {
                        return gv.mod.combatAnimationSpeed.ToString();
                    }
                    /*else if (parm.EndsWith("OnHeartBeatLogicTree"))
                    {
                        return gv.mod.OnHeartBeatLogicTree.ToString();
                    }*/
                    /*else if (parm.EndsWith("OnHeartBeatParms"))
                    {
                        return gv.mod.OnHeartBeatParms.ToString();
                    }*/
                    else if (parm.EndsWith("OnHeartBeatIBScript"))
                    {
                        return gv.mod.OnHeartBeatIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnHeartBeatIBScriptParms"))
                    {
                        return gv.mod.OnHeartBeatIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("debugMode"))
                    {
                        return gv.mod.debugMode.ToString();
                    }
                    else if (parm.EndsWith("allowSave"))
                    {
                        return gv.mod.allowSave.ToString();
                    }
                    else if (parm.EndsWith("PlayerFacingLeft"))
                    {
                        return gv.mod.PlayerFacingLeft.ToString();
                    }
                    else if (parm.EndsWith("showPartyToken"))
                    {
                        return gv.mod.showPartyToken.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleItemsList"))
                    {
                        return gv.mod.moduleItemsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleEncountersLists"))
                    {
                        return gv.mod.moduleEncountersList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleContainersList"))
                    {
                        return gv.mod.moduleContainersList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleShopsList"))
                    {
                        return gv.mod.moduleShopsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleCreaturesList"))
                    {
                        return gv.mod.moduleCreaturesList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleJournal"))
                    {
                        return gv.mod.moduleJournal.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModulePlayerClassLists"))
                    {
                        return gv.mod.modulePlayerClassList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleRacesList"))
                    {
                        return gv.mod.moduleRacesList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleSpellsList"))
                    {
                        return gv.mod.moduleSpellsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleTraitsList"))
                    {
                        return gv.mod.moduleTraitsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleEffectsList"))
                    {
                        return gv.mod.moduleEffectsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleAreasList"))
                    {
                        return gv.mod.moduleAreasList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleConvosList"))
                    {
                        return gv.mod.moduleConvosList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleAreasObjects"))
                    {
                        return gv.mod.moduleAreasObjects.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleGlobalInts"))
                    {
                        return gv.mod.moduleGlobalInts.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfmoduleGlobalStrings"))
                    {
                        return gv.mod.moduleGlobalStrings.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfModuleConvoSavedValuesList"))
                    {
                        return gv.mod.moduleConvoSavedValuesList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfPlayerList"))
                    {
                        return gv.mod.playerList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfPartyRosterList"))
                    {
                        return gv.mod.partyRosterList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfPartyInventoryRefsList"))
                    {
                        return gv.mod.partyInventoryRefsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfPartyJournalQuests"))
                    {
                        return gv.mod.partyJournalQuests.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfPartyJournalCompleted"))
                    {
                        return gv.mod.partyJournalCompleted.Count.ToString();
                    }
                }

                #endregion

                #region Encounter
                if (parm.StartsWith("%Encounter"))
                {
                    if (parm.EndsWith("encounterName"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterName.ToString();
                    }
                    else if (parm.EndsWith("MapImage"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].MapImage.ToString();
                    }
                    else if (parm.EndsWith("UseMapImage"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].UseMapImage.ToString();
                    }
                    else if (parm.EndsWith("UseDayNightCycle"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].UseDayNightCycle.ToString();
                    }
                    else if (parm.EndsWith("MapSizeX"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].MapSizeX.ToString();
                    }
                    else if (parm.EndsWith("MapSizeY"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].MapSizeY.ToString();
                    }
                    else if (parm.EndsWith("goldDrop"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].goldDrop.ToString();
                    }
                    else if (parm.EndsWith("AreaMusic"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].AreaMusic.ToString();
                    }
                    else if (parm.EndsWith("AreaMusicDelay"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].AreaMusicDelay.ToString();
                    }
                    else if (parm.EndsWith("AreaMusicDelayRandomAdder"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].AreaMusicDelayRandomAdder.ToString();
                    }
                    else if (parm.EndsWith("OnSetupCombatIBScript"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].OnSetupCombatIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnSetupCombatIBScriptParms"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].OnSetupCombatIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("OnStartCombatRoundIBScript"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].OnStartCombatRoundIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnStartCombatRoundIBScriptParms"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].OnStartCombatRoundIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("OnStartCombatTurnIBScript"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].OnStartCombatTurnIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnStartCombatTurnIBScriptParms"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].OnStartCombatTurnIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("OnEndCombatIBScript"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].OnEndCombatIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnEndCombatIBScriptParms"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].OnEndCombatIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterTiles"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterTiles.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterCreatureRefsList")) 
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterCreatureList"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterInventoryRefsList"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterInventoryRefsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterPcStartLocations"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterPcStartLocations.Count.ToString();
                    }
                }
                #endregion

                 #region CurrentEncounter
                if (parm.StartsWith("%CurrentEncounter"))
                {

                    if (parm.EndsWith("encounterName"))
                    {
                        return gv.mod.currentEncounter.encounterName.ToString();
                    }
                    else if (parm.EndsWith("MapImage"))
                    {
                        return gv.mod.currentEncounter.MapImage.ToString();
                    }
                    else if (parm.EndsWith("UseMapImage"))
                    {
                        return gv.mod.currentEncounter.UseMapImage.ToString();
                    }
                    else if (parm.EndsWith("UseDayNightCycle"))
                    {
                        return gv.mod.currentEncounter.UseDayNightCycle.ToString();
                    }
                    else if (parm.EndsWith("MapSizeX"))
                    {
                        return gv.mod.currentEncounter.MapSizeX.ToString();
                    }
                    else if (parm.EndsWith("MapSizeY"))
                    {
                        return gv.mod.currentEncounter.MapSizeY.ToString();
                    }
                    else if (parm.EndsWith("goldDrop"))
                    {
                        return gv.mod.currentEncounter.goldDrop.ToString();
                    }
                    else if (parm.EndsWith("AreaMusic"))
                    {
                        return gv.mod.currentEncounter.AreaMusic.ToString();
                    }
                    else if (parm.EndsWith("AreaMusicDelay"))
                    {
                        return gv.mod.currentEncounter.AreaMusicDelay.ToString();
                    }
                    else if (parm.EndsWith("AreaMusicDelayRandomAdder"))
                    {
                        return gv.mod.currentEncounter.AreaMusicDelayRandomAdder.ToString();
                    }
                    else if (parm.EndsWith("OnSetupCombatIBScript"))
                    {
                        return gv.mod.currentEncounter.OnSetupCombatIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnSetupCombatIBScriptParms"))
                    {
                        return gv.mod.currentEncounter.OnSetupCombatIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("OnStartCombatRoundIBScript"))
                    {
                        return gv.mod.currentEncounter.OnStartCombatRoundIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnStartCombatRoundIBScriptParms"))
                    {
                        return gv.mod.currentEncounter.OnStartCombatRoundIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("OnStartCombatTurnIBScript"))
                    {
                        return gv.mod.currentEncounter.OnStartCombatTurnIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnStartCombatTurnIBScriptParms"))
                    {
                        return gv.mod.currentEncounter.OnStartCombatTurnIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("OnEndCombatIBScript"))
                    {
                        return gv.mod.currentEncounter.OnEndCombatIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnEndCombatIBScriptParms"))
                    {
                        return gv.mod.currentEncounter.OnEndCombatIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterTiles"))
                    {
                        return gv.mod.currentEncounter.encounterTiles.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterCreatureRefsList"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureRefsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterCreatureList"))
                    {
                        return gv.mod.currentEncounter.encounterCreatureList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterInventoryRefsList"))
                    {
                        return gv.mod.currentEncounter.encounterInventoryRefsList.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfEncounterPcStartLocations"))
                    {
                        return gv.mod.currentEncounter.encounterPcStartLocations.Count.ToString();
                    }

                }
                 #endregion



                /*#region ItemResRefParty
                else if (parm.StartsWith("%ItemResRefParty"))
                {
                    if (parm.EndsWith("resRef"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].creatureResRef.ToString();
                    }
                    else if (parm.EndsWith("tag"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureTag.ToString();
                    }
                    else if (parm.EndsWith("name"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX.ToString();
                    }
                    else if (parm.EndsWith("creatureStartLocationY"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY.ToString();
                    }
                }

                #endregion

                #region ItemResRefContainer
                else if (parm.StartsWith("%ItemResRefContainer"))
                {
                    if (parm.EndsWith("resRef"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].creatureResRef.ToString();
                    }
                    else if (parm.EndsWith("tag"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureTag.ToString();
                    }
                    else if (parm.EndsWith("name"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX.ToString();
                    }
                    else if (parm.EndsWith("creatureStartLocationY"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY.ToString();
                    }
                }

                #endregion

                #region ItemResRefShop
                else if (parm.StartsWith("%ItemResRefShop"))
                {
                    if (parm.EndsWith("resRef"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].creatureResRef.ToString();
                    }
                    else if (parm.EndsWith("tag"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureTag.ToString();
                    }
                    else if (parm.EndsWith("name"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationX.ToString();
                    }
                    else if (parm.EndsWith("creatureStartLocationY"))
                    {
                        return gv.mod.moduleEncountersList[indexNum].encounterCreatureRefsList[indexNum2].creatureStartLocationY.ToString();
                    }
                }

                #endregion*/



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
                else if (parm.StartsWith("#numCreaturesInCurrentEncounter"))
                {
                    return gv.mod.currentEncounter.encounterCreatureList.Count.ToString();
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
