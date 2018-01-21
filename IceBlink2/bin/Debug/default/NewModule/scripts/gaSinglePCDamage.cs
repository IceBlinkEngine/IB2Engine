//gaSinglePCDamage.cs - Apply damage to each PC in party
//parm1 = (int) damage to apply (can be a variable, rand(minInt-maxInt), or just type in a number)
//parm2 = keywords used to identify the affected pc
"leader" - (without the "") this is used to appaly the damage o current speaker (ie useful for party chat) or generally party leader
"0" to "5" - (without the "") this applies teh damage to teh pc with this order number in player list
"Drin" - (without the "") this target a pc by his name, note that"Drin" is just an example palceholder for the name of the pc here 
//parm3 = none
//parm4 = none
//a check will be made to see if any PCs drop below 0 hp and change their status to dead (unconscious).
//for example, to apply damage between 10 and 15 type "rand(10-15)" without the quotes.
