using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using com.mobiquity.packer.Contracts;
using com.mobiquity.packer.Validations;

namespace com.mobiquity.packer
{
    public class Packer
    {
        private readonly Config _config;

        private Packer(Config config)
        {
            _config = config;
        }

        // ReSharper disable once InconsistentNaming
        public static string pack(string filePath)
        {
            var packer = new Packer(Config.LoadFromAppSettingsJson());
            return packer.ValidateAndPack(filePath);
        }

        private string ValidateAndPack(string filePath)
        {
            var fileValidation = new FileValidation(filePath, _config);
            fileValidation.Validate();
            return PackFromFile(filePath);
        }

        private string PackFromFile(string filePath)
        {
            var output = new StringBuilder();

            var lineNumber = 1;
            using var file = new StreamReader(filePath);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                var result = PackLine(line, lineNumber++, _config);
                output.AppendLine(result);
            }

            return output.ToString();
        }

        public static string PackLine(string line, int lineNumber, Config config)
        {
            var (maxWeight, items) = line.ParseAndValidate(lineNumber, config);
            return PackIntoPackage(maxWeight, items);
        }

        private static string PackIntoPackage(float maxWeight, List<Item> items)
        {
            var combinations = FindValidCombinations(maxWeight, items);
            if (!combinations.Any())
            {
                return "-";
            }
            var bestCombination = combinations
                .OrderByDescending(combo => combo.TotalCost)
                .ThenBy(combo => combo.TotalWeight)
                .First();
            
            return string.Join(",", bestCombination.Items.Select(item => item.Index));
        }

        private static List<SummedItems> FindValidCombinations(float maxWeight, List<Item> items)
        {
            return GenerateAllCombinations(items)
                .Select(combinations => new SummedItems
                {
                    TotalWeight = combinations.Sum(item => item.Weight),
                    TotalCost = combinations.Sum(item => item.Cost),
                    Items = combinations
                })                
                .Where(combinations => combinations.TotalWeight <= maxWeight && combinations.Items.Any())
                .ToList();
        }

        // Adapted from: https://stackoverflow.com/a/57058345/333427
        private static IEnumerable<List<Item>> GenerateAllCombinations(List<Item> items)
        {
            return Enumerable
                .Range(0, 1 << items.Count)
                .Select(index => items.Where((_, innerIndex) => (index & (1 << innerIndex)) != 0).ToList());
        }
    }
}