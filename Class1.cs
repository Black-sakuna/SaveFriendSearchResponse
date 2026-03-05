using Gallop;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using UmamusumeResponseAnalyzer;
using UmamusumeResponseAnalyzer.Plugin;

namespace SaveFriendSearchResponse
{
    public class SaveFriendSearchResponse : IPlugin
    {

        [PluginDescription("保存好友种马信息")]
        public string Name => "SaveFriendSearchResponse";

        public Version Version => new Version(1, 0, 0);

        public string Author => "匿名";

        public string[] Targets => [];

        internal string PLUGIN_DATA_DIRECTORY = string.Empty;

        internal string charaViewerId = string.Empty;

        internal string charaUmaName = string.Empty;

        internal string joData = string.Empty;

        private CancellationTokenSource cts = new CancellationTokenSource();

        private Task task1;

        public Task UpdatePlugin(ProgressContext ctx)
        {
            return Task.CompletedTask;
        }
        public void Initialize()
        {
            if(UmamusumeResponseAnalyzer.UmamusumeResponseAnalyzer.Started)
            {
                PLUGIN_DATA_DIRECTORY = Path.Combine("PluginData", Name);
                task1 = Task.Run(ConsoleReadKeyCheck);
            }
        }
        public void Dispose()
        {
            cts.Cancel();
        }
        [Analyzer(true, 0)]
        public void Analyze(JObject jo)
        {
            if (jo.ContainsKey("data") && jo["data"] is JObject data && data.ContainsKey("user_info_summary"))
            {
                bool b1 = data.ContainsKey("partner_chara_info_array");
                bool b2 = data.ContainsKey("support_card_data_array");
                bool b3 = data.ContainsKey("follower_num");
                bool b4 = data.ContainsKey("own_follow_num");
                if (b1 && b2 && b3 && b4)
                {
                    string charaViewerId = string.Empty;
                    string charaUmaName = string.Empty;
                    string joData = string.Empty;
                    FriendSearchResponse @event = jo.ToObject<FriendSearchResponse>();
                }
            }
        }
        private void ConsoleReadKeyCheck()
        {
            CancellationToken token = cts.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    string keyChar = Console.ReadKey(true).KeyChar.ToString();
                    if (keyChar == "x" || keyChar == "X")
                    {
                        if (File.Exists(PLUGIN_DATA_DIRECTORY + "\\tmp.txt"))
                        {
                            JObject jo = JObject.Parse(File.ReadAllText(PLUGIN_DATA_DIRECTORY + "\\tmp.txt"));
                        }
                    }
                    else if (keyChar == "c" || keyChar == "C")
                    {
                        ShowFriendSearch();
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("ConsoleReadKeyCheck：" + ex.Message);
            }
        }

        private void ShowFriendSearch()
        {
            string selection = string.Empty;
            do
            {
                List<string> choices = new List<string>
            {
                "插件_ " + Name,
                $"版本_ {Version}",
                "作者_ " + Author
            };
                choices.Add("返回");
                SelectionPrompt<string> selectionPrompt = new SelectionPrompt<string>().Title("种马信息：").WrapAround().AddChoices(choices);
                selection = AnsiConsole.Prompt(selectionPrompt);
                if (selection != "返回")
                {
                    JObject jo = JObject.Parse(File.ReadAllText(Path.Combine(PLUGIN_DATA_DIRECTORY, "tmp.txt")));
                    ParseFriendSearch.ParseFriendSearchResponse(jo.ToObject<FriendSearchResponse>());
                    Console.WriteLine("按任意键返回。");
                    Console.ReadKey();
                }
            }
            while (selection != "返回");
        }
    }
}
