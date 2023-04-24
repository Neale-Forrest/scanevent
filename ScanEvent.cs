using System;
using System.Collections.Generic;
using System.Text;

namespace ScanEvent
{
    public class Device
    {
        public int DeviceTransactionId { get; set; }
        public int DeviceId { get; set; }
    }

    public class ScanEvent
    {
        public int EventId { get; set; }
        public int ParcelId { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public string StatusCode { get; set; }
        public Device Device { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public string UserId { get; set; }
        public string CarrierId { get; set; }
        public string RunId { get; set; }
    }

    public enum EventType
    {
        PICKUP,
        DELIVERY,
        STATUS

    }
}
