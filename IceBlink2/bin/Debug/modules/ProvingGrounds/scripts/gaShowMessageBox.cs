//gaShowMessageBox.cs - A pop-up message box appears with some text (this message box does not pause the android system so if three of these are called from the same LogicTree, all three will show up on the screen at the same time after the LogicTree completes)
//parm1 = (string) html (see examples below) text to be shown in the message box (can be a variable, rand(minInt-maxInt), or just text)
//parm2 = none
//parm3 = none
//parm4 = none
//Examples:
//ex 1) "enter your message here<br>forced line break before this text"
//ex 2) "<font color='red'>enter your message here</font><br>"
//ex 3) "<font color='yellow'><b>this will be bold</b> and <u><i>this will be italics and underlined</i></u></font><br>"
//colors available (red, lime, yellow, teal, blue, fuchsia, white...any html color)
//HTML Tags available
//<b> Bold
//<i> Italics
//<u> Underline
//<sub> Subtext
//<sup> Supertext
//<big> Big
//<small> Small
//<tt> Monospace
//<font> Font face and color
//<br> Linefeed