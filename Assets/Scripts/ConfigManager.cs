using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigManager {

    public List<int[]> steps;
    readonly string targetFile = Application.dataPath + "/embedded/steps.txt";
    public int maxPanda;
    public int speed;
    public int wait;

    public ConfigManager() {
        LoadFromFile();
    }

    private void LoadFromFile() {
        string single = null;
        FileStream fs = new FileStream(targetFile, FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        steps = new List<int[]>();
        while((single = sr.ReadLine()) != null) {
            if (single.StartsWith("#")) {
                continue;
            }
            string[] sLr = single.Split(new char[] { ':', ' ' });
            if(sLr.Length != 2) {
                Debug.LogError("Unknwon config: "+single);
                continue;
            }
            int left = 0;
            int right = int.Parse(sLr[1].Trim());
            if (!int.TryParse(sLr[0].Trim(), out left)) {
                switch (sLr[0].Trim()) {
                    case "count":
                        maxPanda = right;
                        break;
                    case "time":
                        speed = right;
                        break;
                    case "wait":
                        wait = right;
                        break;
                }
            } else {
                steps.Add(new int[] { left, right });
            }
        }
    }
}
