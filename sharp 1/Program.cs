using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneSearchApp
{
    class Program
    {
        class ProteinData
        {
            public string Name { get; set; }
            public string Organism { get; set; }
            public string Sequence { get; set; }
        }

        static void Main(string[] args)
        {
            string sequencesPath = "sequences.txt";
            string commandsPath = "commands.txt";
            string outputPath = "genedata.txt";

            if (!File.Exists(sequencesPath) || !File.Exists(commandsPath))
            {
                Console.WriteLine("Ошибка: Отсутствуют входные файлы sequences.txt или commands.txt!");
                return;
            }

            List<ProteinData> proteins = new List<ProteinData>();
            foreach (var line in File.ReadLines(sequencesPath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('\t');
                if (parts.Length >= 3)
                {
                    proteins.Add(new ProteinData
                    {
                        Name = parts[0].Trim(),
                        Organism = parts[1].Trim(),
                        Sequence = DecodeRLE(parts[2].Trim())
                    });
                }
            }

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("Андрей Архипов");
                writer.WriteLine("Генетический поиск");

                int commandCounter = 1;

                foreach (var line in File.ReadLines(commandsPath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split('\t');
                    string commandType = parts[0].Trim().ToLower();
                    string cmdNumberStr = commandCounter.ToString("D3");

                    writer.WriteLine("-----------------------------------------------------------------");

                    if (commandType == "search")
                    {
                        string searchTarget = DecodeRLE(parts[1].Trim());
                        writer.WriteLine($"{cmdNumberStr}  search  {searchTarget}");

                        var matches = proteins.Where(p => p.Sequence.Contains(searchTarget)).ToList();

                        if (matches.Count > 0)
                        {
                            foreach (var match in matches)
                            {
                                writer.WriteLine($"{match.Organism}     {match.Name}");
                            }
                        }
                        else
                        {
                            writer.WriteLine("NOT FOUND");
                        }
                    }
                    commandCounter++;
                }
            }
            Console.WriteLine("Обработка завершена. Результаты сохранены в genedata.txt");
        }

        static string DecodeRLE(string input)
        {
            var result = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    int count = input[i] - '0';
                    char letter = input[i + 1];
                    result.Append(letter, count);
                    i++;
                }
                else
                {
                    result.Append(input[i]);
                }
            }
            return result.ToString();
        }
    }
}
