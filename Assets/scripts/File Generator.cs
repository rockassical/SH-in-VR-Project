using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Palmmedia.ReportGenerator.Core;
using Palmmedia.ReportGenerator.Core.Common;
using UnityEngine;

public class FileGenerator : MonoBehaviour
{
    public static FileGenerator instance;
    public static int CDTotal, ADTotal, CRTotal, ARTotal; //in order cue direct, Action direct, Cue report, Action report
    public static int CDActual, ADActual, CRActual, ARActual; //in order cue direct, Action direct, Cue report, Action report
    void Start()
    {
        instance = this;
        createText();
        saveFile();
    }

    void createText()
    {
        String path = Application.persistentDataPath + "/Log.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "DATA\n\nCue Condition, Direct Message\n0/0\nAction Condition, Direct Message\n0/0\nCue Condition, Report\n0/0\nAction Condition, Report\n0/0\n");
        }

        StreamReader streamReader = new StreamReader(path);
        bool end = false;
        while (!end)
        {
            var line = streamReader.ReadLine();
            if (line.Equals("Cue Condition, Direct Message"))
            {
                String[] vals = streamReader.ReadLine().Split('/');
                CDActual = vals[0].ParseLargeInteger();
                CDTotal = vals[1].ParseLargeInteger();
            }
            if (line.Equals("Action Condition, Direct Message"))
            {
                String[] vals = streamReader.ReadLine().Split("/");
                CDActual = vals[0].ParseLargeInteger();
                CDTotal = vals[1].ParseLargeInteger();
            }
            if (line.Equals("Cue Condition, Report"))
            {
                String[] vals = streamReader.ReadLine().Split("/");
                CDActual = vals[0].ParseLargeInteger();
                CDTotal = vals[1].ParseLargeInteger();
            }
            if (line.Equals("Action Condition, Report"))
            {
                String[] vals = streamReader.ReadLine().Split("/");
                CDActual = vals[0].ParseLargeInteger();
                CDTotal = vals[1].ParseLargeInteger();
                end = true;
            }
        }
    }

    public void saveFile()
    {
        String path = Application.persistentDataPath + "/Log.txt";
        StreamWriter streamWriter = new(path);
        streamWriter.Write("DATA\n\nCue Condition, Direct Message\n" + CDActual + "/" + CDTotal + "\nAction Condition, Direct Message\n" + ADActual + "/" + ADTotal + "\nCue Condition, Report\n" + CRActual + "/" + CRTotal + "\nAction Condition, Report\n" + ARActual + "/" + ARTotal + "\n");
    }

}
