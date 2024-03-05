using SimpleJSON;
using System;
using System.Collections.Generic;
using Hexstar;
using Hexstar.CSE;

namespace CSE
{
    public static class XAPI_Builder
    {
        private static JSONNode actor = new JSONObject();
        private static List<string> statements = new();

        public static void AutoSetupActor()
        {
            if (actor.HasKey("name")) return;
            actor.Add("name", SesionHandler.nickname);
            actor.Add("mbox", SesionHandler.email);
        }

        /// <summary>
        /// Envía todos los registros que se hayan acumulado al servidor y comprueba que este los haya aceptado.
        /// </summary>
        public static async void SendAllStatements()
        {
            AutoSetupActor();

            //Formar json con la lista de statements
            JSONObject json = new();
            json.Add("lista",new JSONArray());
            foreach (var statement in statements) json["lista"].Add(statement);
            statements.Clear();

            //Enviar json al servidor
            UnityEngine.WWWForm form = new();
            form.AddField("authorization", SesionHandler.sessionKEY);
            form.AddField("registros", json.ToString());
            await ConexionHandler.APost(ConexionHandler.baseUrl + "lrs", form);
            _ = ConexionHandler.ExtraerJson(ConexionHandler.download); //Para comprobar que se ha enviado bien.
            //FIX: Sigue estando bugueada la llamada API ???
        }

        #region High Level Statements
        // started - session    // Done
        // finished - session   // Done

        public static void CreateStatement_GameSession( bool start, bool newgame = false)
        {
            string start_desc = "The moment the player transition from the main menu to the gameplay";
            string end_desc = "The moment the player finish playing the game or exits it";

            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            if(start)
            {
                statement.Add("verb", ConstructSimpleVerb("started"));
                statement.Add("object", ConstructSimpleObject("high_level", "session", start_desc));
                JSONObject context = new();
                context.Add("new_game", newgame);
                statement.Add("context", context);
            }
            else
            {
                statement.Add("verb", ConstructSimpleVerb("finished"));
                statement.Add("object", ConstructSimpleObject("high_level", "session", end_desc));
            }
            statement.Add("timestamp", GetTimeStamp());
            
            statements.Add(statement.ToString());
        }
        #endregion

