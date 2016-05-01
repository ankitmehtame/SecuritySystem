using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SecuritySystemService.Models
{
    public class SecurityCallStateResult : SecurityCallResult
    {
        public SecurityAlarmState? AlarmState { get; set; }
    }
}
