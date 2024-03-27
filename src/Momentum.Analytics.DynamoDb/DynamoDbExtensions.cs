using Amazon.DynamoDBv2.Model;
using NodaTime;

namespace Momentum.Analytics.DynamoDb
{
    public static class DynamoDbExtensions
    {
        public static Dictionary<string, AttributeValue> AddField(
            this Dictionary<string, AttributeValue> fields, 
            string name, 
            string value)
        {
            if(fields == null)
            {
                fields = new Dictionary<string, AttributeValue>();
            } // end if

            if(!string.IsNullOrWhiteSpace(value))
            {
                fields.Add(name, new AttributeValue(value));
            } // end if            

            return fields;
        } // end method

        public static string ReadString(
            this Dictionary<string, AttributeValue> fields,
            string name,
            string defaultValue = null)
        {
            var result = defaultValue;
            if(fields != null 
                && fields.TryGetValue(name, out AttributeValue attr)
                && attr != null)
            {
                result = attr.S;
            } // end if 

            return result;
        } // end method

        public static Dictionary<string, AttributeValue> AddField(
            this Dictionary<string, AttributeValue> fields, 
            string name, 
            Guid value)
        {
            return fields.AddField(name, value.ToString());
        } // end method

        public static Guid? ReadGuid(
            this Dictionary<string, AttributeValue> fields,
            string name,
            bool throwExceptionOnNull = false)
        {
            Guid? result = null;
            var fieldValue = fields.ReadString(name);
            if(!string.IsNullOrWhiteSpace(fieldValue))
            {
                result = Guid.Parse(fieldValue);
            }
            else if(throwExceptionOnNull)
            {
                throw new ArgumentNullException(name, $"{name} field was not found in dynamodb fields dict.");
            } // end if

            return result;
        } // end method

        public static Dictionary<string, AttributeValue> AddField(
            this Dictionary<string, AttributeValue> fields, 
            string name, 
            int? value)
        {
            if(fields == null)
            {
                fields = new Dictionary<string, AttributeValue>();
            } // end if

            if(value.HasValue)
            {
                fields.Add(name, new AttributeValue()
                {
                    N = value.ToString()
                });
            } // end if            

            return fields;
        } // end method

        public static Dictionary<string, AttributeValue> AddField(
            this Dictionary<string, AttributeValue> fields, 
            string name, 
            int value)
        {
            if(fields == null)
            {
                fields = new Dictionary<string, AttributeValue>();
            } // end if

            fields.Add(name, new AttributeValue()
            {
                N = value.ToString()
            });           

            return fields;
        } // end method

        public static int ReadInteger(
            this Dictionary<string, AttributeValue> fields,
            string name,
            int defaultValue = 0)
        {
            var result = defaultValue;
            if(fields != null 
                && fields.TryGetValue(name, out AttributeValue attr)
                && attr != null)
            {
                result = int.Parse(attr.N);
            } // end if

            return result;
        } // end method

        public static int? ReadNullableInteger(
            this Dictionary<string, AttributeValue> fields,
            string name)
        {
            int? result = null;
            if(fields != null 
                && fields.TryGetValue(name, out AttributeValue attr)
                && attr != null
                && attr.N != null)
            {
                result = int.Parse(attr.N);
            } // end if

            return result;
        } // end method

        public static Dictionary<string, AttributeValue> AddField(
            this Dictionary<string, AttributeValue> fields, 
            string name, 
            double value)
        {
            if(fields == null)
            {
                fields = new Dictionary<string, AttributeValue>();
            } // end if

            fields.Add(name, new AttributeValue()
            {
                N = value.ToString()
            });

            return fields;
        } // end method

        public static long ReadLong(
            this Dictionary<string, AttributeValue> fields,
            string name,
            long defaultValue = 0)
        {
            var result = defaultValue;
            if(fields != null 
                && fields.TryGetValue(name, out AttributeValue attr)
                && attr != null)
            {
                result = long.Parse(attr.N);
            } // end if

            return result;
        } // end method

        public static Dictionary<string, AttributeValue> AddField(
            this Dictionary<string, AttributeValue> fields, 
            string name, 
            Instant? value)
        {
            if(value.HasValue)
            {
                var msSinceEpoch = value.Value.ToUnixTimeMilliseconds();
                fields = fields.AddField(name, msSinceEpoch);
            } // end if            

            return fields;
        } // end method

        public static Instant? ReadDateTime(
            this Dictionary<string, AttributeValue> fields,
            string name,
            bool throwExceptionOnNull = false)
        {
            Instant? result = null;
            var msSinceEpoch = fields.ReadLong(name);
            if(msSinceEpoch != 0)
            {
                result = Instant.FromUnixTimeMilliseconds(msSinceEpoch);
            }
            else if(throwExceptionOnNull)
            {
                throw new ArgumentNullException(name, "${name} DateTime field was not present.");
            } // end if

            return result;
        } // end method

        public static Dictionary<string, AttributeValue> AddField(
            this Dictionary<string, AttributeValue> fields, 
            string name, 
            bool value)
        {
            if(fields == null)
            {
                fields = new Dictionary<string, AttributeValue>();
            } // end if

            fields.Add(name, new AttributeValue()
            {
                N = value ? 1.ToString() : 0.ToString()
            });

            return fields;
        } // end method

        public static bool? ReadBoolean(
            this Dictionary<string, AttributeValue> fields,
            string name,
            bool? defaultValue = null)
        {
            var result = defaultValue;
            if(fields != null 
                && fields.TryGetValue(name, out AttributeValue attr)
                && attr != null)
            {
                if(attr.N == "1")
                {
                    result = true;
                }
                else
                {
                    result = false;
                } // end if
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace