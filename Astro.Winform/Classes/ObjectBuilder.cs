using Astro.Models;
using Astro.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Astro.Winform.Classes
{
    internal class ObjectBuilder : BinaryToObjectBuilder<User>
    {
        ObjectBuilder(Stream stream): base(stream) { }
        protected override User CreateObject(BinaryReader reader)
        {
            var values = new object?[]
            {
                ReadInt16(),                         // user_id
                ReadString(),                        // user_firstname
                ReadString(),                        // user_lastname
                ReadInt16(),                         // role_id
                ReadString(),                        // user_name
                ReadString(),                        // normalized_user_name
                ReadString(),                        // email
                ReadBoolean(),                       // email_confirmed
                ReadString(),                        // phone_number
                ReadBoolean(),                       // phone_number_confirmed
                ReadDateTime(),                      // date_of_birth
                ReadInt16(),                         // sex
                ReadInt16(),                         // marital_status
                ReadString(),                        // street_address
                ReadInt32(),                         // city_id
                ReadInt16(),                         // state_id
                ReadInt16(),
                ReadString(),                        // zip_code
                ReadBoolean(),                       // two_factor_enabled
                ReadInt16(),                         // access_failed_count
                ReadBoolean(),                       // lockout_enabled
                ReadNullableDateTime(),     // lockout_end
                ReadString(),                        // security_stamp
                ReadString(),                        // concurrency_stamp
                ReadBoolean(),                       // use_password_expiration
                ReadNullableDateTime(),     // password_expiration_date
                ReadString()                         // password_hash
            };
            return User.Create(values);
        }
    }
}
