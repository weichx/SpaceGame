using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using SpaceGame.Util;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace SpaceGame {

    public class FactionAttribute : PropertyAttribute { }

    // todo -- move the json stuff into a scriptable object instead

    public class Faction {

        private int index;
        private List<Disposition> dispositions;

        private static List<Faction> factions;

        private static int idGenerator;

        public string Name { get; private set; }

        public Faction(string name) {
            this.index = idGenerator++;
            this.Name = name;
        }

        public void SetDisposition(Faction other, Disposition disposition) {
            if (disposition == Disposition.Self) return;
            dispositions[other.index] = disposition;
        }

        public Disposition GetDisposition(Faction other) {
            return dispositions[other.index];
        }

        public bool IsFriendlyOrNeutral(Faction other) {
            return IsFriendly(other) || IsNeutral(other);
        }

        public bool IsFriendly(Faction other) {
            return (dispositions[other.index] & (Disposition.Friendly | Disposition.Self)) != 0;
        }

        public bool IsNeutral(Faction other) {
            return (dispositions[other.index] & (Disposition.Neutral | Disposition.Unknown)) != 0;
        }

        public bool IsHostile(Faction other) {
            return dispositions[other.index] == Disposition.Hostile;
        }

        public static Faction GetFaction(string name) {
            if (factions == null) {
                CreateFactions();
                ParseFactionFile();
            }

            return factions.Find((f) => f.Name == name);
        }

        public static Faction GetFaction(int id) {
            if (factions == null) {
                CreateFactions();
                ParseFactionFile();
            }

            if (id >= 0 && id < factions.Count) return factions[id];
            return null;
        }

        public static void SetFriendly(Faction faction0, Faction faction1) {
            SetDisposition(faction0, faction1, Disposition.Friendly);
        }

        public static void SetNeutral(Faction faction0, Faction faction1) {
            SetDisposition(faction0, faction1, Disposition.Neutral);
        }

        public static void SetHostile(Faction faction0, Faction faction1) {
            SetDisposition(faction0, faction1, Disposition.Hostile);
        }

        public static void SetDisposition(Faction faction0, Faction faction1, Disposition disposition) {
            if (faction0 == faction1 || disposition == Disposition.Self) return;
            faction0.dispositions[faction1.index] = disposition;
            faction1.dispositions[faction0.index] = disposition;
        }

        public static Faction[] GetFactions() {
            if (factions == null) {
                CreateFactions();
                ParseFactionFile();
            }

            Faction[] retn = new Faction[factions.Count];
            for (int i = 0; i < factions.Count; i++) {
                retn[i] = factions[i];
            }

            return retn;
        }

        public static string[] GetFactionNames() {
            if (factions == null) {
                CreateFactions();
                ParseFactionFile();
            }

            string[] retn = new string[factions.Count];
            for (int i = 0; i < retn.Length; i++) {
                retn[i] = factions[i].Name;
            }

            return retn;
        }

        [Serializable]
        private struct DispPair {

            public string name;
            public string disposition;

            public Disposition GetDispositionValue() {
                string disp = disposition.ToLower();
                switch (disp) {
                    case "hostile":
                        return Disposition.Hostile;
                    case "friendly":
                        return Disposition.Friendly;
                    case "neutral":
                        return Disposition.Neutral;
                    case "unknown":
                        return Disposition.Unknown;
                }

                return Disposition.Neutral;
            }

        }

        [Serializable]
        private struct FactionDefinition {

            public string factionName;
            public DispPair[] dispositions;

            public FactionDefinition(Faction faction) {
                factionName = faction.Name;
                dispositions = new DispPair[faction.dispositions.Count];
                for (int i = 0; i < dispositions.Length; i++) {
                    Disposition disposition = faction.GetDisposition(factions[i]);
                    dispositions[i] = new DispPair();
                    dispositions[i].name = factions[i].Name;
                    dispositions[i].disposition = Enum.GetName(typeof(Disposition), disposition);
                }
            }

        }

        [Serializable]
        private struct Wrapper {

            public FactionDefinition[] data;

        }

        // creation order is very important! 
        // entities will serialize an index into this list!
        public static void CreateFactions() {
            factions = new List<Faction>();
            factions.Add(new Faction("SpreeHaulers"));
            factions.Add(new Faction("Water Guild"));
            factions.Add(new Faction("Miners Guild"));
            factions.Add(new Faction("Junkers"));
            factions.Add(new Faction("Gerus Corp."));
            AssignIndices();
        }

        private static void AssignIndices() {
            for (int i = 0; i < factions.Count; i++) {
                Faction faction = factions[i];
                faction.index = i;
                faction.dispositions = new List<Disposition>(factions.Count);
                for (int j = 0; j < factions.Count; j++) {
                    faction.dispositions.Add(Disposition.Neutral);
                }

                faction.dispositions[i] = Disposition.Self;
            }
        }

        private static void AssignDispositions(Faction faction, DispPair[] dispositions) {
            for (int i = 0; i < dispositions.Length; i++) {
                DispPair pair = dispositions[i];
                Faction other = GetFaction(pair.name);
                if (other == null) {
                    DebugConsole.Log("Unable to find faction called ", pair.name);
                }
                else {
                    faction.SetDisposition(other, pair.GetDispositionValue());
                }
            }
        }

        public static void ParseFactionFile() {
            TextAsset json = Resources.Load<TextAsset>("Factions");
            FactionDefinition[] factionDefinitions = JsonUtility.FromJson<Wrapper>(json.text).data;
            for (int i = 0; i < factionDefinitions.Length; i++) {
                FactionDefinition def = factionDefinitions[i];
                Faction faction = GetFaction(def.factionName);
                if (faction == null) {
                    DebugConsole.Log("Unable to find faction called ", def.factionName);
                }
                else {
                    AssignDispositions(faction, def.dispositions);
                }
            }
        }

    #if UNITY_EDITOR
        public static void SaveToJson() {
            Wrapper root = new Wrapper();
            root.data = new FactionDefinition[factions.Count];
            for (int i = 0; i < root.data.Length; i++) {
                root.data[i] = new FactionDefinition(factions[i]);
            }

            const string path = "Assets/Resources/Factions.json";

            using (FileStream fs = new FileStream(path, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(fs)) {
                    writer.Write(JsonUtility.ToJson(root));
                }
            }

            AssetDatabase.Refresh();
        }
    #endif

    }

}