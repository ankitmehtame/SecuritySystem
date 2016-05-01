preferences
{
  input("hostaddress", "text", title: "REST Host <IP:Port>")
  input("statepath", "text", title: "Get State REST Path e.g. /api/security/state")
  input("offpath", "text", title: "Off Action REST Path e.g. /api/security/off")
  input("notifypath", "text", title: "Notify Action REST Path e.g. /api/security/notify")
  input("warnpath", "text", title: "Warn Action REST Path e.g. /api/security/warn")
  input("alarmpath", "text", title: "Alarm Action REST Path e.g. /api/security/alarm")
}

metadata
{
  definition (name: "Ankit REST Siren", author: "Ankit Mehta")
  {
    capability "Alarm"
    capability "Refresh"
    command "activatenotifystate"
    command "activatewarnstate"
    command "activatealarmstate"
    command "activateoffstate"
  }
  simulator
  { }
  tiles
  {
    standardTile("state", "device.state", inactiveLabel: true, width: 2, height: 2, canChangeIcon: true)
    {
      state "off", label:"Off", backgroundColor:"#ffffff", defaultState: true
      state "offline", label:"Offline", backgroundColor:"#ff0000"
      state "notify", label:"Notify", backgroundColor:"#D7EB2A"
      state "warn", label:"Warn", backgroundColor:"#EBB82A"
      state "alarm", label:"Alarm", backgroundColor:"#F5411D"
    }
    standardTile("offAction", "Off", height: 1, width: 1, canChangeIcon: false)
    {
      state "off", label:"Off", action:"activateoffstate"
    }
    standardTile("notifyAction", "Notify", height: 1, width: 1, canChangeIcon: false)
    {
      state "notify", label:"Notify", action:"activatenotifystate"
    }
    standardTile("warnAction", "Warn", height: 1, width: 1, canChangeIcon: false)
    {
      state "warn", label:"Warn", action:"activatewarnstate"
    }
    standardTile("alarmAction", "Alarm", height: 1, width: 1, canChangeIcon: false)
    {
      state "alarm", label:"Alarm", action:"activatealarmstate"
    }
    standardTile("refresh", "device.refresh", inactiveLabel: true, decoration: "flat")
    {
      state "default", label:"", action:"refresh.refresh", icon:"st.secondary.refresh"
    }
    main("state")
    details(["state", "refresh"])
  }
}

def strobe()
{
}

def siren()
{
  activatewarnstate()
}

def both()
{
  strobe()
  siren()
}

def off()
{
  activateoffstate()
}

def activatenotifystate()
{
  callRestApi("notify")
}

def activatewarnstate()
{
  callRestApi("warn")
}

def activatealarmstate()
{
  callRestApi("alarm")
}

def activateoffstate()
{
  callRestApi("off")
}

def parse(String description)
{
    log.debug "parse called with ${description}"
    def msg = parseLanMessage(description)

    def status = msg.status          // => http status code of the response
    // log.debug("status ${status}")
    def json = msg.json              // => any JSON included in response body, as a data structure of lists and maps
    // log.debug("json ${json}")
    //def data = msg.data
    
    if(status >= 300 || status < 200 || json.isSuccessful == null)
    {
        log.debug("status is not in 200s")
        updateStatus("offline")
        refreshComplete()
        return
    }
    def nextState = json.alarmState
    if(nextState == null)
    {
        nextState = "offline"
    }
    log.debug("setting state to " + nextState)
    updateStatus(nextState)
    refreshComplete()
}

def callRestApi(status)
{
  if(state.isrefreshing == true)
  {
    log.debug("already refreshing. not proceeding.")
    return
  }
  state.isrefreshing = true
  state.nextStateOnSuccessfulCall = status
  def commandToCall
  if(status == "off") commandToCall = getOffCommand()
  else if(status == "notify") commandToCall = getNotifyCommand()
  else if(status == "warn") commandToCall = getWarnCommand()
  else if(status == "alarm") commandToCall = getAlarmCommand()
  else state.nextStateOnSuccessfulCall = null
  try
  {
      log.debug "Calling ${hostaddress}"
      updateStatusBeforeRefresh()
      sendHubCommand(commandToCall)
  }
  catch(all)
  {
      log.debug("Error during callRestApi in catch all for httpget. Error: ${all}")
      updateStatus("offline")
      refreshComplete()
  }
}

def refresh()
{
  log.debug("refresh called.")
  if(state.isrefreshing == true)
  {
    log.debug("already refreshing. not proceeding.")
    return
  }
  state.isrefreshing = true
  try
  {
      log.debug "Calling ${hostaddress}"
      updateStatusBeforeRefresh()
      def cmd = myCommand()
      sendHubCommand(cmd)
  }
  catch(all)
  {
      log.debug("Error during refresh in catch all for httpget. Error: ${all}")
      updateStatus("offline")
      refreshComplete()
  }
}

def refreshComplete()
{
  state.isrefreshing = false
  if(state.nextStateOnSuccessfulCall == null)
  {
      log.debug("refresh complete")
  }
  else
  {
      log.debug("call complete for " + state.nextStateOnSuccessfulCall)
  }
  state.nextStateOnSuccessfulCall = null
  
  if(state.ispolling)
  {
    runIn(60 * 10, refresh)
  }
}

def getStateCommand() {
    def result = new physicalgraph.device.HubAction(
        method: "GET",
        path: "${statepath}",
        headers: [
            HOST: "${hostaddress}"
        ]
    )
    return result
}

def getOffCommand() {
    log.debug("${hostaddress}${offpath}")
    def result = new physicalgraph.device.HubAction(
        method: "GET",
        path: "${offpath}",
        headers: [
            HOST: "${hostaddress}"
        ]
    )
    return result
}

def getNotifyCommand() {
    log.debug("${hostaddress}${notifypath}")
    def result = new physicalgraph.device.HubAction(
        method: "GET",
        path: "${notifypath}",
        headers: [
            HOST: "${hostaddress}"
        ]
    )
    return result
}

def getWarnCommand() {
    log.debug("${hostaddress}${warnpath}")
    def result = new physicalgraph.device.HubAction(
        method: "GET",
        path: "${warnpath}",
        headers: [
            HOST: "${hostaddress}"
        ]
    )
    return result
}

def getAlarmCommand() {
    log.debug("${hostaddress}${alarmpath}")
    def result = new physicalgraph.device.HubAction(
        method: "GET",
        path: "${alarmpath}",
        headers: [
            HOST: "${hostaddress}"
        ]
    )
    return result
}

def updateStatusBeforeRefresh()
{
    runIn(15, handler)
}

def handler()
{
    if(state.isrefreshing == true)
    {
        log.debug("Looks like the call did not complete. setting state as offline")
        updateStatus("offline")
        refreshComplete()
    }
}

def updateStatus(status)
{
    if (status == "offline") { sendEvent(name: "state", value: "offline", display: true, descriptionText: device.displayName + " is offline") }
    if (status == "off") { sendEvent(name: "state", value: "off", display: true, descriptionText: device.displayName + " is off ") }
    if (status == "notify") { sendEvent(name: "state", value: "notify", display: true, descriptionText: device.displayName + " is at notify stage") }
    if (status == "warn") { sendEvent(name: "state", value: "warn", display: true, descriptionText: device.displayName + " is at warn stage") }
    if (status == "alarm") { sendEvent(name: "state", value: "alarm", display: true, descriptionText: device.displayName + " is at alarm stage") }
}
