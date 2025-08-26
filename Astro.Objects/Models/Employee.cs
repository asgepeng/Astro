using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Employee
    {
        private Employee(Streams.Reader reader)
        {
            Id = reader.ReadInt16();
            FullName = reader.ReadString();
            PlaceOfBirth = reader.ReadString();
            DateOfBirth = reader.ReadDateTime();
            Sex = reader.ReadByte();
            MaritalStatus = reader.ReadByte();
            StreetAddress = reader.ReadString();
            VillageId = reader.ReadInt64();
            ZipCode = reader.ReadString();
            PhoneNumber = reader.ReadString();
            Email = reader.ReadString();
            HiredDate = reader.ReadNullableDateTime();
            RoleId = reader.ReadInt16();
            IsActive = reader.ReadBoolean();
            TerminationDate = reader.ReadNullableDateTime();
            PayrollDate = reader.ReadInt16();
            PayrollMethod = reader.ReadInt16();
            Notes = reader.ReadString();
        }
        [JsonPropertyName("id")]
        public short Id { get; set; }
        [JsonPropertyName("fullname")]
        public string FullName { get; set; } = "";
        [JsonPropertyName("placeOfBirth")]
        public string PlaceOfBirth { get; set; } = "";
        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        [JsonPropertyName("sex")]
        public short Sex { get; set; }
        [JsonPropertyName("maritalStatus")]
        public short MaritalStatus { get; set; }
        [JsonPropertyName("streetAddress")]
        public string StreetAddress { get; set; } = "";
        [JsonPropertyName("villageId")]
        public long VillageId { get; set; }
        [JsonPropertyName("postalCode")]
        public string ZipCode { get; set; } = "";
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = "";
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";
        [JsonPropertyName("hiredDate")]
        public DateTime? HiredDate { get; set; }
        [JsonPropertyName("roleId")]
        public short RoleId { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("terminationDate")]
        public DateTime? TerminationDate { get; set; }
        [JsonPropertyName("payrollDate")]
        public short PayrollDate { get; set; }
        [JsonPropertyName("payrollMethod")]
        public short PayrollMethod { get; set; }
        [JsonPropertyName("notes")]
        public string Notes { get; set; } = "";

        public static Employee Create(Streams.Reader reader) => new Employee(reader);
    }
}
