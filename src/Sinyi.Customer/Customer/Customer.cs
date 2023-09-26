using AutoMapper;
using Sinyi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Customers
{
    // Class
    public class Customer
    {
        // Fields
        private string _customerId = string.Empty;

        private string _agentId = string.Empty;

        private string _name = string.Empty;

        private string _identityNumber = string.Empty;

        private string _lineId = string.Empty;

        private string _emailAddress = string.Empty;

        private string _mobilePhoneNumber = string.Empty;

        private string _postCode = string.Empty;

        private string _residencePostCode = string.Empty;

        private string _residenceAddress = string.Empty;

        private readonly PostCodeParser _postCodeParser = null;


        // Consturctor
        public Customer()
        {
            // Default
            _postCodeParser = PostCodeParser.Instance;
        }


        // Properties
        public string CustomerId
        {
            get => _customerId;
            set => _customerId = value?.Trim();
        }

        public string AgentId
        { 
            get => _agentId;
            set => _agentId = value?.Trim();
        }

        public string StoreId { get; set; }                                     // 維護資料當下業務所在分店

        public string Name                                                      // 客戶姓名
        {
            get => _name;
            set => _name = value?.Trim();
        }

        public string Alias { get; set; } = string.Empty;                       // 客戶別名

        public int? Gender { get; set; } = null;                                // 性別 (0 無，1 男，2 女)

        public string IdentityNumber                                            // 身分證號碼
        { 
            get => _identityNumber;
            set => _identityNumber = value?.Trim();
        }

        public DateTime? Birthday { get; set; }                                 // 生日

        public string LineId                                                    // Line
        {
            get => _lineId;
            set => _lineId = value?.Trim();
        }

        public string EmailAddress                                              // 電子郵件
        {
            get => _emailAddress;
            set => _emailAddress = value?.Trim();
        }

        public string MobilePhoneNumber                                         // 行動電話號碼
        {
            get => _mobilePhoneNumber;
            set => _mobilePhoneNumber = value?.Trim()?.Replace(",", ""); 
        }

        public string PostCode                                                  // 區駐地郵遞區號
        {
            get => _postCode;
            set => _postCode = value?.Trim();
        }

        public string City { get; set; }                                        // 區住縣市

        public string District { get; set; }                                    // 區住區域

        public string Address                                                   // 區住路段 (等庭吟切換到下面的就要廢棄)
        {
            get => NoCityDistrictAddress;
            set => NoCityDistrictAddress = value;
        }

        public string NoCityDistrictAddress { get; set; } = string.Empty;       // 區住路段

        public string HomeAddress                                               // 完整區住地址 (等庭吟切換到下面的就要廢棄)
        {
            get => FullAddress;
        }

        public string FullAddress                                               // 完整區住地址
        {
            get => GetFullAddress();
        }

        public string ResidencePostCode                                         // 戶籍地郵遞區號
        {
            get => _residencePostCode; 
            set => _residencePostCode = value?.Trim();
        }

        public string ResidenceNoCityDistrictAddress                            // 戶籍地址
        {
            get => _residenceAddress;
            set => _residenceAddress = this.GetNoCityDistrictAddress(this.ResidencePostCode, value?.Trim());
        }

        public string CommunityName { get; set; } = string.Empty;               // 社區名稱

        public int SourceTypeId { get; set; }                                   // 客戶來源 ID


        public string Remark { get; set; } = string.Empty;                      // 備註

        public DateTime? CreateDate { get; set; }                               // 資料建立日

        public DateTime? UpdateDate { get; set; }                               // 最後更新日

        public DataSource DataSource { get; set; } = DataSource.None;           // 紀錄資料來源


        // Methods
        // 為了符合內網 CS101F 地址特性
        private string GetNoCityDistrictAddress(string areaCode, string noCityDistrictAddress)
        {
            // Requirement
            if (areaCode == null) areaCode = string.Empty;
            if (noCityDistrictAddress == null) noCityDistrictAddress = string.Empty;

            // Fields
            var result = noCityDistrictAddress;

            // Get
            var cityDistrict = _postCodeParser.GetCityAndDistrict(areaCode);
            if (cityDistrict != default(ValueTuple<string, string>) && noCityDistrictAddress.StartsWith($"{cityDistrict.city}{cityDistrict.district}") != true)
                result = $"{cityDistrict.city}{cityDistrict.district}{noCityDistrictAddress}";

            // Return
            return result;
        }

        private string GetFullAddress()
        {
             // Requirement
             if (string.IsNullOrEmpty(this.City) != true)       // City 有資料就用來合併完整地址
                return this.City + this.District + this.NoCityDistrictAddress;

            // Convert
            var cityDistrict = _postCodeParser.GetCityAndDistrict(this.PostCode);   // 內網址有郵遞區號沒有 city 跟 district

            // Return
            return $"{cityDistrict.city}{cityDistrict.district}{this.NoCityDistrictAddress}";
        }
    }


    // Enum
    public enum DataSource
    {
        None,
        SuperAgent,
        Intra,
    }
}
