using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace com.mobiquity.packer
{
    public class Config
    {
        public Encoding ExpectedFileEncoding { get; private init; } = Encoding.UTF8;
        public string IndexKey { get; private init; } = "index";
        public string ItemWeightKey { get; private init; } = "item_weight";
        public string CostKey { get; private init; } = "cost";
        public string MaxPackageWeightKey { get; private init; } = "max_weight";
        public string GroupRegex { get; private init; } = "\\s*\\((?<index>\\d*),(?<item_weight>[0-9]+\\.?[0-9]*),€?(?<cost>[0-9]+\\.?[0-9]*)\\)";
        public string LineRegex { get; private init; } = "^(?<max_weight>[0-9]+\\.?[0-9]*)\\s*:(?<items>.*)$";
        public float MaxPackageWeight { get; private init; } = 100f;
        public float MaxItemWeight { get; private init; } = 100f;
        public float MaxItemCost { get; private init; } = 100f;
        public int MaxItems { get; private init; } = 15;
        public bool OnlyAllowAbsolutePaths { get; private init; } = true;

        public static Config LoadFromAppSettingsJson()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var encoding = configuration.GetValue<string>("FileEncoding");
            var expectedEncoding = encoding switch
            {
                "UTF8" => Encoding.UTF8,
                "UTF32" => Encoding.UTF32,
                "ASCII" => Encoding.ASCII,
                "Latin1" => Encoding.Latin1,
                "Unicode" => Encoding.Unicode,
                "BigEndianUnicode" => Encoding.BigEndianUnicode,
                "Default" => Encoding.Default,
                _ => throw new APIException($"Unexpected FileEncoding ({encoding}). Available options: UTF8,UTF32,ASCII,Latin1,Unicode,BigEndianUnicode,Default.")
            };
            var config = new Config
            {
                ExpectedFileEncoding = expectedEncoding,
                GroupRegex = configuration.GetValue<string>("GroupRegex"),
                LineRegex = configuration.GetValue<string>("LineRegex"),
                MaxPackageWeight = configuration.GetValue<float>("MaxPackageWeight"),
                MaxItemWeight = configuration.GetValue<float>("MaxItemWeight"),
                MaxItemCost = configuration.GetValue<float>("MaxItemCost"),
                MaxPackageWeightKey = configuration.GetValue<string>("MaxPackageWeightKey"),
                IndexKey = configuration.GetValue<string>("IndexKey"),
                ItemWeightKey = configuration.GetValue<string>("ItemWeightKey"),
                CostKey = configuration.GetValue<string>("CostKey"),
                MaxItems = configuration.GetValue<int>("MaxItems"),
                OnlyAllowAbsolutePaths = configuration.GetValue<bool>("OnlyAllowAbsolutePaths")
            };
            if (!config.LineRegex.Contains(config.MaxPackageWeightKey))
            {
                throw new APIException($"LineRegex should contain a group that matches the MaxPackageWeightKey configuration.");
            }
            if (!config.GroupRegex.Contains(config.IndexKey))
            {
                throw new APIException($"GroupRegex should contain a group that matches the IndexKey configuration.");
            }
            if (!config.GroupRegex.Contains(config.ItemWeightKey))
            {
                throw new APIException($"GroupRegex should contain a group that matches the ItemWeightKey configuration.");
            }
            if (!config.GroupRegex.Contains(config.CostKey))
            {
                throw new APIException($"GroupRegex should contain a group that matches the CostKey configuration.");
            }
            return config;
        }
    }
}