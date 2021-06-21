################################################## Old Version ###########################################################
--- Version < 1 Python
  -- concept console apllication
   - show cpu load 
  --Windows XP - 10 support

--- Version 1.0.0 Python
  -- console application
   - show cpu and ram usage %
   - monitor temp in degree C use openhardwaremonitor.dll and python for .net 
  -- Windows 10 support 
  -- Windows 7 support

--- Version < 1.1.0 Python
  -- GUI Application
   - show cpu and ram usage %
   - monitor temp in degree C use openhardwaremonitor.dll and python for .net 
   - add connect mysql server and send data from this pc
   * bad performance more ram use when long time idle
   * can't reconnect to database
   * program end when can't connect to database
   * No color gui application bad UX and Ui 
  -- Windows 10 support 
  -- Windows 7 support

---Version >= 1.1.0 windows Python
  -- minor change connect database
  -- decrease ram usage 
  -- reduce first time runing
  -- add errorbox when can't connect to database
  -- windows 7 and 10 support
  -- no longer support update for version 1 in windows

################################################## Under Develop ###########################################################
---Version >= 1.1.0 Linux Python
  * root user required
  -- use structure from windows version
  -- edit some code to supoprt Linux
   - remove openhardwaremonitor.dll
  -- add system uptime display in the program
  -- add program uptime display in the program
  -- Tested on Ubuntu 16.04 desktop
  # under develop 18-june-2021
    -- add check box for backup and send data to server
    -- add send system up time to server
    * porgrame end when can't connect to data base

