using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using com.mobiquity.packer.Contracts;

namespace com.mobiquity.packer.Validations
{
    public static class LineValidation
    {
        public static (float maxWeight, List<Item> items) ParseAndValidate(this string line, int lineNumber, Config config)
        {
            var maxWeight  = line.ValidateLineFormatAndGetMaxWeight(lineNumber, config);
            var items = line.ValidateAndParseItems(lineNumber, config);
            return (maxWeight, items);
        }

        public static float ValidateLineFormatAndGetMaxWeight(this string line, int lineNumber, Config config)
        {
            var match = Regex.Match(line, config.LineRegex);
            if (!match.Success)
            {
                // This could be improved by making the format configurable.
                throw new APIException($"Line {lineNumber}: should be in the format 'max_weight:items' where each item matches GroupRegex.");
            }

            if (float.TryParse(match.Groups[config.MaxPackageWeightKey].Value, out var maxWeight) && maxWeight > config.MaxPackageWeight)
            {
                throw new APIException($"Line {lineNumber}: Max package weight is {config.MaxPackageWeight} but value was {maxWeight}.");
            }

            return maxWeight;
        }

        public static List<Item> ValidateAndParseItems(this string line, int lineNumber, Config config)
        {
            var matches = Regex.Matches(line, config.GroupRegex);
            ValidateItemFormat(lineNumber, matches);
            ValidateMaxItems(lineNumber, config, matches);

            var errors = new StringBuilder(matches.Count);
            var items = new List<Item>(matches.Count);
            foreach (Match match in matches)
            {
                var index = match.Groups[config.IndexKey].Value;
                var weight = ValidateItemWeight(lineNumber, index, config, match, errors);
                var cost = ValidateItemCost(lineNumber, index, config, match, errors);
                items.Add(new Item(index, weight, cost));
            }
            ValidateDuplicates(lineNumber, items.Select(item => item.Index), errors);

            if (errors.Length > 0)
            {
                throw new APIException(errors.ToString());
            }
            return items;
        }

        private static void ValidateMaxItems(int lineNumber, Config config, MatchCollection matches)
        {
            if (matches.Count > config.MaxItems)
            {
                throw new APIException($"Line {lineNumber}: no more than {config.MaxItems} items allowed.");
            }
        }

        private static void ValidateItemFormat(int lineNumber, MatchCollection matches)
        {
            if (matches.Count == 0)
            {
                throw new APIException($"Line {lineNumber}: items should be in the format in GroupRegex.'");
            }
        }

        private static void ValidateDuplicates(int lineNumber, IEnumerable<string> indexes, StringBuilder errors)
        {
            var duplicates = indexes
                .GroupBy(value => value)
                .Where(grouping => grouping.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicates.Any())
            {
                errors.AppendLine($"Line {lineNumber}: duplicates indexes found: {string.Join(", ", duplicates)}.");
            }
        }

        private static float ValidateItemCost(int lineNumber, string index, Config config, Match match, StringBuilder errors)
        {
            if (float.TryParse(match.Groups[config.CostKey].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var cost) &&
                cost > config.MaxItemCost)
            {
                // This could be improved by making the currency symbol configurable.
                errors.AppendLine(
                    $"Line {lineNumber}: The max weight for an item is {config.MaxItemCost} but index {index} costs €{cost}.");
            }
            return cost;
        }

        private static float ValidateItemWeight(int lineNumber, string index, Config config, Match match, StringBuilder errors)
        {
            if (float.TryParse(match.Groups[config.ItemWeightKey].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var itemWeight) &&
                itemWeight > config.MaxItemWeight)
            {
                errors.AppendLine(
                    $"Line {lineNumber}: The max weight for an item is {config.MaxItemWeight} but index {index} weighs {itemWeight}.");
            }
            return itemWeight;
        }
    }
}