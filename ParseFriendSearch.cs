using Gallop;
using Spectre.Console;
using System.Text;
using UmamusumeResponseAnalyzer;

namespace SaveFriendSearchResponse
{
    public class ParseFriendSearch
    {
        private static readonly string I18N_Friend = "好友：{0}\t\tID：{1}\t\tFollower数：{2}";

        private static readonly string I18N_Uma = "种马：{0}\t胜鞍：{1}\t\t评分：{2}";

        private static readonly string I18N_WinSaddle = "胜鞍列表：{0}";

        private static readonly string I18N_Factor = "因子";

        private static readonly string I18N_UmaFactor = "代表";

        private static readonly string I18N_ParentFactor = "祖辈@{0}";

        public static void ParseFriendSearchResponse(FriendSearchResponse @event)
        {
            FriendSearchResponse.CommonResponse data = @event.data;
            TrainedChara chara = data.partner_chara_info_array[0];
            IEnumerable<int> charaWinSaddle = chara.win_saddle_id_array.Intersect(Database.SaddleIds);
            IEnumerable<int> parentWinSaddle_a = chara.succession_chara_array[0].win_saddle_id_array.Intersect(Database.SaddleIds);
            IEnumerable<int> parentWinSaddle_b = chara.succession_chara_array[1].win_saddle_id_array.Intersect(Database.SaddleIds);
            int friendAndDadWinSaddle = charaWinSaddle.Intersect(parentWinSaddle_a).Count() * 3;
            int friendAndMomWinSaddle = charaWinSaddle.Intersect(parentWinSaddle_b).Count() * 3;
            AnsiConsole.Write(new Rule());
            AnsiConsole.WriteLine(I18N_Friend, data.user_info_summary.name, data.user_info_summary.viewer_id, data.follower_num);
            AnsiConsole.WriteLine(I18N_Uma, Database.Names.GetUmamusume(chara.card_id).FullName, friendAndDadWinSaddle + friendAndMomWinSaddle, chara.rank_score);
            AnsiConsole.WriteLine(I18N_WinSaddle, string.Join(',', charaWinSaddle));
            Tree tree = new Tree(I18N_Factor);
            int max = chara.factor_info_array.Select((FactorInfo x) => x.factor_id).Concat(chara.succession_chara_array[0].factor_info_array.Select((FactorInfo x) => x.factor_id)).Concat(chara.succession_chara_array[1].factor_info_array.Select((FactorInfo x) => x.factor_id))
                .Where((int x, int index) => index % 2 == 0)
                .Max((int x) => GetRenderWidth(Database.FactorIds[x]));
            Tree representative = AddFactors(I18N_UmaFactor, chara.factor_info_array.Select((FactorInfo x) => x.factor_id).ToArray(), max);
            Tree inheritanceA = AddFactors(string.Format(I18N_ParentFactor, chara.succession_chara_array[0].owner_viewer_id), chara.succession_chara_array[0].factor_info_array.Select((FactorInfo x) => x.factor_id).ToArray(), max);
            Tree inheritanceB = AddFactors(string.Format(I18N_ParentFactor, chara.succession_chara_array[1].owner_viewer_id), chara.succession_chara_array[1].factor_info_array.Select((FactorInfo x) => x.factor_id).ToArray(), max);
            tree.AddNodes(representative, inheritanceA, inheritanceB);
            AnsiConsole.Write(tree);
            AnsiConsole.Write(new Rule());
        }

        public static int GetRenderWidth(string text)
        {
            return text.Sum((char x) => x.GetCellWidth());
        }

        public static Tree AddFactors(string title, int[] id_array, int max)
        {
            Tree tree = new Tree(title);
            IEnumerable<int> ordered = id_array.Take(2).Append(id_array[^1]).Concat(id_array.Skip(2).SkipLast(1));
            int[] even = ordered.Where((int x, int index) => index % 2 == 0).ToArray();
            int[] odd = ordered.Where((int x, int index) => index % 2 != 0).ToArray();
            foreach (int index2 in Enumerable.Range(0, even.Length))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(FactorName(even[index2]));
                int gap = 12 + max - GetRenderWidth(Database.FactorIds[even[index2]]);
                if (gap < 0)
                {
                    gap = 2;
                }
                sb.Append(string.Join(string.Empty, Enumerable.Repeat(' ', gap)));
                sb.Append((odd.Length > index2) ? FactorName(odd[index2]) : "");
                tree.AddNode(sb.ToString());
            }
            return tree;
        }
        public static string FactorName(int factorId)
        {
            string name = Database.FactorIds[factorId];
            int length = factorId.ToString().Length;
            if (1 == 0)
            {
            }
            string result = length switch
            {
                3 => "[#FFFFFF on #37B8F4]" + name + " [/]",
                4 => "[#FFFFFF on #FF78B2]" + name + " [/]",
                8 => "[#794016 on #91D02E]" + name + " [/]",
                _ => "[#794016 on #E1E2E1]" + name + " [/]",
            };
            if (1 == 0)
            {
            }
            return result;
        }
    }
}