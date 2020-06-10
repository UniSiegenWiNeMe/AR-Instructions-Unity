using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AR_Instructions.Scripts
{
    public class ActionLog
    {
        private List<ActionLogEntry> actionLog;

        public ActionLog()
        {
            actionLog = new List<ActionLogEntry>();
        }

        public void Add(ActionLogEntry entry)
        {
            actionLog.Add(entry);
            QuickSave();
        }

        private string ListToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var action in actionLog)
            {
                var s = string.Format("{0};{1};{2}",
                                action.StepNumber,
                                action.ActionType,
                                action.Timestamp.ToString("HH:mm:ss.ffff"));
                s += Environment.NewLine;
                sb.Append(s);
            }
            return sb.ToString();
        }

        private void QuickSave()
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "QuickSave-" + actionLog[0].Timestamp.ToString("yyyy.MM.dd-HH.mm.ss") + ".txt"), ListToString());
        }

        /// <summary>
        /// Exports the action log to a default location on hard disk. Formatted similar to csv. Adds a summary of the time per step at the end of file
        /// </summary>
        public void Export()
        {
                        
            var filteredActionLog = actionLog.Where(x => x.ActionType == ActionType.StepBackward ||
            x.ActionType == ActionType.StepForward ||
            x.ActionType == ActionType.InstructionCompleted ||
            x.ActionType == ActionType.InstructionStarted).OrderBy(x => x.Timestamp).ToList();

            Dictionary<int, TimeSpan> tmp = new Dictionary<int, TimeSpan>();
            for (int i = 0; i < filteredActionLog.Count; i++)
            {
                if (i > 0)
                {
                    if (!tmp.ContainsKey(filteredActionLog[i].StepNumber))
                    {
                        var timespan = filteredActionLog[i].Timestamp - filteredActionLog[i - 1].Timestamp;
                        tmp.Add(filteredActionLog[i].StepNumber, timespan);
                    }
                    else
                    {
                        var timespan = filteredActionLog[i].Timestamp - filteredActionLog[i - 1].Timestamp;
                        tmp[filteredActionLog[i].StepNumber] = tmp[filteredActionLog[i].StepNumber].Add(timespan);
                    }
                }
            }
                       
            var listAsString = ListToString();

            foreach (var keyValuePair in tmp)
            {
                var s = String.Format("{0};{1}",
                                keyValuePair.Key,
                                keyValuePair.Value);
                s += Environment.NewLine;
                listAsString+= s;
            }

            var totalTime = actionLog.Last().Timestamp - actionLog[0].Timestamp;
            listAsString += "Totaltime:" + totalTime;

            File.WriteAllText(Path.Combine(Application.persistentDataPath, "ActionLog-" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + ".txt"), listAsString);

            actionLog.Clear();
        }
    }

    public enum ActionType
    {
        InstructionStarted,
        InstructionCompleted,
        StepForward,
        StepBackward,
        VideoPaused,
        VideoRestarted,
        VideoUnpaused,
        VideoStarted
    }

    public class ActionLogEntry
    {
        public int StepNumber;
        public ActionType ActionType;
        public DateTime Timestamp;

        public ActionLogEntry(int stepNumber, ActionType actionType, DateTime timestamp)
        {
            StepNumber = stepNumber;
            ActionType = actionType;
            Timestamp = timestamp;
        }
    }
}