        #region General Statements
        // requested - case     //Done
        // finished - day       //Done
        // increased - difficulty
        public static void CreateStatement_CaseRequest(bool accepted)
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("requested"));
            statement.Add("object", ConstructSimpleObject("general", "case", "The player tried to buy a case"));
            JSONObject result = new();
            result.Add("accepted",accepted);
            statement.Add("result", result);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        public static void CreateStatement_DayFinished(int completedCases, int triedCases)
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("finished"));
            statement.Add("object", ConstructSimpleObject("general", "day", "The player ran out of queries"));
            JSONObject context = new();
            context.Add("completed_cases", completedCases);
            context.Add("tried_cases", triedCases);
            statement.Add("context", context);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        public static void CreateStatement_DifficultyIncrease(int newDifficulty)
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("increased"));
            statement.Add("object", ConstructSimpleObject("general", "difficulty", "The player completed a hard case and will unlock new ones"));
            JSONObject context = new();
            context.Add("new_difficulty", newDifficulty);
            statement.Add("context", context);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        #endregion

        #region Case and Query Statements
        // attempted - send query
        // generated block - block
        // modified block - block
        // removed block - block
        // connected block - block
        // used - block grouper
        // constructed - query
        // attempted - solve query
        // surrendered - case
        public enum BlockAction {GENERATED,MODIFIED,REMOVED,CONNECTED};
        public static void CreateStatement_TrySendQuery(bool empty, bool valid, bool caseLost)
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("attempted"));
            statement.Add("object", ConstructSimpleObject("case&queries", "query", "The player has send a query"));
            
            JSONObject result = new();
            result.Add("empty",empty);
            result.Add("correct_syntax",valid);
            statement.Add("result", result);
            
            JSONObject context = new();
            var caso = PuzzleManager.GetCasoActivo();
            if (caso != null) context.Add("case_id", caso.id);
            else context.Add("case_id", -1);
            context.Add("is_case_lost", caseLost);
            statement.Add("context", context);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        public static void CreateStatement_BlockAction(BlockAction blockAction, string blockName, string optionalBlock = "")
        {
            string verb = blockAction switch
            {
                BlockAction.GENERATED => "generated",
                BlockAction.MODIFIED => "modified",
                BlockAction.REMOVED => "removed",
                BlockAction.CONNECTED => "connected",
                _ => "UNKNOWN_ACTION",
            };

            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb(verb));
            statement.Add("object", ConstructSimpleObject("case&queries", blockName, 
                "The player is in block construction query mode and is interacting with this block"));
            
            JSONObject context = new();
            var caso = PuzzleManager.GetCasoActivo();
            if (caso != null) context.Add("case_id", caso.id);
            else context.Add("case_id", -1);
            if (blockAction == BlockAction.CONNECTED) context.Add("other_end_of_connection",optionalBlock);
            statement.Add("context", context);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        public static void CreateStatement_BlockGroup(bool isGrouping)
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("used"));
            statement.Add("object", ConstructSimpleObject("case&queries", "block_grouper",
                "The player has used the machine to group / ungroup blocks to form a query"));
            
            JSONObject context = new();
            var caso = PuzzleManager.GetCasoActivo();
            if (caso != null) context.Add("case_id", caso.id);
            else context.Add("case_id", -1);
            statement.Add("context", context);
            
            JSONObject result = new();
            result.Add("effect",isGrouping?"group":"ungroup");
            statement.Add("result", result);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        public static void CreateStatement_QueryConstruction(string query)
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("constructed"));
            statement.Add("object", ConstructSimpleObject("case&queries", "query",
                "The player has prepared a query to send it"));
            
            JSONObject context = new();
            var caso = PuzzleManager.GetCasoActivo();
            if (caso != null) context.Add("case_id", caso.id);
            else context.Add("case_id", -1);
            context.Add("using_manual_query_mode", QueryModeController.IsQueryModeOnManual());
            statement.Add("context", context);
            
            JSONObject result = new();
            result.Add("content", query);
            statement.Add("result", result);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        private enum CaseType {MAIN, SECONDARY, EXAM};
        private static CaseType ReadCaseType(Caso caso)
        {
            if (caso.secundario) return CaseType.SECONDARY;
            if (caso.examen) return CaseType.EXAM;
            return CaseType.MAIN;
        }
        public static void CreateStatement_TrySolveCase(bool completed, bool success, Caso caso, float timeSpent, int queriesUsed, int finalScore)
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("attempted"));
            statement.Add("object", ConstructSimpleObject("case&queries", "solve_case",
                "The player has pushed the button to check if their solution is valid"));

            JSONObject context = new();
            if (caso != null) context.Add("case_id", caso.id);
            else context.Add("case_id", -1);
            statement.Add("context", context);

            JSONObject result = new();
            result.Add("completed",completed);
            result.Add("success",success);
            if(caso != null)
            {
                result.Add("case_type", ReadCaseType(caso) switch
                {
                    CaseType.MAIN => "main",
                    CaseType.SECONDARY => "secondary",
                    CaseType.EXAM => "exam",
                    _ => "CASE_TYPE_ERROR"
                });
            }
            JSONObject score = new();
            score.Add("time",timeSpent);
            score.Add("queries_used",queriesUsed);
            score.Add("full_score",finalScore);
            result.Add("score", score);
            statement.Add("result", result);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        public static void CreateStatement_Surrender()
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("surrendered"));
            statement.Add("object", ConstructSimpleObject("case&queries", "case",
                "The player has decided not to continue with this case"));

            JSONObject context = new();
            var caso = PuzzleManager.GetCasoActivo();
            if (caso != null) context.Add("case_id", caso.id);
            else context.Add("case_id", -1);
            statement.Add("context", context);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        #endregion

        #region Specific Statements
        // skipped - tutorial
        // switched - desktop zone

        public static void CreateStatement_TutorialSkip() //Unused right now
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("skipped"));
            statement.Add("object", ConstructSimpleObject("general", "tutorial", ""));
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        public static void CreateStatement_SwitchDesktopZone(bool switchedToOutput)
        {
            JSONNode statement = new JSONObject();
            statement.Add("actor", actor);
            statement.Add("verb", ConstructSimpleVerb("skipped"));
            statement.Add("object", ConstructSimpleObject("general", "tutorial", ""));

            JSONObject result = new();
            result.Add("currentDesktopZone", switchedToOutput ? "output" : "input");
            statement.Add("result", result);
            statement.Add("timestamp", GetTimeStamp());

            statements.Add(statement.ToString());
        }
        #endregion

        #region Utils
        private static string GetTimeStamp()
        {
            return DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
        }
        private static JSONObject GetSimpleTranslation(string lang, string data)
        {
            JSONObject t = new();
            t.Add(lang, data);
            return t;
        }

        private static JSONObject ConstructSimpleVerb(string action)
        {
            JSONObject verb = new();
            verb.Add("id", "http://cse.uca/xapi/verbs/"+action);
            //Se presupone que el verbo está en inglés
            verb.Add("display",GetSimpleTranslation("en-US",action));
            return verb;
        }
        private static JSONObject ConstructSimpleObject(string type,string obj, string description)
        {
            JSONObject obj_node = new();
            obj_node.Add("id", "http://cse.uca/xapi/"+type+"/"+obj);
            JSONObject definition = new();
            definition.Add("name",GetSimpleTranslation("en-US",obj));
            definition.Add("description",GetSimpleTranslation("en-US",description));
            obj_node.Add("definition",definition);
            return obj_node;
        }
        #endregion
    }
}