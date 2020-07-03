using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StockFocus.Helper
{
    public class Serializer
    {
        public static string SerializeObject<CustomObject>(CustomObject obj)
        {
            string xmlString = null;
            try
            {
                XmlSerializer objSerializer = new XmlSerializer(typeof(CustomObject));
                using (StringWriter stream = new StringWriter())
                {
                    objSerializer.Serialize(stream, obj);
                    stream.Flush();
                    xmlString = stream.ToString();
                }
            }
            catch
            {

            }
            return xmlString;
        }

        public static string SerializeObject(Type type , object obj)
        {
            string xmlString = null;
            try
            {
                XmlSerializer objSerializer = new XmlSerializer(type);
                using (StringWriter stream = new StringWriter())
                {
                    objSerializer.Serialize(stream, obj);
                    stream.Flush();
                    xmlString = stream.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return xmlString;
        }

        public static CustomObject DeserializeObject<CustomObject>(string xmlValue)
        {
            CustomObject retVal = default(CustomObject);

            XmlSerializer deserializer = new XmlSerializer(typeof(CustomObject));
            using (StringReader stream = new StringReader(xmlValue))
            {

                retVal = (CustomObject)deserializer.Deserialize(stream);

            }
            return retVal;

        }

        public static object DeserializeObject(string xmlValue, Type type)
        {
            // CustomObject retVal = default(CustomObject);
            object retVal;
            XmlSerializer deserializer = new XmlSerializer(type);
            using (StringReader stream = new StringReader(xmlValue))
            {

                retVal = deserializer.Deserialize(stream);

            }
            return retVal;

        }
    }
}
