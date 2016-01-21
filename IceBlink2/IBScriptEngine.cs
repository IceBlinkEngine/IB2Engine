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
            if (parms != "fullScreenEffectScript")
            {
                lines = File.ReadAllLines(gv.cc.GetModulePath() + "\\ibscript\\" + filename + ".ibs");
            }
            else
            {
                lines = File.ReadAllLines(gv.cc.GetModulePath() + "\\ibscript\\" + "\\fullScreenEffectScripts\\" + filename + ".ibs");
            }
            List<string> converttolist = lines.ToList();
            converttolist.Insert(0, "//line 0");
            lines = converttolist.ToArray();

            //set-up Block lists
            fillForBlocksList();
            fillIfBlocksList();

            //convert the parms into a List<String> by comma delimination and remove white space
            if (parms != "fullScreenEffectScript")
            {
                parmsList = parms.Split(',').Select(x => x.Trim()).ToList();
            }
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

            int indexNum = 0;
            int indexNum2 = 0;
            int indexNum3 = 0;
            int indexNum4 = 0;

            
                //get index of object in its List
                string index = GetBetween(element[0], '[', ']');
            if (index != element[0])
            {
                string indexReplaced = ReplaceParameter(index);
                indexNum = (int)Convert.ToDouble(indexReplaced);
            }
           

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
            else if (element[0].StartsWith("%CurrentArea"))
            {
                CurrentAreaAssignment(element);
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
                {
                    gv.mod.allowAutosave = true;
                }
                else
                {
                    gv.mod.allowAutosave = false;
                }
            }
            else if (element[0].EndsWith("currentWeatherDuration"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentWeatherDuration = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentWeatherDuration += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentWeatherDuration -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentWeatherDuration *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentWeatherDuration;
                    helpResult /= val;
                    gv.mod.currentWeatherDuration = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentWeatherDuration %= val;
                }
            }
            else if (element[0].EndsWith("useFirstPartOfWeatherScript"))
            {

                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.useFirstPartOfWeatherScript = true;
                }
                else
                {
                    gv.mod.useFirstPartOfWeatherScript = false;
                }
            }
            else if (element[0].EndsWith("currentWeatherName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentWeatherName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentWeatherName += val;
                }
            }
            else if (element[0].EndsWith("longEntryWeathersList"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.longEntryWeathersList = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.longEntryWeathersList += val;
                }
            }

        }
        public void PropAssignment(string[] element, int indexNum, int indexNum2)
        {
            if (element[0].EndsWith("isShown"))
            {
                
                string val = ConcateString(element[2]);
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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

            #region full screen layer 1
            else if (element[0].EndsWith("useFullScreenEffectLayer1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer1 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer1 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive1 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive1 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders1 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders1 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer1IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer1IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer1IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging1 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging1 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade1 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade1 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript1 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript1 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName1"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName1 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride1"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride1 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource1"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource1 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed1 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX1 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY1 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter1 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter1 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit1 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter1 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames1 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX1 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY1 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX1 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY1 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY1 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur1 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur1 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence1 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence1 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter1 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter1 %= val;
                }
            }

            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel1 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel1 %= val;
                }
            }

            else if (element[0].EndsWith("overrideDelayLimit1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit1 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit1 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter1;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter1 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter1 %= val;
                }
            }
            #endregion
            #region full screen layer 2
            else if (element[0].EndsWith("useFullScreenEffectLayer2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer2 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer2 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive2 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive2 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders2 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders2 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer2IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer2IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer2IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging2 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging2 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade2 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade2 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript2 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript2 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName2"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName2 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride2"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride2 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource2"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource2 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed2 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX2 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY2 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter2 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter2 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit2 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter2 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames2 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX2 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY2 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX2 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY2 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY2 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur2 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur2 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence2 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence2 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter2 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter2 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel2 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel2 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit2 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit2 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter2;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter2 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter2 %= val;
                }
            }
            #endregion
            #region full screen layer 3
            else if (element[0].EndsWith("useFullScreenEffectLayer3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer3 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer3 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive3 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive3 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders3 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders3 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer3IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer3IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer3IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging3 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging3 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade3 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade3 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript3 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript3 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName3"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName3 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride3"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride3 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource3"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource3 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed3 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX3 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY3 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter3 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter3 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit3 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter3 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames3 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX3 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY3 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX3 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY3 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur3 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence3 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter3 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel3 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit3 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter3;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter3 %= val;
                }
            }
            #endregion
            #region full screen layer 4
            else if (element[0].EndsWith("useFullScreenEffectLayer4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer4 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer4 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive4 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive4 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders4 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders4 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer4IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer4IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer4IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging4 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging4 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade4 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade4 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript4 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript4 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName4"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName4 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride4"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride4 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource4"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource4 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY4 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter4 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter4 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit4 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter4 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY4 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX4 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY4 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur4 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter4 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel4 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit4 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter4;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter4 %= val;
                }
            }
            #endregion
            #region full screen layer 5
            else if (element[0].EndsWith("useFullScreenEffectLayer5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer5 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer5 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive5 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive5 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders5 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders5 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer5IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer5IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer5IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging5 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging5 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade5 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade5 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript5 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript5 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName5"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName5 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride5"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride5 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource5"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource5 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY5 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter5 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter5 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit5 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter5 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY5 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX5 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY5 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur5 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter5 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel5 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit5 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter5;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter5 %= val;
                }
            }
            #endregion
            #region full screen layer 6
            else if (element[0].EndsWith("useFullScreenEffectLayer6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer6 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer6 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive6 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive6 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders6 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders6 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer6IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer6IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer6IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging6 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging6 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade6 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade6 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript6 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript6 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName6"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName6 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride6"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride6 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource6"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource6 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY6 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter6 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter6 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit6 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter6 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY6 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX6 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY6 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur6 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter6 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel6 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit6 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter6;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter6 %= val;
                }
            }
            #endregion
            #region full screen layer 7
            else if (element[0].EndsWith("useFullScreenEffectLayer7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer7 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer7 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive7 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive7 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders7 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders7 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer7IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer7IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer7IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging7 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging7 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade7 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade7 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript7 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript7 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName7"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName7 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride7"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride7 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource7"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource7 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY7 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter7 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter7 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit7 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter7 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY7 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX7 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY7 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur7 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter7 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel7 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit7 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter7;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter7 %= val;
                }
            }
            #endregion
            #region full screen layer 8
            else if (element[0].EndsWith("useFullScreenEffectLayer8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer8 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer8 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive8 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive8 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders8 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders8 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer8IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer8IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer8IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging8 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging8 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade8 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade8 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript8 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript8 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName8"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName8 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride8"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride8 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource8"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource8 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY8 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter8 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter8 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit8 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter8 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY8 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX8 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY8 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur8 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter8 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel8 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit8 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter8;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter8 %= val;
                }
            }
            #endregion
            #region full screen layer 9
            else if (element[0].EndsWith("useFullScreenEffectLayer9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer9 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer9 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive9 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive9 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders9 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders9 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer9IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer9IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer9IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging9 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging9 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade9 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade9 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript9 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript9 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName9"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName9 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride9"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride9 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource9"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource9 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed9 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX9 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY9 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter9 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter9 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit9 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter9 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames9 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX9 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY9 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX9 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY9 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur9 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence9 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter9 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel9 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit9 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter9;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter9 %= val;
                }
            }
            #endregion
            #region full screen layer 10
            else if (element[0].EndsWith("useFullScreenEffectLayer10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer10 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer10 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive10 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerIsActive10 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders10 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders10 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer10IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer10IsTop = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer10IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging10 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].isChanging10 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade10 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].useCyclicFade10 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript10 = true;
                }
                else
                {
                    gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript10 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName10"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName10 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride10"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].directionalOverride10 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource10"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource10 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY10 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].cycleCounter10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].cycleCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeCounter10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeCounter10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeLimit10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeLimit10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeLimit10 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeFrameCounter10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeFrameCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY10 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedX10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedX10 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideSpeedY10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideSpeedY10 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur10 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel10 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit10 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter10;
                    helpResult /= val;
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("effectChannelScript1"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].effectChannelScript1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].effectChannelScript1 += val;
                }
            }
            else if (element[0].EndsWith("effectChannelScript2"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].effectChannelScript2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].effectChannelScript2 += val;
                }
            }
            else if (element[0].EndsWith("effectChannelScript3"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].effectChannelScript3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].effectChannelScript3 += val;
                }
            }
            else if (element[0].EndsWith("effectChannelScript4"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.moduleAreasObjects[indexNum].effectChannelScript4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.moduleAreasObjects[indexNum].effectChannelScript4 += val;
                }
            }
            #endregion
        }

        public void CurrentAreaAssignment(string[] element)
        {
            if (element[0].EndsWith("TimePerSquare"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.TimePerSquare = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.TimePerSquare += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.TimePerSquare -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.TimePerSquare *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.TimePerSquare;
                    helpResult /= val;
                    gv.mod.currentArea.TimePerSquare = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.TimePerSquare %= val;
                }
            }

            else if (element[0].EndsWith("MapSizeX"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.MapSizeX = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.MapSizeX += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.MapSizeX -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.MapSizeX *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.MapSizeX;
                    helpResult /= val;
                    gv.mod.currentArea.MapSizeX = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.MapSizeX %= val;
                }
            }

            else if (element[0].EndsWith("MapSizeY"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.MapSizeY = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.MapSizeY += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.MapSizeY -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.MapSizeY *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.MapSizeY;
                    helpResult /= val;
                    gv.mod.currentArea.MapSizeY = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.MapSizeY %= val;
                }
            }

            else if (element[0].EndsWith("UseMiniMapFogOfWar"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.UseMiniMapFogOfWar = true;
                }
                else
                {
                    gv.mod.currentArea.UseMiniMapFogOfWar = false;
                }
            }

            else if (element[0].EndsWith("areaDark"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.areaDark = true;
                }
                else
                {
                    gv.mod.currentArea.areaDark = false;
                }
            }

            else if (element[0].EndsWith("UseDayNightCycle"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.UseDayNightCycle = true;
                }
                else
                {
                    gv.mod.currentArea.UseDayNightCycle = false;
                }
            }

            else if (element[0].EndsWith("Filename"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.Filename = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.Filename += val;
                }
            }

            else if (element[0].EndsWith("MusicFileName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.MusicFileName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.MusicFileName += val;
                }
            }

            else if (element[0].EndsWith("ImageFileName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.ImageFileName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.ImageFileName += val;
                }
            }

            else if (element[0].EndsWith("AreaMusic"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.AreaMusic = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.AreaMusic += val;
                }
            }

            else if (element[0].EndsWith("AreaSounds"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.AreaSounds = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.AreaSounds += val;
                }
            }

            /*else if (element[0].EndsWith("OnHeartBeatLogicTree"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.OnHeartBeatLogicTree = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.OnHeartBeatLogicTree += val;
                }
            }

            else if (element[0].EndsWith("OnHeartBeatParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.OnHeartBeatParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.OnHeartBeatParms += val;
                }
            }*/

            else if (element[0].EndsWith("OnHeartBeatIBScript"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.OnHeartBeatIBScript = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.OnHeartBeatIBScript += val;
                }
            }

            else if (element[0].EndsWith("OnHeartBeatIBScriptParms"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.OnHeartBeatIBScriptParms = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.OnHeartBeatIBScriptParms += val;
                }
            }
            else if (element[0].EndsWith("inGameAreaName"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.inGameAreaName = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.inGameAreaName += val;
                }
            }
            #region full screen layer 1
            else if (element[0].EndsWith("useFullScreenEffectLayer1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer1= true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer1= false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive1= true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive1= false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders1= true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders1= false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer1IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer1IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer1IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging1= true;
                }
                else
                {
                    gv.mod.currentArea.isChanging1= false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade1= true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade1= false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript1"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript1= true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript1= false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName1"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName1+= val;
                }
            }
            else if (element[0].EndsWith("directionalOverride1"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride1+= val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource1"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource1+= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed1;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed1%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX1;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX1%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY1;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY1%= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter1;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter1%= val;
                }
            }
            else if (element[0].EndsWith("changeCounter1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter1;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter1%= val;
                }
            }
            else if (element[0].EndsWith("changeLimit1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit1;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit1%= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter1;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter1%= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames1;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames1%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX1;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX1%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY1;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY1%= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX1;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX1%= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY1"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY1;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY1= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY1%= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur1;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur1= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur1%= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence1;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence1= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence1%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter1;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter1= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter1%= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit1;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit1= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit1%= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter1"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter1= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter1+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter1-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter1*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter1;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter1= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter1%= val;
                }
            }
            #endregion
            #region full screen layer 2
            else if (element[0].EndsWith("useFullScreenEffectLayer2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer2= true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer2= false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive2= true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive2= false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders2= true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders2= false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer2IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer2IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer2IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging2= true;
                }
                else
                {
                    gv.mod.currentArea.isChanging2= false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade2= true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade2= false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript2"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript2= true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript2= false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName2"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName2+= val;
                }
            }
            else if (element[0].EndsWith("directionalOverride2"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride2+= val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource2"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource2+= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed2;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed2%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX2;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX2%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY2;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY2%= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter2;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter2%= val;
                }
            }
            else if (element[0].EndsWith("changeCounter2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter2;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter2%= val;
                }
            }
            else if (element[0].EndsWith("changeLimit2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit2;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit2%= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter2;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter2%= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames2;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames2%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX2;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX2%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY2;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY2%= val;
                }
            }
            
            else if (element[0].EndsWith("overrideSpeedX2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX2;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX2%= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY2"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY2;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY2= helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY2%= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur2;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur2= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur2%= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence2;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence2= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence2%= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter2;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter2= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter2%= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2 %= val;
                }
            }

            else if (element[0].EndsWith("overrideDelayLimit2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit2;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit2= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit2%= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter2"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter2= val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter2+= val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter2-= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter2*= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter2;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter2= (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter2%= val;
                }
            }
            #endregion
            #region full screen layer 3
            else if (element[0].EndsWith("useFullScreenEffectLayer3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer3 = true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer3 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive3 = true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive3 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders3 = true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders3 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer3IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer3IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer3IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging3 = true;
                }
                else
                {
                    gv.mod.currentArea.isChanging3 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade3 = true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade3 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript3"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript3 = true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript3 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName3"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName3 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride3"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride3 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource3"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource3 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed3;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed3 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX3;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX3 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY3;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY3 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter3;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter3 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter3;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter3 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit3;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit3 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter3;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter3 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames3;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames3 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX3;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX3 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY3;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY3 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX3;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX3 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY3"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY3;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY3 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY3 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur3;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur3 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence3;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence3 %= val;
                }
            }
            
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter3;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter3 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit3;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit3 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter3"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter3 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter3 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter3 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter3;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter3 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter3 %= val;
                }
            }
            #endregion
            #region full screen layer 4
            else if (element[0].EndsWith("useFullScreenEffectLayer4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer4 = true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer4 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive4 = true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive4 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders4 = true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders4 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer4IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer4IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer4IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging4 = true;
                }
                else
                {
                    gv.mod.currentArea.isChanging4 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade4 = true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade4 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript4"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript4 = true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript4 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName4"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName4 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride4"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride4 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource4"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource4 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed4;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX4;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY4;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY4 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter4;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter4 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter4;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter4 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit4;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit4 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter4;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter4 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames4;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX4;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY4;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY4 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX4;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX4 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY4"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY4;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY4 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY4 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur4;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur4 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence4;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence4 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter4;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter4 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit4;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit4 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter4"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter4 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter4 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter4 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter4;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter4 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter4 %= val;
                }
            }
            #endregion
            #region full screen layer 5
            else if (element[0].EndsWith("useFullScreenEffectLayer5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer5 = true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer5 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive5 = true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive5 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders5 = true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders5 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer5IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer5IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer5IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging5 = true;
                }
                else
                {
                    gv.mod.currentArea.isChanging5 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade5 = true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade5 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript5"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript5 = true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript5 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName5"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName5 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride5"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride5 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource5"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource5 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed5;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX5;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY5;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY5 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter5;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter5 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter5;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter5 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit5;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit5 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter5;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter5 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames5;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX5;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY5;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY5 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX5;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX5 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY5"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY5 = val;
                    
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY5;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY5 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY5 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur5;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur5 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence5;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence5 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter5;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter5 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit5;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit5 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter5"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter5 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter5 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter5 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter5 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter5;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter5 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter5 %= val;
                }
            }
            #endregion
            #region full screen layer 6
            else if (element[0].EndsWith("useFullScreenEffectLayer6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer6 = true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer6 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive6 = true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive6 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders6 = true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders6 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer6IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer6IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer6IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging6 = true;
                }
                else
                {
                    gv.mod.currentArea.isChanging6 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade6 = true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade6 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript6"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript6 = true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript6 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName6"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName6 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride6"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride6 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource6"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource6 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed6;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX6;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY6;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY6 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter6;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter6 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter6;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter6 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit6;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit6 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter6;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter6 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames6;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX6;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY6;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY6 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX6;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX6 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY6"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY6;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY6 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY6 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur6;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur6 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence6;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence6 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter6;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter6 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit6;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit6 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter6"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter6 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter6 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter6 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter6 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter6;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter6 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter6 %= val;
                }
            }
            #endregion
            #region full screen layer 7
            else if (element[0].EndsWith("useFullScreenEffectLayer7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer7 = true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer7 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive7 = true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive7 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders7 = true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders7 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer7IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer7IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer7IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging7 = true;
                }
                else
                {
                    gv.mod.currentArea.isChanging7 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade7 = true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade7 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript7"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript7 = true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript7 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName7"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName7 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride7"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride7 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource7"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource7 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed7;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX7;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY7;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY7 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter7;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter7 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter7;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter7 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit7;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit7 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter7;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter7 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames7;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX7;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY7;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY7 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX7;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX7 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY7"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY7;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY7 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY7 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur7;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur7 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence7;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence7 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter7;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter7 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit7;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit7 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter7"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter7 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter7 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter7 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter7 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter7;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter7 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter7 %= val;
                }
            }
            #endregion
            #region full screen layer 8
            else if (element[0].EndsWith("useFullScreenEffectLayer8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer8 = true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer8 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive8 = true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive8 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders8 = true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders8 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer8IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer8IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer8IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging8 = true;
                }
                else
                {
                    gv.mod.currentArea.isChanging8 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade8 = true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade8 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript8"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript8 = true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript8 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName8"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName8 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride8"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride8 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource8"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource8 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed8;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX8;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY8;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY8 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter8;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter8 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter8;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter8 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit8;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit8 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter8;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter8 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames8;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX8;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY8;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY8 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX8;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX8 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY8"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY8;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY8 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY8 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur8;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur8 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence8;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence8 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter8;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter8 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit8;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit8 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter8"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter8 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter8 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter8 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter8 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter8;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter8 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter8 %= val;
                }
            }
            #endregion
            #region full screen layer 9
            else if (element[0].EndsWith("useFullScreenEffectLayer9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer9 = true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer9 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive9 = true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive9 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders9 = true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders9 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer9IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer9IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer9IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging9 = true;
                }
                else
                {
                    gv.mod.currentArea.isChanging9 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade9 = true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade9 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript9"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript9 = true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript9 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName9"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName9 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride9"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride9 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource9"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource9 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed9;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed9 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX9;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX9 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY9;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY9 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter9;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter9 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter9;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter9 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit9;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit9 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter9;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter9 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames9;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames9 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX9;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX9 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY9;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY9 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX9;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX9 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY9"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY9;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY9 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY9 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur9;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur9 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence9;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence9 %= val;
                }
            }
            
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter9;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter9 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit9;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit9 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter9"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter9 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter9 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter9 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter9 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter9;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter9 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter9 %= val;
                }
            }
            #endregion
            #region full screen layer 10
            else if (element[0].EndsWith("useFullScreenEffectLayer10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useFullScreenEffectLayer10 = true;
                }
                else
                {
                    gv.mod.currentArea.useFullScreenEffectLayer10 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerIsActive10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive10 = true;
                }
                else
                {
                    gv.mod.currentArea.fullScreenEffectLayerIsActive10 = false;
                }
            }
            else if (element[0].EndsWith("containEffectInsideAreaBorders10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders10 = true;
                }
                else
                {
                    gv.mod.currentArea.containEffectInsideAreaBorders10 = false;
                }
            }
            else if (element[0].EndsWith("FullScreenEffectLayer10IsTop"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.FullScreenEffectLayer10IsTop = true;
                }
                else
                {
                    gv.mod.currentArea.FullScreenEffectLayer10IsTop = false;
                }
            }
            else if (element[0].EndsWith("isChanging10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.isChanging10 = true;
                }
                else
                {
                    gv.mod.currentArea.isChanging10 = false;
                }
            }
            else if (element[0].EndsWith("useCyclicFade10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.useCyclicFade10 = true;
                }
                else
                {
                    gv.mod.currentArea.useCyclicFade10 = false;
                }
            }
            else if (element[0].EndsWith("changeableByWeatherScript10"))
            {
                string val = ConcateString(element[2]);
                if (val == "True")
                {
                    gv.mod.currentArea.changeableByWeatherScript10 = true;
                }
                else
                {
                    gv.mod.currentArea.changeableByWeatherScript10 = false;
                }
            }
            else if (element[0].EndsWith("fullScreenEffectLayerName10"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectLayerName10 += val;
                }
            }
            else if (element[0].EndsWith("directionalOverride10"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.directionalOverride10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.directionalOverride10 += val;
                }
            }
            else if (element[0].EndsWith("overrideIsNoScrollSource10"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideIsNoScrollSource10 += val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeed10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeed10;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeed10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeed10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedX10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedX10;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedX10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedX10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationSpeedY10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationSpeedY10;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationSpeedY10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationSpeedY10 %= val;
                }
            }
            else if (element[0].EndsWith("cycleCounter10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.cycleCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.cycleCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.cycleCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.cycleCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.cycleCounter10;
                    helpResult /= val;
                    gv.mod.currentArea.cycleCounter10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.cycleCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("changeCounter10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeCounter10;
                    helpResult /= val;
                    gv.mod.currentArea.changeCounter10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("changeLimit10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeLimit10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeLimit10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeLimit10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeLimit10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeLimit10;
                    helpResult /= val;
                    gv.mod.currentArea.changeLimit10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeLimit10 %= val;
                }
            }
            else if (element[0].EndsWith("changeFrameCounter10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeFrameCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeFrameCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeFrameCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeFrameCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeFrameCounter10;
                    helpResult /= val;
                    gv.mod.currentArea.changeFrameCounter10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeFrameCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("changeNumberOfFrames10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.changeNumberOfFrames10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.changeNumberOfFrames10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.changeNumberOfFrames10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.changeNumberOfFrames10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.changeNumberOfFrames10;
                    helpResult /= val;
                    gv.mod.currentArea.changeNumberOfFrames10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.changeNumberOfFrames10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterX10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterX10;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterX10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounterY10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounterY10;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounterY10 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedX10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedX10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedX10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedX10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedX10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedX10;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedX10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedX10 %= val;
                }
            }
            else if (element[0].EndsWith("overrideSpeedY10"))
            {
                float val = CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideSpeedY10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideSpeedY10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideSpeedY10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideSpeedY10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideSpeedY10;
                    helpResult /= val;
                    gv.mod.currentArea.overrideSpeedY10 = helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideSpeedY10 %= val;
                }
            }

            else if (element[0].EndsWith("fullScreenEffectChanceToOccur10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenEffectChanceToOccur10;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenEffectChanceToOccur10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenEffectChanceToOccur10 %= val;
                }
            }
            else if (element[0].EndsWith("numberOfCyclesPerOccurence10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.numberOfCyclesPerOccurence10;
                    helpResult /= val;
                    gv.mod.currentArea.numberOfCyclesPerOccurence10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.numberOfCyclesPerOccurence10 %= val;
                }
            }
            else if (element[0].EndsWith("fullScreenAnimationFrameCounter10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.fullScreenAnimationFrameCounter10;
                    helpResult /= val;
                    gv.mod.currentArea.fullScreenAnimationFrameCounter10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.fullScreenAnimationFrameCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("activateTargetChannelInParallelToThisChannel10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10;
                    helpResult /= val;
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayLimit10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayLimit10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayLimit10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayLimit10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayLimit10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayLimit10;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayLimit10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayLimit10 %= val;
                }
            }
            else if (element[0].EndsWith("overrideDelayCounter10"))
            {
                int val = (int)CalcualteNumberEquation(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.overrideDelayCounter10 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.overrideDelayCounter10 += val;
                }
                else if (element[1] == "-=")
                {
                    gv.mod.currentArea.overrideDelayCounter10 -= val;
                }
                else if (element[1] == "*=")
                {
                    gv.mod.currentArea.overrideDelayCounter10 *= val;
                }
                else if (element[1] == "/=")
                {
                    helpResult = gv.mod.currentArea.overrideDelayCounter10;
                    helpResult /= val;
                    gv.mod.currentArea.overrideDelayCounter10 = (int)helpResult;
                }
                else if (element[1] == "./.=")
                {
                    gv.mod.currentArea.overrideDelayCounter10 %= val;
                }
            }
            else if (element[0].EndsWith("effectChannelScript1"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.effectChannelScript1 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.effectChannelScript1 += val;
                }
            }
            else if (element[0].EndsWith("effectChannelScript2"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.effectChannelScript2 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.effectChannelScript2 += val;
                }
            }
            else if (element[0].EndsWith("effectChannelScript3"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.effectChannelScript3 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.effectChannelScript3 += val;
                }
            }
            else if (element[0].EndsWith("effectChannelScript4"))
            {
                string val = ConcateString(element[2]);
                if (element[1] == "=")
                {
                    gv.mod.currentArea.effectChannelScript4 = val;
                }
                else if (element[1] == "+=")
                {
                    gv.mod.currentArea.effectChannelScript4 += val;
                }
            }
            #endregion

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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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
                if (val == "True")
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

                int indexNum = 0;
                int indexNum2 = 0;
                int indexNum3 = 0;
                int indexNum4 = 0;

                if (index != parm)
                {
                    string indexTrimmed = index.Trim();
                    string indexReplaced = ReplaceParameter(indexTrimmed);
                    indexNum = (int)Convert.ToDouble(indexReplaced);
                }

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
                    
                    #region full screen effect layer 1
                    else if (parm.EndsWith("useFullScreenEffectLayer1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY1.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer1IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer1IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur1.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence1.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter1.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders1.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging1.ToString();
                    }
                    else if (parm.EndsWith("changeCounter1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter1.ToString();
                    }
                    else if (parm.EndsWith("changeLimit1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit1.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter1.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames1.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter1.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel1.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride1.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX1.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY1.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit1.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter1.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource1.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript1.ToString();
                    }
                    #endregion
                    #region full screen effect layer 2
                    else if (parm.EndsWith("useFullScreenEffectLayer2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY2.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer2IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer2IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur2.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence2.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter2.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders2.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging2.ToString();
                    }
                    else if (parm.EndsWith("changeCounter2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter2.ToString();
                    }
                    else if (parm.EndsWith("changeLimit2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit2.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter2.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames2.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter2.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel2.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride2.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX2.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY2.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit2.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter2.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource2.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript2.ToString();
                    }
                    #endregion
                    #region full screen effect layer 3
                    else if (parm.EndsWith("useFullScreenEffectLayer3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY3.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer3IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer3IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur3.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence3.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter3.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders3.ToString();
                    }

                    else if (parm.EndsWith("isChanging3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging3.ToString();
                    }
                    else if (parm.EndsWith("changeCounter3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter3.ToString();
                    }
                    else if (parm.EndsWith("changeLimit3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit3.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter3.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames3.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter3.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel3.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride3.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX3.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY3.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit3.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter3.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource3.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript3.ToString();
                    }
                    #endregion
                    #region full screen effect layer 4
                    else if (parm.EndsWith("useFullScreenEffectLayer4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY4.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer4IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer4IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur4.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence4.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter4.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders4.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging4.ToString();
                    }
                    else if (parm.EndsWith("changeCounter4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter4.ToString();
                    }
                    else if (parm.EndsWith("changeLimit4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit4.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter4.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames4.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter4.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel4.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride4.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX4.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY4.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit4.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter4.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource4.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript4.ToString();
                    }
                    #endregion
                    #region full screen effect layer 5
                    else if (parm.EndsWith("useFullScreenEffectLayer5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY5.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer5IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer5IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur5.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence5.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter5.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders5.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging5.ToString();
                    }
                    else if (parm.EndsWith("changeCounter5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter5.ToString();
                    }
                    else if (parm.EndsWith("changeLimit5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit5.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter5.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames5.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter5.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel5.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride5.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX5.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY5.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit5.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter5.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource5.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript5"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript5.ToString();
                    }
                    #endregion
                    #region full screen effect layer 6
                    else if (parm.EndsWith("useFullScreenEffectLayer6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY6.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer6IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer6IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur6.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence6.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter6.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders6.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging6.ToString();
                    }
                    else if (parm.EndsWith("changeCounter6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter6.ToString();
                    }
                    else if (parm.EndsWith("changeLimit6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit6.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter6.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames6.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter6.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel6.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride6.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX6.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY6.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit6.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter6.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource6.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript6"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript6.ToString();
                    }
                    #endregion
                    #region full screen effect layer 7
                    else if (parm.EndsWith("useFullScreenEffectLayer7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY7.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer7IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer7IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur7.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence7.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter7.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders7.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging7.ToString();
                    }
                    else if (parm.EndsWith("changeCounter7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter7.ToString();
                    }
                    else if (parm.EndsWith("changeLimit7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit7.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter7.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames7.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter7.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel7.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride7.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX7.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY7.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit7.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter7.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource7.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript7"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript7.ToString();
                    }
                    #endregion
                    #region full screen effect layer 8
                    else if (parm.EndsWith("useFullScreenEffectLayer8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY8.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer8IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer8IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur8.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence8.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter8.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders8.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging8.ToString();
                    }
                    else if (parm.EndsWith("changeCounter8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter8.ToString();
                    }
                    else if (parm.EndsWith("changeLimit8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit8.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter8.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames8.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter8.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel8.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride8.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX8.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY8.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit8.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter8.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource8.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript8"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript8.ToString();
                    }
                    #endregion
                    #region full screen effect layer 9
                    else if (parm.EndsWith("useFullScreenEffectLayer9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY9.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer9IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer9IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur9.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence9.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter9.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders9.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging9.ToString();
                    }
                    else if (parm.EndsWith("changeCounter9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter9.ToString();
                    }
                    else if (parm.EndsWith("changeLimit9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit9.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter9.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames9.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter9.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel9.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride9.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX9.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY9.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit9.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter9.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource9.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript9"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript9.ToString();
                    }
                    #endregion
                    #region full screen effect layer 100
                    else if (parm.EndsWith("useFullScreenEffectLayer10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useFullScreenEffectLayer10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectLayerName10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeed10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedX10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationSpeedY10.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer10IsTop"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].FullScreenEffectLayer10IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenEffectChanceToOccur10.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].numberOfCyclesPerOccurence10.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].cycleCounter10.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].containEffectInsideAreaBorders10.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].isChanging10.ToString();
                    }
                    else if (parm.EndsWith("changeCounter10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeCounter10.ToString();
                    }
                    else if (parm.EndsWith("changeLimit10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeLimit10.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeFrameCounter10.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeNumberOfFrames10.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].useCyclicFade10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterX10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounterY10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].fullScreenAnimationFrameCounter10.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].activateTargetChannelInParallelToThisChannel10.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].directionalOverride10.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedX10.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideSpeedY10.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayLimit10.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideDelayCounter10.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].overrideIsNoScrollSource10.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript10"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].changeableByWeatherScript10.ToString();
                    }
                    else if (parm.EndsWith("effectChannelScript1"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].effectChannelScript1.ToString();
                    }
                    else if (parm.EndsWith("effectChannelScript2"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].effectChannelScript2.ToString();
                    }
                    else if (parm.EndsWith("effectChannelScript3"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].effectChannelScript3.ToString();
                    }
                    else if (parm.EndsWith("effectChannelScript4"))
                    {
                        return gv.mod.moduleAreasObjects[indexNum].effectChannelScript4.ToString();
                    }
                    #endregion
                }

                #endregion

                #region currentArea
                else if (parm.StartsWith("%CurrentArea"))
                {
                    if (parm.EndsWith("SizeOfProps"))
                    {
                        return gv.mod.currentArea.Props.Count.ToString();
                    }
                    else if (parm.EndsWith("effectChannelScript1"))
                    {
                        return gv.mod.currentArea.effectChannelScript1.ToString();
                    }
                    else if (parm.EndsWith("effectChannelScript2"))
                    {
                        return gv.mod.currentArea.effectChannelScript2.ToString();
                    }
                    else if (parm.EndsWith("effectChannelScript3"))
                    {
                        return gv.mod.currentArea.effectChannelScript3.ToString();
                    }
                    else if (parm.EndsWith("effectChannelScript4"))
                    {
                        return gv.mod.currentArea.effectChannelScript4.ToString();
                    }
                    else if (parm.EndsWith("Filename"))
                    {
                        return gv.mod.currentArea.Filename.ToString();
                    }
                    else if (parm.EndsWith("UseMiniMapFogOfWar"))
                    {
                        return gv.mod.currentArea.UseMiniMapFogOfWar.ToString();
                    }
                    else if (parm.EndsWith("areaDark"))
                    {
                        return gv.mod.currentArea.areaDark.ToString();
                    }
                    else if (parm.EndsWith("UseDayNightCycle"))
                    {
                        return gv.mod.currentArea.UseDayNightCycle.ToString();
                    }
                    else if (parm.EndsWith("TimePerSquare"))
                    {
                        return gv.mod.currentArea.TimePerSquare.ToString();
                    }
                    else if (parm.EndsWith("MusicFileName"))
                    {
                        return gv.mod.currentArea.MusicFileName.ToString();
                    }
                    else if (parm.EndsWith("ImageFileName"))
                    {
                        return gv.mod.currentArea.ImageFileName.ToString();
                    }
                    else if (parm.EndsWith("MapSizeX"))
                    {
                        return gv.mod.currentArea.MapSizeX.ToString();
                    }
                    else if (parm.EndsWith("MapSizeY"))
                    {
                        return gv.mod.currentArea.MapSizeY.ToString();
                    }
                    else if (parm.EndsWith("AreaMusic"))
                    {
                        return gv.mod.currentArea.AreaMusic.ToString();
                    }
                    else if (parm.EndsWith("AreaSounds"))
                    {
                        return gv.mod.currentArea.AreaSounds.ToString();
                    }
                    /*else if (parm.EndsWith("OnHeartBeatLogicTree"))
                    {
                        return gv.mod.currentArea.OnHeartBeatLogicTree.ToString();
                    }
                    else if (parm.EndsWith("OnHeartBeatParms"))
                    {
                        return gv.mod.currentArea.OnHeartBeatParms.ToString();
                    }*/
                    else if (parm.EndsWith("SizeOfTriggers"))
                    {
                        return gv.mod.currentArea.Triggers.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOFAreaLocalInts"))
                    {
                        return gv.mod.currentArea.AreaLocalInts.Count.ToString();
                    }
                    else if (parm.EndsWith("SizeOfAreaLocalStrings"))
                    {
                        return gv.mod.currentArea.AreaLocalStrings.Count.ToString();
                    }
                    else if (parm.EndsWith("OnHeartBeatIBScript"))
                    {
                        return gv.mod.currentArea.OnHeartBeatIBScript.ToString();
                    }
                    else if (parm.EndsWith("OnHeartBeatIBScriptParms"))
                    {
                        return gv.mod.currentArea.OnHeartBeatIBScriptParms.ToString();
                    }
                    else if (parm.EndsWith("inGameAreaName"))
                    {
                        return gv.mod.currentArea.inGameAreaName.ToString();
                    }

                    #region full screen effect layer 1
                    else if (parm.EndsWith("useFullScreenEffectLayer1"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName1"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed1"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX1"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY1"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY1.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer1IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer1IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur1"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur1.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence1"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence1.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter1"))
                    {
                        return gv.mod.currentArea.cycleCounter1.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders1"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders1.ToString();
                    }
                   
                    else if (parm.EndsWith("isChanging1"))
                    {
                        return gv.mod.currentArea.isChanging1.ToString();
                    }
                    else if (parm.EndsWith("changeCounter1"))
                    {
                        return gv.mod.currentArea.changeCounter1.ToString();
                    }
                    else if (parm.EndsWith("changeLimit1"))
                    {
                        return gv.mod.currentArea.changeLimit1.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter1"))
                    {
                        return gv.mod.currentArea.changeFrameCounter1.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames1"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames1.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade1"))
                    {
                        return gv.mod.currentArea.useCyclicFade1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX1"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY1"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY1.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter1"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter1.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel1"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel1.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride1"))
                    {
                        return gv.mod.currentArea.directionalOverride1.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX1"))
                    {
                        return gv.mod.currentArea.overrideSpeedX1.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY1"))
                    {
                        return gv.mod.currentArea.overrideSpeedY1.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit1"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit1.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter1"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter1.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource1"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource1.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript1"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript1.ToString();
                    }
                    #endregion
                    #region full screen effect layer 2
                    else if (parm.EndsWith("useFullScreenEffectLayer2"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName2"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed2"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX2"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY2"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY2.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer2IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer2IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur2"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur2.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence2"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence2.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter2"))
                    {
                        return gv.mod.currentArea.cycleCounter2.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders2"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders2.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging2"))
                    {
                        return gv.mod.currentArea.isChanging2.ToString();
                    }
                    else if (parm.EndsWith("changeCounter2"))
                    {
                        return gv.mod.currentArea.changeCounter2.ToString();
                    }
                    else if (parm.EndsWith("changeLimit2"))
                    {
                        return gv.mod.currentArea.changeLimit2.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter2"))
                    {
                        return gv.mod.currentArea.changeFrameCounter2.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames2"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames2.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade2"))
                    {
                        return gv.mod.currentArea.useCyclicFade2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX2"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY2"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY2.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter2"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter2.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel2"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel2.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride2"))
                    {
                        return gv.mod.currentArea.directionalOverride2.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX2"))
                    {
                        return gv.mod.currentArea.overrideSpeedX2.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY2"))
                    {
                        return gv.mod.currentArea.overrideSpeedY2.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit2"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit2.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter2"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter2.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource2"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource2.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript2"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript2.ToString();
                    }
                    #endregion
                    #region full screen effect layer 3
                    else if (parm.EndsWith("useFullScreenEffectLayer3"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName3"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed3"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX3"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY3"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY3.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer3IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer3IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur3"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur3.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence3"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence3.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter3"))
                    {
                        return gv.mod.currentArea.cycleCounter3.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders3"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders3.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging3"))
                    {
                        return gv.mod.currentArea.isChanging3.ToString();
                    }
                    else if (parm.EndsWith("changeCounter3"))
                    {
                        return gv.mod.currentArea.changeCounter3.ToString();
                    }
                    else if (parm.EndsWith("changeLimit3"))
                    {
                        return gv.mod.currentArea.changeLimit3.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter3"))
                    {
                        return gv.mod.currentArea.changeFrameCounter3.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames3"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames3.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade3"))
                    {
                        return gv.mod.currentArea.useCyclicFade3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX3"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY3"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY3.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter3"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter3.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel3"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel3.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride3"))
                    {
                        return gv.mod.currentArea.directionalOverride3.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX3"))
                    {
                        return gv.mod.currentArea.overrideSpeedX3.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY3"))
                    {
                        return gv.mod.currentArea.overrideSpeedY3.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit3"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit3.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter3"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter3.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource3"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource3.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript3"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript3.ToString();
                    }
                    #endregion
                    #region full screen effect layer 4
                    else if (parm.EndsWith("useFullScreenEffectLayer4"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName4"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed4"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX4"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY4"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY4.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer4IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer4IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur4"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur4.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence4"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence4.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter4"))
                    {
                        return gv.mod.currentArea.cycleCounter4.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders4"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders4.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging4"))
                    {
                        return gv.mod.currentArea.isChanging4.ToString();
                    }
                    else if (parm.EndsWith("changeCounter4"))
                    {
                        return gv.mod.currentArea.changeCounter4.ToString();
                    }
                    else if (parm.EndsWith("changeLimit4"))
                    {
                        return gv.mod.currentArea.changeLimit4.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter4"))
                    {
                        return gv.mod.currentArea.changeFrameCounter4.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames4"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames4.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade4"))
                    {
                        return gv.mod.currentArea.useCyclicFade4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX4"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY4"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY4.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter4"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter4.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel4"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel4.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride4"))
                    {
                        return gv.mod.currentArea.directionalOverride4.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX4"))
                    {
                        return gv.mod.currentArea.overrideSpeedX4.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY4"))
                    {
                        return gv.mod.currentArea.overrideSpeedY4.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit4"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit4.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter4"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter4.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource4"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource4.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript4"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript4.ToString();
                    }
                    #endregion
                    #region full screen effect layer 5
                    else if (parm.EndsWith("useFullScreenEffectLayer5"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName5"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed5"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX5"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY5"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY5.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer5IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer5IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur5"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur5.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence5"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence5.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter5"))
                    {
                        return gv.mod.currentArea.cycleCounter5.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders5"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders5.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging5"))
                    {
                        return gv.mod.currentArea.isChanging5.ToString();
                    }
                    else if (parm.EndsWith("changeCounter5"))
                    {
                        return gv.mod.currentArea.changeCounter5.ToString();
                    }
                    else if (parm.EndsWith("changeLimit5"))
                    {
                        return gv.mod.currentArea.changeLimit5.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter5"))
                    {
                        return gv.mod.currentArea.changeFrameCounter5.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames5"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames5.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade5"))
                    {
                        return gv.mod.currentArea.useCyclicFade5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX5"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY5"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY5.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter5"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter5.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel5"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel5.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride5"))
                    {
                        return gv.mod.currentArea.directionalOverride5.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX5"))
                    {
                        return gv.mod.currentArea.overrideSpeedX5.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY5"))
                    {
                        return gv.mod.currentArea.overrideSpeedY5.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit5"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit5.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter5"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter5.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource5"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource5.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript5"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript5.ToString();
                    }
                    #endregion
                    #region full screen effect layer 6
                    else if (parm.EndsWith("useFullScreenEffectLayer6"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName6"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed6"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX6"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY6"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY6.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer6IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer6IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur6"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur6.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence6"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence6.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter6"))
                    {
                        return gv.mod.currentArea.cycleCounter6.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders6"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders6.ToString();
                    }
                   
                    else if (parm.EndsWith("isChanging6"))
                    {
                        return gv.mod.currentArea.isChanging6.ToString();
                    }
                    else if (parm.EndsWith("changeCounter6"))
                    {
                        return gv.mod.currentArea.changeCounter6.ToString();
                    }
                    else if (parm.EndsWith("changeLimit6"))
                    {
                        return gv.mod.currentArea.changeLimit6.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter6"))
                    {
                        return gv.mod.currentArea.changeFrameCounter6.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames6"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames6.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade6"))
                    {
                        return gv.mod.currentArea.useCyclicFade6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX6"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY6"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY6.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter6"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter6.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel6"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel6.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride6"))
                    {
                        return gv.mod.currentArea.directionalOverride6.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX6"))
                    {
                        return gv.mod.currentArea.overrideSpeedX6.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY6"))
                    {
                        return gv.mod.currentArea.overrideSpeedY6.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit6"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit6.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter6"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter6.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource6"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource6.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript6"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript6.ToString();
                    }
                    #endregion
                    #region full screen effect layer 7
                    else if (parm.EndsWith("useFullScreenEffectLayer7"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName7"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed7"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX7"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY7"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY7.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer7IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer7IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur7"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur7.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence7"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence7.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter7"))
                    {
                        return gv.mod.currentArea.cycleCounter7.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders7"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders7.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging7"))
                    {
                        return gv.mod.currentArea.isChanging7.ToString();
                    }
                    else if (parm.EndsWith("changeCounter7"))
                    {
                        return gv.mod.currentArea.changeCounter7.ToString();
                    }
                    else if (parm.EndsWith("changeLimit7"))
                    {
                        return gv.mod.currentArea.changeLimit7.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter7"))
                    {
                        return gv.mod.currentArea.changeFrameCounter7.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames7"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames7.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade7"))
                    {
                        return gv.mod.currentArea.useCyclicFade7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX7"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY7"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY7.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter7"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter7.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel7"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel7.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride7"))
                    {
                        return gv.mod.currentArea.directionalOverride7.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX7"))
                    {
                        return gv.mod.currentArea.overrideSpeedX7.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY7"))
                    {
                        return gv.mod.currentArea.overrideSpeedY7.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit7"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit7.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter7"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter7.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource7"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource7.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript7"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript7.ToString();
                    }
                    #endregion
                    #region full screen effect layer 8
                    else if (parm.EndsWith("useFullScreenEffectLayer8"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName8"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed8"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX8"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY8"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY8.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer8IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer8IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur8"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur8.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence8"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence8.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter8"))
                    {
                        return gv.mod.currentArea.cycleCounter8.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders8"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders8.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging8"))
                    {
                        return gv.mod.currentArea.isChanging8.ToString();
                    }
                    else if (parm.EndsWith("changeCounter8"))
                    {
                        return gv.mod.currentArea.changeCounter8.ToString();
                    }
                    else if (parm.EndsWith("changeLimit8"))
                    {
                        return gv.mod.currentArea.changeLimit8.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter8"))
                    {
                        return gv.mod.currentArea.changeFrameCounter8.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames8"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames8.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade8"))
                    {
                        return gv.mod.currentArea.useCyclicFade8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX8"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY8"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY8.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter8"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter8.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel8"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel8.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride8"))
                    {
                        return gv.mod.currentArea.directionalOverride8.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX8"))
                    {
                        return gv.mod.currentArea.overrideSpeedX8.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY8"))
                    {
                        return gv.mod.currentArea.overrideSpeedY8.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit8"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit8.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter8"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter8.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource8"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource8.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript8"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript8.ToString();
                    }
                    #endregion
                    #region full screen effect layer 9
                    else if (parm.EndsWith("useFullScreenEffectLayer9"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName9"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed9"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX9"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY9"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY9.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer9IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer9IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur9"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur9.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence9"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence9.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter9"))
                    {
                        return gv.mod.currentArea.cycleCounter9.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders9"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders9.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging9"))
                    {
                        return gv.mod.currentArea.isChanging9.ToString();
                    }
                    else if (parm.EndsWith("changeCounter9"))
                    {
                        return gv.mod.currentArea.changeCounter9.ToString();
                    }
                    else if (parm.EndsWith("changeLimit9"))
                    {
                        return gv.mod.currentArea.changeLimit9.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter9"))
                    {
                        return gv.mod.currentArea.changeFrameCounter9.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames9"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames9.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade9"))
                    {
                        return gv.mod.currentArea.useCyclicFade9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX9"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY9"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY9.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter9"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter9.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel9"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel9.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride9"))
                    {
                        return gv.mod.currentArea.directionalOverride9.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX9"))
                    {
                        return gv.mod.currentArea.overrideSpeedX9.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY9"))
                    {
                        return gv.mod.currentArea.overrideSpeedY9.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit9"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit9.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter9"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter9.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource9"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource9.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript9"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript9.ToString();
                    }
                    #endregion
                    #region full screen effect layer 10
                    else if (parm.EndsWith("useFullScreenEffectLayer10"))
                    {
                        return gv.mod.currentArea.useFullScreenEffectLayer10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectLayerName10"))
                    {
                        return gv.mod.currentArea.fullScreenEffectLayerName10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeed10"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeed10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedX10"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedX10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationSpeedY10"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationSpeedY10.ToString();
                    }
                    else if (parm.EndsWith("FullScreenEffectLayer10IsTop"))
                    {
                        return gv.mod.currentArea.FullScreenEffectLayer10IsTop.ToString();
                    }
                    else if (parm.EndsWith("fullScreenEffectChanceToOccur10"))
                    {
                        return gv.mod.currentArea.fullScreenEffectChanceToOccur10.ToString();
                    }
                    else if (parm.EndsWith("numberOfCyclesPerOccurence10"))
                    {
                        return gv.mod.currentArea.numberOfCyclesPerOccurence10.ToString();
                    }
                    else if (parm.EndsWith("cycleCounter10"))
                    {
                        return gv.mod.currentArea.cycleCounter10.ToString();
                    }
                    else if (parm.EndsWith("containEffectInsideAreaBorders10"))
                    {
                        return gv.mod.currentArea.containEffectInsideAreaBorders10.ToString();
                    }
                    
                    else if (parm.EndsWith("isChanging10"))
                    {
                        return gv.mod.currentArea.isChanging10.ToString();
                    }
                    else if (parm.EndsWith("changeCounter10"))
                    {
                        return gv.mod.currentArea.changeCounter10.ToString();
                    }
                    else if (parm.EndsWith("changeLimit10"))
                    {
                        return gv.mod.currentArea.changeLimit10.ToString();
                    }
                    else if (parm.EndsWith("changeFrameCounter10"))
                    {
                        return gv.mod.currentArea.changeFrameCounter10.ToString();
                    }
                    else if (parm.EndsWith("changeNumberOfFrames10"))
                    {
                        return gv.mod.currentArea.changeNumberOfFrames10.ToString();
                    }
                    else if (parm.EndsWith("useCyclicFade10"))
                    {
                        return gv.mod.currentArea.useCyclicFade10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterX10"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterX10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounterY10"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounterY10.ToString();
                    }
                    else if (parm.EndsWith("fullScreenAnimationFrameCounter10"))
                    {
                        return gv.mod.currentArea.fullScreenAnimationFrameCounter10.ToString();
                    }
                    else if (parm.EndsWith("activateTargetChannelInParallelToThisChannel10"))
                    {
                        return gv.mod.currentArea.activateTargetChannelInParallelToThisChannel10.ToString();
                    }
                    else if (parm.EndsWith("directionalOverride10"))
                    {
                        return gv.mod.currentArea.directionalOverride10.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedX10"))
                    {
                        return gv.mod.currentArea.overrideSpeedX10.ToString();
                    }
                    else if (parm.EndsWith("overrideSpeedY10"))
                    {
                        return gv.mod.currentArea.overrideSpeedY10.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayLimit10"))
                    {
                        return gv.mod.currentArea.overrideDelayLimit10.ToString();
                    }
                    else if (parm.EndsWith("overrideDelayCounter10"))
                    {
                        return gv.mod.currentArea.overrideDelayCounter10.ToString();
                    }
                    else if (parm.EndsWith("overrideIsNoScrollSource10"))
                    {
                        return gv.mod.currentArea.overrideIsNoScrollSource10.ToString();
                    }
                    else if (parm.EndsWith("changeableByWeatherScript10"))
                    {
                        return gv.mod.currentArea.changeableByWeatherScript10.ToString();
                    }
                    #endregion
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
                    else if (parm.EndsWith("currentWeatherName"))
                    {
                        return gv.mod.currentWeatherName.ToString();
                    }
                    else if (parm.EndsWith("currentWeatherDuration"))
                    {
                        return gv.mod.currentWeatherDuration.ToString();
                    }
                    else if (parm.EndsWith("longEntryWeathersList"))
                    {
                        return gv.mod.longEntryWeathersList.ToString();
                    }
                    else if (parm.EndsWith("useFirstPartOfWeatherScript"))
                    {
                        return gv.mod.useFirstPartOfWeatherScript.ToString();
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
