using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class endCode : MonoBehaviour
{
     public static endCode instance;
    public static int participantTotal, participantID;
    void Start()
    {
        instance = this;
        createText();
        saveFile();
    }

    void createText()
    {
        String path = Application.persistentDataPath + "/Participants.txt";
        if (!File.Exists(path))
        {
            participantTotal = 1;
            participantID = participantTotal.GetHashCode();
            File.WriteAllText(path, "Participant 1 : " + participantID);
            return;
        }
        participantTotal = 0;
        StreamReader streamReader = new StreamReader(path);
        bool end = false;
        while (!end)
        {
            streamReader.ReadLine();
            participantTotal++;
            if (streamReader.EndOfStream)
            {
                end = true;
            }
        }
        participantTotal++;
        participantID = participantTotal.GetHashCode();
    }

    public void saveFile(){
        String path = Application.persistentDataPath + "/Participants.txt";
        StreamWriter streamWriter = new(path);
        streamWriter.Write("Participant "+participantTotal+" : " +participantID);
    }   
    
}