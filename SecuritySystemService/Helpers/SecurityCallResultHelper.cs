using SecuritySystemService.Models;

namespace SecuritySystemService.Helpers
{
    public static class SecurityCallResultHelper
    {
        public static SecurityCallStateResult FromSuccessful(SecurityAlarmState? alarmState)
        {
            return new SecurityCallStateResult { IsSuccessful = true, AlarmState = alarmState };
        }

        public static SecurityCallStateResult FromUnsuccessful(SecurityAlarmState? alarmState)
        {
            return new SecurityCallStateResult { IsSuccessful = false, AlarmState = alarmState };
        }
    }
}
