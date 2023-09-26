using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Sinyi.Helpers
{
    public class PostCodeParser
    {
        // Fields
        private List<(string city, string district, string postCode)> _postCodeList = null;

        private static Lazy<PostCodeParser> _instance = new Lazy<PostCodeParser>(() => new PostCodeParser());


        // Constructor
        private PostCodeParser()
        {
            // Default
            var postCodeDictionary = this.LoadFile();
            _postCodeList = this.ExpandDict(postCodeDictionary);
        }


        // Properties
        public static PostCodeParser Instance => _instance.Value;


        // Methods
        public bool TryGetPostCode(string input, out string postCode)
        {
            // Feilds
            postCode = string.Empty;

            // Requirement
            if (string.IsNullOrWhiteSpace(input) == true) return false;

            // Get
            postCode = this.GetPostCode(input);
            if (string.IsNullOrEmpty(postCode) == true) return false;

            // Return
            return true;
        }

        public string GetPostCode(string cityDistrict)
        {
            #region Contracts

            if (string.IsNullOrEmpty(cityDistrict) == true) return string.Empty;

            #endregion

            // Requirment
            if (cityDistrict.Length < 5) return string.Empty;  // 最短的如台中市西區

            // Fields
            var city = this.CheckCity(cityDistrict.Substring(0, 3));
            var district = cityDistrict.Substring(3, cityDistrict.Length - 3);

            // Find
            var result = _postCodeList.Find(post => city == post.city && district.StartsWith(post.district));
            if (result == default(ValueTuple<string, string, string>)) return string.Empty;

            // Return
            return result.postCode;
        }

        public string GetPostCode(string city, string district)
        {
            #region Contracts

            if (string.IsNullOrEmpty(city) == true) return string.Empty;
            if (string.IsNullOrEmpty(district) == true) return string.Empty; ;

            #endregion

            // Fields
            city = this.CheckCity(city);

            // Find
            var result = _postCodeList.Find(post => city == post.city && district.StartsWith(post.district));
            if (result == default(ValueTuple<string, string, string>)) return string.Empty;

            // Return
            return result.postCode;
        }

        public (string city, string district) GetCityAndDistrict(string postCode)
        {
            #region Contracts

            if (string.IsNullOrWhiteSpace(postCode) == true) return default(ValueTuple<string, string>);

            #endregion

            // Find
            var result = _postCodeList.Find(post => post.postCode == postCode.Trim());
            if (result == default(ValueTuple<string, string, string>)) return default(ValueTuple<string, string>);

            // Return
            return (result.city, result.district);
        }

        private Dictionary<string, Dictionary<string, string>> LoadFile()
        {
            // Fields
            var filePath = @"Address\PostCode.json";

            // Read file
            var provider = new EmbeddedFileProvider(GetType().GetTypeInfo().Assembly);
            var fileInfo = provider.GetFileInfo(filePath);

            // Deserialize
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(fileInfo.CreateReadStream());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private List<(string city, string district, string postCode)> ExpandDict(Dictionary<string, Dictionary<string, string>> dict)
        {
            #region Contracts

            if (dict == null) throw new ArgumentException(nameof(dict));

            #endregion

            // Convert
            var result = dict.SelectMany(cityDic => cityDic.Value.Select(districtDic => (cityDic.Key, districtDic.Key, districtDic.Value)));

            // Return
            return result.ToList();
        }

        private string CheckCity(string city)
        {
            // Requirment
            if (string.IsNullOrEmpty(city) == true) return string.Empty;

            // Return
            return city.Replace("臺", "台");
        }
    }
}
