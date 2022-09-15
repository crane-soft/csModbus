using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csModbusView;
using Newtonsoft.Json;
using System.Drawing;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace csModMaster
{
    class MbViewSerialize
    {
    
        string jsonStr;

        public class ModbusViewContractResolver : DefaultContractResolver
        {
            private string[] propertyList = { "Name","Title", "BaseAddr", "NumItems", "ItemColumns", "ItemNames", "Location", "Size" };

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                if (Array.IndexOf(propertyList, member.Name) >= 0) {
                    var property = base.CreateProperty(member, memberSerialization);
                    return property;
                }
                return null;
            }

        }


        public void Serialize(Control.ControlCollection InputCollection)
        {
           List<Control> serList = new List<Control>();
            foreach (Control controlobj in InputCollection) {
                Type cType = controlobj.GetType();
                
                if (cType.IsSubclassOf(typeof(ModbusView))) {
                    serList.Add(controlobj);
                }
            }

            var settings = new JsonSerializerSettings {
                ContractResolver = new ModbusViewContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto
            };
            jsonStr = JsonConvert.SerializeObject(serList, Formatting.Indented, settings);
            System.IO.File.WriteAllText(@"mbviewerdefault.jsdon", jsonStr);
        
        }

        public void Deserialize(Control.ControlCollection OutCollection)
        {
            var settings = new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto
            };
            List<ModbusView> outList = JsonConvert.DeserializeObject<List<ModbusView>>(jsonStr, settings);
            foreach (ModbusView mbview in outList) {
                OutCollection.Add(mbview);
            }
        }
    }
}
