using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class LoadSaveSystem
{
    /*public static void Save(List<CellData> lvlData)
    {
        StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                           "/" + "testingLvlData");

        foreach (var data in lvlData)
        {
            sw.WriteLine(data.Position);
            sw.WriteLine(data.Token);
        }

        sw.Close();
    }*/

    public static void Load()
    {
        StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                           "/" + "testingLvlData");
        
        string txt = String.Empty;
        
        while (sr.Peek() > -1)
        {
            txt += sr.ReadLine().Split();
        }
        
        Debug.Log(txt);
        
        sr.Close(); // закрываем файл
    }
}