using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using test.HelperClasses;

namespace test.Analitycs
{
    public class PersonStats
    {
        public string Name { get; set; }
        public int MessageCount { get; set; }
        public int SentReactions { get; set; }
        public int ReceivedReactions { get; set; }
        public int SharedLinks { get; set; }

        public Dictionary<string, int> TopWords { get; set; }
    }

    public static class PersonStatsAnalyzer
    {
        public static List<PersonStats> Analize(Root root)
        {
            var result = CalculateStats(root);
            result.ForEach(x => x.TopWords =  GetTopUsedWords(root.messages.Where(m => m.sender_name == x.Name).Select(x => x.content).Where(x => x != null).ToList(),30 ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
            return result;
        }

        public static List<PersonStats> CalculateStats(Root root)
        {
            if (root == null || root.participants == null || root.messages == null)
                return new List<PersonStats>();

            return root.participants.Select(p => new PersonStats
            {
                Name = p.name,
                MessageCount = root.messages.Count(m => m.sender_name == p.name),
                SentReactions = root.messages.SelectMany(m => m.reactions ?? new List<Reaction>())
                                             .Count(r => r.actor == p.name),
                ReceivedReactions = root.messages.Where(m => m.sender_name == p.name)
                                                 .SelectMany(m => m.reactions ?? new List<Reaction>())
                                                 .Count(),
                SharedLinks = root.messages.Count(m => m.sender_name == p.name && m.share != null)
            }).ToList();

        }

        private static Dictionary<string, int> GetTopUsedWords(List<string> contents, int topN)
        {
            var wordCount = new Dictionary<string, int>();

            foreach (var content in contents)
            {
                var words = Regex.Matches(content.ToLower(), @"\b\w{5,}\b"); // min. 3 betűs szavak
                foreach (Match match in words)
                {
                    var word = match.Value;
                    if (wordCount.ContainsKey(word))
                        wordCount[word]++;
                    else
                        wordCount[word] = 1;
                }
            }

            return wordCount.OrderByDescending(kvp => kvp.Value)
                .Take(topN)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

    }
}
