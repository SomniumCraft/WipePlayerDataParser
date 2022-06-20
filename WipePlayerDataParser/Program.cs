using WipePlayerDataParser;
using fNbt;

var playerDatas = Directory.GetFiles("C:/playerdata", "*.dat");

var inventoryCleaner = new InventoryCleaner();

foreach (string playerData in playerDatas)
{
    var myFile = new NbtFile();
    try
    {
        myFile.LoadFromFile(playerData);
        inventoryCleaner.CleanPlayerInventory(myFile);
        myFile.RootTag["foodLevel"] = new NbtInt("foodLevel", 20);
        myFile.RootTag["foodSaturatuionLevel"] = new NbtFloat("foodSaturatuionLevel", 20f);
        myFile.RootTag["Health"] = new NbtFloat("Health", 20f);
        myFile.RootTag["XpLevel"] = new NbtInt("XpLevel", 0);
        myFile.RootTag["XpP"] = new NbtFloat("XpP", 0f);
        myFile.RootTag["XpTotal"] = new NbtInt("XpTotal", 0);
    }
    catch (Exception ex) { 
        Console.WriteLine(ex.Message);
    }
    myFile.SaveToFile(myFile.FileName,NbtCompression.GZip);
}
