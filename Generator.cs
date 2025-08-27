using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CPHInline
{
	// added bias away from "none" option
	List<string> objectiveOptions = new List<string>(){
		"none", "mode",	"random", "mode", "random"
		}; //["none", "mode", "random"];
	
	List<string> objectiveModes = new List<string>(){
		"classicforge", "classicgiant", "fiends", "dkmatter"
		};
	
	List<string> objectiveWin = new List<string>(){
		"game", "crystal"
		};
	
	List<string> KeyItemMixes = new List<string>(){
		"", "/summon", "/moon", "/summon/moon"
		};
	
	List<string> KeyItemUnderworldAccessForce = new List<string>(){
		"", "/force:hook", "/force:magma"
		};
	
	List<string> TreasureOptions = new List<string>(){
		"shuffle", "standard", "pro", "wild", "wildish"
		};
	
	List<string> ShopOptions = new List<string>(){
		"shuffle", "standard", "pro", "wild"
		};
	
	// set to "toggle" to make my life easier
	List<string> EncounterOptions = new List<string>(){
		"toggle"//, "vanilla", "reduce", "noencounters"
		};

	List<string> GlitchOptions = new List<string>(){
		"dupe", "mp", "warp", "life", "sylph", "backrow"
		};
		
	// removed "random" as pointless
	List<string> StarterKits = new List<string>(){
		"basic", "better", "loaded", "cata", "freedom", "cid", "yang", "money", "grabbag",
		"trap", "archer", "fabul", "castlevania", "summon", "meme", "defense", "mist", 
		"mysidia", "baron", "dwarf", "eblan", "libra", "99", "green" //, "random"
		};
		
	Random rnd = new Random();
	
	public bool Execute()
	{
		string username = args["user"].ToString();
		string actionTime = args["actionQueuedAt"].ToString().Split(' ')[0].Replace(@"/", "");
		string globalPath = CPH.GetGlobalVar<string>("textOutputPath", true);
		StringBuilder sbFlagset = new StringBuilder();
		
		// Call each of the Handler Methods and append to StringBuilder
		sbFlagset.Append(ObjectiveHandler());
		sbFlagset.Append(KeyItemsHandler());
		sbFlagset.Append(PassHandler());
		sbFlagset.Append(CharHandler());
		sbFlagset.Append(TreasureHandler());
		sbFlagset.Append(ShopHandler());
		sbFlagset.Append(BossHandler());
		sbFlagset.Append(EncounterHandler());
		sbFlagset.Append(GlitchHandler());
		sbFlagset.Append(OtherHandler(sbFlagset.ToString()));
		
		// Append Spoiler as a "Just in case"
		sbFlagset.Append("-spoil:all");
		CPH.SendMessage(sbFlagset.ToString());
		//Save File to a globalPath using username and actionTime from Trigger
		File.WriteAllText($"{globalPath}\\FF4FE_Flagset_{username}_{actionTime}.txt",
		 sbFlagset.ToString(), Encoding.Default);
		return true;
	}
	
	// CoinFlip Method feels self explanitory
	// Used to decide which flags to use
	public bool CoinFlip()
	{
		return rnd.Next(2) == 1;
	}
	
	// Decide the Objective for the flagset (if any)
	public string ObjectiveHandler()
	{
		StringBuilder sbObj = new StringBuilder("O");
		
		string objectiveChosen = objectiveOptions[rnd.Next(objectiveOptions.Count)];
		sbObj.Append(objectiveChosen);
		
		switch (objectiveChosen)
		{
			case "none":
				break;
			case "mode":
				sbObj.Append(ReturnModeVariables());
				if (CoinFlip())
					{
						sbObj.Append("/random");
						sbObj.Append(ReturnRandomVariables(5));
					}
				break;
			case "random":
				sbObj.Append(ReturnRandomVariables());
				break;
			default:
				break;
		};
		
		if (objectiveChosen != "none")
		{
			if (sbObj.ToString().Contains("classicforge"))
				sbObj.Append("/win:crystal");
			else
				{
					string winPrize = objectiveWin[rnd.Next(objectiveWin.Count)];
					sbObj.Append($"/win:{winPrize}");
				}
		}
		
		return sbObj.Append(" ").ToString();
	}
	
	// Return value from the Mode List when called
	public string ReturnModeVariables()
	{
		StringBuilder sbModeVars = new StringBuilder(":");
		string modeChosen = objectiveModes[rnd.Next(objectiveModes.Count)];
		
		sbModeVars.Append(modeChosen);
		return sbModeVars.ToString();
	}
	
	// Return value(s) from the randomObjectives List
	public string ReturnRandomVariables(int restrictObjectives = 9)
	{
		StringBuilder sbRandomVars = new StringBuilder(":");
		int randomObjectiveCount = rnd.Next(1, restrictObjectives);
		
		sbRandomVars.Append(randomObjectiveCount);
		
		int randomObjectiveReq = rnd.Next(1, restrictObjectives);
		
		if (CoinFlip())
			sbRandomVars.Append(",quest");
		else
			sbRandomVars.Append(",tough_quest");
		
		if (CoinFlip())
			sbRandomVars.Append(",boss");
		
		if (CoinFlip())
			sbRandomVars.Append(",char");
		
		if (randomObjectiveCount < 4
		|| randomObjectiveReq >= randomObjectiveCount)
			sbRandomVars.Append("/req:all");
		else
			sbRandomVars.Append($"/req:{randomObjectiveReq}");
		
		return sbRandomVars.ToString();
	}
	
	// Decide the Key Items parameters
	public string KeyItemsHandler()
	{
		StringBuilder sbKeyItems = new StringBuilder("Kmain");
		
		sbKeyItems.Append(KeyItemMixes[rnd.Next(KeyItemMixes.Count)]);
		
		if(CoinFlip())
			sbKeyItems.Append("/nofree");
		
		sbKeyItems.Append(KeyItemUnderworldAccessForce[rnd.Next(KeyItemUnderworldAccessForce.Count)]);
				
		return sbKeyItems.Append(" ").ToString();
	}
	
	// Decice the Pass parameters
	public string PassHandler()
	{
		StringBuilder sbPass = new StringBuilder("P");
		
		if (CoinFlip())
			sbPass.Append("shop/");
		if (CoinFlip())
			sbPass.Append("key/");
		if (CoinFlip())
			sbPass.Append("chests/");
		
		string passString = sbPass.ToString();
		
		if (passString.Length == 1)
			passString = "Pnone";
		else
			passString = passString.Substring(0, passString.Length - 1);
		
		return passString + " ";
	}
	
	// Decice the Character parameters
	public string CharHandler()
	{
		StringBuilder sbChar = new StringBuilder("C");
		
		if (CoinFlip())
			sbChar.Append("standard");
		else
			sbChar.Append("relaxed");
		
		if (CoinFlip())
			sbChar.Append("/nofree");
		
		if (CoinFlip())
			sbChar.Append("/maybe");
		
		if (CoinFlip())
		{
			sbChar.Append("/distinct:");
			sbChar.Append(rnd.Next(5, 11));
		}
		
		if (CoinFlip())
			sbChar.Append("/j:abilities");
		
		if (CoinFlip())
			sbChar.Append("/nekkie");
		
		if (CoinFlip())
			sbChar.Append("/nodupes");
		
		if (CoinFlip())
			sbChar.Append("/hero");
		
		return sbChar.Append(" ").ToString();
	}
	
	// Decice the Treasure parameters using TreasureOptions List
	public string TreasureHandler()
	{
		StringBuilder sbTreasure = new StringBuilder("T");
		
		string treasureChoice = TreasureOptions[rnd.Next(TreasureOptions.Count)];
		sbTreasure.Append(treasureChoice);
		
		if (CoinFlip())
			{
				int sparcity = rnd.Next(1, 10);
				sbTreasure.Append($"/sparse:{sparcity}0");
			}
		
		if (treasureChoice != "shuffle")
		{
			if (CoinFlip())
				{
					int maxTier = rnd.Next(3, 8);
					sbTreasure.Append($"/maxtier:{maxTier}");
				}
		}
		
		return sbTreasure.Append(" ").ToString();
	}
	
	// Decice the Shop parameters using ShopOptions List
	public string ShopHandler()
	{
		StringBuilder sbShop = new StringBuilder("S");
		
		string shopChoice = ShopOptions[rnd.Next(ShopOptions.Count)];
		sbShop.Append(shopChoice);
		
		if (CoinFlip())
			sbShop.Append("/free");
		
		return sbShop.Append(" ").ToString();
	}
	
	// Decice the Boss parameters
	public string BossHandler()
	{
		StringBuilder sbBoss = new StringBuilder("Bstandard");
		
		if (CoinFlip())
			sbBoss.Append("/alt:gauntlet");
			
		if (CoinFlip())
			{
				if (CoinFlip())
					sbBoss.Append("/whyburn");
				else
					sbBoss.Append("/whichburn");
			}
		
		return sbBoss.Append(" ").ToString();
	}
	
	// Hardcode the Encounters parameters
	public string EncounterHandler()
	{
		//return "Etoggle ";
		StringBuilder sbEnc = new StringBuilder("E");
		
		string encChoice = EncounterOptions[rnd.Next(EncounterOptions.Count)];
		sbEnc.Append(encChoice);

		// Commented out for my benefit
		/*if (encChoice != "vanilla")
			{
				if (CoinFlip())
					sbEnc.Append("/keep:");
					if (CoinFlip())
						if (CoinFlip())
							sbEnc.Append("doors,behemoths");
						else
							sbEnc.Append("behemoths");
					else
						sbEnc.Append("doors");
				
				if ((encChoice != "reduce") && CoinFlip())
					sbEnc.Append("/danger");
			}
		else
			if (CoinFlip())
				sbEnc.Append("/cantrun");
			if (CoinFlip())
				sbEnc.Append("/noexp");
			if (CoinFlip())
				if (CoinFlip())
					sbEnc.Append("/no:jdrops");
				else
					sbEnc.Append("/no:sirens");*/

		return sbEnc.Append(" ").ToString();
	}
	
	public string GlitchHandler()
	{
		StringBuilder sbGlitches = new StringBuilder("G");
		
		foreach (string glitch in GlitchOptions)
		{
			if (CoinFlip())
				sbGlitches.Append($"{glitch}/");
		}
		
		string glitchFlag = sbGlitches.ToString();
		
		if (glitchFlag.Length == 1)
			glitchFlag += "none";
		else
			glitchFlag = glitchFlag.Substring(0, glitchFlag.Length - 1);
		
		return glitchFlag + " ";
	}
	
	public string OtherHandler(string currentString)
	{
		StringBuilder sbOthers = new StringBuilder("");
		
		int kitCount = rnd.Next(4);
		
		for (int i = 0; i < kitCount; i++)
		{
			string kitPick = StarterKits[rnd.Next(StarterKits.Count)];
			string kitNo = i < 1 ? "" : (i + 1).ToString();
			sbOthers.Append($"-kit{kitNo}:{kitPick} ");
		}
		
		if (CoinFlip())
			sbOthers.Append("-spoon ");
			
		if (CoinFlip() 
		&& !currentString.ToLower().Contains("chero")
		&& !currentString.ToLower().Contains("classicforge"))
		{
			if (CoinFlip())
				sbOthers.Append("-smith:super ");
			else
				sbOthers.Append("-smith:alt ");
		}

		if (CoinFlip() && CoinFlip() && CoinFlip())
			sbOthers.Append("-pushbtojump ");
		
		return sbOthers.Append(" ").ToString();
	}
}

