using System.Runtime.Serialization;

namespace Defence.In.Depth.DataContracts
{
    [DataContract]
    public class ProductDataContract
    {
        [DataMember]
        public string Id { get; set; }
        
        [DataMember]
        public string Name { get; set; }
    }
}